using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToPool : MonoBehaviour
{
    [SerializeField] private float timeToPoolreturn;
    private void Start()
    {
        Destroy(gameObject, timeToPoolreturn);
    }
}
