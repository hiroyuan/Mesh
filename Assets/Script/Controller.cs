﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour {
    public List<GameObject> list = new List<GameObject>();

    public Bounds bounds;
    public Transform trans;
    public MeshHolder meshHolder;
    public UnityEngine.Mesh mesh;

    public BoundHolder[] subBounds;
    public GameObject indicator;

    public int xAxisSplitter;
    public int yAxisSplitter;
    public int zAxisSplitter;

    private int xDirCount;
    private int yDirCount;
    private int zDirCount;

    private readonly int SPHERE_COUNT = 10000;

    // Use this for initialization
    void Start()
    {
        xDirCount = xAxisSplitter;
        yDirCount = yAxisSplitter;
        zDirCount = zAxisSplitter;

        GenerateSphereRamdomPos();

        addToList();
        //foreach (GameObject g in list)
        //{
        //    g.GetComponent<Renderer>().enabled = false;
        //}
        bounds = DrawBoundingBox(list);

        meshHolder = new MeshHolder();
        mesh = new UnityEngine.Mesh();
        subBounds = new BoundHolder[xDirCount * yDirCount * zDirCount];
        indicator = GameObject.Find("IndicatorObject");
    }

    private void Update()
    {
        xDirCount = xAxisSplitter;
        yDirCount = yAxisSplitter;
        zDirCount = zAxisSplitter;
        subBounds = new BoundHolder[xDirCount * yDirCount * zDirCount];
        SplitBounds();
        ActivateSubBoundsByIndicator();
        DisplayMeshesOnActiveBounds();
    }

    void OnDrawGizmos()
    {
        //Gizmos.DrawWireCube(bounds.center, bounds.size);
        if (subBounds != null)
        {
            for (int i = 0; i < subBounds.Length; i++)
            {
                Gizmos.DrawWireCube(subBounds[i].GetBounds().center, subBounds[i].GetBounds().size);
            }
            //Gizmos.DrawCube(subBounds[3].GetBounds().center, subBounds[3].GetBounds().size);
        }
    }

    public void addToList()
    {
        //for (int i = 0; i < 10; i++)
        //{
        //    if (i == 0)
        //        list.Add(GameObject.Find("Cube"));
        //    else
        //        list.Add(GameObject.Find("Cube (" + i + ")"));
        //}
        for (int i = 0; i < SPHERE_COUNT; i++)
        {
            list.Add(GameObject.Find("Sphere" + i));
        }
    }

    public void CombineMeshes()
    {
        int arrayCount = 0;
        foreach (GameObject g in list)
        {
            trans = g.transform;
            UnityEngine.Mesh m = g.GetComponent<MeshFilter>().mesh;
            Vector3[] vertices = transformToWorldPoint(m.vertices);
            Vector3[] normals = transformToWorldPoint(m.normals);
            Color[] colors = m.colors;
            int[] triangles = m.triangles;
            Vector2[] uv = m.uv;

            arrayCount = meshHolder.CombineMeshElement(vertices, normals, colors, triangles, arrayCount, uv);
        }
    }

    public void DrawNewMesh()
    {
        GameObject newGameObject = new GameObject();
        MeshFilter mf = newGameObject.AddComponent<MeshFilter>();
        mf.name = "NewMesh";
        MeshRenderer mr = newGameObject.AddComponent<MeshRenderer>();

        mesh.vertices = meshHolder.GetVertices();
        mesh.normals = meshHolder.GetNormals();
        mesh.colors = meshHolder.GetColors();
        mesh.triangles = meshHolder.GetTriangles();
        mesh.uv = meshHolder.GetUVs();

        mf.mesh = mesh;
        mr.material = new Material(Shader.Find("Transparent/Diffuse"));
    }

    public void MergeIntoParent()
    {
        GameObject gameObject = new GameObject();
        //gameObject.AddComponent<MeshFilter>();
        //gameObject.AddComponent<MeshRenderer>();

        foreach(GameObject g in list)
        {
            g.transform.parent = gameObject.transform;
        }
    }

    /// <summary>
    /// This function finds the bounding box for all game objects in the List
    /// passed into the parameter.
    /// Referenced from Unity community and API.
    /// https://answers.unity.com/questions/777855/bounds-finding-box.html
    /// https://docs.unity3d.com/ScriptReference/Bounds.html
    /// </summary>
    /// <param name="l"></param>
    /// <returns>bounds</returns>
    public static Bounds DrawBoundingBox(List<GameObject> l)
    {
        if (l.Count == 0)
        {
            return new Bounds(Vector3.zero, Vector3.one);
        }

        float minX = Mathf.Infinity;
        float maxX = -Mathf.Infinity;
        float minY = Mathf.Infinity;
        float maxY = -Mathf.Infinity;
        float minZ = Mathf.Infinity;
        float maxZ = -Mathf.Infinity;

        Vector3[] points = new Vector3[8];

        foreach (GameObject go in l)
        {
            getBoundsPointsNoAlloc(go, points);
            foreach (Vector3 v in points)
            {
                if (v.x < minX) minX = v.x;
                if (v.x > maxX) maxX = v.x;
                if (v.y < minY) minY = v.y;
                if (v.y > maxY) maxY = v.y;
                if (v.z < minZ) minZ = v.z;
                if (v.z > maxZ) maxZ = v.z;
            }
        }

        float sizeX = maxX - minX;
        float sizeY = maxY - minY;
        float sizeZ = maxZ - minZ;

        Vector3 center = new Vector3(minX + sizeX / 2.0f, minY + sizeY / 2.0f, minZ + sizeZ / 2.0f);

        return new Bounds(center, new Vector3(sizeX, sizeY, sizeZ));
    }

    public void DestroyObjInList()
    {
        foreach (GameObject g in list)
        {
            Destroy(g); 
        }
    }

    /// <summary>
    /// This function splits the bounds into 8 sub parts.
    /// </summary>
    public void SplitBounds()
    {
        float sizeX = (bounds.max.x - bounds.min.x) / xDirCount;
        float sizeY = (bounds.max.y - bounds.min.y) / yDirCount;
        float sizeZ = (bounds.max.z - bounds.min.z) / zDirCount;
        Vector3 subSize = new Vector3(sizeX, sizeY, sizeZ);

        Vector3 subCenter;
        int index = 0;
        float xStartPoint = bounds.min.x;
        float yStartPoint = bounds.min.y;
        float zStartPoint = bounds.min.z;
        for (int i = 0; i < xDirCount; i++)
        {
            for (int j = 0; j < yDirCount; j++)
            {
                for (int k = 0; k < zDirCount; k++)
                {
                    subCenter = new Vector3(xStartPoint + sizeX / 2, yStartPoint + sizeY / 2, zStartPoint + sizeZ / 2);
                    subBounds[index++] = new BoundHolder(subCenter, subSize);
                    zStartPoint += sizeZ;
                }
                zStartPoint = bounds.min.z;
                yStartPoint += sizeY;
            }
            zStartPoint = bounds.min.z;
            yStartPoint = bounds.min.y;
            xStartPoint += sizeX;
        }
    }

    public void DisplayMeshesOnActiveBounds()
    {
        for (int i = 0; i < subBounds.Length; i++)
        {
            if (subBounds[i].GetStatus())
            {
                foreach (GameObject g in list)
                {
                    if (subBounds[i].CheckIntersects(g.transform.position))
                    {
                        g.GetComponent<Renderer>().enabled = true;
                    }
                    else
                    {
                        g.GetComponent<Renderer>().enabled = false;
                    }
                }
            }
        }
    }

    public void ActivateSubBoundsByIndicator()
    {
        for (int i = 0; i < subBounds.Length; i++)
        {
            if (subBounds[i].CheckIntersects(indicator.transform.position))
            {
                subBounds[i].SetStatus(true);
            }
            else
            {
                subBounds[i].SetStatus(false);
            }
        }   
    }

    public void GenerateSphereRamdomPos()
    {
        for (int i = 0; i < SPHERE_COUNT; i++)
        {
            float x = Random.Range(-10, 10);
            float y = Random.Range(-10, 10);
            float z = Random.Range(-10, 10);
            Vector3 pos = new Vector3(x, y, z);
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.name = "Sphere" + i;
            sphere.transform.position = pos;
        }
    }

    /// <summary>
    /// This function is support function of drawBoundingBox.
    /// Referenced from Unity community.
    /// https://answers.unity.com/questions/777855/bounds-finding-box.html
    /// https://docs.unity3d.com/ScriptReference/Bounds.html
    /// </summary>
    /// <param name="go"></param>
    /// <param name="points"></param>
    private static void getBoundsPointsNoAlloc(GameObject go, Vector3[] points)
    {
        if (points == null || points.Length < 8)
        {
            Debug.Log("Bad Array");
            return;
        }
        MeshFilter mf = go.GetComponent<MeshFilter>();
        if (mf == null)
        {
            Debug.Log("No MeshFilter on object");
            for (int i = 0; i < points.Length; i++)
                points[i] = go.transform.position;
            return;
        }

        Transform tr = go.transform;

        Vector3 v3Center = mf.mesh.bounds.center;
        Vector3 v3ext = mf.mesh.bounds.extents;

        points[0] = tr.TransformPoint(new Vector3(v3Center.x - v3ext.x, v3Center.y + v3ext.y, v3Center.z - v3ext.z));  // Front top left corner
        points[1] = tr.TransformPoint(new Vector3(v3Center.x + v3ext.x, v3Center.y + v3ext.y, v3Center.z - v3ext.z));  // Front top right corner
        points[2] = tr.TransformPoint(new Vector3(v3Center.x - v3ext.x, v3Center.y - v3ext.y, v3Center.z - v3ext.z));  // Front bottom left corner
        points[3] = tr.TransformPoint(new Vector3(v3Center.x + v3ext.x, v3Center.y - v3ext.y, v3Center.z - v3ext.z));  // Front bottom right corner
        points[4] = tr.TransformPoint(new Vector3(v3Center.x - v3ext.x, v3Center.y + v3ext.y, v3Center.z + v3ext.z));  // Back top left corner
        points[5] = tr.TransformPoint(new Vector3(v3Center.x + v3ext.x, v3Center.y + v3ext.y, v3Center.z + v3ext.z));  // Back top right corner
        points[6] = tr.TransformPoint(new Vector3(v3Center.x - v3ext.x, v3Center.y - v3ext.y, v3Center.z + v3ext.z));  // Back bottom left corner
        points[7] = tr.TransformPoint(new Vector3(v3Center.x + v3ext.x, v3Center.y - v3ext.y, v3Center.z + v3ext.z));  // Back bottom right corner
    }

    private Vector3[] transformToWorldPoint(Vector3[] array)
    {
        Vector3[] worldPosArray = new Vector3[array.Length];
        for (int index = 0; index < array.Length; index++)
        {
            worldPosArray[index] = trans.TransformPoint(array[index]);
        }

        return worldPosArray;
    }
}
