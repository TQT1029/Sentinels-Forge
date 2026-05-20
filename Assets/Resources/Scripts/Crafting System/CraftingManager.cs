using System;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : PersistentSingleton<CraftingManager>
{
    // Kho chứa TẤT CẢ công thức trong game
    private Dictionary<string, RecipePatternSO> allRecipes = new Dictionary<string, RecipePatternSO>();

    // Kho chứa các công thức ĐÃ ĐƯỢC MỞ KHÓA (nhờ nhặt Blueprint)
    private HashSet<string> unlockedRecipes = new HashSet<string>();

    // Events để UI Canvas lắng nghe và hiển thị thông báo
    public event Action<RecipePatternSO> OnCraftSuccess; 
    public event Action<string> OnCraftFailed; 
    protected override void Awake() 
    {
        base.Awake();

        LoadAllRecipes(Resources.LoadAll<RecipePatternSO>("Data/ScriptableObjects/Recipes"));
    }

    private void LoadAllRecipes(RecipePatternSO[] recipes)
    {
        foreach (var recipe in recipes)
        {
            if (!allRecipes.ContainsKey(recipe.recipeName))
            {
                allRecipes.Add(recipe.recipeName, recipe);

                // Nếu công thức có sẵn, cho luôn vào danh sách đã Unlock
                if (recipe.isUnlockedByDefault)
                {
                    unlockedRecipes.Add(recipe.recipeName);
                }
            }
        }
    }
    // Dùng khi người chơi nhặt/sử dụng Blueprint
    public void UnlockRecipe(string recipeName)
    {
        if (allRecipes.ContainsKey(recipeName) && !unlockedRecipes.Contains(recipeName))
        {
            unlockedRecipes.Add(recipeName);
            Debug.Log($"[Crafting] Mở khóa công thức mới: {recipeName}");
        }
    }

    // Tách riêng hàm Check để UI có thể gọi (VD: Làm mờ nút Craft nếu thiếu đồ)
    public bool CanCraft(RecipePatternSO recipe)
    {
        if (recipe == null) return false;

        // 1. Kiểm tra xem đã mở khóa công thức chưa
        if (!unlockedRecipes.Contains(recipe.recipeName)) return false;

        // 2. Kiểm tra đủ nguyên liệu không (Tối ưu vòng lặp, Early Exit)
        foreach (var req in recipe.requiredItems)
        {
            if (!InventoryManager.Instance.HasItem(req.itemData.itemID, req.amount))
            {
                return false;
            }
        }
        return true;
    }

    // Hàm gọi khi bấm nút "Chế Tạo" trên UI
    public void CraftItem(string recipeName)
    {
        if (!allRecipes.TryGetValue(recipeName, out RecipePatternSO recipe))
        {
            OnCraftFailed?.Invoke("Không tìm thấy công thức này!");
            return;
        }

        if (!CanCraft(recipe))
        {
            OnCraftFailed?.Invoke("Chưa mở khóa hoặc không đủ nguyên liệu!");
            return;
        }

        // ==========================================
        // THỰC THI CHẾ TẠO (CONSUME & REWARD)
        // ==========================================

        // 1. Trừ nguyên liệu
        foreach (var req in recipe.requiredItems)
        {
            InventoryManager.Instance.RemoveItem(req.itemData.itemID, req.amount);
        }

        // 2. Thêm đồ mới vào túi
        InventoryManager.Instance.AddItem(recipe.resultItem.itemData, recipe.resultItem.amount);

        // 3. Thông báo thành công
        OnCraftSuccess?.Invoke(recipe);
        Debug.Log($"[Crafting] Chế tạo thành công: {recipe.resultItem.itemData.itemName} x{recipe.resultItem.amount}");
    }
}
