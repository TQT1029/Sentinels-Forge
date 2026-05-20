using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct LootDropInfo
{
    public BaseItemSO item;
    [Range(0f, 100f)] public float dropChance; // Đổi sang tỷ lệ % cho dễ thiết kế (VD: 25.5%)
    [Min(1)] public int minAmount;
    [Min(1)] public int maxAmount;
}

[CreateAssetMenu(fileName = "New Loot Table", menuName = "Game/System/Loot Table")]
public class LootTableSO : ScriptableObject
{
    public List<LootDropInfo> drops;

    public List<InventorySlot> GetRandomDrops()
    {
        List<InventorySlot> result = new List<InventorySlot>();

        foreach (var drop in drops)
        {
            // Mỗi item tự quay xổ số 1 lần
            if (RandomUtils.ChancePercent(drop.dropChance))
            {
                int amount = Random.Range(drop.minAmount, drop.maxAmount + 1);
                if (amount > 0)
                {
                    result.Add(new InventorySlot(drop.item, amount));
                }
            }
        }
        return result; // Quái có thể rớt nhiều loại đồ cùng lúc!
    }
}