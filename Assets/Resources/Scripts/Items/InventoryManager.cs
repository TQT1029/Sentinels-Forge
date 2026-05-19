using System.Collections.Generic;

[System.Serializable]
public class InventorySlot
{
    public BaseItemSO itemData;
    public int amount;

    public InventorySlot(BaseItemSO data, int quantity)
    {
        itemData = data;
        amount = quantity;
    }
}

public class InventoryManager
{
    // Quản lý inventory theo Dictionary để lookup O(1) khi craft/check item
    // Key là itemID
    private Dictionary<string, InventorySlot> inventory = new Dictionary<string, InventorySlot>();

    public void AddItem(BaseItemSO item, int amount)
    {
        if (item == null || amount <= 0) return;

        if (inventory.TryGetValue(item.itemID, out InventorySlot slot))
        {
            slot.amount += amount;
            // Xử lý maxStackSize nếu cần thiết ở đây
        }
        else
        {
            inventory.Add(item.itemID, new InventorySlot(item, amount));
        }

        // Trigger event cập nhật UI ở đây
    }

    public bool HasItem(string itemID, int requiredAmount)
    {
        return inventory.TryGetValue(itemID, out InventorySlot slot) && slot.amount >= requiredAmount;
    }

    public void RemoveItem(string itemID, int amount)
    {
        if (HasItem(itemID, amount))
        {
            inventory[itemID].amount -= amount;
            if (inventory[itemID].amount <= 0)
            {
                inventory.Remove(itemID);
            }
        }
    }
}