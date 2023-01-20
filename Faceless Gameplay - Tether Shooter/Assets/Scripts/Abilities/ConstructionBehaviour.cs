using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionBehaviour : MonoBehaviour
{
    public KeyCode generate;
    public KeyCode itemSwap;
    public KeyCode editMode;

    public bool rotate = false;

    public GameObject newMesh;

    /// <summary>
    /// Use meters as measure
    /// 
    /// Acts as a "bounding box"
    /// </summary>
    public Vector3 size;
    public Vector3 rotation;

    public float delta;
    public float scalar;

    public Vector3[] newVertices;
    public Vector2[] newUV;
    public int[] newTriangles;

    void Start()
    {
        size = Vector3.one;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(editMode))
        {
            rotate = !rotate;
        }

        MeshPlanning();
    }

    /// <summary>
    /// Receives player input to prepare for mesh generation
    /// </summary>
    void MeshPlanning()
    {
        delta = Input.mouseScrollDelta.y * scalar;

        //Rotation
        if (rotate)
        {
            //X
            if (Input.GetKey(KeyCode.LeftControl))
            {
                rotation += new Vector3(delta, 0, 0);
            }
            //Z
            else if (Input.GetKey(KeyCode.LeftAlt))
            {
                rotation += new Vector3(0, 0, delta);
            }
            //Y
            else
            {
                rotation += new Vector3(0, delta, 0);
            }
        }
        //Scale
        else
        {
            //X
            if (Input.GetKey(KeyCode.LeftControl))
            {
                size += new Vector3(delta, 0, 0);
            }
            //Y
            else if (Input.GetKey(KeyCode.LeftAlt))
            {
                size += new Vector3(0, delta, 0);
            }
            //Z
            else if (Input.GetKey(KeyCode.LeftShift))
            {
                size += new Vector3(0, 0, delta);
            }
            else
            {
                size += new Vector3(delta, delta, delta);
            }
        }

        if (Input.GetKeyDown(generate))
        {

        }
    }

    /// <summary>
    /// Generates the mesh
    /// </summary>
    void GenerateMesh()
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.vertices = newVertices;
        mesh.uv = newUV;
        mesh.triangles = newTriangles;
    }
}
