using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAutoAim : MonoBehaviour
{
    VisibilityCompute visibilityCompute;
    [SerializeField] private GameObject shotButton;
    [SerializeField] private Transform playerEyes;
    [SerializeField] private int lookRange;
    [SerializeField] private int countRays;
    [SerializeField] private float angleRay;
    private const string botTag = "Bot";

    public static System.Action<bool> onShot = delegate { };

    private bool hitObject = default;
    public bool HitObject
    {
        get
        {
            return hitObject;
        }

        set
        {
            if (hitObject != value)
            {
                onShot?.Invoke(value);
            }
            hitObject = value;
        }
    }


    private void Awake()
    {
        if (LevelSettings.IsAutoAIM == 1)
        {
            visibilityCompute = new VisibilityCompute(botTag, countRays, angleRay);
            shotButton.SetActive(false);
        }
    }

    private void Update()
    {
        if (LevelSettings.IsAutoAIM == 1)
        {
            if (visibilityCompute.RayToScan(playerEyes, lookRange))
            {
                HitObject = true;
            }
            else
            {
                HitObject = false;
            }
        }
    }
}
