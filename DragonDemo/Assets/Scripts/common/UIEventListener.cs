//引入aciton命名空间
using System;
using UnityEngine;
//引入ui事件命名空间
using UnityEngine.EventSystems;


/// <summary>
/// UI事件监听器
/// </summary>

public class UIEventListener : MonoBehaviour, 
    IPointerClickHandler,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerEnterHandler,
    IPointerExitHandler,
    IDragHandler,
    IBeginDragHandler,
    IEndDragHandler
{
    /// <summary>
    /// 点击回调函数
    /// </summary>
    public  Action<GameObject, PointerEventData> onClick;
    public  Action<GameObject, PointerEventData> onDown;
    public  Action<GameObject, PointerEventData> onUp;

    public  Action<GameObject, PointerEventData> OnEnter;
    public  Action<GameObject, PointerEventData> onExit;

    /// <summary>
    /// 拖拽相关事件
    /// </summary>
    public Action<GameObject, PointerEventData> onBeginDragCallback;
    public Action<GameObject, PointerEventData> onDragCallback;
    public Action<GameObject, PointerEventData> onEndDragCallback;

    public static UIEventListener Get(GameObject obj)
    {
        UIEventListener uIEnvent = obj.GetComponent<UIEventListener>();
        if (uIEnvent == null)
        {
            uIEnvent = obj.AddComponent<UIEventListener>();
        }
        return uIEnvent;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (onClick != null)
        {
            onClick(gameObject, eventData);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        onDown?.Invoke(gameObject, eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onUp?.Invoke(gameObject, eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnEnter?.Invoke(gameObject, eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
         onExit?.Invoke(gameObject, eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
       onDragCallback?.Invoke(gameObject, eventData);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        onBeginDragCallback?.Invoke(gameObject, eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        
        onEndDragCallback?.Invoke(gameObject, eventData);
    }
}

