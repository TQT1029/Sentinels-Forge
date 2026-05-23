using UnityEngine;

public class GameConstants : PersistentSingleton<GameConstants>
{
    [Header("SCENE NAMES")]
    public const string SCENE_MAIN_MENU = "MainMenu";
    public const string SCENE_GAMEPLAY = "Playing-Wave";

    [Header("SCENES INDEX")]
    public const int INDEX_MAIN_MENU = 0;
    public const int INDEX_GAMEPLAY = 1;

    [Header("TAGS")]
    public const string PLAYER_TAG = "Player";
    public const string GROUND_TAG = "Ground";
    public const string WALL_TAG = "Wall";
    public const string TOWER_TAG = "Tower";
    public const string WEAPON_TAG = "Weapon";
    public const string FIRE_POINT_TAG = "FirePoint";
    public const string RANGED_ENEMY_TAG = "RangedEnemy";
    public const string MELEE_ENEMY_TAG = "MeleeEnemy";
    public const string FLYING_ENEMY_TAG = "FlyingEnemy";
    public const string SUPPORT_ENEMY_TAG = "SupportEnemy";
    public const string BOSS_ENEMY_TAG = "BossEnemy";
    public const string UI_ROOT_TAG = "UIRoot";

    [Header("LAYER NAMES")]
    public const string LAYER_DEFAULT = "Default";
    public const string LAYER_TRANSPARENT_FX = "TransparentFX";
    public const string LAYER_IGNORE_RAYCAST = "Ignore Raycast";
    public const string LAYER_TOWER = "Tower";
    public const string LAYER_WATER = "Water";
    public const string LAYER_UI = "UI";
    public const string LAYER_ENEMY_HITBOX = "EnemyHitbox";
    public const string LAYER_PROJECTILE = "Projectile";
    public const string LAYER_ENEMY_BODY= "EnemyBody";
    public const string LAYER_SPAWNER_ZONE = "SpawnerZone";
    public const string LAYER_ENEMY_PROJECTILE = "EnemyProjectile";
    public const string LAYER_BORDER = "Border";
    public const string LAYER_ITEM = "Item";

    [Header("LAYER INDICES")]
    public const int INDEX_DEFAULT_LAYER = 0;
    public const int INDEX_TRANSPARENT_FX_LAYER = 1;
    public const int INDEX_IGNORE_RAYCAST_LAYER = 2;
    public const int INDEX_TOWER_LAYER = 3;
    public const int INDEX_WATER_LAYER = 4;
    public const int INDEX_UI_LAYER = 5;
    public const int INDEX_ENEMY_HITBOX_LAYER = 6;
    public const int INDEX_PROJECTILE_LAYER = 7;
    public const int INDEX_ENEMY_BODY_LAYER = 8;
    public const int INDEX_SPAWNER_ZONE_LAYER = 9;
    public const int INDEX_ENEMY_PROJECTILE_LAYER = 10;
    public const int INDEX_BORDER_LAYER = 11;
    public const int INDEX_ITEM_LAYER = 12;

    [Header("LAYER BITMASKS")]
    public const int MASK_DEFAULT = 1 << INDEX_DEFAULT_LAYER;
    public const int MASK_TRANSPARENT_FX = 1 << INDEX_TRANSPARENT_FX_LAYER;
    public const int MASK_IGNORE_RAYCAST = 1 << INDEX_IGNORE_RAYCAST_LAYER;
    public const int MASK_TOWER = 1 << INDEX_TOWER_LAYER;
    public const int MASK_WATER = 1 << INDEX_WATER_LAYER;
    public const int MASK_UI = 1 << INDEX_UI_LAYER;
    public const int MASK_ENEMY_HITBOX = 1 << INDEX_ENEMY_HITBOX_LAYER;
    public const int MASK_PROJECTILE = 1 << INDEX_PROJECTILE_LAYER;
    public const int MASK_ENEMY_BODY = 1 << INDEX_ENEMY_BODY_LAYER;
    public const int MASK_SPAWNER_ZONE = 1 << INDEX_SPAWNER_ZONE_LAYER;
    public const int MASK_ENEMY_PROJECTILE = 1 << INDEX_ENEMY_PROJECTILE_LAYER;
    public const int MASK_BORDER = 1 << INDEX_BORDER_LAYER;
    public const int MASK_ITEM = 1 << INDEX_ITEM_LAYER;

    // --- CÁC MASK KẾT HỢP (COMBINED MASKS) ---

    // Dành cho Đạn của Player: Chỉ xét va chạm với Quái, Border và SpawnerZone
    public const int MASK_PLAYER_PROJECTILE_TARGETS = MASK_ENEMY_HITBOX | MASK_BORDER | MASK_SPAWNER_ZONE;

    // Dành cho Đạn của Enemy: Chỉ xét va chạm với Tower (Base), Border
    public const int MASK_ENEMY_PROJECTILE_TARGETS = MASK_TOWER | MASK_BORDER;

    // Dành cho Homing Modifier: Tầm nhìn radar chỉ quét các đối tượng là Enemy
    public const int MASK_RADAR_TARGETS = MASK_ENEMY_HITBOX;

    public const int MASK_PLAYER_PROJECTILE_COLLISION_IGNORE = MASK_ENEMY_HITBOX | MASK_BORDER | MASK_TOWER;
}