/*****************************************************
 * 作者: 刘靖
 * 创建时间：2016.n.n
 * 版本：1.0.0
 * 描述：UGUI图集的设置
 ****************************************************/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEditor.Sprites;
using System.Reflection;

public class UGUIEditorTools : Editor
{

    /// <summary>创建图集方法</summary>
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

            FileInfo[] infos = Dir.GetFiles("*.png");
            if(infos.Length > 0)
            {
                string dicPath = infos[0].ToString().Remove(0, infos[0].ToString().IndexOf("Assets")).Replace("\\", "/");
                AtlasData atlasData = ScriptableObject.CreateInstance<AtlasData>();
                atlasData.spDataList = new List<SpriteData>();

                foreach (FileInfo f in infos) //查找文件
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
                    SpriteData data = new SpriteData(sprite.name, sprite);
                    atlasData.spDataList.Add(data);
                }
                string prePath = resPath + "/" + new DirectoryInfo(Path.GetDirectoryName(dicPath)).Name + ".asset";
                prePath = prePath.Substring(prePath.IndexOf("Assets"));
                AssetDatabase.CreateAsset(atlasData, prePath);
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogError("文件夹下没有png文件!");
                return;
            }
        }
    }

    /// <summary>查找图集方法</summary>
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

    /// <summary>提高Image效率，去掉碰撞检测</summary>
    [MenuItem("GameObject/UI/Image")]
    public static void CreatImage()
    {
        if (Selection.activeTransform)
        {
            if (Selection.activeTransform.GetComponentInParent<Canvas>())
            {
                GameObject go = new GameObject("Image", typeof(Image));
                go.GetComponent<Image>().raycastTarget = false;
                go.transform.SetParent(Selection.activeTransform);
                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = Vector3.one;
                go.transform.localRotation = Quaternion.identity;
            }
        }
    }

    /// <summary>提高Text效率，去掉碰撞检测</summary>
    [MenuItem("GameObject/UI/Text")]
    public static void CreatText()
    {
        if (Selection.activeTransform)
        {
            if (Selection.activeTransform.GetComponentInParent<Canvas>())
            {
                GameObject go = new GameObject("Text", typeof(Text));
                go.GetComponent<Text>().raycastTarget = false;
                go.transform.SetParent(Selection.activeTransform);
                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = Vector3.one;
                go.transform.localRotation = Quaternion.identity;
            }
        }
    }


}
