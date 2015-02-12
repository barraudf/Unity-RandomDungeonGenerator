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
    }
}
