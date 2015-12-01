using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

/*
 * @author 戴佳霖
 * 
 */
public class LoopBaseNode : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    private Transform m_trans;
    public Transform localTrans
    {
        get
        {
            if (!m_trans)
                m_trans = transform;
            return m_trans;
        }
    }

    public LoopScrollView loopScrollView
    {
        get
        {
            if (m_loopScrollView != null)
                return m_loopScrollView;
            else
                FindLoopScrollView(localTrans);
            return m_loopScrollView;
        }
    }
    private LoopScrollView m_loopScrollView;

    [HideInInspector]
    public int index;

    public object data;

    public virtual void OnDataChange()
    {
        if (loopScrollView.needItemEvent && loopScrollView.OnDataChange != null)
        {
            loopScrollView.OnDataChange(this);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (loopScrollView.needItemEvent && loopScrollView.OnItemClick != null)
        {
            loopScrollView.OnItemClick(this);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (loopScrollView.needItemEvent && loopScrollView.OnItemPress != null)
        {
            loopScrollView.OnItemPress(this, true);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (loopScrollView.needItemEvent && loopScrollView.OnItemPress != null)
        {
            loopScrollView.OnItemPress(this, false);
        }
    }

    private void FindLoopScrollView(Transform trans)
    {
        m_loopScrollView = trans.GetComponent<LoopScrollView>();
        if (m_loopScrollView == null)
        {
            trans = trans.parent;
            FindLoopScrollView(trans);
        }
    }

}