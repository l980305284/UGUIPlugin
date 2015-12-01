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
        for (int i = 0; i < 99; i++)
        {
            testList.Add(i);
        }

        LoopScrollView loopScrollView = GetComponent<LoopScrollView>();
        loopScrollView.SetData(testList);
        loopScrollView.OnItemClick = OnItemClick;
        loopScrollView.MoveToIndex(72);
    }


    private void OnItemClick(LoopBaseNode node)
    {
        Debug.Log(node.data);
    }

}