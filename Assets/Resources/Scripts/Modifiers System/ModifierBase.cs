using UnityEngine;

public abstract class ModifierBase : ScriptableObject
{
    [Header("Base Settings")]
    public string modifierName;
    public string description;

    /// <summary>
    /// Chạy 1 lần khi đạn vừa được bắn ra khỏi nòng để thiết lập các chỉ số (State) ban đầu cho viên đạn.
    /// </summary>
    public virtual void OnLaunch(Projectile projectile) { }

    /// <summary>
    /// Cập nhật mỗi frame (Dùng cho Homing, bay lượn...)
    /// </summary>
    public virtual void OnUpdate(Projectile projectile) { }

    /// <summary>
    /// Xử lý va chạm. 
    /// TRUE = Đã xử lý và muốn giữ đạn sống (bỏ qua các Mod phía sau trong lần va chạm này).
    /// FALSE = Không can thiệp hoặc để đạn chết (nhường quyền cho Mod phía sau).
    /// </summary>
    public virtual bool OnHitEnemy(Projectile projectile, EnemyAI enemy) { return false; }

    public virtual bool OnEnvironmentHit(Projectile projectile, RaycastHit2D hit) { return false; }
}