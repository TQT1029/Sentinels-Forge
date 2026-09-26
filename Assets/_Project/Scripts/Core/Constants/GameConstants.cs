using UnityEngine;

public static class GameConstants
{
    public static class Scenes
    {
        public const string MAIN_MENU = "MainMenu";
        public const string GAMEPLAY = "Playing-Wave";
        public const int INDEX_MAIN_MENU = 0;
        public const int INDEX_GAMEPLAY = 1;
    }

    public static class Tags
    {
        public const string PLAYER = "Player";
        public const string GROUND = "Ground";
        public const string WALL = "Wall";
        public const string TOWER = "Tower";
        public const string WEAPON = "Weapon";
        public const string FIRE_POINT = "FirePoint";
        public const string RANGED_ENEMY = "RangedEnemy";
        public const string MELEE_ENEMY = "MeleeEnemy";
        public const string FLYING_ENEMY = "FlyingEnemy";
        public const string SUPPORT_ENEMY = "SupportEnemy";
        public const string BOSS_ENEMY = "BossEnemy";
        public const string UI_ROOT = "UIRoot";
    }

    public static class LayerNames
    {
        public const string DEFAULT = "Default";
        public const string TRANSPARENT_FX = "TransparentFX";
        public const string IGNORE_RAYCAST = "Ignore Raycast";
        public const string TOWER = "Tower";
        public const string WATER = "Water";
        public const string UI = "UI";
        public const string ENEMY_HITBOX = "EnemyHitbox";
        public const string PROJECTILE = "Projectile";
        public const string ENEMY_BODY = "EnemyBody";
        public const string SPAWNER_ZONE = "SpawnerZone";
        public const string ENEMY_PROJECTILE = "EnemyProjectile";
        public const string BORDER = "Border";
        public const string ITEM = "Item";
    }

    public static class LayerIndices
    {
        public const int DEFAULT = 0;
        public const int TRANSPARENT_FX = 1;
        public const int IGNORE_RAYCAST = 2;
        public const int TOWER = 3;
        public const int WATER = 4;
        public const int UI = 5;
        public const int ENEMY_HITBOX = 6;
        public const int PROJECTILE = 7;
        public const int ENEMY_BODY = 8;
        public const int SPAWNER_ZONE = 9;
        public const int ENEMY_PROJECTILE = 10;
        public const int BORDER = 11;
        public const int ITEM = 12;
    }

    public static class LayerMasks
    {
        public const int DEFAULT = 1 << LayerIndices.DEFAULT;
        public const int TRANSPARENT_FX = 1 << LayerIndices.TRANSPARENT_FX;
        public const int IGNORE_RAYCAST = 1 << LayerIndices.IGNORE_RAYCAST;
        public const int TOWER = 1 << LayerIndices.TOWER;
        public const int WATER = 1 << LayerIndices.WATER;
        public const int UI = 1 << LayerIndices.UI;
        public const int ENEMY_HITBOX = 1 << LayerIndices.ENEMY_HITBOX;
        public const int PROJECTILE = 1 << LayerIndices.PROJECTILE;
        public const int ENEMY_BODY = 1 << LayerIndices.ENEMY_BODY;
        public const int SPAWNER_ZONE = 1 << LayerIndices.SPAWNER_ZONE;
        public const int ENEMY_PROJECTILE = 1 << LayerIndices.ENEMY_PROJECTILE;
        public const int BORDER = 1 << LayerIndices.BORDER;
        public const int ITEM = 1 << LayerIndices.ITEM;

        // COMBINED MASKS
        public const int PLAYER_PROJECTILE_TARGETS = ENEMY_HITBOX | BORDER | SPAWNER_ZONE;
        public const int ENEMY_PROJECTILE_TARGETS = TOWER | BORDER;
        public const int RADAR_TARGETS = ENEMY_HITBOX;
        public const int PLAYER_PROJECTILE_COLLISION_IGNORE = ENEMY_HITBOX | BORDER | TOWER;
    }

    public static class Config
    {
        public const float DEATH_PLANE_Y = -50f;
        public const float ENEMY_INVINCIBILITY_TIME = 0.15f;
        public const string PROJECTILE_STORAGE_NAME = "Projectiles Storage Obj";
        public const string ITEM_STORAGE_NAME = "Item Storage Obj";
        public const string ENEMIES_STORAGE_NAME = "Enemies Storage Obj";
    }
}