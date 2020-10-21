using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickRotate : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] private GameObject sd;
    PointerEventData pointerData;
    public float Xaxis;
    private float xAxis;
    private int clickCount;

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            if (eventData.pointerCurrentRaycast.gameObject.name == sd.name)
            {
                Xaxis = xAxis;
            }
            else
            {
                Xaxis = 0;
            }
            pointerData = eventData;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Xaxis = 0;
        clickCount = 0;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Xaxis = 0;
        clickCount++;
    }

    private void Update()
    {
        xAxis = Input.GetAxis("Mouse X");
        if (pointerData != null)
        {
            if (!pointerData.IsPointerMoving())
            {
                Xaxis = 0;
            }
        }

    }

}