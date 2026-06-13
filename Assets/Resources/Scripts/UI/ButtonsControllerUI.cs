using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ButtonsControllerUI : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 0.3f;

    [Space,Header("Settings Button")]
    [SerializeField] private Button settingsButton;
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        canvasGroup.alpha = 1.0f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
    private void Start()
    {
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettingsButtonClicked);
    }

    private void OnSettingsButtonClicked()
    {
        // Gọi hàm mở bảng Setting trong GameManager
        GameManager.Instance.ChangeState(GameState.Paused);
    }


    private void ShowGroup()
    {
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.DOKill();

        // .SetUpdate(true) giúp Tween vẫn chạy khi Time.timeScale = 0
        canvasGroup.DOFade(1f, fadeDuration).SetUpdate(true).SetEase(Ease.OutQuad);
    }

    private void HideGroup()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.DOKill();

        canvasGroup.DOFade(0f, fadeDuration).SetUpdate(true).SetEase(Ease.InQuad);
    }

}
