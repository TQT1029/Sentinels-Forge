using UnityEngine;

public class ReferenceManager : Singleton<ReferenceManager>
{
    [Header("Game References")]
    public Camera MainCamera { get; private set; }

    public Transform TowerTransform { get; private set; }

    public Bounds TowerBounds { get; private set; }

    public TowerController TowerController { get; private set; }

    [Header("UI References")]
    public Canvas UIRoot { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        MainCamera = Camera.main;
        
        TowerTransform = GameObject.FindGameObjectWithTag(GameConstants.TOWER_TAG).transform;
        TowerBounds = TowerTransform.GetComponent<Collider2D>().bounds;
        TowerController = TowerTransform.GetComponent<TowerController>();

        UIRoot = GameObject.FindGameObjectWithTag(GameConstants.UI_ROOT_TAG).GetComponent<Canvas>();

    }
}
