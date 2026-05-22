using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [field:SerializeField] public Image globalTowerHealthBar { get; private set; }
}
