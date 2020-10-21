using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Weapons
{
    AssaultRiffle,
    HandGun
}

/// <summary>
/// Класс управления сменой оружия
/// </summary>
public class ChangeWeaponController : MonoBehaviour
{
    public static Weapons Weapon;

    /// <summary>
    /// Метод показа автомата при нажатии на кнопку
    /// </summary>
    public void ShowAssault()
    {
        Weapon = Weapons.AssaultRiffle;
    }

    /// <summary>
    /// Метод показа пистолета при нажатии на кнопку
    /// </summary>
    public void ShowHandleGun()
    {
        Weapon = Weapons.HandGun;
    }
}
