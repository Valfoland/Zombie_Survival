using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : Health
{
    [SerializeField] private Slider Slider;
    [SerializeField] private Image FillImage;
    [SerializeField] private Text healthTextPlayer;

    private WaitForSeconds waitTakeDamage;

    [Tooltip("Количество теряемого здоровья при сложном уровнем")]
    [SerializeField] private int countOfLeftHP;
    public static System.Action onDead;

    public static int HealthRepair;

    protected override void Start()
    {
        waitTakeDamage = new WaitForSeconds(1);
        AttackAction.OnAttack += TakeDamage;
        TakeMedKid.onGetMedKid += GetIncHP;
        healthTextPlayer.text = StartingHealth.ToString();

        if (LevelMode.IsHardMode)
        {
            StartCoroutine(TakeDamageHardMode());
        }

        base.Start();
    }

    private void OnDestroy()
    {
        AttackAction.OnAttack -= TakeDamage;
    }

    private IEnumerator TakeDamageHardMode()
    {
        while (true)
        {

            yield return waitTakeDamage;
            TakeDamage(countOfLeftHP);
        }
    }

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);
        if (currentHealth <= 0f && !Dead)
        {
            onDead?.Invoke();
        }
        if (currentHealth >= 0)
        {
            SetHealthUI();
        }
    }

    private void GetIncHP()
    {
        HealthRepair += (int)(StartingHealth - currentHealth);
        currentHealth = StartingHealth;
        SetHealthUI();
    }

    private void SetHealthUI()
    {
        Slider.value = currentHealth;
        healthTextPlayer.text = currentHealth.ToString();
    }

}
