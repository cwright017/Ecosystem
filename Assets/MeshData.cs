using System;
using System.Collections.Generic;
using UnityEngine;

public class MeshData
{
    public List<Vector3> verts = new List<Vector3>();
    public List<Vector2> uv = new List<Vector2>();
    public List<int> tris = new List<int>();
    public List<Vector3> normals = new List<Vector3>();
    public List<Color> colors = new List<Color>();

    public void attach(Mesh mesh)
    {
        mesh.SetVertices(verts);
        mesh.SetTriangles(tris, 0, true);
        mesh.SetColors(colors);

        mesh.RecalculateNormals();
    }
}