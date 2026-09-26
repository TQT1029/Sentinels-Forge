using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Recipe Pattern", menuName = "Game/Crafting Recipe/Recipe Pattern")]
public class RecipePatternSO : ScriptableObject
{
    [Header("Recipe Info")]
    public string recipeName;
    [TextArea] public string sourceDescription;
    [Tooltip("Nếu True: Luôn hiện trong lò rèn. Nếu False: Phải dùng Blueprint để unlock")]
    public bool isUnlockedByDefault = true;

    [Header("Required Materials")]
    public List<InventorySlot> requiredItems;

    [Header("Crafting Result")]
    public InventorySlot resultItem;
}