using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    [field:Header("HUD")]
    [field: SerializeField] public GameObject globalHUDPanel { get; private set; }
    [field:SerializeField] public Image globalTowerHealthBar { get; private set; }
    [field:SerializeField] public TextMeshProUGUI globalTowerHealthText { get; private set; }
}
