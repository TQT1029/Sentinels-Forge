using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct LootDropInfo
{
    public BaseItemSO item;
    public int weight; // Trọng số, giá trị càng cao tỷ lệ rớt càng lớn
    public int minAmount;
    public int maxAmount;
}

[CreateAssetMenu(fileName = "New Loot Table", menuName = "Game/System/Loot Table")]
public class LootTableSO : ScriptableObject
{
    public List<LootDropInfo> drops;

    public List<InventorySlot> GetRandomDrops()
    {
        List<InventorySlot> result = new List<InventorySlot>();

        int totalWeight = 0;
        foreach (var drop in drops)
            totalWeight += drop.weight;

        int randomValue = Random.Range(0, totalWeight);
        int currentWeight = 0;

        foreach (var drop in drops)
        {
            currentWeight += drop.weight;
            if (randomValue < currentWeight)
            {
                int amount = Random.Range(drop.minAmount, drop.maxAmount + 1);
                if (amount > 0)
                {
                    result.Add(new InventorySlot(drop.item, amount));
                }
                break;
            }
        }
        return result;
    }
}