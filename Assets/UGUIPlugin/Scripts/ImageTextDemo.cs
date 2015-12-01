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

public class ImageTextDemo : MonoBehaviour {

    public Image image1;
    public Image image2;
    public Image image3;

    public Text text;

    public Image icon;

    public RectTransform rectTran;

    public GameObject cube;

	// Use this for initialization
	void Start () {
        RegisterEventListener();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void RegisterEventListener()
    {
        UGUIEventListener.Get(image1.gameObject).onClick = ImageDepthOnClick;
        UGUIEventListener.Get(image2.gameObject).onClick = ImageDepthOnClick;
        UGUIEventListener.Get(image3.gameObject).onClick = ImageDepthOnClick;
        UGUIEventListener.Get(text.gameObject).onClick = TextDepthOnClick;
        UGUIEventListener.Get(icon.gameObject).onClick = IconChangeOnClick;
        UGUIEventListener.Get(rectTran.gameObject).onDrag = SpriteOnDrag;
        UGUIEventListener.Get(cube).onDrag = CubeOnDrag;
    }

    private void ImageDepthOnClick(GameObject go, PointerEventData data)
    {
        Image image = go.GetComponent<Image>();
        image.SetDepth(10);
        Debug.Log(image.GetDepth());
    }

    private void TextDepthOnClick(GameObject go, PointerEventData data)
    {
        Text text = go.GetComponent<Text>();
        text.SetDepth(10);
        Debug.Log(text.GetDepth());
    }

    private void IconChangeOnClick(GameObject go, PointerEventData data)
    {
        int index = int.Parse(icon.sprite.name);
        index++;
        if (index > 14)
        {
            index = 1;
        }
        icon.LoadSprite("HeroIcon", index.ToString());
    }

    private void SpriteOnDrag(GameObject go, PointerEventData data)
    {
        Vector3 localPos;
        if(RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTran, data.position, data.pressEventCamera, out localPos))
        {
            rectTran.position = localPos;
        }
    }

    private void CubeOnDrag(GameObject go, PointerEventData data)
    {
        //Debug.Log(data.pointerCurrentRaycast.worldPosition);
        Vector3 worldPos = data.pressEventCamera.ScreenToWorldPoint(new Vector3(data.position.x, data.position.y, 107));
        go.transform.position = worldPos;        
    }

}
