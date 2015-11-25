/*****************************************************
 * 作者: 刘靖
 * 创建时间：2015.n.n
 * 版本：1.0.0
 * 描述：无限循环内容列表
 ****************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UGUIWrapContent : MonoBehaviour {

    public delegate void OnInitializeItem(GameObject go, int wrapIndex, int realIndex);

    public int itemSize = 100;

    public OnInitializeItem onInitializeItem;

    public int minIndex = 0;

    public int maxIndex = 0;


    RectTransform mRectTran;
    ScrollRect mScroll;
    bool mHorizontal = true;
    List<RectTransform> mChildren = new List<RectTransform>();
    bool mFirstTime = true;

	// Use this for initialization
	void Start () 
    {
        OnInit();
        SortBasedOnScrollMovement();
        WrapContent();
        if(mScroll != null)
        {
            mScroll.onValueChanged.AddListener(OnMove);
        }
        mFirstTime = false;
	}
	
    private void OnMove(Vector2 pos)
    {
        WrapContent();
    }

    public void SortBasedOnScrollMovement()
    {

        if (!CacheScrollView()) return;

        mChildren.Clear();
        for(int i = 0; i < mRectTran.childCount; i++)
        {
            mChildren.Add(mRectTran.GetChild(i).GetComponent<RectTransform>());
        }
        if(mHorizontal)
        {
            mChildren.Sort(UGUIUtility.SortHorizontal);
        }
        else
        {
            mChildren.Sort(UGUIUtility.SortVertical);
        }
        ResetChildPositions();
    }

    public void SortAlphabetically ()
    {
        if (!CacheScrollView()) return;


        mChildren.Clear();
        for (int i = 0; i < mRectTran.childCount; i++)
        {
            mChildren.Add(mRectTran.GetChild(i).GetComponent<RectTransform>());
        }

        mChildren.Sort(UGUIUtility.SortByName);

        ResetChildPositions();
    }
    
    private bool CacheScrollView()
    {
        mRectTran = gameObject.GetComponent<RectTransform>();
        mScroll = UGUIUtility.FindInParents<ScrollRect>(gameObject);
        if(mScroll == null)
        {
            return false;
        }
        if(mScroll.horizontal)
        {
            mHorizontal = true;
        }
        else if(mScroll.vertical)
        {
            mHorizontal = false;
        }
        else
        {
            return false;
        }
        return true;

    }

    void ResetChildPositions()
    {
        if(mHorizontal)
        {
            mScroll.content.sizeDelta = new Vector2((maxIndex - minIndex + 1) * itemSize, mScroll.content.sizeDelta.y);
        }
        else
        {
            mScroll.content.sizeDelta = new Vector2(mScroll.content.sizeDelta.x, (maxIndex - minIndex + 1) * itemSize);
        }
        
        for (int i = 0, imax = mChildren.Count; i < imax; ++i)
        {
            RectTransform t = mChildren[i];
            if(mHorizontal)
            {
                //t.sizeDelta = new Vector2(itemSize, t.sizeDelta.y);
                t.anchoredPosition = new Vector2(i * itemSize, 0f);
            }
            else
            {
                //t.sizeDelta = new Vector2(t.sizeDelta.x, itemSize);
                t.anchoredPosition = new Vector2(0f, -i * itemSize);
            }
            UpdateItem(t, i);
        }
    }

    private void WrapContent()
    {
        if (!CacheScrollView()) return;
        float extents = itemSize * mChildren.Count * 0.5f;

        Vector3[] worldCorner = new Vector3[4];
        mScroll.GetComponent<RectTransform>().GetWorldCorners(worldCorner);

        for (int i = 0; i < 4; ++i)
        {
            Vector3 v = worldCorner[i];
            v = mRectTran.InverseTransformPoint(v);
            worldCorner[i] = v;

        }

        Vector3 center = Vector3.Lerp(worldCorner[0], worldCorner[2], 0.5f);
        float ext2 = extents * 2.0f;

        if(mHorizontal)
        {
            for (int i = 0, imax = mChildren.Count; i < imax; ++i)
            {
                RectTransform t = mChildren[i];
                float distance = t.localPosition.x - center.x;

                if(distance < -extents)
                {
                    Vector3 pos = t.localPosition;
                    pos.x += ext2;
                    distance = pos.x - center.x;
                    int realIndex = Mathf.RoundToInt(pos.x / itemSize);

                    if (minIndex == maxIndex || (minIndex <= realIndex && realIndex <= maxIndex))
                    {
                        t.localPosition = pos;
                        UpdateItem(t, i);
                    }
                }
                else if (distance > extents)
                {
                    Vector3 pos = t.localPosition;
                    pos.x -= ext2;
                    distance = pos.x - center.x;
                    int realIndex = Mathf.RoundToInt(pos.x / itemSize);

                    if (minIndex == maxIndex || (minIndex <= realIndex && realIndex <= maxIndex))
                    {
                        t.localPosition = pos;
                        UpdateItem(t, i);
                    }
                }
                else if (mFirstTime) UpdateItem(t, i);
            }
        }
        else
        {
            for (int i = 0, imax = mChildren.Count; i < imax; ++i)
            {
                RectTransform t = mChildren[i];
                float distance = t.localPosition.y - center.y;

                if (distance < -extents)
                {
                    Vector3 pos = t.localPosition;
                    pos.y += ext2;
                    distance = pos.y - center.y;
                    int realIndex = Mathf.RoundToInt(pos.y / itemSize);

                    if (minIndex == maxIndex || (minIndex <= realIndex && realIndex <= maxIndex))
                    {
                        t.localPosition = pos;
                        UpdateItem(t, i);
                    }
                }
                else if (distance > extents)
                {
                    Vector3 pos = t.localPosition;
                    pos.y -= ext2;
                    distance = pos.y - center.y;
                    int realIndex = Mathf.RoundToInt(pos.y / itemSize);

                    if (minIndex == maxIndex || (minIndex <= realIndex && realIndex <= maxIndex))
                    {
                        t.localPosition = pos;
                        UpdateItem(t, i);
                    }
                }
                else if (mFirstTime) UpdateItem(t, i);
            }
        }
    }

    private void OnInit()
    {
        if (maxIndex < minIndex)
            maxIndex = minIndex;
        if (minIndex > maxIndex)
            maxIndex = minIndex;
    }

    private void UpdateItem(RectTransform item, int index)
    {
        if(onInitializeItem != null)
        {

            int realIndex = mHorizontal ?
                Mathf.RoundToInt(item.anchoredPosition.x / itemSize) :
                Mathf.RoundToInt(item.anchoredPosition.y / itemSize);
            onInitializeItem(item.gameObject, index, realIndex);
        }
    }
}
