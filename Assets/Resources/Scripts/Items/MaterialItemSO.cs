using UnityEngine;

[CreateAssetMenu(fileName = "New Material", menuName = "Game/Items/Material")]
public class MaterialItemSO : BaseItemSO
{
    // Cấu hình các đặc tính riêng của nguyên liệu nếu cần (VD: hệ số rèn)
    private void Awake()
    {
        itemType = ItemType.Material;
    }
}

