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
using UnityEditor.Sprites;
using System.Reflection;

public class UIAtlasCreate : Editor
{
    [MenuItem("Assets/AtlasCreate", false, 1)]
    public static void AtlasCreate()
    {
        string[] guids = Selection.assetGUIDs;
        for (int i = 0; i < guids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
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

    [MenuItem("Assets/SearchAtlas", false, 1)]
    public static void SearchAtlas()
    {
        string[] guids = Selection.assetGUIDs;
        if (guids.Length <= 0)
        {
            return;
        }
        string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
        Debug.Log(assetPath);
        if(!assetPath.EndsWith(".png") && !assetPath.EndsWith(".jpg") && !assetPath.EndsWith(".psd"))
        {
            return;
        }

        TextureImporter texImport = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        

        //需要Sprite Packer界面定位的图集名称
        string spriteName = texImport.spritePackingTag;
        //设置使用采取图集的方式
        EditorSettings.spritePackerMode = SpritePackerMode.AlwaysOn;
        //打包图集
        Packer.RebuildAtlasCacheIfNeeded(EditorUserBuildSettings.activeBuildTarget, true);
        //打开SpritePack窗口
        EditorApplication.ExecuteMenuItem("Window/Sprite Packer");
        var window = EditorWindow.focusedWindow;

        //反射遍历所有图集
        var type = typeof(EditorWindow).Assembly.GetType("UnityEditor.Sprites.PackerWindow");
        FieldInfo infoNames = type.GetField("m_AtlasNames", BindingFlags.NonPublic | BindingFlags.Instance);
        string[] infoNamesArray = (string[])infoNames.GetValue(window);

        if (infoNamesArray != null)
        {
            for (int i = 0; i < infoNamesArray.Length; i++)
            {
                if (infoNamesArray[i] == spriteName)
                {
                    //找到后设置索引
                    FieldInfo info = type.GetField("m_SelectedAtlas", BindingFlags.NonPublic | BindingFlags.Instance);
                    info.SetValue(window, i);
                    break;
                }
            }
        }
    }

}
