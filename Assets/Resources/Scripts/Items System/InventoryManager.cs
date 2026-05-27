using System;
using System.Collections.Generic;
using UnityEngine;

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

public class InventoryManager : PersistentSingleton<InventoryManager>
{
    // Quản lý inventory theo Dictionary để lookup O(1) khi craft/check item
    // Key là itemID
    private Dictionary<string, InventorySlot> inventory = new Dictionary<string, InventorySlot>();
    public event Action OnInventoryChanged; // Sự kiện thông báo cho UI cập nhật mỗi khi kho đồ thay đổi
    protected override void Awake()
    {
        base.Awake();
    }

    public void ShowInventory()
    {
        if (inventory != null && inventory.Count > 0)
        {
            Debug.Log("=== INVENTORY CONTENT ===");
            foreach (var slot in inventory.Values)
            {
                Debug.Log($"Item: {slot.itemData.itemName}, Amount: {slot.amount}");
            }
            Debug.Log("=========================");
        }
        else
        {
            Debug.Log("Inventory is empty or null!");
        }
    }
    public void AddItem(BaseItemSO item, int amount)
    {
        if (item == null || amount <= 0) return;

        if (inventory.TryGetValue(item.itemID, out InventorySlot slot))
        {
            // Xử lý giới hạn Stack tối đa của Item
            if (slot.amount + amount > item.maxStackSize)
            {
                // Nếu vượt quá, có thể xử lý tràn slot (tùy thuộc vào thiết kế UI sau này)
                slot.amount = item.maxStackSize;
            }
            else
            {
                slot.amount += amount;
            }
        }
        else
        {
            inventory.Add(item.itemID, new InventorySlot(item, amount));
        }

        // Bắn sự kiện thông báo cho UI và hệ thống Crafting cập nhật
        OnInventoryChanged?.Invoke();
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

            OnInventoryChanged?.Invoke();
        }
    }

    // Tiện ích dành cho UI lấy toàn bộ danh sách vật phẩm để vẽ lên màn hình
    public IReadOnlyDictionary<string, InventorySlot> GetAllItems() => inventory;
}