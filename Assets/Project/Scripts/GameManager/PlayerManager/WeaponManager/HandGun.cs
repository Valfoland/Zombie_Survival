using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Класс наследуемый от WeaponController для управления пистолетом
/// </summary>
public class HandGun : Weapon
{
    [SerializeField] private int maxHandGunDamage = 3;
    [SerializeField] private float delayShotGun = 0.3f;
    private WaitForSeconds waitForSeconds;
    protected override void Awake()
    {
        ButtonController.onClickShotDown += Shot;
        PlayerAutoAim.onShot += AutoShot;
        base.Awake();
    }
    protected override void OnDestroy()
    {
        ButtonController.onClickShotDown -= Shot;
        PlayerAutoAim.onShot -= AutoShot;
        base.OnDestroy();
    }

    protected override void Start()
    {
        waitForSeconds = new WaitForSeconds(delayShotGun);
        base.Start();
    }

    private void AutoShot(bool isShot)
    {
        if (isShot)
        {
            StartCoroutine(AutoShotDoing());
        }
        else
        {
            StopAllCoroutines();
        }
    }

    private IEnumerator AutoShotDoing()
    {
        while (true)
        {
            Shot();
            yield return waitForSeconds;
        }
    }

    protected override void Shot()
    {
        if (!outOfAmmo && !isReloading && !hasBeenHolstered)
        {
            TakeDamage = maxHandGunDamage;
            base.Shot();
        }
    }
}