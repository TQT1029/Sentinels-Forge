using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Game/Items/Consumable")]
public class ConsumableItemSO : BaseItemSO
{
    private void Awake()
    {
        itemType = ItemType.Consumable;
    }
}