/*****************************************************
 * 作者: 刘靖
 * 创建时间：2015.n.n
 * 版本：1.0.0
 * 描述：UGUI的使用工具
 ****************************************************/

using UnityEngine;
using System.Collections;

public class UGUIUtility : MonoBehaviour
{
	public static Sprite LoadSprite(string tag, string spriteName)
    {
        return Resources.Load<GameObject>(UGUIConfig.SpriteRes + tag + "/" + spriteName).GetComponent<SpriteRenderer>().sprite;
    }
}
