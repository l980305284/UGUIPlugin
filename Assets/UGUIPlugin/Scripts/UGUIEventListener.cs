/*****************************************************
 * 作者: 刘靖
 * 创建时间：2015.n.n
 * 版本：1.0.0
 * 描述：UGUI的事件监听
 ****************************************************/

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class UGUIEventListener : UnityEngine.EventSystems.EventTrigger 
{
    public delegate void VoidDelegate(GameObject go, PointerEventData data);
    public VoidDelegate onClick;
    public VoidDelegate onEnter;
    public VoidDelegate onDown;
    public VoidDelegate onUp;
    public VoidDelegate onExit;
    public VoidDelegate onBeginDrag;
    public VoidDelegate onDrag;
    public VoidDelegate onEndDrag;
    public VoidDelegate onDrop;
    public VoidDelegate onScroll;

    static public UGUIEventListener Get(GameObject go)
    {
        UGUIEventListener listener = go.GetComponent<UGUIEventListener>();
        if(listener == null)
        {
            listener = go.AddComponent<UGUIEventListener>();
        }
        return listener;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (onClick != null) onClick(gameObject, eventData);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (onEnter != null) onEnter(gameObject, eventData);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (onDown != null) onDown(gameObject, eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (onUp != null) onUp(gameObject, eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (onExit != null) onExit(gameObject, eventData);
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (onBeginDrag != null) onBeginDrag(gameObject, eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (onDrag != null) onDrag(gameObject, eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (onEndDrag != null) onEndDrag(gameObject, eventData);
    }

    public override void OnDrop(PointerEventData eventData)
    {
        if (onDrop != null) onDrop(gameObject, eventData);
    }

    public override void OnScroll(PointerEventData eventData)
    {
        if (onScroll != null) onScroll(gameObject, eventData);
    }
}
