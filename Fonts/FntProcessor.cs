using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

public class FntProcessor : AssetPostprocessor
{
    public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath)
    {
        foreach (var filename in importedAssets)
        {
            if (Path.GetExtension(filename).ToLower() == ".fnt")
                ParseFnt(filename);
        }
    }

    static string BuildFilenameWithExtension(string filename, string ext)
    {
        return string.Format("{0}/{1}.{2}", Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename), ext);
    }

    static string BuildFilenameInSamePath(string filenameWithPath, string targetFilename)
    {
        return string.Format("{0}/{1}", Path.GetDirectoryName(filenameWithPath), targetFilename);
    }

    static void ParseFnt(string filename)
    {
        var txt = File.ReadAllText(filename);

        var common = Regex.Matches(txt, "^common lineHeight=([0-9]+) base=([0-9]+) scaleW=([0-9]+) scaleH=([0-9]+)", RegexOptions.Multiline)
            .OfType<Match>()
            .Select(m => m.Groups.OfType<Group>().Select(g => g.Value).Skip(1).Select(x => int.Parse(x)).ToArray())
            .First();

        var page = Regex.Matches(txt, "^page id=0 file=\"([^\"]+)\"", RegexOptions.Multiline)
            .OfType<Match>()
            .Select(m => m.Groups[1].Value)
            .First();

        // Create and set the material
        var atlas = AssetDatabase.LoadAssetAtPath<Texture2D>(BuildFilenameInSamePath(filename, page));
        var materialFilename = BuildFilenameWithExtension(BuildFilenameInSamePath(filename, page), "mat");
        Material material = AssetDatabase.LoadAssetAtPath<Material>(materialFilename);
        bool matWasCreated = false;
        if (material == null)
        {
            var shader = AssetDatabase.LoadAssetAtPath<Shader>("Assets/TextMesh Pro/Resources/Shaders/TMP_Sprite.shader");
            material = new Material(shader);
            matWasCreated = true;
        }
        material.SetTexture("_MainTex", atlas);

        if (matWasCreated)
            AssetDatabase.CreateAsset(material, materialFilename);
        else
            AssetDatabase.SaveAssets();

        var fntFilename = BuildFilenameWithExtension(filename, "asset");
        bool fntWasCreated = false;
        var fnt = AssetDatabase.LoadAssetAtPath<TMPro.TMP_FontAsset>(fntFilename);
        if (fnt == null)
        {
            fnt = new TMPro.TMP_FontAsset();
            fntWasCreated = true;
        }

        // Create the atlas asset
        fnt.atlas = atlas;
        fnt.material = AssetDatabase.LoadAssetAtPath<Material>(materialFilename);
        
        // Build the face and glyphs
        fnt.AddFaceInfo(new TMPro.FaceInfo()
        {
            AtlasWidth = fnt.atlas.width,
            AtlasHeight = fnt.atlas.height,
            Baseline = common[1],
            Ascender = common[3],
            Descender = 0,
            CharacterCount = 42,
            LineHeight = common[0],
            Name = Path.GetFileNameWithoutExtension(page),
            Scale = 1,
            PointSize = 24,
            Padding = 4
        });
        fnt.AddGlyphInfo(
            Regex.Matches(txt, "^char id=([0-9]+) x=([0-9]+) y=([0-9]+) width=([0-9]+) height=([0-9]+) xoffset=([0-9]+) yoffset=([0-9]+) xadvance=([0-9]+) page=([0-9]+) chnl=([0-9]+)", RegexOptions.Multiline)
                .OfType<Match>()
                .Select(m => m.Groups.OfType<Group>().Select(g => g.Value).Skip(1).Select(x => int.Parse(x)).ToArray())
                .Select(i => new TMPro.TMP_Glyph()
                {
                    id = i[0],
                    x = i[1],
                    y = i[2],
                    width = i[3],
                    height = i[4],
                    xOffset = i[5],
                    yOffset = i[6],
                    xAdvance = i[7],
                    scale = 1
                }).ToArray()
            );

        if (fntWasCreated)
            AssetDatabase.CreateAsset(fnt, fntFilename);
        else
            AssetDatabase.SaveAssets();
    }
}
