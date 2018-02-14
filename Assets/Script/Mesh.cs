using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mesh : MonoBehaviour {

	// Use this for initialization
	void Awake () {
        GameObject obj = GameObject.Find("Cube");
        UnityEngine.Mesh mesh = obj.GetComponent<MeshFilter>().mesh;

        GameObject newObj = new GameObject();
        MeshFilter filter = newObj.AddComponent<MeshFilter>();
        MeshRenderer renderer = newObj.AddComponent<MeshRenderer>();

        UnityEngine.Mesh newMesh = new UnityEngine.Mesh();
        newMesh.vertices = mesh.vertices;
        newMesh.triangles = mesh.triangles;
        filter.mesh = newMesh;
        
        renderer.material = new Material(Shader.Find("Transparent/Diffuse"));
        renderer.material.color = Color.green;
    }
}
