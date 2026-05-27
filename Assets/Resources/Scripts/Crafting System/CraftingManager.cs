using System;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : PersistentSingleton<CraftingManager>
{
    // SO reference làm key — không còn phụ thuộc vào string khớp
    private Dictionary<RecipePatternSO, bool> _unlockedRecipes = new Dictionary<RecipePatternSO, bool>();

    // Giữ string lookup để tương thích với code gọi CraftItem(string) và UnlockRecipe(string)
    private Dictionary<string, RecipePatternSO> _recipesByName = new Dictionary<string, RecipePatternSO>();

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
            if (recipe == null) continue;

            if (_recipesByName.ContainsKey(recipe.recipeName))
            {
                Debug.LogWarning($"[CraftingManager] Trùng tên recipe: '{recipe.recipeName}'. Bỏ qua.");
                continue;
            }

            _recipesByName[recipe.recipeName] = recipe;
            _unlockedRecipes[recipe] = recipe.isUnlockedByDefault;
        }
    }

    // --- Public API dùng SO reference (ưu tiên dùng cái này) ---

    public void UnlockRecipe(RecipePatternSO recipe)
    {
        if (recipe == null || !_unlockedRecipes.ContainsKey(recipe)) return;
        if (_unlockedRecipes[recipe]) return;

        _unlockedRecipes[recipe] = true;
        Debug.Log($"[Crafting] Mở khóa công thức mới: {recipe.recipeName}");
    }

    public bool CanCraft(RecipePatternSO recipe)
    {
        if (recipe == null) return false;
        if (!_unlockedRecipes.TryGetValue(recipe, out bool unlocked) || !unlocked) return false;

        foreach (var req in recipe.requiredItems)
        {
            if (!InventoryManager.Instance.HasItem(req.itemData.itemID, req.amount))
                return false;
        }
        return true;
    }

    public void CraftItem(RecipePatternSO recipe)
    {
        if (!CanCraft(recipe))
        {
            OnCraftFailed?.Invoke("Chưa mở khóa hoặc không đủ nguyên liệu!");
            return;
        }

        foreach (var req in recipe.requiredItems)
            InventoryManager.Instance.RemoveItem(req.itemData.itemID, req.amount);

        InventoryManager.Instance.AddItem(recipe.resultItem.itemData, recipe.resultItem.amount);

        OnCraftSuccess?.Invoke(recipe);
        Debug.Log($"[Crafting] Chế tạo thành công: {recipe.resultItem.itemData.itemName} x{recipe.resultItem.amount}");
    }

    // --- String overloads giữ lại để không breaking change ---

    public void UnlockRecipe(string recipeName)
    {
        if (_recipesByName.TryGetValue(recipeName, out RecipePatternSO recipe))
            UnlockRecipe(recipe);
        else
            Debug.LogWarning($"[CraftingManager] Không tìm thấy recipe: '{recipeName}'");
    }

    public void CraftItem(string recipeName)
    {
        if (!_recipesByName.TryGetValue(recipeName, out RecipePatternSO recipe))
        {
            OnCraftFailed?.Invoke($"Không tìm thấy công thức: '{recipeName}'");
            Debug.LogWarning($"[CraftingManager] Không tìm thấy recipe: '{recipeName}'. Kiểm tra lại tên trong Inspector.");
            return;
        }
        CraftItem(recipe);
    }

    public bool IsUnlocked(RecipePatternSO recipe) =>
        recipe != null && _unlockedRecipes.TryGetValue(recipe, out bool unlocked) && unlocked;

    public IReadOnlyDictionary<RecipePatternSO, bool> GetAllRecipes() => _unlockedRecipes;
}