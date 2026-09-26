using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using SentinelForge.Core.Interfaces;

namespace SentinelForge.Core.Pooling
{
    /// <summary>
    /// Trình quản lý Object Pool tập trung cho Projectile, Enemy, VFX, Floating Text.
    /// Sử dụng UnityEngine.Pool.ObjectPool với khả năng tracking tự động và hỗ trợ IPoolable.
    /// </summary>
    public class PoolManager : IDisposable
    {
        private readonly Dictionary<int, ObjectPool<GameObject>> _poolsByPrefabId = new();
        private readonly Dictionary<int, int> _instanceToPrefabId = new();
        private Transform _poolRoot;

        public PoolManager()
        {
            InitializeRoot();
        }

        private void InitializeRoot()
        {
            if (_poolRoot == null)
            {
                var go = new GameObject("[Centralized_PoolManager]");
                UnityEngine.Object.DontDestroyOnLoad(go);
                _poolRoot = go.transform;
            }
        }

        /// <summary>
        /// Khởi tạo trước một số lượng object trong pool để tránh giật lag lúc đang chơi.
        /// </summary>
        public void Prewarm<T>(T prefab, int count, Transform parent = null) where T : Component
        {
            if (prefab == null || count <= 0) return;

            GameObject prefabGo = prefab.gameObject;
            int prefabId = prefabGo.GetInstanceID();
            var pool = GetOrCreatePool(prefabGo, parent);

            List<GameObject> prewarmed = new List<GameObject>(count);
            for (int i = 0; i < count; i++)
            {
                prewarmed.Add(pool.Get());
            }

            foreach (var go in prewarmed)
            {
                pool.Release(go);
            }
        }

        /// <summary>
        /// Lấy một object từ pool với vị trí và góc xoay chỉ định.
        /// </summary>
        public T Get<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent = null) where T : Component
        {
            if (prefab == null)
            {
                Debug.LogError("[PoolManager] Không thể Get() một prefab null!");
                return null;
            }

            GameObject prefabGo = prefab.gameObject;
            int prefabId = prefabGo.GetInstanceID();
            var pool = GetOrCreatePool(prefabGo, parent);

            GameObject instance = pool.Get();
            instance.transform.SetPositionAndRotation(position, rotation);
            if (parent != null)
            {
                instance.transform.SetParent(parent);
            }

            _instanceToPrefabId[instance.GetInstanceID()] = prefabId;

            if (instance.TryGetComponent<IPoolable>(out var poolable))
            {
                poolable.OnGetFromPool();
            }

            return instance.GetComponent<T>();
        }

        /// <summary>
        /// Thu hồi object về lại pool.
        /// </summary>
        public void Release<T>(T instance) where T : Component
        {
            if (instance == null) return;
            Release(instance.gameObject);
        }

        /// <summary>
        /// Thu hồi GameObject về lại pool tương ứng.
        /// </summary>
        public void Release(GameObject instance)
        {
            if (instance == null) return;

            int instanceId = instance.GetInstanceID();
            if (!_instanceToPrefabId.TryGetValue(instanceId, out int prefabId))
            {
                Debug.LogWarning($"[PoolManager] Object '{instance.name}' không được tạo từ PoolManager. Hủy bằng Destroy().");
                UnityEngine.Object.Destroy(instance);
                return;
            }

            if (!_poolsByPrefabId.TryGetValue(prefabId, out var pool))
            {
                Debug.LogWarning($"[PoolManager] Không tìm thấy pool cho prefabId {prefabId}. Hủy bằng Destroy().");
                UnityEngine.Object.Destroy(instance);
                _instanceToPrefabId.Remove(instanceId);
                return;
            }

            if (instance.TryGetComponent<IPoolable>(out var poolable))
            {
                poolable.OnReturnToPool();
            }

            pool.Release(instance);
        }

        private ObjectPool<GameObject> GetOrCreatePool(GameObject prefab, Transform customParent = null)
        {
            int prefabId = prefab.GetInstanceID();
            if (_poolsByPrefabId.TryGetValue(prefabId, out var existingPool))
            {
                return existingPool;
            }

            Transform poolParent = customParent != null ? customParent : _poolRoot;

            var newPool = new ObjectPool<GameObject>(
                createFunc: () =>
                {
                    GameObject obj = UnityEngine.Object.Instantiate(prefab, poolParent);
                    obj.name = prefab.name;
                    return obj;
                },
                actionOnGet: (obj) =>
                {
                    obj.SetActive(true);
                },
                actionOnRelease: (obj) =>
                {
                    obj.SetActive(false);
                    if (_poolRoot != null && obj.transform.parent != _poolRoot)
                    {
                        obj.transform.SetParent(_poolRoot);
                    }
                },
                actionOnDestroy: (obj) =>
                {
                    if (obj != null)
                    {
                        UnityEngine.Object.Destroy(obj);
                    }
                },
                collectionCheck: true,
                defaultCapacity: 20,
                maxSize: 1000
            );

            _poolsByPrefabId[prefabId] = newPool;
            return newPool;
        }

        public void Dispose()
        {
            foreach (var pool in _poolsByPrefabId.Values)
            {
                pool.Clear();
            }
            _poolsByPrefabId.Clear();
            _instanceToPrefabId.Clear();

            if (_poolRoot != null)
            {
                UnityEngine.Object.Destroy(_poolRoot.gameObject);
                _poolRoot = null;
            }
        }
    }
}
