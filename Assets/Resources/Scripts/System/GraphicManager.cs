using UnityEngine;

public class GraphicManager : PersistentSingleton<GraphicManager>
{

    public bool IsFullscreenEnabled { get; private set; } = true;
    public bool IsVSyncEnabled { get; private set; } = true;
    public bool IsVFXEnabled { get; private set; } = true;

    public void SetFullscreen(bool isFullscreen)
    {
        IsFullscreenEnabled = isFullscreen;
        Screen.fullScreen = IsFullscreenEnabled;
    }
    public void SetVSyncEnabled(bool isEnabled)
    {
        IsVSyncEnabled = isEnabled;
        QualitySettings.vSyncCount = IsVSyncEnabled ? 1 : 0;
    }

    public void SetVFXState(bool isEnabled)
    {
        IsVFXEnabled = isEnabled;

        // Tùy chọn: Dọn dẹp ngay lập tức các hiệu ứng đang bay trên màn hình nếu người chơi vừa tắt setting
        if (!isEnabled && VFXManager.Instance != null)
        {
            VFXManager.Instance.ClearAllActiveVFX();
        }
    }
}
