using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeMedKid : MonoBehaviour, ITakePack
{
    public static System.Action onGetMedKid;
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
            onGetMedKid?.Invoke();
            Destroy(gameObject);
        }
    }
}
