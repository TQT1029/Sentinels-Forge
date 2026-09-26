using UnityEngine;

[CreateAssetMenu(fileName = "New Modifier Item", menuName = "Game/Items/Modifier Item")]
public class ModifierItemSO : BaseItemSO
{
    public BaseModifier modifierLogic;

    private void Awake()
    {
        itemType = ItemType.Modifier;
        maxStackSize = 1; // Thường modifier gắn súng không stack
    }
}

