using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> source: http://ilkinulas.github.io/development/unity/2016/04/30/cube-mesh-in-unity3d.html </summary>
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(BoxCollider))]
public class StoreBehavior : MonoBehaviour
{
    public Vector3 size;
    public Material material;

    // Start is called before the first frame update
    void Start()
    {
        CreateBox();
    }



    void CreateBox()
    {
        Vector3[] vertices = {
            new Vector3 (0, 0, 0),
            new Vector3 (size.x, 0, 0) ,
            new Vector3 (size.x, size.y, 0),
            new Vector3 (0, size.y, 0),
            new Vector3 (0, size.y, size.z),
            new Vector3 (size.x, size.y, size.z),
            new Vector3 (size.x, 0, size.z),
            new Vector3 (0, 0, size.z),
        };

        int[] triangles = {
            0, 2, 1, //face front
			0, 3, 2,
            2, 3, 4, //face top
			2, 4, 5,
            1, 2, 5, //face right
			1, 5, 6,
            0, 7, 4, //face left
			0, 4, 3,
            5, 4, 7, //face back
			5, 7, 6,
            0, 6, 7, //face bottom
			0, 1, 6
        };

        GetComponent<BoxCollider>().size = size;
        GetComponent<BoxCollider>().center = size * 0.5f;
        GetComponent<MeshRenderer>().material = material;
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.Optimize();
        mesh.RecalculateNormals();
    }
}
