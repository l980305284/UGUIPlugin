using UnityEngine;
using System.Collections;
using UnityEditor;

/*
 * @author 戴佳霖
 * 一键创建图片字体
 */
public class FontCreat : Editor
{
    [MenuItem("Assets/BMFontCreat", false, 1)]
    public static void BMFontCreat()
    {
        string txtPath = string.Empty;
        string texturePath = string.Empty;
        string[] guids = Selection.assetGUIDs;
        if (guids.Length != 2)
        {
            Debug.Log("please select fontImage and fontText!");
            return;
        }

        Font mFont = new Font();
        TextAsset mText = null;
        Material material = new Material(Shader.Find("GUI/Text Shader"));

        for (int i = 0; i < guids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            int index = assetPath.ToString().IndexOf("Assets");
            string modPath = assetPath.ToString().Remove(0, index);
            modPath = modPath.Replace("\\", "/");
            if (assetPath.Contains("txt"))//txt
            {
                txtPath = modPath;
                mText = (TextAsset)AssetDatabase.LoadAssetAtPath(modPath, typeof(TextAsset));
            }
            else//png
            {
                texturePath = modPath;
                TextureImporter texImport = AssetImporter.GetAtPath(modPath) as TextureImporter;
                texImport.textureFormat = TextureImporterFormat.AutomaticTruecolor;
                texImport.textureType = TextureImporterType.Image;
                texImport.wrapMode = TextureWrapMode.Clamp;
                AssetDatabase.ImportAsset(modPath);
                material.mainTexture = (Texture)AssetDatabase.LoadAssetAtPath(modPath, typeof(Texture));
            }
        }
        if (mText == null || material == null)
            return;

        mFont.name = mText.name;
        material.name = mText.name;
        BMFont mbFont = new BMFont();
        BMFontReader.Load(mbFont, mText.name, mText.bytes);
        CharacterInfo[] characterInfo = new CharacterInfo[mbFont.glyphs.Count];
        for (int j = 0; j < mbFont.glyphs.Count; j++)
        {
            BMGlyph bmInfo = mbFont.glyphs[j];
            CharacterInfo info = new CharacterInfo();
            info.index = bmInfo.index;
            info.uv.x = (float)bmInfo.x / (float)mbFont.texWidth;
            info.uv.y = 1 - (float)bmInfo.y / (float)mbFont.texHeight;
            info.uv.width = (float)bmInfo.width / (float)mbFont.texWidth;
            info.uv.height = -1f * (float)bmInfo.height / (float)mbFont.texHeight;
            info.vert.x = (float)bmInfo.offsetX;
            info.vert.y = 0f;
            info.vert.width = (float)bmInfo.width;
            info.vert.height = (float)bmInfo.height;
            info.width = (float)bmInfo.advance;
            characterInfo[j] = info;
        }
        mFont.characterInfo = characterInfo;
        mFont.material = material;
        string newFontPath = txtPath.Replace("txt", "fontsettings");
        string newMatrilPath = txtPath.Replace("txt", "mat");
        AssetDatabase.CreateAsset(material, newMatrilPath);
        AssetDatabase.CreateAsset(mFont, newFontPath);
        AssetDatabase.Refresh();
    }



}