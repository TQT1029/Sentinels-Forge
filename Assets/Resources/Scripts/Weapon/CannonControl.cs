using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CannonControl : WeaponControl
{
    private float angleAfterVibration;
    private float launchVelocityAfterVibration;


    protected override void VibrateWeapon()
    {
        base.VibrateWeapon();

        angleAfterVibration = currentBaseAngle + weaponData.GetVibrationAngle();

        launchVelocityAfterVibration = launchBaseVelocity + weaponData.GetVibrationLaunchVelocity();

        Debug.DrawRay(transform.position, new Vector3(Mathf.Cos(angleAfterVibration * Mathf.Deg2Rad), Mathf.Sin(angleAfterVibration * Mathf.Deg2Rad), 0) * launchVelocityAfterVibration, Color.red);
    }

}
