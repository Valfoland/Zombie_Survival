using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Класс управления оружием
/// </summary>
public class WeaponController : MonoBehaviour
{
    [SerializeField] private int countAmmoShop;
    [SerializeField] private AutomaticGun automaticGun;
    [SerializeField] private Text countOfBullets;
    [SerializeField] private Image[] frameWeapon;
    [SerializeField] private List<GameObject> weapons;

    public static System.Action<GameObject> onChangeWeapon;

    public static int CountAmmo;

    private void OnEnable()
    {
        AutomaticGun.onShot += SetTextBullet;
        TakeAmmo.onGetAmmo += GetShopAmmo;
    }

    private void OnDisable()
    {
        AutomaticGun.onShot -= SetTextBullet;
        TakeAmmo.onGetAmmo -= GetShopAmmo;
    }

    private void Start()
    {
        CountAmmo = automaticGun.TotalCountOfAmmo * automaticGun.CurrentAmmo;
        countOfBullets.text = CountAmmo.ToString();

        if (weapons.Count > 0)
        {
            onChangeWeapon?.Invoke(weapons[0]);
        }
        ShowFrameWeapon(0);
    }

    private void GetShopAmmo()
    {
        CountAmmo += countAmmoShop;
        countOfBullets.text = CountAmmo.ToString();
    }

    private void SetTextBullet()
    {
        CountAmmo--;
        countOfBullets.text = CountAmmo.ToString();
    }

    /// <summary>
    /// Метод срабатывающий при нажатии на кнопку показать оружие
    public void ShowGun(int idGun)
    {
        Mathf.Clamp(idGun, 0, weapons.Count);
        if (weapons.Count > 0)
        {
            onChangeWeapon?.Invoke(weapons[idGun]);
        }
        ShowFrameWeapon(idGun);
    }

    private void ShowFrameWeapon(int idGun)
    {
        for (int i = 0; i < frameWeapon.Length; i++)
        {
            if (i == idGun)
            {
                frameWeapon[i].enabled = true;
            }
            else
            {
                frameWeapon[i].enabled = false;
            }
        }
    }
}
