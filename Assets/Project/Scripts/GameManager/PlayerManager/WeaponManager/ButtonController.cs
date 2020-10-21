using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Класс управления кнопкой стрельбы
/// </summary>
public class ButtonController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public static System.Action onClickShotDown;
    public static System.Action onClickShotUp;

    /// <summary>
    /// Метод для вызова события при нажатии на кнопку стрелять
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        onClickShotDown?.Invoke();
    }

    /// <summary>
    /// Метод для вызова события при отпуска кнопки стрелять
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        onClickShotUp?.Invoke();
    }
}
