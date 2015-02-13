using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(Sample2DRenderer))]
public class Sample2DInspector : Editor
{

    public override void OnInspectorGUI()
    {
        Sample2DRenderer r = (Sample2DRenderer)target;
        DrawDefaultInspector();
        if(GUILayout.Button("Renegerate"))
        {
            dungeon d = r.GenerateDungeon();
            r.BuildMesh(d);
        }
        if(GUILayout.Button("New seed"))
        {
            r.Seed = System.Environment.TickCount;
        }
        if(GUILayout.Button("Export To PNG"))
        {
            string path = EditorUtility.SaveFilePanel(
                    "Save texture as PNG",
                    "",
                    "Dungeon.png",
                    "png");

            if (path.Length != 0)
            {
                Texture2D tex = r.MeshRenderer.sharedMaterials[0].mainTexture as Texture2D;

                // Convert the texture to a format compatible with EncodeToPNG
                if (tex.format != TextureFormat.ARGB32 && tex.format != TextureFormat.RGB24)
                {
                    Texture2D newTexture = new Texture2D(tex.width, tex.height);
                    newTexture.SetPixels(tex.GetPixels(0), 0);
                    tex = newTexture;
                }
                byte[] pngData = tex.EncodeToPNG();
                if (pngData != null)
                    System.IO.File.WriteAllBytes(path, pngData);
            }
        }
    }
}
