/*****************************************************
 * 作者: 刘靖
 * 创建时间：2015.n.n
 * 版本：1.0.0
 * 描述：demo
 ****************************************************/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StartDemo : MonoBehaviour {

	// Use this for initialization
	void Start () {
        UGUIEventListener[] listeners = gameObject.GetComponentsInChildren<UGUIEventListener>(true);
        foreach (UGUIEventListener listener in listeners)
        {
            listener.onClick = SceneOnClick;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void SceneOnClick(GameObject go, PointerEventData data)
    {
        string levName = "Demo_" + go.name;
        StartCoroutine(LoadLevelByName(levName));
    }

    private IEnumerator LoadLevelByName(string name)
    {
        AsyncOperation async = Application.LoadLevelAsync(name);
        yield return async;
    }
}
