using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


[RequireComponent(typeof(CanvasGroup))]
public class SettingsPanelUI : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 0.3f;

    [Header("UI References")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Toggle vfxToggle;
    [SerializeField] private Button mainMenuButton;
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        // Ẩn bảng Setting khi mới vào game
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    private void Start()
    {
        // Liên kết các UI Element bằng code. Tránh lỗi mất liên kết do kéo thả nhầm trên Inspector.
        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);

        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);

        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        if (fullscreenToggle != null)
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggled);

        if (vfxToggle != null)
            vfxToggle.onValueChanged.AddListener(OnVFXToggled);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);

        // Load dữ liệu cũ lên giao diện khi game vừa bắt đầu
        LoadCurrentSettingsToUI();
    }

    private void OnEnable()
    {
        // Đăng ký nghe lỏm từ GameManager (Static Event cực kỳ an toàn)
        GameManager.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
        canvasGroup.DOKill();
    }

    private void OnDestroy()
    {
        // Gỡ liên kết khi Object bị hủy để giải phóng bộ nhớ
        masterVolumeSlider?.onValueChanged.RemoveAllListeners();
        musicVolumeSlider?.onValueChanged.RemoveAllListeners();
        sfxVolumeSlider?.onValueChanged.RemoveAllListeners();
        fullscreenToggle?.onValueChanged.RemoveAllListeners();
        vfxToggle?.onValueChanged.RemoveAllListeners();
        mainMenuButton?.onClick.RemoveAllListeners();
    }

    // Tự động bật/tắt UI dựa vào trạng thái của Game
    private void HandleGameStateChanged(GameState state)
    {
        if (state == GameState.Paused)
        {
            ShowPanel();
        }
        else if (state == GameState.Playing)
        {
            HidePanel();
        }
    }

    private void ShowPanel()
    {
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.DOKill();

        // .SetUpdate(true) giúp Tween vẫn chạy khi Time.timeScale = 0
        canvasGroup.DOFade(1f, fadeDuration).SetUpdate(true).SetEase(Ease.OutQuad);
    }

    private void HidePanel()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.DOKill();

        canvasGroup.DOFade(0f, fadeDuration).SetUpdate(true).SetEase(Ease.InQuad);
    }

    // Hàm này dùng để gắn vào nút "Resume" (Tiếp tục) trên UI
    public void OnResumeButtonClicked()
    {
        // Yêu cầu GameManager chuyển trạng thái về Playing
        GameManager.Instance.ChangeState(GameState.Playing);
    }

    // ============== Các hàm xử lý sự kiện của Slider, Toggle, Button ==============
    private void OnMasterVolumeChanged(float value)
    {
        // SettingsPanelUI KHÔNG tự đổi volume, nó gọi AudioManager làm việc đó
        // AudioManager.Instance.SetMasterVolume(value);
    }

    private void OnMusicVolumeChanged(float value)
    {
        // AudioManager.Instance.SetMusicVolume(value);
    }

    private void OnSFXVolumeChanged(float value)
    {
        // AudioManager.Instance.SetSFXVolume(value);
    }

    private void OnFullscreenToggled(bool isFullscreen)
    {
        // Giao việc thay đổi màn hình cho Engine/GraphicsManager
        Screen.fullScreen = isFullscreen;
    }

    private void OnVFXToggled(bool isVFXOn)
    {
        // GraphicsManager.Instance.SetVFXEnabled(isVFXOn);
    }

    private void OnMainMenuButtonClicked()
    {
        // Yêu cầu GameManager chuyển về MainMenu
        GameManager.Instance.ChangeState(GameState.MainMenu);
    }

    private void LoadCurrentSettingsToUI()
    {
        // Cập nhật lại thanh trượt dựa trên dữ liệu đã lưu

        // masterVolumeSlider.value = AudioManager.Instance.GetMasterVolume();
        // musicVolumeSlider.value = AudioManager.Instance.GetMusicVolume();
        // sfxVolumeSlider.value = AudioManager.Instance.GetSFXVolume();
        // fullscreenToggle.isOn = Screen.fullScreen;
        // vfxToggle.isOn = GraphicsManager.Instance.IsVFXEnabled();

    }
}