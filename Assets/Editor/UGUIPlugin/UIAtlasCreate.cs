/*****************************************************
 * 作者: 刘靖
 * 创建时间：2015.n.n
 * 版本：1.0.0
 * 描述：UGUI图集的设置
 ****************************************************/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using UnityEngine.UI;

public class UIAtlasCreate : Editor
{
    [MenuItem("Assets/AtlasCreate", false, 1)]
    public static void AtlasCreate()
    {
        string[] guids = Selection.assetGUIDs;
        for (int i = 0; i < guids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            Debug.Log(assetPath);
            string dictPath = Application.dataPath.Replace("Assets", assetPath);
            DirectoryInfo Dir = new DirectoryInfo(dictPath);
            string resPath = UGUIConfig.SpriteDir + Dir.Name;
            if (!Directory.Exists(resPath))
            {
                Directory.CreateDirectory(resPath);
            }
            foreach (FileInfo f in Dir.GetFiles("*.png")) //查找文件
            {
                int index = f.ToString().IndexOf("Assets");
                string modPath = f.ToString().Remove(0, index);
                modPath = modPath.Replace("\\", "/");

                TextureImporter texImport = AssetImporter.GetAtPath(modPath) as TextureImporter;
                texImport.textureType = TextureImporterType.Sprite;
                texImport.spritePackingTag = new DirectoryInfo(Path.GetDirectoryName(modPath)).Name;
                texImport.mipmapEnabled = false;
                AssetDatabase.ImportAsset(modPath);

                Sprite sprite = AssetDatabase.LoadAssetAtPath(modPath, typeof(Sprite)) as Sprite;
                GameObject go = new GameObject(sprite.name);
                go.AddComponent<SpriteRenderer>().sprite = sprite;
                string prePath = resPath + "/" + sprite.name + ".prefab";
                prePath = prePath.Substring(prePath.IndexOf("Assets"));
                PrefabUtility.CreatePrefab(prePath, go);
                GameObject.DestroyImmediate(go);
            }
        }

    }
}
