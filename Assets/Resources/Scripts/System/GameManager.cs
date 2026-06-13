using System;
using UnityEngine;

public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    GameOver
}
public class GameManager : PersistentSingleton<GameManager>
{
    public static event Action<GameState> OnGameStateChanged;
    public GameState CurrentState { get; private set; }
    private float previousTimeScale = 1f;

    private void Start()
    {
        // Bắt đầu game luôn ở trạng thái Playing
        ChangeState(GameState.Playing);
    }
    private void Update()
    {
        // Nhấn ESC để bật/tắt Pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        // Không cho phép Pause nếu game đã kết thúc
        if (CurrentState == GameState.GameOver) return;

        if (CurrentState == GameState.Playing)
            ChangeState(GameState.Paused);
        else if (CurrentState == GameState.Paused)
            ChangeState(GameState.Playing);
    }
    public void ChangeState(GameState newState)
    {
        CurrentState = newState;


        //Xử lý change state
        switch (newState)
        {
            case GameState.MainMenu:
                Time.timeScale = 1f; // Thời gian bình thường khi ở menu chính
                break;
            case GameState.Playing:
                Time.timeScale = previousTimeScale; // Tiếp tục thời gian khi chơi
                break;
            case GameState.Paused:
                previousTimeScale = Time.timeScale;
                Time.timeScale = 0f; // Dừng thời gian khi tạm dừng
                break;
            case GameState.GameOver:
                Time.timeScale = 0f; // Dừng thời gian khi kết thúc game
                break;
        }

        // Hét lên cho các hệ thống khác biết để phản ứng
        OnGameStateChanged?.Invoke(newState);
    }
}
