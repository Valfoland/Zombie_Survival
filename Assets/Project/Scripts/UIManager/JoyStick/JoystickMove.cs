using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Класс наследуемый от AxisController для поворота персонажа
/// </summary>
public class JoystickMove : AxisController
{
    [SerializeField] private Image[] imgJoyStickDirection;

    private void SetDirection()
    {
        if (Horizontal > 0)
        {
            imgJoyStickDirection[0].enabled = true;
            imgJoyStickDirection[1].enabled = false;
        }
        else if (Horizontal < 0)
        {
            imgJoyStickDirection[0].enabled = false;
            imgJoyStickDirection[1].enabled = true;
        }
        else
        {
            imgJoyStickDirection[0].enabled = false;
            imgJoyStickDirection[1].enabled = false;
        }

        if (Vertical > 0)
        {
            imgJoyStickDirection[2].enabled = true;
            imgJoyStickDirection[3].enabled = false;
        }
        else if (Vertical < 0)
        {
            imgJoyStickDirection[2].enabled = false;
            imgJoyStickDirection[3].enabled = true;
        }
        else
        {
            imgJoyStickDirection[2].enabled = false;
            imgJoyStickDirection[3].enabled = false;
        }
    }

    /// <summary>
    /// Переопределяемый метод класса JoyStick срабатывающий при нажатии на Джойстик
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnDrag(PointerEventData eventData)
    {
        SetDirection();
        HandleInput(input.magnitude, input.normalized);
        base.OnDrag(eventData);
    }
    /// <summary>
    /// Переопределяемый метод класса JoyStick срабатывающий при отпуске Джойстика
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerUp(PointerEventData eventData)
    {
        for (int i = 0; i < imgJoyStickDirection.Length; i++)
        {
            imgJoyStickDirection[i].enabled = false;
        }
        base.OnPointerUp(eventData);
    }
}