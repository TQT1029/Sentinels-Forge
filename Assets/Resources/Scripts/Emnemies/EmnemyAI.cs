using UnityEngine;

public class EmnemyAI : MonoBehaviour
{
    [field: SerializeField] public EmnemyData emnemyData { get; private set; }
    protected Rigidbody2D rb;
    protected Transform towerTransform;

    //protected float percentHealth => (float)emnemyData.CurrentHealth / emnemyData.MaxHealth;
    protected virtual void Awake()
    {
        towerTransform = WaveManager.Instance.TowerTransform;
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        if (IsInAttackRange())
        {
            Attack();

            rb.linearVelocity = new Vector2(0, rb.linearVelocityY);
        }
        else
        {
            Move();
        }
    }

    protected virtual void FixedUpdate()
    {

    }

    protected virtual void Move()
    {
        rb.linearVelocity = new Vector2(-emnemyData.moveSpeed, rb.linearVelocityY);
    }

    
    protected virtual bool IsInAttackRange()
    {
        Debug.LogWarning("Đang chưa hoạt động đúng cần chỉnh lại");

        Vector2 direction = towerTransform.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, emnemyData.attackRange);

        Debug.DrawRay(transform.position, direction.normalized * emnemyData.attackRange, Color.green, 0.1f);

        if (hit==default) return false;

        return true;

    }

    protected virtual void Attack()
    {
        Debug.Log($"[EmnemyAI] {emnemyData.emnemyName} Attacking tower!");
    }
}