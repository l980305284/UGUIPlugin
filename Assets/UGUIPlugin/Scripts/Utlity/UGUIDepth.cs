/*****************************************************
 * 作者: 刘靖
 * 创建时间：2015.n.n
 * 版本：1.0.0
 * 描述：用于处理特效和UI的层次问题
 ****************************************************/

using UnityEngine;
using System.Collections;

public class UGUIDepth : MonoBehaviour {

    public int order;
    public bool isUI = true;
    void Start()
    {
        SetDepth();
    }

    public void SetDepth()
    {
        if (isUI)
        {
            Canvas canvas = GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = gameObject.AddComponent<Canvas>();
            }
            canvas.overrideSorting = true;
            canvas.sortingOrder = order;
        }
        else
        {
            Renderer[] renders = GetComponentsInChildren<Renderer>();

            foreach (Renderer render in renders)
            {
                render.sortingOrder = order;
            }
        }
    }
}
