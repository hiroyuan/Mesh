using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshHolder {
    private Vector3[] vertices;
    private Vector3[] normals;
    private Color32[] colors;
    private int[] triangleIndices;
    private Vector2[] uvs;
    private Vector2[] uv2s;

    private Transform transform;

    public MeshHolder(GameObject g)
    {
        vertices = g.GetComponent<MeshFilter>().mesh.vertices;
        normals = g.GetComponent<MeshFilter>().mesh.normals;
        colors = g.GetComponent<MeshFilter>().mesh.colors32;
        triangleIndices = g.GetComponent<MeshFilter>().mesh.triangles;
        uvs = g.GetComponent<MeshFilter>().mesh.uv;
        uv2s = g.GetComponent<MeshFilter>().mesh.uv2;
        transform = g.transform;
        transform.position.Set(g.transform.position.x, g.transform.position.y, g.transform.position.z);
        transform.rotation.Set(g.transform.rotation.x, g.transform.rotation.y, g.transform.rotation.z, g.transform.rotation.w);
    }

    public Vector3[] GetVertices()
    {
        return vertices;
    }

    public Vector3[] GetNormals()
    {
        return normals;
    }

    public Color32[] GetColors()
    {
        return colors;
    }

    public int[] GetTriangleIndices()
    {
        return triangleIndices;
    }

    public Vector2[] GetUVs()
    {
        return uvs;
    }

    public Vector2[] GetUV2s()
    {
        return uv2s;
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
