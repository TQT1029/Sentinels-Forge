using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("HUD")]
    [field: SerializeField] public GameObject hudPanel { get; private set; }
    [field:SerializeField] public Image globalTowerHealthBar { get; private set; }
}
