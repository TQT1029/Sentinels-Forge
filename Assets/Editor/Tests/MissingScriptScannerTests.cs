using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Text;

namespace SentinelForge.Tests.EditMode
{
    [TestFixture]
    public class MissingScriptScannerTests
    {
        [Test, Order(1)]
        public void FixMissingScriptsInPlayingWaveScene()
        {
            string scenePath = "Assets/Scenes/Playing-Wave.unity";
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            var go = GameObject.Find("[GameScope]");
            if (go != null)
            {
                int removed = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
                if (go.GetComponent<GameplayLifetimeScope>() == null)
                {
                    go.AddComponent<GameplayLifetimeScope>();
                }
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
                Debug.Log($"Removed {removed} missing scripts from [GameScope] and ensured GameplayLifetimeScope is attached.");
            }
        }

        [Test, Order(2)]
        public void ScanForMissingScripts_InSceneAndPrefabs()
        {
            var sb = new StringBuilder();
            int totalMissing = 0;

            // 1. Scan all scenes in Assets/Scenes
            string[] sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets/Scenes" });
            foreach (var guid in sceneGuids)
            {
                string scenePath = AssetDatabase.GUIDToAssetPath(guid);
                var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                var rootObjects = scene.GetRootGameObjects();
                foreach (var root in rootObjects)
                {
                    var allGos = root.GetComponentsInChildren<Transform>(true);
                    foreach (var t in allGos)
                    {
                        int missingCount = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject);
                        if (missingCount > 0)
                        {
                            sb.AppendLine($"[Scene: {scenePath}] Missing {missingCount} script(s) on GameObject: {GetHierarchyPath(t)}");
                            totalMissing += missingCount;
                        }
                    }
                }
                if (scene != UnityEngine.SceneManagement.SceneManager.GetActiveScene())
                {
                    EditorSceneManager.CloseScene(scene, true);
                }
            }

            // 2. Scan all prefabs in Assets/
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });
            foreach (var guid in prefabGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab == null) continue;

                var allTransforms = prefab.GetComponentsInChildren<Transform>(true);
                foreach (var t in allTransforms)
                {
                    int missingCount = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject);
                    if (missingCount > 0)
                    {
                        sb.AppendLine($"[Prefab: {path}] Missing {missingCount} script(s) on GameObject: {GetHierarchyPath(t)}");
                        totalMissing += missingCount;
                    }
                }
            }

            Debug.Log($"[MissingScriptScanResult] Total missing scripts: {totalMissing}\n{sb}");
            Assert.AreEqual(0, totalMissing, $"Found {totalMissing} missing scripts:\n{sb}");
        }

        private string GetHierarchyPath(Transform t)
        {
            if (t.parent == null) return t.name;
            return GetHierarchyPath(t.parent) + "/" + t.name;
        }
    }
}
