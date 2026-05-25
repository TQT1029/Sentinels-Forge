using DG.Tweening;
using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;

public struct RewardData
{
    public Sprite itemSprite;
    public string itemName;
    public int quantity;
    public RewardData(Sprite sprite, string name, int qty)
    {
        itemSprite = sprite;
        itemName = name;
        quantity = qty;
    }
}
[RequireComponent(typeof(CanvasGroup))]
public class RewardsManagerUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RewardsListUI rewardsListUI;

    [Header("Animation Settings")]
    [SerializeField] private float fadeDuration = 0.5f;

    private CanvasGroup canvasGroup;
    private Dictionary<BaseItemSO, int> pendingRewards = new Dictionary<BaseItemSO, int>();

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        // Ẩn UI ngay từ đầu
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    private void OnEnable()
    {
        RewardEvents.OnRewardCollected += HandleRewardCollected;
        RewardEvents.OnLevelEnding += ShowResultPanel;
    }

    private void OnDisable()
    {
        RewardEvents.OnRewardCollected -= HandleRewardCollected;
        RewardEvents.OnLevelEnding -= ShowResultPanel;
        canvasGroup.DOKill();
    }

    private void HandleRewardCollected(BaseItemSO item, int amount)
    {
        if (item == null) return;

        if (pendingRewards.ContainsKey(item))
            pendingRewards[item] += amount;
        else
            pendingRewards.Add(item, amount);
    }

    // Được trigger tự động khi Event OnLevelEnding phát ra
    private void ShowResultPanel()
    {
        ProcessAndDisplayRewards();

        // Mở UI bằng DOTween
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        canvasGroup.DOKill();
        canvasGroup.DOFade(1f, fadeDuration).SetUpdate(true).SetEase(Ease.OutQuad);

        // Dừng thời gian game (Tùy thuộc vào thiết kế Game Over của bạn)
         GameManager.Instance.ChangeState(GameState.Paused);
    }

    private void ProcessAndDisplayRewards()
    {
        List<RewardData> displayList = new List<RewardData>();

        foreach (var kvp in pendingRewards)
        {
            BaseItemSO item = kvp.Key;
            int totalAmount = kvp.Value;

            displayList.Add(new RewardData(item.icon, item.itemName, totalAmount));

            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.AddItem(item, totalAmount);
            }
        }

        if (rewardsListUI != null)
        {
            rewardsListUI.SetRewards(displayList);
        }
    }

    // Gắn vào nút "Continue" / "Back to Menu" trên Result Panel
    public void HideResultPanel()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        canvasGroup.DOKill();
        canvasGroup.DOFade(0f, fadeDuration).SetUpdate(true).SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                pendingRewards.Clear(); // Dọn dẹp kho tạm khi đóng bảng
                // SceneController.Instance.LoadScene(GameConstants.Scenes.MainMenu);
            });
    }
}