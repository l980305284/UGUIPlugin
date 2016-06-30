/*****************************************************
 * 作者: 刘靖
 * 创建时间：2016.n.n
 * 版本：1.0.0
 * 描述：用来存储图集数据类
 ****************************************************/

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


[Serializable]
public class SpriteData
{
    public string spName;
    public Sprite sp;

    public SpriteData(string name, Sprite s)
    {
        spName = name;
        sp = s;
    }
}

public class AtlasData : ScriptableObject
{
    public List<SpriteData> spDataList;

    public Sprite GetSpriteByName(string name)
    {
        for (int i=0; i<spDataList.Count; i++)
        {
            if(spDataList[i].spName == name)
            {
                return spDataList[i].sp;
            }
        }
        Debug.LogError("不存在该Sprite");
        return null;
    }
}