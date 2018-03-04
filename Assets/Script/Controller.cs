using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour {
    public Button btn;
    public List<GameObject> list = new List<GameObject>();

    public Bounds bounds;
    public Transform trans;
    public MeshHolder meshHolder;
    public UnityEngine.Mesh mesh;

    public Bounds subBounds1;
    public Bounds subBounds2;
    public Bounds subBounds3;
    public Bounds subBounds4;
    public Bounds subBounds5;
    public Bounds subBounds6;
    public Bounds subBounds7;
    public Bounds subBounds8;

    // Use this for initialization
    void Start ()
    {
        meshHolder = new MeshHolder();
        mesh = new UnityEngine.Mesh();
        Button button = btn.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }
	
	// Update is called once per frame
	void TaskOnClick ()
    {
        Debug.Log("You have clicked the button!");

        addToList();
        //mergeIntoParent();
        bounds = DrawBoundingBox(list);
        CombineMeshes();
        DrawNewMesh();
        //DestroyObjInList();
        SplitBounds();
        //SplitMesh();
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(bounds.center, bounds.size);
        Gizmos.DrawCube(subBounds1.center, subBounds1.size);
        //Gizmos.DrawCube(subBounds2.center, subBounds2.size);
        //Gizmos.DrawCube(subBounds3.center, subBounds3.size);
        //Gizmos.DrawCube(subBounds4.center, subBounds4.size);
        //Gizmos.DrawCube(subBounds5.center, subBounds5.size);
        //Gizmos.DrawCube(subBounds6.center, subBounds6.size);
        //Gizmos.DrawCube(subBounds7.center, subBounds7.size);
        //Gizmos.DrawCube(subBounds8.center, subBounds8.size);
    }

    public void addToList()
    {
        for (int i = 0; i < 10; i++)
        {
            if (i == 0)
                list.Add(GameObject.Find("Cube"));
            else
                list.Add(GameObject.Find("Cube (" + i + ")"));
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
        float sizeX = (bounds.max.x - bounds.min.x) / 2;
        float sizeY = (bounds.max.y - bounds.min.y) / 2;
        float sizeZ = (bounds.max.z - bounds.min.z) / 2;

        Vector3 subSize = new Vector3(sizeX, sizeY, sizeZ);

        // subBounds1
        Vector3 subCenter1 = new Vector3(bounds.min.x + sizeX / 2, bounds.min.y + sizeY / 2, bounds.min.z + sizeZ / 2);
        subBounds1 = new Bounds(subCenter1, subSize);

        // subBounds2
        Vector3 subCenter2 = new Vector3(bounds.max.x - sizeX / 2, bounds.min.y + sizeY / 2, bounds.min.z + sizeZ / 2);
        subBounds2 = new Bounds(subCenter2, subSize);

        // subBounds3
        Vector3 subCenter3 = new Vector3(bounds.min.x + sizeX / 2, bounds.max.y - sizeY / 2, bounds.min.z + sizeZ / 2);
        subBounds3 = new Bounds(subCenter3, subSize);

        // subBounds4
        Vector3 subCenter4 = new Vector3(bounds.max.x - sizeX / 2, bounds.max.y - sizeY / 2, bounds.min.z + sizeZ / 2);
        subBounds4 = new Bounds(subCenter4, subSize);

        // subBounds5
        Vector3 subCenter5 = new Vector3(bounds.min.x + sizeX / 2, bounds.min.y + sizeY / 2, bounds.max.z - sizeZ / 2);
        subBounds5 = new Bounds(subCenter5, subSize);

        // subBounds6
        Vector3 subCenter6 = new Vector3(bounds.max.x - sizeX / 2, bounds.min.y + sizeY / 2, bounds.max.z - sizeZ / 2);
        subBounds6 = new Bounds(subCenter6, subSize);

        // subBounds7
        Vector3 subCenter7 = new Vector3(bounds.min.x + sizeX / 2, bounds.max.y - sizeY / 2, bounds.max.z - sizeZ / 2);
        subBounds7 = new Bounds(subCenter7, subSize);

        // subBounds8
        Vector3 subCenter8 = new Vector3(bounds.max.x - sizeX / 2, bounds.max.y - sizeY / 2, bounds.max.z - sizeZ / 2);
        subBounds8 = new Bounds(subCenter8, subSize);
    }

    public void SplitMesh()
    {
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            if( subBounds1.Contains(mesh.vertices[i]) )
            {

            }
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
