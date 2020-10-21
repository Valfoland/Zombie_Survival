using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPosition : MonoBehaviour
{
    public static System.Action<Transform> onGetPosition;

    private void Update()
    {
        DetectPosition();
    }
    private void DetectPosition()
    {
        onGetPosition?.Invoke(gameObject.transform);
    }
}
