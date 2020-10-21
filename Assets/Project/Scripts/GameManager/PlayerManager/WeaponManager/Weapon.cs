using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Класс спавна точек попадания от оружия
/// </summary>
[System.Serializable]
public class Spawnpoints
{
    [Header("Spawnpoints")]
    public Transform casingSpawnPoint;
    public Transform bulletSpawnPoint;
}

/// <summary>
/// Класс данных звуков от оружия
/// </summary>
[System.Serializable]
public class SoundClips
{
    public AudioClip shootSound;
    public AudioClip takeOutSound;
    public AudioClip holsterSound;
    public AudioClip reloadSoundOutOfAmmo;
}

/// <summary>
/// Класс данных префабов пуль
/// </summary>
[System.Serializable]
public class Prefabs
{
    [Header("Prefabs")]
    public Transform bulletPrefab;
    public Transform casingPrefab;
}

/// <summary>
/// Класс управления оружием
/// </summary>
public class Weapon : MonoBehaviour
{
    private const string RELOAD_OUT = "Reload Out Of Ammo";
    private const string HOLSTER = "Holster";
    private const string FIRE = "Fire";
    private const string WALK = "Walk";
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    [SerializeField]
    private int idPoolBullet;
    [SerializeField]
    private int idPoolCase;

    protected Animator anim;

    [Header("Data Base")]
    [SerializeField] private Prefabs prefabs;
    [SerializeField] private Spawnpoints spawnPoints;
    [SerializeField] protected SoundClips soundClips;

    [Header("JoyStick Move")]
    [SerializeField] private AxisController joystick;

    [Header("Gun Camera Options")]
    [Tooltip("How fast the camera field of view changes when aiming.")]
    [SerializeField] private float fovSpeed = 15.0f;
    [SerializeField] private Camera gunCamera;

    [Tooltip("Default value for camera field of view (40 is recommended).")]
    [SerializeField] private float defaultFov = 40.0f;
    [SerializeField] private float aimFov = 15.0f;

    [Header("Weapon Sway")]
    [Tooltip("Toggle weapon sway.")]
    [SerializeField] private bool weaponSway;
    [SerializeField] private float swayAmount = 0.02f;
    [SerializeField] private float maxSwayAmount = 0.06f;
    [SerializeField] private float swaySmoothValue = 4.0f;

    [Header("Weapon Settings")]
    [Tooltip("How much ammo the weapon should have.")]
    [SerializeField] private float autoReloadDelay;
    public int Ammo;
    public int CurrentAmmo;
    public int TotalCountOfAmmo;
    protected bool outOfAmmo;
    protected bool isReloading;
    protected bool hasBeenHolstered;
    protected bool holstered;

    [Header("Bullet Settings")]
    [Tooltip("How much force is applied to the bullet when shooting.")]
    [SerializeField] private float bulletForce = 400;

    [Header("Muzzleflash Settings")]
    [SerializeField] private bool randomMuzzleflash;
    private int randomMuzzleflashValue;
    private readonly int minRandomValue = 1;
    [Range(2, 25)]
    [SerializeField] private int maxRandomValue = 5;
    [SerializeField] private bool enableMuzzleflash = true;
    [SerializeField] private ParticleSystem muzzleParticles;
    [SerializeField] private ParticleSystem sparkParticles;
    [SerializeField] private bool enableSparks = true;
    [SerializeField] private int minSparkEmission = 1;
    [SerializeField] private int maxSparkEmission = 7;

    [Header("Muzzleflash Light Settings")]
    [SerializeField] private Light muzzleflashLight;
    [SerializeField] private float lightDuration = 0.02f;

    [Header("Audio Source")]
    [SerializeField] protected AudioSource mainAudioSource;
    [SerializeField] protected AudioSource shootAudioSource;

    public static int TakeDamage;

    protected virtual void OnDestroy()
    {
        WeaponController.onChangeWeapon -= WeaponShowHide;
    }

    protected virtual void Awake()
    {
        WeaponController.onChangeWeapon += WeaponShowHide;
        anim = GetComponent<Animator>();
        CurrentAmmo = Ammo;
        muzzleflashLight.enabled = false;
    }

    protected virtual void Start()
    {
        shootAudioSource.clip = soundClips.shootSound;
    }

    private void WeaponShowHide(GameObject weapon)
    {
        if (weapon == gameObject)
        {
            ShowGun();
        }
        else
        {
            HideGun();
        }
    }

    protected void ShowGun()
    {

        mainAudioSource.clip = soundClips.takeOutSound;
        mainAudioSource.Play();

        hasBeenHolstered = false;
        anim.SetBool(HOLSTER, false);
        gameObject.SetActive(true);
    }

    protected void HideGun()
    {
        mainAudioSource.clip = soundClips.holsterSound;
        mainAudioSource.Play();

        hasBeenHolstered = true;
        anim.SetBool(HOLSTER, true);
        gameObject.SetActive(false);
    }

    protected virtual void Shot()
    {
        CurrentAmmo--;
        shootAudioSource.clip = soundClips.shootSound;
        shootAudioSource.Play();
        anim.Play(FIRE, 0, 0f);
        if (!randomMuzzleflash && enableMuzzleflash == true)
        {
            muzzleParticles.Emit(1);
            StartCoroutine(MuzzleFlashLight());
        }
        else if (randomMuzzleflash)
        {
            if (randomMuzzleflashValue == 1)
            {
                if (enableSparks == true)
                {
                    sparkParticles.Emit(Random.Range(minSparkEmission, maxSparkEmission));
                }
                if (enableMuzzleflash == true)
                {
                    muzzleParticles.Emit(1);
                    StartCoroutine(MuzzleFlashLight());
                }
            }
        }
        UnityNightPool.PoolObject poolBullet = UnityNightPool.PoolManager.Get(idPoolBullet);
        poolBullet.transform.position = spawnPoints.bulletSpawnPoint.transform.position;
        poolBullet.transform.rotation = spawnPoints.bulletSpawnPoint.transform.rotation;
        poolBullet.GetComponent<Rigidbody>().velocity = poolBullet.transform.forward * bulletForce;

        UnityNightPool.PoolObject poolCaseBullet = UnityNightPool.PoolManager.Get(idPoolCase);
        poolCaseBullet.transform.position = spawnPoints.casingSpawnPoint.transform.position;
        poolCaseBullet.transform.rotation = spawnPoints.casingSpawnPoint.transform.rotation;

        if (CurrentAmmo <= 0)
        {
            outOfAmmo = true;
            if (!isReloading)
            {
                StartCoroutine(AutoReload());
            }
        }
    }

    protected virtual void Update()
    {
        AnimationCheck();

        if (randomMuzzleflash == true)
        {
            randomMuzzleflashValue = Random.Range(minRandomValue, maxRandomValue);
        }

        if (joystick.Vertical != 0 || joystick.Horizontal != 0 || Input.GetAxisRaw(HORIZONTAL) != 0 || Input.GetAxisRaw(VERTICAL) != 0)
        {
            anim.SetBool(WALK, true);
        }
        else
        {
            anim.SetBool(WALK, false);
        }
    }

    private IEnumerator AutoReload()
    {

        yield return new WaitForSeconds(autoReloadDelay);
        if (outOfAmmo == true)
        {
            anim.Play(RELOAD_OUT, 0, 0f);

            mainAudioSource.clip = soundClips.reloadSoundOutOfAmmo;
            mainAudioSource.Play();
        }
        CurrentAmmo = Ammo;
        outOfAmmo = false;
    }

    private IEnumerator MuzzleFlashLight()
    {
        muzzleflashLight.enabled = true;
        yield return new WaitForSeconds(lightDuration);
        muzzleflashLight.enabled = false;
    }

    private void AnimationCheck()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName(RELOAD_OUT))
        {
            isReloading = true;
        }
        else
        {
            isReloading = false;
        }
    }
}
