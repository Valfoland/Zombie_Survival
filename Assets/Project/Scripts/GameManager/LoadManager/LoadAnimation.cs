using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LoadAnimation : MonoBehaviour
{
    private float leftTime;
    [SerializeField] private Text textLoad;
    [SerializeField] private float targetRate = 0.3f;

    private void Update()
    {
        if (Time.time - leftTime > targetRate)
        {
            textLoad.text = ".";
        }
        if (Time.time - leftTime > targetRate * 2)
        {
            textLoad.text = "..";
        }
        if (Time.time - leftTime > targetRate * 3)
        {
            textLoad.text = "...";
            leftTime = Time.time;
        }

    }
}