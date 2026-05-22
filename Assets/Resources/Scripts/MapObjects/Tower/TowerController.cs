using System;
using UnityEngine;

public class TowerController : MonoBehaviour, IHealth
{
    [SerializeField] protected TowerData towerData;
    protected float currentHealth;

    public float PercentHealth => currentHealth / towerData.maxHealth;
    public bool IsDestroyed => currentHealth <= 0f;

    public event Action<float> OnHealthChanged;
    public event Action OnTowerDestroyed;

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

    public virtual void TakeDamage(DamageInfo info)
    {
        if (PercentHealth > 0)
        {
            currentHealth -= info.damage;

            OnHealthChanged?.Invoke(PercentHealth);

            //Debug.Log($"[TowerController] Took {info.damage} damage. Remaining health: {currentHealth}");
        }

        if (IsDestroyed) OnDestroyTower();
    }
    public virtual void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, towerData.maxHealth);
        OnHealthChanged?.Invoke(PercentHealth);
    }

    public virtual void OnDestroyTower()
    {
        gameObject.SetActive(false);
        OnTowerDestroyed?.Invoke();
        Time.timeScale = 0f;
        //UnityEditor.EditorApplication.isPaused = true;
    }
}
