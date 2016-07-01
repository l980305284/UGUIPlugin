/*****************************************************
 * 作者: 刘靖
 * 创建时间：2016.n.n
 * 版本：1.0.0
 * 描述：自定义RectTransform类的Inspector
 ****************************************************/

using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(RectTransform))]
public class CustomRectTransEditor : DecoratorEditor
{

    public CustomRectTransEditor(): base("RectTransformEditor"){ }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        RectTransform trans = (RectTransform)target;

        if (GUILayout.Button("Pos Clear"))
        {         
            Undo.RecordObject(trans, "Zero Transform Position");
            trans.localPosition = Vector3.zero;
        }

        if(GUILayout.Button("Rot Clear"))
        {
            Undo.RecordObject(trans, "Zero Transform Rotation");
            trans.localEulerAngles = Vector3.zero;
        }

        if (GUILayout.Button("Scale Clear"))
        {
            Undo.RecordObject(trans, "Zero Transform Scale");
            trans.localScale = Vector3.one;
        }

    }
}
