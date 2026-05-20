using UnityEngine;

[CreateAssetMenu(fileName = "New Blueprint", menuName = "Game/Items/Blueprint")]
public class BlueprintItemSO : BaseItemSO
{
    public BaseItemSO resultItem; // Item sẽ tạo ra sau khi craft
    public int craftingTimeSeconds;

    // Dictionary không serialize được trực tiếp trong Inspector của Unity, 
    // thực tế nên dùng List struct lưu MaterialCost

    private void Awake()
    {
        itemType = ItemType.Blueprint;
    }
}

