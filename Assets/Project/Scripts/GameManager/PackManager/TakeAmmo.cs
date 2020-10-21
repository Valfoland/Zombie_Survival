using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeAmmo : MonoBehaviour, ITakePack
{
    public static System.Action onGetAmmo;

    private void OnEnable()
    {
        PlayerPosition.onGetPosition += GetLoot;
    }

    private void OnDisable()
    {
        PlayerPosition.onGetPosition -= GetLoot;
    }

    public void GetLoot(Transform transformPlayer)
    {
        if (Vector3.Distance(gameObject.transform.position, transformPlayer.position) <= 3f)
        {
            onGetAmmo?.Invoke();
            Destroy(gameObject);
        }
    }
}
