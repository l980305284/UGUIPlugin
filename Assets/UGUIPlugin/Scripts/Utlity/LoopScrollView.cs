using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/*
 * @author 戴佳霖
 * 
 */
public class LoopScrollView : MonoBehaviour
{

    enum Direction
    {
        Horizontal,
        Vertical
    }

    [SerializeField]
    private RectTransform m_Cell;

    [SerializeField]
    private Vector2 m_Page;

    [SerializeField]
    Direction direction = Direction.Horizontal;

    [SerializeField, Range(1, 10)]
    private int m_BufferNo;

    private List<RectTransform> m_InstantiateItems = new List<RectTransform>();

    private IList m_Datas;

    public Vector2 CellRect;

    public float CellScale { get { return direction == Direction.Horizontal ? CellRect.x : CellRect.y; } }

    private float m_PrevPos = 0;
    public float DirectionPos { get { return direction == Direction.Horizontal ? m_Rect.anchoredPosition.x : m_Rect.anchoredPosition.y; } }

    private int m_CurrentIndex;//页面的第一行（列）在整个conten中的位置

    private Vector2 m_InstantiateSize = Vector2.zero;
    public Vector2 InstantiateSize
    {
        get
        {
            if (m_InstantiateSize == Vector2.zero)
            {
                float rows, cols;
                if (direction == Direction.Horizontal)
                {
                    rows = m_Page.x;
                    cols = m_Page.y + (float)m_BufferNo;
                }
                else
                {
                    rows = m_Page.x + (float)m_BufferNo;
                    cols = m_Page.y;
                }
                m_InstantiateSize = new Vector2(rows, cols);
            }
            return m_InstantiateSize;
        }
    }

    public int PageCount { get { return (int)m_Page.x * (int)m_Page.y; } }

    public int PageScale { get { return direction == Direction.Horizontal ? (int)m_Page.x : (int)m_Page.y; } }

    private ScrollRect m_ScrollRect;

    private RectTransform m_Rect;
    public int InstantiateCount { get { return (int)InstantiateSize.x * (int)InstantiateSize.y; } }
    void Awake()
    {
        m_ScrollRect = GetComponentInParent<ScrollRect>();
        m_ScrollRect.horizontal = direction == Direction.Horizontal;
        m_ScrollRect.vertical = direction == Direction.Vertical;

        m_Rect = GetComponent<RectTransform>();

        m_Cell.gameObject.SetActive(false);
    }

    public void Data(object data)
    {
        m_Datas = data as IList;

        if (m_Datas.Count > PageCount)
        {
            setBound(getRectByNum(m_Datas.Count));
        }
        else
        {
            setBound(m_Page);
        }

        if (m_Datas.Count > InstantiateCount)
        {
            while (m_InstantiateItems.Count < InstantiateCount)
            {
                createItem(m_InstantiateItems.Count);
            }
        }
        else
        {
            while (m_InstantiateItems.Count > m_Datas.Count)
            {
                removeItem(m_InstantiateItems.Count - 1);
            }

            while (m_InstantiateItems.Count < m_Datas.Count)
            {
                createItem(m_InstantiateItems.Count);
            }
        }
    }

    private void createItem(int index)
    {
        RectTransform item = GameObject.Instantiate(m_Cell);
        item.SetParent(m_ScrollRect.content.transform, false);
        item.anchorMax = Vector2.up;
        item.anchorMin = Vector2.up;
        item.pivot = Vector2.up;
        item.name = "item" + index;

        item.anchoredPosition = direction == Direction.Horizontal ?
            new Vector2(Mathf.Floor(index / InstantiateSize.x) * CellRect.x, -(index % InstantiateSize.x) * CellRect.y) :
            new Vector2((index % InstantiateSize.y) * CellRect.x, -Mathf.Floor(index / InstantiateSize.y) * CellRect.y);
        m_InstantiateItems.Add(item);
        item.gameObject.SetActive(true);

        updateItem(index, item.gameObject);
    }

    private void removeItem(int index)
    {
        RectTransform item = m_InstantiateItems[index];
        m_InstantiateItems.Remove(item);
        RectTransform.Destroy(item.gameObject);
    }

    private Vector2 getRectByNum(int num)
    {
        return direction == Direction.Horizontal ?
            new Vector2(m_Page.x, Mathf.CeilToInt(num / m_Page.x)) :
            new Vector2(Mathf.CeilToInt(num / m_Page.y), m_Page.y);

    }


    private void setBound(Vector2 bound)
    {
        m_Rect.sizeDelta = new Vector2(bound.y * CellRect.x, bound.x * CellRect.y);
    }

    public float MaxPrevPos
    {
        get
        {
            float result;
            Vector2 max = getRectByNum(m_Datas.Count);
            if (direction == Direction.Horizontal)
            {
                result = max.y - m_Page.y;
            }
            else
            {
                result = max.x - m_Page.x;
            }
            return result * CellScale;
        }
    }
    public float scale { get { return direction == Direction.Horizontal ? 1f : -1f; } }
    void Update()
    {
        while (scale * DirectionPos - m_PrevPos < -CellScale * 2)
        {
            if (m_PrevPos <= -MaxPrevPos) return;

            m_PrevPos -= CellScale;

            List<RectTransform> range = m_InstantiateItems.GetRange(0, PageScale);
            m_InstantiateItems.RemoveRange(0, PageScale);
            m_InstantiateItems.AddRange(range);
            for (int i = 0; i < range.Count; i++)
            {
                moveItemToIndex(m_CurrentIndex * PageScale + m_InstantiateItems.Count + i, range[i]);
            }
            m_CurrentIndex++;
        }

        while (scale * DirectionPos - m_PrevPos > -CellScale)
        {
            if (Mathf.RoundToInt(m_PrevPos) >= 0) return;

            m_PrevPos += CellScale;

            m_CurrentIndex--;

            if (m_CurrentIndex < 0) return;

            List<RectTransform> range = m_InstantiateItems.GetRange(m_InstantiateItems.Count - PageScale, PageScale);
            m_InstantiateItems.RemoveRange(m_InstantiateItems.Count - PageScale, PageScale);
            m_InstantiateItems.InsertRange(0, range);
            for (int i = 0; i < range.Count; i++)
            {
                moveItemToIndex(m_CurrentIndex * PageScale + i, range[i]);
            }
        }
    }

    private void moveItemToIndex(int index, RectTransform item)
    {
        item.anchoredPosition = getPosByIndex(index);
        updateItem(index, item.gameObject);
    }

    private Vector2 getPosByIndex(int index)
    {
        float x, y;
        if (direction == Direction.Horizontal)
        {
            x = index % m_Page.x;
            y = Mathf.FloorToInt(index / m_Page.x);
        }
        else
        {
            x = Mathf.FloorToInt(index / m_Page.y);
            y = index % m_Page.y;
        }

        return new Vector2(y * CellRect.x, -x * CellRect.y);
    }

    private void updateItem(int index, GameObject item)
    {
        item.SetActive(index < m_Datas.Count);

        if (item.activeSelf)
        {
            Debug.Log(item.name + " " + index);
            //UILoopItem lit = item.GetComponent<UILoopItem>();
            //lit.UpdateItem(index, item);
            //lit.Data(m_Datas[index]);
        }
    }
}