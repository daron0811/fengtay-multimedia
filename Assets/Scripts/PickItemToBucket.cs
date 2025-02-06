using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PickItemToBucket : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image foodImage;
    private RectTransform _rectTransform;
    private Vector2 _originalPosition;
    public RectTransform bucket;

    public event Action onDragItem;

    private void Awake()
    {
        foodImage = GetComponent<Image>();
        _rectTransform = GetComponent<RectTransform>();
        _originalPosition = _rectTransform.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // // _tectTransform.position = eventData.position;
        // if (eventData.pointerDrag.transform.parent == foodGroup)
        // {
        //     _rectTransform = eventData.pointerDrag.transform.GetComponent<RectTransform>();
        //     _originalPosition = _rectTransform.position;
        // }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_rectTransform == null)
        {
            return;
        }
        _rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_rectTransform == null)
        {
            return;
        }

        if (RectTransformUtility.RectangleContainsScreenPoint(bucket, eventData.position, eventData.pressEventCamera))
        {
            onDragItem?.Invoke();
            // Debug.LogWarning(_rectTransform.name + " dropped on bucket");
            // string fruitName = _rectTransform.name.Split('-')[1];
            // Stage2Panel.Instance.OnTriggerFoodItem(fruitName);
        }
        _rectTransform.position = _originalPosition;
        //_rectTransform = null;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // _rectTransform.position = eventData.position;
    }


    //     private Vector3 originalPosition;
    //     public Image imageA;
    //     public Image imageB;
    //     public Transform foodGroup;

    //     void Start()
    //     {
    //         // 初始化时不需要设置originalPosition
    //     }

    //     void Update()
    //     {
    // #if UNITY_EDITOR || UNITY_STANDALONE
    //         if (Input.GetMouseButtonDown(0))
    //         {
    //             PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
    //             {
    //                 position = Input.mousePosition
    //             };

    //             List<RaycastResult> results = new List<RaycastResult>();
    //             EventSystem.current.RaycastAll(pointerEventData, results);

    //             foreach (var result in results)
    //             {
    //                 if (result.gameObject.transform.parent == foodGroup)
    //                 {
    //                     originalPosition = result.gameObject.GetComponent<RectTransform>().anchoredPosition;
    //                     imageA = result.gameObject.GetComponent<Image>();
    //                 }
    //             }
    //         }
    // #elif UNITY_IOS || UNITY_ANDROID
    //         for (int i = 0; i < Input.touchCount; i++)
    //         {
    //             Touch touch = Input.GetTouch(i);
    //             if (touch.phase == TouchPhase.Began)
    //             {
    //                 PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
    //                 {
    //                     position = touch.position
    //                 };

    //                 List<RaycastResult> results = new List<RaycastResult>();
    //                 EventSystem.current.RaycastAll(pointerEventData, results);

    //                 foreach (var result in results)
    //                 {
    //                     if (result.gameObject.transform.parent == foodGroup)
    //                     {
    //                         originalPosition = result.gameObject.GetComponent<RectTransform>().anchoredPosition;
    //                         imageA = result.gameObject.GetComponent<Image>();
    //                     }
    //                 }
    //             }
    //         }
    // #endif
    //     }

    //     public void OnPointerDown(PointerEventData pointerEventData)
    //     {
    //         if (pointerEventData.pointerCurrentRaycast.gameObject.transform.parent == foodGroup)
    //         {
    //             imageA = pointerEventData.pointerCurrentRaycast.gameObject.GetComponent<Image>();
    //             originalPosition = imageA.rectTransform.anchoredPosition;
    //         }
    //     }

    //     private RectTransform _tectTransform;
    //     private Vector2 _originalPosition;

    //     //拖曳狀態開始
    //     public void OnBeginDrag(PointerEventData eventData)
    //     {
    //         if (imageA == null)
    //         {
    //             return;
    //         }

    //         if (RectTransformUtility.RectangleContainsScreenPoint(imageA.rectTransform, eventData.position, eventData.pressEventCamera))
    //         {
    //             originalPosition = imageA.rectTransform.anchoredPosition;
    //         }
    //     }

    //     public void OnDrag(PointerEventData eventData)
    //     {
    //         if (imageA == null)
    //         {
    //             return;
    //         }
    //         Vector2 localPoint;
    //         if (RectTransformUtility.ScreenPointToLocalPointInRectangle(imageA.rectTransform.parent as RectTransform, eventData.position, eventData.pressEventCamera, out localPoint))
    //         {
    //             imageA.rectTransform.anchoredPosition = localPoint;
    //         }
    //     }

    //     public void OnEndDrag(PointerEventData eventData)
    //     {
    //         if (imageA == null)
    //         {
    //             return;
    //         }
    //         if (RectTransformUtility.RectangleContainsScreenPoint(imageB.rectTransform, eventData.position, eventData.pressEventCamera))
    //         {
    //             Debug.Log("Image A dropped on Image B");
    //         }
    //         imageA.rectTransform.anchoredPosition = originalPosition;
    //         imageA = null;
    //     }
}
