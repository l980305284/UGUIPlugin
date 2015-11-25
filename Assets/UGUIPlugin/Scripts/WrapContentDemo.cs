/*****************************************************
 * 作者: 刘靖
 * 创建时间：2015.n.n
 * 版本：1.0.0
 * 描述：无限列表的测试用例
 ****************************************************/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WrapContentDemo : MonoBehaviour {

    public UGUIWrapContent mWrapContent;

	// Use this for initialization
	void Awake () {
        mWrapContent.onInitializeItem = initItem; 
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void initItem(GameObject go, int wrapIndex, int realIndex)
    {
        Text text = go.GetComponentInChildren<Text>();
        Debug.Log(realIndex);
        text.text = realIndex.ToString();
    }
}
