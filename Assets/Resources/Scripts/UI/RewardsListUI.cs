using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RewardsListUI : MonoBehaviour
{
    [SerializeField] private GameObject content; // Nơi chứa các ItemRewardUI
    [SerializeField] private GameObject itemRewardPrefab;
    
    public List<ItemRewardUI> itemsReward;

    public void SetRewards(List<RewardData> rewards)
    {
        // Xóa các phần tử cũ nếu có
        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);
        }
        itemsReward.Clear();
        // Tạo mới các phần tử dựa trên dữ liệu rewardsStorage
        foreach (var reward in rewards)
        {
            GameObject itemObj = Instantiate(itemRewardPrefab, content.transform);
            ItemRewardUI itemRewardUI = itemObj.GetComponent<ItemRewardUI>();

            itemRewardUI.SetData(reward.itemSprite, reward.itemName, reward.quantity);
            itemsReward.Add(itemRewardUI);
        }
    }



}
