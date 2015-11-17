using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/*
 * @author 戴佳霖
 * 无限循环列表
 */
public class QuickScrollView : MonoBehaviour
{
    public delegate void UpdateItemDelegate(Transform item, int index);
    public enum Direction
    {
        Horizontal, Vertical,
    }
    public Direction m_movement = Direction.Vertical;
    public RectTransform m_itemParent;
    public GameObject m_item;
    public Scrollbar m_sb;

    public int m_itemWidth;
    public int m_itemHeight;

    public int m_fixedColumnCount;//列
    public int m_fixedColCount;//行

    private Vector2 m_allItemArea = Vector2.zero;
    private Vector2 m_showArea = Vector2.zero;

    private Vector2 m_firstItemPos = Vector2.zero;
    private Vector2 m_lastItemPos = Vector2.zero;

    private int m_listMaxLength;
    private int m_curShowStartIndex;
    private int m_curShowEndIndex;

    private UpdateItemDelegate m_updateItem;
    private float m_curSbVal;

    public void Init(int maxLength, UpdateItemDelegate updateItemDelegate)
    {
        m_showArea = GetComponent<RectTransform>().sizeDelta;
        m_item.SetActive(false);
        m_firstItemPos.y += m_itemHeight;
        m_curSbVal = m_sb.value;
        m_curShowEndIndex = m_fixedColCount;

        m_listMaxLength = maxLength;
        m_updateItem = updateItemDelegate;
        for (int i = 0; i < m_fixedColCount; i++)
        {
            Transform item = CreatItem(i);
            m_updateItem(item, i);
        }

    }

    private Transform CreatItem(int index)
    {
        Transform item =
          ((GameObject)GameObject.Instantiate(m_item)).transform;
        item.gameObject.SetActive(true);
        item.SetParent(m_itemParent);
        item.name = index.ToString();
        item.localScale = Vector3.one;
        item.localEulerAngles = Vector3.zero;
        // 1.行  
        int row = index / m_fixedColumnCount;
        // 2.列  
        int col = index % m_fixedColumnCount;
        Debug.Log(index + " " + row + " " + col);
        item.localPosition = new Vector3(col * m_itemWidth, -1 * row * m_itemHeight, 0f);

        m_allItemArea.y = (row + 1) * m_itemHeight;
        m_lastItemPos.y = -1 * (int)m_allItemArea.y;
        return item;
    }

    public void OnDragSlider(float val)
    {
        UpdateListByFloat(m_sb.value * (m_listMaxLength - m_fixedColCount));
    }
    // 通过一个浮点值滑动列表  
    public void UpdateListByFloat(float val)
    {
        UpdateListPos(val);
        if (val > m_curSbVal)
        {
            if (m_curShowEndIndex >= m_listMaxLength - 1) return;
            UpdateItemPos(true);
        }
        else
        {
            if (m_curShowStartIndex <= 0) return;
            UpdateItemPos(false);
        }
        m_curSbVal = val;
    }
    // 更新item父节点位置  
    private void UpdateListPos(float val)
    {
        // 获取多出来的高度  
        float excess = 0f;
        if (m_allItemArea.y > m_showArea.y)
        {
            excess = m_allItemArea.y - m_showArea.y;
        }
        m_itemParent.localPosition = new Vector2(0, excess * val);
    }

    private void UpdateItemPos(bool isDown)
    {
        if (isDown)  // 下滑  
        {
            for (int i = 0; i < m_itemParent.childCount; i++)
            {
                Transform item = m_itemParent.GetChild(i);
                float curPos = item.localPosition.y + m_itemParent.localPosition.y;
                if (curPos > m_itemHeight)
                {
                    item.localPosition = new Vector3(0, m_lastItemPos.y, 0);
                    m_lastItemPos.y -= m_itemHeight;
                    m_firstItemPos.y -= m_itemHeight;

                    m_updateItem(item, m_curShowEndIndex + 1);
                    m_curShowStartIndex++;
                    m_curShowEndIndex++;
                    //break;  
                }
            }
        }
        else
        {
            for (int i = m_itemParent.childCount - 1; i >= 0; i--)
            {
                Transform item = m_itemParent.GetChild(i);
                float curPos = item.localPosition.y + m_itemParent.localPosition.y;
                if (curPos < -1 * m_showArea.y)
                {
                    item.localPosition = new Vector3(0, m_firstItemPos.y, 0);
                    m_firstItemPos.y += m_itemHeight;
                    m_lastItemPos.y += m_itemHeight;

                    m_updateItem(item, m_curShowStartIndex - 1);
                    m_curShowEndIndex--;
                    m_curShowStartIndex--;
                    //break;  
                }
            }
        }
    }



}