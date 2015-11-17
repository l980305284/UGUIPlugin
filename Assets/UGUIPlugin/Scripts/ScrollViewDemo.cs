using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * @author 戴佳霖
 * 
 */
public class ScrollViewDemo : MonoBehaviour
{

    List<int> testList = new List<int>();

    void Start()
    {
        for (int i = 0; i < 100; i++)
        {
            testList.Add(i);
        }

        LoopScrollView quick = GetComponent<LoopScrollView>();
        quick.Data(testList);
    }

    private void UpdateItem(Transform item, int index)
    {
        item.name = index.ToString();
        Debug.Log(item.localPosition + " " + item.position);
    }

}