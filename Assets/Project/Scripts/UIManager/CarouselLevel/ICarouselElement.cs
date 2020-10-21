using System;
using UnityEngine;

/// <summary>
/// Интерфейс элмементов карусели
/// </summary>
public interface ICarouselElement
{
    Vector2 Position { get; }
    Vector2 Size { get; }
    void Translate(Vector2 translation);
}
