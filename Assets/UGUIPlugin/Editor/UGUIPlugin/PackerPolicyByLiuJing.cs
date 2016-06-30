/*****************************************************
 * 作者: 刘靖
 * 创建时间：2016.n.n
 * 版本：1.0.0
 * 描述：自定义打包的工具类
 ****************************************************/

using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.Sprites;
using System.Collections.Generic;

public class PackerPolicyByLiuJing : IPackerPolicy
{
    protected class Entry
    {
        public Sprite sprite;
        public AtlasSettings settings;
        public string atlasName;
        public SpritePackingMode packingMode;
    }

    protected enum CustomTextureFormat
    {
        Alpha8 = 1,
        RGB24,
        RGBA32,
        ARGB32,
        R16,
        ETC_RGB4,
        PVRTC_RGB2,
        PVRTC_RGBA2,
        PVRTC_RGB4,
        PVRTC_RGBA4,
    }

    public virtual int GetVersion() { return 1; }

    protected virtual string TagPrefix { get { return "[TIGHT]"; } }

    protected virtual char SignPrefix { get { return '_'; } }

    protected virtual bool AllowTightWhenTagged { get { return true; } }

    public void OnGroupAtlases(BuildTarget target, PackerJob job, int[] textureImporterInstanceIDs)
    {
        List<Entry> entries = new List<Entry>();

        foreach (int instanceID in textureImporterInstanceIDs)
        {
            TextureImporter ti = EditorUtility.InstanceIDToObject(instanceID) as TextureImporter;

            //TextureImportInstructions ins = new TextureImportInstructions();
            //ti.ReadTextureImportInstructions(ins, target);

            TextureImporterSettings tis = new TextureImporterSettings();
            ti.ReadTextureSettings(tis);

            Sprite[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(ti.assetPath).Select(x => x as Sprite).Where(x => x != null).ToArray();
            foreach (Sprite sprite in sprites)
            {
                //在这里设置每个图集的参数
                Entry entry = new Entry();
                entry.sprite = sprite;
                entry.settings.format = (TextureFormat)Enum.Parse(typeof(TextureFormat), ParseTextuerFormat(ti.spritePackingTag).ToString());
                entry.settings.filterMode = FilterMode.Bilinear;
                entry.settings.colorSpace = ColorSpace.Linear;
                entry.settings.compressionQuality = (int)TextureCompressionQuality.Normal;
                entry.settings.filterMode = Enum.IsDefined(typeof(FilterMode), ti.filterMode) ? ti.filterMode : FilterMode.Bilinear;
                entry.settings.maxWidth = ParseTextureWidth(ti.spritePackingTag);
                entry.settings.maxHeight = ParseTextureHeight(ti.spritePackingTag);
                entry.atlasName = ParseAtlasName(ti.spritePackingTag);
                entry.packingMode = GetPackingMode(ti.spritePackingTag, tis.spriteMeshType);

                entries.Add(entry);
            }

            Resources.UnloadAsset(ti);
        }

        var atlasGroups =
            from e in entries
            group e by e.atlasName;
        foreach (var atlasGroup in atlasGroups)
        {
            int page = 0;
            // Then split those groups into smaller groups based on texture settings
            var settingsGroups =
                from t in atlasGroup
                group t by t.settings;
            foreach (var settingsGroup in settingsGroups)
            {
                string atlasName = atlasGroup.Key;
                if (settingsGroups.Count() > 1)
                    atlasName += string.Format(" (Group {0})", page);

                job.AddAtlas(atlasName, settingsGroup.Key);
                foreach (Entry entry in settingsGroup)
                {
                    job.AssignToAtlas(atlasName, entry.sprite, entry.packingMode, SpritePackingRotation.None);
                }
                ++page;
            }
        }
    }

    protected bool IsTagPrefixed(string packingTag)
    {
        packingTag = packingTag.Trim();
        if (packingTag.Length < TagPrefix.Length)
            return false;
        return (packingTag.Substring(0, TagPrefix.Length) == TagPrefix);
    }

    private string ParseAtlasName(string packingTag)
    {
        string name = packingTag.Trim();
        string[] names = name.Split(SignPrefix);
        if (IsTagPrefixed(names[0]))
            name = names[0].Substring(TagPrefix.Length).Trim();
        return (names[0].Length == 0) ? "(unnamed)" : names[0];
    }

    private int ParseTextureWidth(string packingTag)
    {
        string name = packingTag.Trim();
        string[] names = name.Split(SignPrefix);
        if (names.Length >= 3)
        {
            string[] size = names[2].Split('x');
            return int.Parse(size[0]);
        }
        else
        {
            return 1024;
        }
    }

    private int ParseTextureHeight(string packingTag)
    {
        string name = packingTag.Trim();
        string[] names = name.Split(SignPrefix);
        if (names.Length >= 3)
        {
            string[] size = names[2].Split('x');
            return int.Parse(size[1]);
        }
        else
        {
            return 1024;
        }
    }

    private CustomTextureFormat ParseTextuerFormat(string packingTag)
    {
        string name = packingTag.Trim();
        string[] names = name.Split(SignPrefix);
        if (names.Length >= 2)
        {
            return (CustomTextureFormat)Enum.Parse(typeof(CustomTextureFormat), names[1]);         
        }
        else
        {
            return CustomTextureFormat.RGBA32;
        }
    }

    private SpritePackingMode GetPackingMode(string packingTag, SpriteMeshType meshType)
    {
        if (meshType == SpriteMeshType.Tight)
            if (IsTagPrefixed(packingTag) == AllowTightWhenTagged)
                return SpritePackingMode.Tight;
        return SpritePackingMode.Rectangle;
    }
}
