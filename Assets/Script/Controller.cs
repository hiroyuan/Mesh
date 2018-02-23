using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour {
    public Button btn;
    public List<GameObject> list = new List<GameObject>();
    public Bounds bounds;
    public MeshHolder meshHolder;
    public UnityEngine.Mesh mesh;

    public List<Vector3> totalVertices;
    public List<Vector3> totalNormals;
    public List<Color32> totalColors;
    public List<int> totalTriangleIndices;
    public List<Vector2> totalUVs;
    public List<Vector2> totalUV2s;

    // Use this for initialization
    void Start ()
    {
        Button button = btn.GetComponent<Button>();
        button.onClick.AddListener(TaskOnClick);
    }
	
	// Update is called once per frame
	void TaskOnClick ()
    {
        Debug.Log("You have clicked the button!");

        //mergeIntoParent();
        //bounds = drawBoundingBox(list);
        mergeMeshElements();
        drawNewMesh();
    }

    /*
    void OnDrawGizmos()
    {
        Gizmos.DrawCube(bounds.center, bounds.size);
    }
    */

    private void mergeMeshElements()
    {
        for (int i = 0; i < 10; i++)
        {
            if (i == 0)
                list.Add(GameObject.Find("Cube"));
            else
                list.Add(GameObject.Find("Cube (" + i + ")"));
        }


        foreach (GameObject g in list)
        {
            meshHolder = new MeshHolder(g);
            totalVertices.AddRange(meshHolder.GetVertices());
            totalNormals.AddRange(meshHolder.GetNormals());
            totalColors.AddRange(meshHolder.GetColors());
            totalTriangleIndices.AddRange(meshHolder.GetTriangleIndices());
            totalUVs.AddRange(meshHolder.GetUVs());
            totalUV2s.AddRange(meshHolder.GetUV2s());
        }
    }

    private void drawNewMesh()
    {
        GameObject newGameObject = new GameObject();
        MeshFilter mf = newGameObject.AddComponent<MeshFilter>();
        MeshRenderer mr = newGameObject.AddComponent<MeshRenderer>();

        UnityEngine.Mesh newMesh = new UnityEngine.Mesh();
        newMesh.vertices = totalVertices.ToArray();
        newMesh.normals = totalNormals.ToArray();
        newMesh.colors32 = totalColors.ToArray();
        newMesh.triangles = totalTriangleIndices.ToArray();
        newMesh.uv = totalUVs.ToArray();

        mf.mesh = newMesh;
        mr.material = new Material(Shader.Find("Transparent/Diffuse"));
        mr.material.color = Color.green;
    }

    private void mergeIntoParent()
    {
        for (int i = 0; i < 10; i++)
        {
            if (i == 0)
                list.Add(GameObject.Find("Cube"));
            else
                list.Add(GameObject.Find("Cube (" + i + ")"));
        }

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
    private static Bounds drawBoundingBox(List<GameObject> l)
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
}
