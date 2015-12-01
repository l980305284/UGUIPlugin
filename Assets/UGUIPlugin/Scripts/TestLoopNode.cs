using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/*
 * @author 戴佳霖
 * 
 */
public class TestLoopNode : LoopBaseNode
{
    private Text m_text;

    public override void OnDataChange()
    {
        base.OnDataChange();
        int index = (int)data;
        if (m_text == null)
            m_text = GetComponentInChildren<Text>();
        m_text.text = index.ToString();

    }


}