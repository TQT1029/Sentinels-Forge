using UnityEngine;
public struct HitData
{
    public Vector2 Point;
    public Vector2 Normal; // Vector pháp tuyến tại điểm va chạm
    public Collider2D Collider;
    public EnemyAI Enemy; // Có thể null nếu trúng tường

    public HitData(Vector2 point, Vector2 normal, EnemyAI enemy = null)
    {
        Point = point;
        Normal = normal;
        Enemy = enemy;

        Collider = null;
    }

    // Helper Constructor cho Raycast
    public HitData(RaycastHit2D hit, EnemyAI enemy = null)
    {
        Point = hit.point;
        Normal = hit.normal;
        Collider = hit.collider;
        Enemy = enemy;
    }

    // Helper Constructor cho Collision
    public HitData(Collision2D col, EnemyAI enemy = null)
    {
        Point = col.contacts[0].point;
        Normal = col.contacts[0].normal;
        Collider = col.collider;
        Enemy = enemy;
    }

}