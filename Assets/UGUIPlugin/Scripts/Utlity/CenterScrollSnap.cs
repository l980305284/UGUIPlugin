/*****************************************************
 * 作者: 刘靖
 * 创建时间：2015.n.n
 * 版本：1.0.0
 * 描述：用来做页面居中滑动
 ****************************************************/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(ScrollRect))]
public class CenterScrollSnap : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public int SpaceOffset = 20;
    public int ItemWidth = 200;
    public int ItemHeight = 200;

    [Tooltip("Button to go to the next page. (optional)")]
    public GameObject NextButton;
    [Tooltip("Button to go to the previous page. (optional)")]
    public GameObject PrevButton;
    [Tooltip("Text to show current page. (optional)")]
    public Text CurrentPageText;

    public Boolean UseFastSwipe = true;
    public int FastSwipeThreshold = 100;

    private Transform m_ScreensContent;
    private ScrollRect m_ScrollRect;

    private List<Vector3> m_Positions;

    private int m_Page = 1;
    private bool m_Lerp;
    private RectTransform m_RectTran;
    private Vector3 m_LerpTarget;

    private Vector3 m_StartPosition = new Vector3();

    private int m_CurrentPage;
    private bool m_StartDrag = true;

    private bool m_FastSwipeTimer = false;
    private int m_FastSwipeCounter = 0;
    private int m_FastSwipeTarget = 30;

    private bool m_Horizontal = true;

	// Use this for initialization
	void Start () 
    {
        m_RectTran = transform as RectTransform;
        m_ScrollRect = gameObject.GetComponent<ScrollRect>();
        m_ScreensContent = m_ScrollRect.content;

        m_ScrollRect.movementType = ScrollRect.MovementType.Unrestricted;
        m_ScrollRect.inertia = false;
        m_Horizontal = m_ScrollRect.horizontal;

        DistributePages();

        m_Page = m_ScreensContent.childCount;

        m_Positions = new List<Vector3>();

        Vector3[] worldCorner = new Vector3[4];
        m_RectTran.GetWorldCorners(worldCorner);
        Vector3 center = Vector3.Lerp(worldCorner[0], worldCorner[2], 0.5f);


        if (m_Page > 0)
        {
            for (int i = 0; i < m_Page; ++i)
            {
                RectTransform child = m_ScreensContent.transform.GetChild(i) as RectTransform;
                if(m_Horizontal)
                {
                    float distance = child.position.x - center.x + ItemWidth / 2.0f;
                    m_ScreensContent.position = m_ScreensContent.position - new Vector3(distance, 0, 0);
                    m_Positions.Add(m_ScreensContent.localPosition);

                    m_ScrollRect.horizontalNormalizedPosition = 0;
                }
                else
                {
                    float distance = child.position.y - center.y + ItemHeight / 2.0f;
                    m_ScreensContent.position = m_ScreensContent.position - new Vector3(0, distance, 0);
                    m_Positions.Add(m_ScreensContent.localPosition);

                    m_ScrollRect.verticalNormalizedPosition = 0;
                }               
            }
        }

        m_LerpTarget = m_Positions[0];
        m_Lerp = true;

        if (NextButton)
            UGUIEventListener.Get(NextButton).onClick = NextScreen;

        if (PrevButton)
            UGUIEventListener.Get(PrevButton).onClick = PreviousScreen;

	}
	
	// Update is called once per frame
	void Update () 
    {
        if (m_Lerp)
        {
            m_ScreensContent.localPosition = Vector3.Lerp(m_ScreensContent.localPosition, m_LerpTarget, 10.0f * Time.deltaTime);
            
            if (Vector3.Distance(m_ScreensContent.localPosition, m_LerpTarget) < 0.005f)
            {
                m_Lerp = false;
            }

            if (Vector3.Distance(m_ScreensContent.localPosition, m_LerpTarget) < 10f)
            {
                if(CurrentPageText)
                {
                    ChangeBulletsInfo(CurrentScreen());
                }
                
            }
        }

        if (m_FastSwipeTimer)
        {
            m_FastSwipeCounter++;
        }
	}

    public void NextScreen(GameObject go, PointerEventData data)
    {
        if (CurrentScreen() < m_Page)
        {
            m_Lerp = true;
            m_LerpTarget = m_Positions[CurrentScreen()];

            ChangeBulletsInfo(CurrentScreen() + 1);
        }
    }

    public void PreviousScreen(GameObject go, PointerEventData data)
    {
        if (CurrentScreen() > 1)
        {
            m_Lerp = true;
            m_LerpTarget = m_Positions[CurrentScreen() - 1 - 1];

            ChangeBulletsInfo(CurrentScreen() - 1);
        }
    }

    private void NextScreenCommand()
    {
        if (m_CurrentPage < m_Page)
        {
            m_Lerp = true;
            m_LerpTarget = m_Positions[m_CurrentPage];

            ChangeBulletsInfo(m_CurrentPage + 1);
        }
        else
        {
            m_Lerp = true;
            m_LerpTarget = m_Positions[m_CurrentPage - 1];
        }
    }

    private void PrevScreenCommand()
    {
        if (m_CurrentPage > 1)
        {
            m_Lerp = true;
            m_LerpTarget = m_Positions[m_CurrentPage - 1 - 1];

            ChangeBulletsInfo(m_CurrentPage - 1);
        }
        else
        {
            m_Lerp = true;
            m_LerpTarget = m_Positions[m_CurrentPage - 1];
        }
    }

    private void ChangeBulletsInfo(int currentScreen)
    {
        if (CurrentPageText)
        {
            CurrentPageText.text = currentScreen.ToString();
        }
        Debug.Log("Current Page:" + currentScreen);
            
    }

    private Vector3 FindClosestFrom(Vector3 start, System.Collections.Generic.List<Vector3> positions)
    {
        Vector3 closest = Vector3.zero;
        float distance = Mathf.Infinity;

        foreach (Vector3 position in m_Positions)
        {
            if (Vector3.Distance(start, position) < distance)
            {
                distance = Vector3.Distance(start, position);
                closest = position;
            }
        }

        return closest;
    }

    public int CurrentScreen()
    {
        float distance = Mathf.Infinity;
        int page = 1;
        for (int i = 0; i < m_Positions.Count; i++)
        {
            if (Vector3.Distance(m_ScreensContent.localPosition, m_Positions[i]) < distance)
            {
                distance = Vector3.Distance(m_ScreensContent.localPosition, m_Positions[i]);
                page = i + 1;
            }
        }

        return page;
    }


    private void DistributePages()
    {
        int currentPosition = 0;

        for (int i = 0; i < m_ScreensContent.transform.childCount; i++)
        {
            RectTransform child = m_ScreensContent.transform.GetChild(i).gameObject.GetComponent<RectTransform>();
            if (i > 0)
            {
                if (m_Horizontal)
                {
                    currentPosition = SpaceOffset * i + i * ItemWidth;
                }
                else
                {
                    currentPosition = -(SpaceOffset * i + i * ItemHeight);
                }
            }
            if (m_Horizontal)
            {
                child.anchoredPosition = new Vector2(currentPosition, child.anchoredPosition.y);
            }
            else
            {
                child.anchoredPosition = new Vector2(child.anchoredPosition.x, currentPosition);
            }
            
            child.sizeDelta = new Vector2(ItemWidth, ItemHeight);
        }

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        m_StartPosition = m_ScreensContent.localPosition;
        m_FastSwipeCounter = 0;
        m_FastSwipeTimer = true;
        m_CurrentPage = CurrentScreen();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        m_StartDrag = true;
        if (m_Horizontal)
        {
            if (UseFastSwipe)
            {
                bool fastSwipe = false;
                m_FastSwipeTimer = false;
                if (m_FastSwipeCounter <= m_FastSwipeTarget)
                {
                    if (Math.Abs(m_StartPosition.x - m_ScreensContent.localPosition.x) > FastSwipeThreshold)
                    {
                        fastSwipe = true;
                    }
                }
                if (fastSwipe)
                {
                    if (m_StartPosition.x - m_ScreensContent.localPosition.x > 0)
                    {
                        NextScreenCommand();
                    }
                    else
                    {
                        PrevScreenCommand();
                    }
                }
                else
                {
                    m_Lerp = true;
                    m_LerpTarget = FindClosestFrom(m_ScreensContent.localPosition, m_Positions);
                }
            }
            else
            {
                m_Lerp = true;
                m_LerpTarget = FindClosestFrom(m_ScreensContent.localPosition, m_Positions);
                Debug.Log(m_LerpTarget);
            }
        }
        else
        {
            if (UseFastSwipe)
            {
                bool fastSwipe = false;
                m_FastSwipeTimer = false;
                if (m_FastSwipeCounter <= m_FastSwipeTarget)
                {
                    if (Math.Abs(m_StartPosition.y - m_ScreensContent.localPosition.y) > FastSwipeThreshold)
                    {
                        fastSwipe = true;
                    }
                }
                if (fastSwipe)
                {
                    if (m_ScreensContent.localPosition.y - m_StartPosition.y > 0)
                    {
                        NextScreenCommand();
                    }
                    else
                    {
                        PrevScreenCommand();
                    }
                }
                else
                {
                    m_Lerp = true;
                    m_LerpTarget = FindClosestFrom(m_ScreensContent.localPosition, m_Positions);
                }
            }
            else
            {
                m_Lerp = true;
                m_LerpTarget = FindClosestFrom(m_ScreensContent.localPosition, m_Positions);
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        m_Lerp = false;
        if (m_StartDrag)
        {
            OnBeginDrag(eventData);
            m_StartDrag = false;
        }
    }
}
