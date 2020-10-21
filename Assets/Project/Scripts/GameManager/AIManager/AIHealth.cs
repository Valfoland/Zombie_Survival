using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIHealth : Health
{
    private WaitForSeconds waitForDestroy;

    [SerializeField] private Transform slider;
    [SerializeField] private Transform backgroundSlider;
    [SerializeField] private SpriteRenderer fillSliderColor;
    [SerializeField] private Transform fillSlider;
    [SerializeField] private int idTextHealthZombie;

    private const byte COEFF_COLOR = 25;

    private float ColorR;
    private float ColorG;

    public static int CountDamage;

    protected override void Start()
    {
        //healthTextAI.text = StartingHealth.ToString();

        ColorR = (byte)(fillSliderColor.color.r * 255);
        ColorG = (byte)(fillSliderColor.color.g * 255);

        base.Start();
        waitForDestroy = new WaitForSeconds(0.3f);
    }

    private void Update()
    {
        if (slider != null)
        {
            slider.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, slider.transform.localPosition.y, gameObject.transform.localPosition.z);
        }
    }

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);
        if (currentHealth <= 0f && !Dead)
        {
            OnDeath();
        }
        if (currentHealth >= 0)
        {
            SetHealthUI(amount);
        }

        CountDamage += (int)amount;
    }

    private void OnDeath()
    {
        Dead = true;
    }

    private void SetHealthUI(float amount)
    {
        Debug.Log(amount);

        fillSlider.transform.localPosition -= new Vector3((amount * 0.66f) / StartingHealth, 0, 0);
        fillSlider.transform.localScale -= new Vector3((amount * 1) / StartingHealth, 0, 0);

        UnityNightPool.PoolObject poolTextHealthZombie = UnityNightPool.PoolManager.Get(idTextHealthZombie);
        poolTextHealthZombie.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 0.5f, gameObject.transform.position.z);

        if (ColorR < 255 - (255 * amount) / StartingHealth)
        {
            ColorR += (255 * amount) / StartingHealth;
        }
        if (ColorR > 0 + (255 * amount) / StartingHealth)
        {
            ColorG -= (255 * amount) / StartingHealth;
        }

        Debug.Log(ColorR + " COLOR_R " + ColorG + " COLOR_G");
        fillSliderColor.color = new Color32((byte)ColorR, (byte)ColorG, 0, 255);

        foreach (Transform child in poolTextHealthZombie.transform)
        {
            child.GetComponent<TextMesh>().text = "-" + amount.ToString();
        }
    }
}
