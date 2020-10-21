using UnityEngine;
using System.Collections;

/// <summary>
/// Класс наследуемый от  WeaponController для контроля автомата
/// </summary>
public class AutomaticGun : Weapon
{
    [Header("Weapon Settings")]
    [Tooltip("How fast the weapon fires, higher value means faster rate of fire.")]
    public float FireRate;

    private bool isStop = true;
    private bool isReload;
    private float leftTime;

    [SerializeField] private int maxAutoGunDamage = 10;
    public static System.Action onShot;

    protected override void Awake()
    {
        ButtonController.onClickShotDown += Shot;
        ButtonController.onClickShotUp += ShotStop;
        PlayerAutoAim.onShot += AutoShot;
        TakeAmmo.onGetAmmo += GetShopAmmo;
        base.Awake();
    }

    protected override void OnDestroy()
    {
        ButtonController.onClickShotDown -= Shot;
        ButtonController.onClickShotUp -= ShotStop;
        PlayerAutoAim.onShot -= AutoShot;
        TakeAmmo.onGetAmmo -= GetShopAmmo;
        base.OnDestroy();
    }

    private void ShotStop()
    {
        isStop = true;
    }

    protected override void Shot()
    {
        isStop = false;
    }

    private void GetShopAmmo()
    {
        TotalCountOfAmmo++;
    }

    private void AutoShot(bool isShot)
    {
        if (isShot)
        {
            Shot();
        }
        else
        {
            ShotStop();
        }
    }

    protected override void Update()
    {
        base.Update();

        if (outOfAmmo && !isReload)
        {
            isReload = true;
            TotalCountOfAmmo--;
        }

        if (!outOfAmmo && !isReloading && !hasBeenHolstered && !isStop && TotalCountOfAmmo > 0)
        {
            TakeDamage = maxAutoGunDamage;
            isReload = false;
            if (Time.time - leftTime > 1 / FireRate)
            {
                leftTime = Time.time;

                onShot?.Invoke();
                base.Shot();
            }
        }
    }
}
