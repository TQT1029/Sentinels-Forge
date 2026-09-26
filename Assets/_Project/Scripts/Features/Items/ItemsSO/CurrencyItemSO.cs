using UnityEngine;

[CreateAssetMenu(fileName = "New Currency", menuName = "Game/Items/Currency")]
public class CurrencyItemSO : BaseItemSO
{
    private void Awake()
    {
        itemType = ItemType.Currency;
    }
}