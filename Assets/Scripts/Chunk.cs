using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    Mesh mesh;
    Material mat;

    List<Vector3> Points;
    List<Vector3> Verts;

    List<int> Tris;
    List<Vector2> UVs;
    public GameObject gameObject;

    float size = 0.5f;
    
    public Chunk(Vector3 position, List<Vector3> data)
    {
        gameObject = new GameObject("Chunk" + position);
        gameObject.transform.position = new Vector3(-0.5f, -0.5f, -0.5f);
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        gameObject.AddComponent<MeshCollider>();

        mat = Resources.Load<Material>("Materials/MyTestMaterial");
        if (mat == null)
        {
            Debug.LogError("Material not found!");
            return;
        }

        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("MeshFilter not found!");
            return;
        }

        mesh = meshFilter.sharedMesh;
        if (mesh == null)
        {
            meshFilter.mesh = new Mesh();
            mesh = meshFilter.sharedMesh;
        }

        MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
        if (meshCollider == null)
        {
            Debug.LogError("meshCollider not found!");
            return;
        }

        mesh.Clear();


        Dictionary<string, Vector3> directions = new Dictionary<string, Vector3>();
        directions.Add("front",     new Vector3( 0,  0, -1));
        directions.Add("back",      new Vector3( 0,  0,  1));
        directions.Add("left",      new Vector3(-1,  0,  0));
        directions.Add("right",     new Vector3( 1,  0,  0));
        directions.Add("top",       new Vector3( 0,  1,  0));
        directions.Add("bottom",    new Vector3( 0, -1,  0));

        Points = new List<Vector3>();
        Verts = new List<Vector3>();
        Tris = new List<int>();
        UVs = new List<Vector2>();

        int i = 0;
        int triIndex = 0;
        foreach (Vector3 item in data)
        {

            Points.Add(new Vector3(item.x -size, item.y + size, item.z -size));
            Points.Add(new Vector3(item.x + size, item.y + size, item.z -size));
            Points.Add(new Vector3(item.x + size, item.y -size, item.z -size));
            Points.Add(new Vector3(item.x -size, item.y -size, item.z -size));

            Points.Add(new Vector3(item.x + size, item.y + size, item.z + size));
            Points.Add(new Vector3(item.x -size, item.y + size, item.z + size));
            Points.Add(new Vector3(item.x -size, item.y -size, item.z + size));
            Points.Add(new Vector3(item.x + size, item.y  -size, item.z + size));

            if (!data.Contains(item + directions["front"]))
            {
               
                Verts.Add(Points[(8 * i) + 0]); Verts.Add(Points[(8 * i) + 1]); Verts.Add(Points[(8 * i) + 2]); Verts.Add(Points[(8 * i) + 3]);

                Tris.Add(triIndex); Tris.Add(triIndex + 1); Tris.Add(triIndex + 2);
                Tris.Add(triIndex + 2); Tris.Add(triIndex + 3); Tris.Add(triIndex);
                triIndex = triIndex + 4;
                UVs.Add(new Vector2(0, 1));
                UVs.Add(new Vector2(1, 1));
                UVs.Add(new Vector2(1, 0));
                UVs.Add(new Vector2(0, 0));
            }
            
            if (!data.Contains(item + directions["back"]))
            {

                Verts.Add(Points[(8 * i) + 4]); Verts.Add(Points[(8 * i) + 5]); Verts.Add(Points[(8 * i) + 6]); Verts.Add(Points[(8 * i) + 7]);
                Tris.Add(triIndex); Tris.Add(triIndex + 1); Tris.Add(triIndex + 2);
                Tris.Add(triIndex + 2); Tris.Add(triIndex + 3); Tris.Add(triIndex);
                triIndex = triIndex + 4;
                UVs.Add(new Vector2(0, 1));
                UVs.Add(new Vector2(1, 1));
                UVs.Add(new Vector2(1, 0));
                UVs.Add(new Vector2(0, 0));
            }
            
            if (!data.Contains(item + directions["left"]))
            {

                Verts.Add(Points[(8 * i) + 5]); Verts.Add(Points[(8 * i) + 0]); Verts.Add(Points[(8 * i) + 3]); Verts.Add(Points[(8 * i) + 6]);
                Tris.Add(triIndex); Tris.Add(triIndex + 1); Tris.Add(triIndex + 2);
                Tris.Add(triIndex + 2); Tris.Add(triIndex + 3); Tris.Add(triIndex);
                triIndex = triIndex + 4;
                UVs.Add(new Vector2(0, 1));
                UVs.Add(new Vector2(1, 1));
                UVs.Add(new Vector2(1, 0));
                UVs.Add(new Vector2(0, 0));
            }
            
            if (!data.Contains(item + directions["right"]))
            {

                Verts.Add(Points[(8 * i) + 1]); Verts.Add(Points[(8 * i) + 4]); Verts.Add(Points[(8 * i) + 7]); Verts.Add(Points[(8 * i) + 2]);
                Tris.Add(triIndex); Tris.Add(triIndex + 1); Tris.Add(triIndex + 2);
                Tris.Add(triIndex + 2); Tris.Add(triIndex + 3); Tris.Add(triIndex);
                triIndex = triIndex + 4;
                UVs.Add(new Vector2(0, 1));
                UVs.Add(new Vector2(1, 1));
                UVs.Add(new Vector2(1, 0));
                UVs.Add(new Vector2(0, 0));
            }
            
            if (!data.Contains(item + directions["top"]))
            {

                Verts.Add(Points[(8 * i) + 5]); Verts.Add(Points[(8 * i) + 4]); Verts.Add(Points[(8 * i) + 1]); Verts.Add(Points[(8 * i) + 0]);
                Tris.Add(triIndex); Tris.Add(triIndex + 1); Tris.Add(triIndex + 2);
                Tris.Add(triIndex + 2); Tris.Add(triIndex + 3); Tris.Add(triIndex);
                triIndex = triIndex + 4;
                UVs.Add(new Vector2(0, 1));
                UVs.Add(new Vector2(1, 1));
                UVs.Add(new Vector2(1, 0));
                UVs.Add(new Vector2(0, 0));
            }
           
            if (!data.Contains(item + directions["bottom"]))
            {

                Verts.Add(Points[(8 * i) + 3]); Verts.Add(Points[(8 * i) + 2]); Verts.Add(Points[(8 * i) + 7]); Verts.Add(Points[(8 * i) + 6]);
                Tris.Add(triIndex); Tris.Add(triIndex + 1); Tris.Add(triIndex + 2);
                Tris.Add(triIndex + 2); Tris.Add(triIndex + 3); Tris.Add(triIndex);
                triIndex = triIndex + 4;
                UVs.Add(new Vector2(0, 1));
                UVs.Add(new Vector2(1, 1));
                UVs.Add(new Vector2(1, 0));
                UVs.Add(new Vector2(0, 0));
            }
           
            i++;
        }

        mesh.vertices = Verts.ToArray();
        mesh.triangles = Tris.ToArray();
        mesh.uv = UVs.ToArray();

        Verts.Clear();
        Tris.Clear();
        UVs.Clear();
        
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = mesh;

        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        renderer.material = mat;
        mesh.Optimize();
    }
}
