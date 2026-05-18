using System.Collections.Generic;
using UnityEngine;

public static class EnemyPhysicsRegistry
{
    private static readonly Dictionary<Collider2D, EnemyCache> registry = new Dictionary<Collider2D, EnemyCache>(500);

    public struct EnemyCache
    {
        public EnemyAI AI;
        public Collider2D SolidCollider;
    }

    public static void Register(Collider2D hitboxCollider, EnemyAI ai, Collider2D solidCollider)
    {
        registry[hitboxCollider] = new EnemyCache { AI = ai, SolidCollider = solidCollider };
    }

    public static void Unregister(Collider2D hitboxCollider)
    {
        registry.Remove(hitboxCollider);
    }

    public static bool TryGet(Collider2D hitboxCollider, out EnemyCache cache)
    {
        return registry.TryGetValue(hitboxCollider, out cache);
    }
}