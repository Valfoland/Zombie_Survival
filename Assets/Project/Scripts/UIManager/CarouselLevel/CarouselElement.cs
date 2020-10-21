using System;
using UnityEngine;

/// <summary>
/// Класс элементов карусели
/// </summary>
public class CarouselElement : MonoBehaviour, ICarouselElement
{
    private RectTransform rectTransform;

    public Vector2 Position => GetRectTransform().anchoredPosition;

    public Vector2 Size => GetRectTransform().sizeDelta;

    /// <summary>
    /// Метод определения позиции элемента карусели
    /// </summary>
    /// <param name="translation"></param>
    public void Translate(Vector2 translation)
    {
        var position = GetRectTransform().anchoredPosition;
        position += translation;
        GetRectTransform().anchoredPosition = position;
    }

    private RectTransform GetRectTransform()
    {
        if (rectTransform == null)
        {
            rectTransform = transform as RectTransform;
        }
        return rectTransform;
    }
}