using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeshData
{
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public NavMeshSurface navMeshSurface;
    public MeshCollider meshCollider;

    public Mesh mesh;

    public List<Vector3> verts = new List<Vector3>();
    public List<Vector2> uv = new List<Vector2>();
    public List<int> tris = new List<int>();
    public List<Vector3> normals = new List<Vector3>();
    public List<Color> colors = new List<Color>();

    public MeshData(GameObject holder, bool addNavMesh = false)
    {
        meshFilter = holder.AddComponent<MeshFilter>();

        meshRenderer = holder.AddComponent<MeshRenderer>();
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        if (meshFilter.sharedMesh == null)
        {
            mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            meshFilter.sharedMesh = mesh;
        }
        else
        {
            mesh = meshFilter.sharedMesh;
            mesh.Clear();
        }

        if (addNavMesh)
        {
            navMeshSurface = holder.AddComponent<NavMeshSurface>();
            navMeshSurface.layerMask = LayerMask.GetMask("Default");
        }

        meshCollider = holder.AddComponent<MeshCollider>();
    }

    public void attach()
    {
        mesh.SetVertices(verts);
        mesh.SetTriangles(tris, 0, true);
        mesh.SetColors(colors);

        mesh.RecalculateNormals();
    }
}