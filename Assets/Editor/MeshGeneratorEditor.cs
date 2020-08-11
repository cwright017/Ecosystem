using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (MeshGenerator))]
public class MeshGeneratorEditor : Editor {
    MeshGenerator meshGenerator;

    public override void OnInspectorGUI () {
        DrawDefaultInspector ();

        if (GUILayout.Button ("Generate")) {
            meshGenerator.Generate();
        }
    }

    void OnEnable () {
        meshGenerator = (MeshGenerator) target;
    }
}