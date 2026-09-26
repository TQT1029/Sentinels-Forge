using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class HUDPanelController : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 0.2f;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        // Hiển thị HUD khi mới vào game
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
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

    private void HandleGameStateChanged(GameState state)
    {
        if (state == GameState.Paused || state == GameState.GameOver)
        {
            HidePanel();
        }
        else if (state == GameState.Playing)
        {
            ShowPanel();
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

}
