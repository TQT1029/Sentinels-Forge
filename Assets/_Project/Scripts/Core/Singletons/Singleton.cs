using UnityEngine;

[DefaultExecutionOrder(-100)]
public abstract class Singleton<T> : MonoBehaviour where T : Component
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
        }
        else if (Instance != this)
        {
            // Ngăn chặn việc lỡ tay kéo 2 Manager vào cùng 1 scene
            Destroy(gameObject);
        }
    }
}