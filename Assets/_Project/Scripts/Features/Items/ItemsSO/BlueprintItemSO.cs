using UnityEngine;

[CreateAssetMenu(fileName = "New Blueprint", menuName = "Game/Items/Blueprint")]
public class BlueprintItemSO : BaseItemSO
{
    public RecipePatternSO targetRecipeToUnlock;

    private void Awake()
    {
        itemType = ItemType.Blueprint;
    }

    // Override hàm UseItem từ BaseItemSO của bạn
    public override void UseItem()
    {
        if (targetRecipeToUnlock != null)
        {
            CraftingManager.Instance.UnlockRecipe(targetRecipeToUnlock.recipeName);

            // Dùng xong thì tự xóa bản vẽ khỏi túi đồ (1 Blueprint tiêu hao)
            InventoryManager.Instance.RemoveItem(this.itemID, 1);
        }
    }
}

