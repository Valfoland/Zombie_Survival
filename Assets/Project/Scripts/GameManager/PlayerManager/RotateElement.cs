using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateElement : MonoBehaviour
{
    private void Start()
    {
        FPSControllerLPFP.FpsControllerLPFP.onChangeRotate += SetRotate;
    }

    private void OnDestroy()
    {
        FPSControllerLPFP.FpsControllerLPFP.onChangeRotate -= SetRotate;
    }

    private void SetRotate(Transform transformPlayer)
    {
        gameObject.transform.rotation = transformPlayer.rotation;
    }

}
