using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : PersistentSingleton<SceneController>
{
    public static event Action OnSceneLoadStart;
    public static event Action OnSceneLoadComplete;
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // Kích hoạt Event để bật màn hình Loading (chặn người chơi thao tác)
        OnSceneLoadStart?.Invoke();

        // Tạm dừng thời gian thực nếu game đang Pause trước khi đổi Scene
        Time.timeScale = 1f;

        // Load bất đồng bộ để tránh kẹt frame
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            // Có thể truy xuất asyncLoad.progress tại đây để làm thanh tiến trình (0 đến 1)
            yield return null;
        }

        OnSceneLoadComplete?.Invoke();
    }
}
