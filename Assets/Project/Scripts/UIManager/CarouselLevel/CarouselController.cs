using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Класс контроля карусели
/// </summary>
public class CarouselController : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
	#region FIELDS
	public static Action<int> OnChangeElement;

    [SerializeField] private GameObject[] buttonLvl;
	[SerializeField] private Text textNumberLevel;
	public static int NumberLvl;
	public float TweenSpeed = 1f;

	public Vector2 Size;
	public float Offset;

	private List<ICarouselElement> elements;
	private List<ICarouselElement> elementsIndex;

	private Vector2 dragPosition;
	private Vector2 screenSize;
	private float respawnDistance;

	private ICarouselElement currentElement;

	private bool isLockRespawn;
	private int minCountElements;
	private float deltaX;
	#endregion

	private void Start()
	{
        Initialization();
		UIManager.onChangePage += ChangePage;
	}

	#region INIT
	/// <summary>
	/// Инициализация элементов карусели
	/// </summary>
	public void Initialization()
	{
        NumberLvl = 1;
		screenSize.x = (GetComponentInParent<Canvas>().transform as RectTransform).sizeDelta.x;
		elements = new List<ICarouselElement>();

		foreach (RectTransform child in transform)
		{
			AddElement(child);
		}
		elementsIndex = new List<ICarouselElement>(elements);

		SetCurrentElement(elementsIndex[0]);
        buttonLvl[0].SetActive(false);
    }

	private void AddElement(RectTransform child)
	{
		var element = child.GetComponent<ICarouselElement>();
		if (element != null && child.gameObject.activeSelf)
		{
			elements.Add(element);
			child.sizeDelta = screenSize;
			if (elements.Count > 1)
			{
				Vector2 lastPosition = elements[elements.Count - 2].Position;
				Vector2 horizontalSize = Vector2.right * child.sizeDelta;
				Vector2 offset = Vector2.right * Offset;
				child.anchoredPosition = lastPosition + horizontalSize;
			}
			else
			{
				child.anchoredPosition = Vector2.zero;
			}

		}
	}

	#endregion

	private void ChangePage(bool isNext)
	{
		if (isNext)
		{
			OnNextLvlPack();
		}
		else
		{
			OnPrevLvlPack();
		}
	}

	private void OnNextLvlPack()
	{
		CheckCurrentElement(-0.1f);
		StopAllCoroutines();
		StartCoroutine(TweenToElement(currentElement));
	}

	private void OnPrevLvlPack()
	{
		CheckCurrentElement(0.1f);
		StopAllCoroutines();
		StartCoroutine(TweenToElement(currentElement));
	}

	#region DRAG
	/// <summary>
	/// Метод срабатывающий при старте перетаскивания панели
	/// </summary>
	/// <param name="eventData"></param>
	public void OnBeginDrag(PointerEventData eventData)
	{
		StopAllCoroutines();
		UpdateDragPosition();
	}

	/// <summary>
	/// Метод срабатывающий при перетаскивании панели 
	/// </summary>
	/// <param name="eventData"></param>
	public void OnDrag(PointerEventData eventData)
	{
		if (elements.Count > 1)
		{
			deltaX = Input.mousePosition.x - dragPosition.x;
			foreach (var element in elements)
			{
				element.Translate(Vector2.right * deltaX);
			}
			UpdateDragPosition();

		}
	}

	/// <summary>
	/// Метод срабатывающий при окончании перетаскивания панели
	/// </summary>
	/// <param name="eventData"></param>
	public void OnEndDrag(PointerEventData eventData)
	{
		CheckCurrentElement(deltaX);
		StartCoroutine(TweenToElement(currentElement));
	}

	#endregion

	private void CheckCurrentElement(float deltaX)
	{
		ICarouselElement centerElement = FindCenterElement(deltaX);
		if (centerElement != currentElement)
		{
			SetCurrentElement(centerElement);
		}
	}

	private void SetCurrentElement(ICarouselElement element)
	{
		for (int i = 0; i < elements.Count; i++)
		{
            buttonLvl[0].SetActive(true);
            buttonLvl[1].SetActive(true);
            if (NumberLvl == 1)
            {
                buttonLvl[0].SetActive(false);
            }
            else if (NumberLvl == 2)
            {
                buttonLvl[1].SetActive(false);
            }
                
            if (element == elementsIndex[i])
			{
				OnChangeElement?.Invoke(i);
			}
		}
		currentElement = element;
	}

	private void UpdateDragPosition() => dragPosition = Input.mousePosition;

	private IEnumerator TweenToElement(ICarouselElement target)
	{
		while (true)
		{
			Vector2 currentPosition = target.Position;
			Vector2 targetPosition = Vector2.Lerp(currentPosition, Vector2.zero, TweenSpeed * Time.deltaTime);
			foreach (var element in elements)
			{
				element.Translate(targetPosition - currentPosition);
			}

			if (Mathf.Abs(target.Position.x) < 0.01f)
			{
				yield break;
			}
			yield return null;
		}

	}

	private ICarouselElement FindCenterElement(float deltaX)
	{
		ICarouselElement centerElement = null;
		int k = 0;
		for (int i = 0; i < elements.Count; i++)
		{
			if (elements[i] == currentElement)
			{
				k = i + 1;

				if (deltaX > 0 && i - 1 >= 0)
				{
					centerElement = elements[i - 1];
					NumberLvl = (k - 1);
					textNumberLevel.text = NumberLvl.ToString();
				}
				else if (deltaX < 0 && i + 1 < elements.Count)
				{
					centerElement = elements[i + 1];
					NumberLvl = (k + 1);
					textNumberLevel.text = NumberLvl.ToString();
				}
				else
				{
					centerElement = currentElement;
				}
				break;
			}
        }
		return centerElement;
	}

	#region  OnDisable/OnDestroy

	private void OnDestroy()
	{
		UIManager.onChangePage -= ChangePage;
		StopAllCoroutines();
	}
	#endregion
}
