using UnityEngine;

public enum ItemType
{
    Material,     // Gỗ, Sắt, Lõi năng lượng
    Blueprint,    // Công thức chế tạo Modifier/Vũ khí
    Modifier,     // Item chứa logic Modifier gắn vào súng
    Consumable,   // Bom, máu (nếu có)
    Currency      // Vàng, Gem rớt ra từ quái
}

public enum ItemRarity
{
    Common = 0,
    Rare = 1,
    Epic = 2,
    Legendary = 3
}

public abstract class BaseItemSO : ScriptableObject
{
    [Header("Base Info")]
    public string itemID; // Định danh unique dùng cho Save/Load thay vì reference trực tiếp SO
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;

    [Header("Properties")]
    public ItemType itemType;
    public ItemRarity rarity;
    public int maxStackSize = 99;
    public int baseValue; // Giá trị cơ bản để tính toán shop/crafting

    public bool IsStackable => maxStackSize > 1;

    // Các class con có thể override để tự thực hiện logic riêng khi sử dụng
    public virtual void UseItem() { }
}