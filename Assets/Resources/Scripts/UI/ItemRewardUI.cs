using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ItemRewardUI : MonoBehaviour
{

    private Image imgItem;
    private TextMeshProUGUI txtData; // Hiển thị tên Item và số lượng

    private void Awake()
    {
        imgItem = GetComponentInChildren<Image>();
        txtData = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetData(Sprite itemSprite, string itemName, int quantity)
    {
        imgItem.sprite = itemSprite;
        txtData.text = $"{itemName}: x{quantity}";
    }
}
