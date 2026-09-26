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
    // Quản lý inventory theo List để hỗ trợ nhiều slot của cùng 1 loại item (stack giới hạn)
    private List<InventorySlot> inventory = new List<InventorySlot>();
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
            foreach (var slot in inventory)
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

        // Điền vào các slot đang có sẵn mà chưa đầy
        foreach(var slot in inventory)
        {
            if (slot.itemData.itemID == item.itemID && slot.amount < item.maxStackSize)
            {
                int space = item.maxStackSize - slot.amount;
                if (amount <= space)
                {
                    slot.amount += amount;
                    amount = 0;
                    break;
                }
                else
                {
                    slot.amount = item.maxStackSize;
                    amount -= space;
                }
            }
        }

        // Nếu vẫn còn dư amount, tạo slot mới
        while (amount > 0)
        {
            int toAdd = Mathf.Min(amount, item.maxStackSize);
            inventory.Add(new InventorySlot(item, toAdd));
            amount -= toAdd;
        }

        // Bắn sự kiện thông báo cho UI và hệ thống Crafting cập nhật
        OnInventoryChanged?.Invoke();
    }

    public bool HasItem(string itemID, int requiredAmount)
    {
        int total = 0;
        foreach(var slot in inventory)
        {
            if (slot.itemData.itemID == itemID) total += slot.amount;
        }
        return total >= requiredAmount;
    }

    public void RemoveItem(string itemID, int amount)
    {
        if (!HasItem(itemID, amount)) return;

        for (int i = inventory.Count - 1; i >= 0; i--)
        {
            var slot = inventory[i];
            if (slot.itemData.itemID == itemID)
            {
                if (slot.amount >= amount)
                {
                    slot.amount -= amount;
                    amount = 0;
                    if (slot.amount <= 0) inventory.RemoveAt(i);
                    break;
                }
                else
                {
                    amount -= slot.amount;
                    inventory.RemoveAt(i);
                }
            }
        }

        OnInventoryChanged?.Invoke();
    }

    // Trả về danh sách để UI hiển thị
    public IReadOnlyList<InventorySlot> GetAllItems() => inventory;
}