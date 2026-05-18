using UnityEngine;

public class TowerController : MonoBehaviour
{
    [SerializeField] protected TowerData towerData;
    protected float currentHealth;

    public float PercentHealth => currentHealth / towerData.maxHealth;
    public bool IsDestroyed => currentHealth <= 0f;

    protected virtual void Awake()
    {
        if (towerData != null)
        {
            currentHealth = towerData.maxHealth;
        }
        else
        {
            Debug.LogError("TowerData is not assigned on " + gameObject.name);
        }
    }
    public virtual void TakeDamage(float amount)
    {
        currentHealth -= amount;
    }
    public virtual void Repair(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, towerData.maxHealth);
    }

    public virtual void OnTowerDestroy() { }
}
