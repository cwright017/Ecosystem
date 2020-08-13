using System;
using System.Collections.Generic;
using UnityEngine;

public static class MeshData
{
    public static List<Vector3> verts = new List<Vector3>();
    public static List<Vector2> uv = new List<Vector2>();
    public static List<int> tris = new List<int>();
    public static List<Vector3> normals = new List<Vector3>();
    public static List<Color> colors = new List<Color>();

    public static void attach(Mesh mesh)
    {
        mesh.SetVertices(verts);
        mesh.SetTriangles(tris, 0, true);
        mesh.SetColors(colors);

        mesh.RecalculateNormals();
    }

    public static void Setup()
    {
        verts.Clear();
        uv.Clear();
        tris.Clear();
        normals.Clear();
        colors.Clear();
    }
}