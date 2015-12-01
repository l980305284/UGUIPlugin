using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/*
 * @author 戴佳霖
 * 无限循环列表
 */
public class LoopScrollView : MonoBehaviour
{
    public enum Direction
    {
        Horizontal,
        Vertical
    }

    [SerializeField]
    private RectTransform m_cell;

    [SerializeField]
    private Vector2 m_page;

    [SerializeField]
    Direction direction = Direction.Horizontal;

    [SerializeField, Range(1, 10)]
    private int m_bufferNo;


    public delegate void OnBaseNodeEvent(LoopBaseNode item);
    public delegate void OnBaseNodeEvent<T>(LoopBaseNode item, T t);
    public OnBaseNodeEvent OnItemClick;
    public OnBaseNodeEvent<bool> OnItemPress;
    public OnBaseNodeEvent OnDataChange;

    public bool needItemEvent;

    public Vector2 m_cellRect;
    public int m_currentIndex;

    private List<RectTransform> m_instantiateItems = new List<RectTransform>();
    private IList m_datas;
    private float m_prevPos = 0;

    private ScrollRect m_scrollRect;
    private RectTransform m_contentRect;
    private Vector2 m_instantiateSize = Vector2.zero;

    private Vector3 m_initialContentSize;

    public float cellScale { get { return direction == Direction.Horizontal ? m_cellRect.x : m_cellRect.y; } }
    public float directionPos { get { return direction == Direction.Horizontal ? m_contentRect.anchoredPosition.x : m_contentRect.anchoredPosition.y; } }
    public float scale { get { return direction == Direction.Horizontal ? 1f : -1f; } }
    public int pageCount { get { return (int)m_page.x * (int)m_page.y; } }
    public int pageScale { get { return direction == Direction.Horizontal ? (int)m_page.x : (int)m_page.y; } }
    public int instantiateCount { get { return (int)instantiateSize.x * (int)instantiateSize.y; } }

    public Vector2 instantiateSize
    {
        get
        {
            if (m_instantiateSize == Vector2.zero)
            {
                float rows, cols;
                if (direction == Direction.Horizontal)
                {
                    rows = m_page.x;
                    cols = m_page.y + (float)m_bufferNo;
                }
                else
                {
                    rows = m_page.x + (float)m_bufferNo;
                    cols = m_page.y;
                }
                m_instantiateSize = new Vector2(rows, cols);
            }
            return m_instantiateSize;
        }
    }

    public float maxPrevPos
    {
        get
        {
            float result;
            Vector2 max = GetRectByNum(m_datas.Count);
            if (direction == Direction.Horizontal)
            {
                result = max.y - m_page.y - m_bufferNo;
            }
            else
            {
                result = max.x - m_page.x - m_bufferNo;
            }
            return result * cellScale;
        }
    }


    void Awake()
    {
        m_scrollRect = GetComponentInParent<ScrollRect>();
        m_scrollRect.horizontal = direction == Direction.Horizontal;
        m_scrollRect.vertical = direction == Direction.Vertical;
        m_contentRect = m_scrollRect.content.GetComponent<RectTransform>();
        m_initialContentSize = m_contentRect.transform.localPosition;
        m_cell.gameObject.SetActive(false);
    }

    public void SetData(object data)
    {
        m_datas = data as IList;

        if (m_datas.Count > pageCount)
        {
            SetBound(GetRectByNum(m_datas.Count));
        }
        else
        {
            SetBound(m_page);
        }

        if (m_datas.Count > instantiateCount)
        {
            while (m_instantiateItems.Count < instantiateCount)
            {
                CreateItem(m_instantiateItems.Count);
            }
        }
        else
        {
            while (m_instantiateItems.Count > m_datas.Count)
            {
                RemoveItem(m_instantiateItems.Count - 1);
            }

            while (m_instantiateItems.Count < m_datas.Count)
            {
                CreateItem(m_instantiateItems.Count);
            }
        }
    }

    public void MoveToIndex(int index)
    {
        if (direction == Direction.Horizontal)
        {
            int rowIndex = Mathf.FloorToInt(index / m_page.x);
            rowIndex = rowIndex - m_bufferNo;
            m_contentRect.transform.localPosition = new Vector3(m_initialContentSize.x - rowIndex * m_cellRect.x, m_initialContentSize.y, m_initialContentSize.z);
        }
        else
        {
            int rowIndex = Mathf.FloorToInt(index / m_page.y);
            rowIndex = rowIndex - m_bufferNo;
            m_contentRect.transform.localPosition = new Vector3(m_initialContentSize.x, m_initialContentSize.y + rowIndex * m_cellRect.y, m_initialContentSize.z);
        }
    }

    private void CreateItem(int index)
    {
        RectTransform item = GameObject.Instantiate(m_cell);
        item.SetParent(m_contentRect.transform, false);
        item.anchorMax = Vector2.up;
        item.anchorMin = Vector2.up;
        item.pivot = Vector2.up;
        item.name = "item" + index;

        item.anchoredPosition = direction == Direction.Horizontal ?
            new Vector2(Mathf.Floor(index / instantiateSize.x) * m_cellRect.x, -(index % instantiateSize.x) * m_cellRect.y) :
            new Vector2((index % instantiateSize.y) * m_cellRect.x, -Mathf.Floor(index / instantiateSize.y) * m_cellRect.y);
        m_instantiateItems.Add(item);
        item.gameObject.SetActive(true);
        LoopBaseNode baseNode = item.GetComponent<LoopBaseNode>();
        baseNode.index = index;
        baseNode.data = m_datas[index];
        baseNode.OnDataChange();
    }

    private void RemoveItem(int index)
    {
        RectTransform item = m_instantiateItems[index];
        m_instantiateItems.Remove(item);
        RectTransform.Destroy(item.gameObject);
    }

    private Vector2 GetRectByNum(int num)
    {
        return direction == Direction.Horizontal ?
            new Vector2(m_page.x, Mathf.CeilToInt(num / m_page.x)) :
            new Vector2(Mathf.CeilToInt(num / m_page.y), m_page.y);
    }


    private void SetBound(Vector2 bound)
    {
        if (direction == Direction.Horizontal)
            m_contentRect.sizeDelta = new Vector2(bound.y * m_cellRect.x, 0);
        else
            m_contentRect.sizeDelta = new Vector2(0, bound.x * m_cellRect.y);
    }

    void Update()
    {
        if (scale * directionPos - m_prevPos < -cellScale * m_bufferNo)
        {
            if (m_prevPos <= -maxPrevPos) return;
            List<RectTransform> range = m_instantiateItems.GetRange(0, pageScale);
            m_instantiateItems.RemoveRange(0, pageScale);
            m_instantiateItems.AddRange(range);
            for (int i = 0; i < range.Count; i++)
            {
                int itemIndex = m_currentIndex * pageScale + m_instantiateItems.Count + i;
                if (itemIndex < m_datas.Count)
                    MoveItemToIndex(m_currentIndex * pageScale + m_instantiateItems.Count + i, range[i]);
            }
            m_prevPos -= cellScale;
            m_currentIndex++;
        }

        if (scale * directionPos - m_prevPos > -cellScale * m_bufferNo)
        {
            if (Mathf.RoundToInt(m_prevPos) >= 0) return;
            m_prevPos += cellScale;
            m_currentIndex--;

            if (m_currentIndex < 0) return;

            List<RectTransform> range = m_instantiateItems.GetRange(m_instantiateItems.Count - pageScale, pageScale);
            m_instantiateItems.RemoveRange(m_instantiateItems.Count - pageScale, pageScale);
            m_instantiateItems.InsertRange(0, range);
            for (int i = 0; i < range.Count; i++)
            {
                MoveItemToIndex(m_currentIndex * pageScale + i, range[i]);
            }
        }
    }

    private void MoveItemToIndex(int index, RectTransform item)
    {
        item.anchoredPosition = GetPosByIndex(index);
        UpdateItem(index, item.gameObject);
    }

    private Vector2 GetPosByIndex(int index)
    {
        float x, y;
        if (direction == Direction.Horizontal)
        {
            x = index % m_page.x;
            y = Mathf.FloorToInt(index / m_page.x);
        }
        else
        {
            x = Mathf.FloorToInt(index / m_page.y);
            y = index % m_page.y;
        }

        return new Vector2(y * m_cellRect.x, -x * m_cellRect.y);
    }

    private void UpdateItem(int index, GameObject item)
    {
        item.SetActive(index < m_datas.Count);

        if (item.activeSelf)
        {
            LoopBaseNode baseNode = item.GetComponent<LoopBaseNode>();
            baseNode.index = index;
            baseNode.data = m_datas[index];
            baseNode.OnDataChange();
        }
    }
}