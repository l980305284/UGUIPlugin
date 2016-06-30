/*****************************************************
 * 作者: 刘靖
 * 创建时间：2015.n.n
 * 版本：1.0.0
 * 描述：UGUI的使用工具
 ****************************************************/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public static class UGUIUtility
{
	public static void LoadSprite(this Image image, string tag, string spriteName)
    {
        image.sprite = Resources.Load<AtlasData>(UGUIConfig.SpriteRes + tag + "/" + tag).GetSpriteByName(spriteName);
    }

    public static int GetDepth(this Image image)
    {
        CanvasRenderer render = image.gameObject.GetComponent<CanvasRenderer>();
        return render.absoluteDepth;
    }

    public static void SetDepth(this Image image, int depth)
    {
        image.transform.SetSiblingIndex(depth);
    }

    public static int GetDepth(this Text text)
    {
        CanvasRenderer render = text.gameObject.GetComponent<CanvasRenderer>();
        return render.absoluteDepth;
    }

    public static void SetDepth(this Text text, int depth)
    {
        text.transform.SetSiblingIndex(depth);
    }

    /// <summary>判断是否点击在UI上</summary>
    public static bool IsClickUI()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Check if the mouse was clicked over a UI element
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                return false;
            }
            return true;
        }
        return false;
    }

    public static T FindInParents<T>(GameObject go) where T : Component
    {
        if (go == null) return null;
        var comp = go.GetComponent<T>();

        if (comp != null)
            return comp;

        var t = go.transform.parent;
        while (t != null && comp == null)
        {
            comp = t.gameObject.GetComponent<T>();
            t = t.parent;
        }
        return comp;
    }

    public static int SortHorizontal(RectTransform a, RectTransform b) { return a.localPosition.x.CompareTo(b.localPosition.x); }

    public static int SortVertical(RectTransform a, RectTransform b) { return b.localPosition.y.CompareTo(a.localPosition.y); }

    public static int SortByName(RectTransform a, RectTransform b) { return string.Compare(a.name, b.name); }
}
