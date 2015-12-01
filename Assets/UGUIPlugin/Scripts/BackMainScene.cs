/*****************************************************
 * 作者: 刘靖
 * 创建时间：2015.n.n
 * 版本：1.0.0
 * 描述：
 ****************************************************/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BackMainScene : MonoBehaviour {

    public GameObject m_btn;

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject);
        UGUIEventListener listener = UGUIEventListener.Get(m_btn);
        listener.onClick = BackMainSceneOnClick;
	}
	

    private void BackMainSceneOnClick(GameObject go, PointerEventData data)
    {
        if (Application.loadedLevelName == "Demo_Start")
        {
            Debug.Log("return");
            return;
        }
        Debug.Log("test");
        StartCoroutine(LoadLevelByName("Demo_Start"));
    }

    private IEnumerator LoadLevelByName(string name)
    {
        AsyncOperation async = Application.LoadLevelAsync(name);
        yield return async;
    }
}
