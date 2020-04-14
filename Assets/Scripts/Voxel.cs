﻿using System.Collections.Generic;
using UnityEngine;

public class Voxel
{

    Mesh mesh;
    Material mat;

    List<Vector3> Points;
    List<Vector3> Verts;

    List<int> Tris;
    List<Vector2> UVs;
    GameObject gameObject;

    float size = 0.5f;


    public void Start(string name, Vector3 position)
    {
        Debug.Log("Started");
        gameObject = new GameObject(name);
        gameObject.transform.SetPositionAndRotation(position, Quaternion.identity);

        Points = new List<Vector3>();
        Points.Add(new Vector3(-size, size, -size));
        Points.Add(new Vector3(size, size, -size));
        Points.Add(new Vector3(size, -size, -size));
        Points.Add(new Vector3(-size, -size, -size));

        Points.Add(new Vector3(size, size, size));
        Points.Add(new Vector3(-size, size, size));
        Points.Add(new Vector3(-size, -size, size));
        Points.Add(new Vector3(size, -size, size));

        Verts = new List<Vector3>();
        Tris = new List<int>();
        UVs = new List<Vector2>();

        CreateMesh();
    }

    private void CreateMesh()
    {
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

        UpdateMesh();
    }

    private void UpdateMesh()
    {
        // front
        Verts.Add(Points[0]); Verts.Add(Points[1]); Verts.Add(Points[2]); Verts.Add(Points[3]);
        // back
        Verts.Add(Points[4]); Verts.Add(Points[5]); Verts.Add(Points[6]); Verts.Add(Points[7]);
        // left
        Verts.Add(Points[5]); Verts.Add(Points[0]); Verts.Add(Points[3]); Verts.Add(Points[6]);
        // right
        Verts.Add(Points[1]); Verts.Add(Points[4]); Verts.Add(Points[7]); Verts.Add(Points[2]);
        // top
        Verts.Add(Points[5]); Verts.Add(Points[4]); Verts.Add(Points[1]); Verts.Add(Points[0]);
        // bottom
        Verts.Add(Points[3]); Verts.Add(Points[2]); Verts.Add(Points[7]); Verts.Add(Points[6]);

        // Front Plane
        Tris.Add(0); Tris.Add(1); Tris.Add(2);
        Tris.Add(2); Tris.Add(3); Tris.Add(0);
        // Back Plane
        Tris.Add(4); Tris.Add(5); Tris.Add(6);
        Tris.Add(6); Tris.Add(7); Tris.Add(4);
        // Left Plane
        Tris.Add(8); Tris.Add(9); Tris.Add(10);
        Tris.Add(10); Tris.Add(11); Tris.Add(8);
        // Right Plane
        Tris.Add(12); Tris.Add(13); Tris.Add(14);
        Tris.Add(14); Tris.Add(15); Tris.Add(12);
        // Top Plane
        Tris.Add(16); Tris.Add(17); Tris.Add(18);
        Tris.Add(18); Tris.Add(19); Tris.Add(16);
        // Bottom Plane
        Tris.Add(20); Tris.Add(21); Tris.Add(22);
        Tris.Add(22); Tris.Add(23); Tris.Add(20);

        // Front Plane
        UVs.Add(new Vector2(0, 1));
        UVs.Add(new Vector2(1, 1));
        UVs.Add(new Vector2(1, 0));
        UVs.Add(new Vector2(0, 0));
        // Back Plane
        UVs.Add(new Vector2(0, 1));
        UVs.Add(new Vector2(1, 1));
        UVs.Add(new Vector2(1, 0));
        UVs.Add(new Vector2(0, 0));
        // Left Plane
        UVs.Add(new Vector2(0, 1));
        UVs.Add(new Vector2(1, 1));
        UVs.Add(new Vector2(1, 0));
        UVs.Add(new Vector2(0, 0));
        // Right Plane
        UVs.Add(new Vector2(0, 1));
        UVs.Add(new Vector2(1, 1));
        UVs.Add(new Vector2(1, 0));
        UVs.Add(new Vector2(0, 0));
        // Top Plane
        UVs.Add(new Vector2(0, 1));
        UVs.Add(new Vector2(1, 1));
        UVs.Add(new Vector2(1, 0));
        UVs.Add(new Vector2(0, 0));
        // Bottom Plane
        UVs.Add(new Vector2(0, 1));
        UVs.Add(new Vector2(1, 1));
        UVs.Add(new Vector2(1, 0));
        UVs.Add(new Vector2(0, 0));

        mesh.vertices = Verts.ToArray();
        mesh.triangles = Tris.ToArray();
        mesh.uv = UVs.ToArray();
    

        Verts.Clear();
        Tris.Clear();
        UVs.Clear();

        MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        RecalculateTangents(mesh);
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = mesh;

        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        renderer.material = mat;
        mesh.Optimize();
    }
    
    private static void RecalculateTangents(Mesh mesh)
    {
        int[] triangles = mesh.triangles;
        Vector3[] vertices = mesh.vertices;
        Vector2[] uv = mesh.uv;
        Vector3[] normals = mesh.normals;

        int triangleCount = triangles.Length;
        int vertexCount = vertices.Length;

        Vector3[] tan1 = new Vector3[vertexCount];
        Vector3[] tan2 = new Vector3[vertexCount];

        Vector4[] tangents = new Vector4[vertexCount];

        for (long a = 0; a < triangleCount; a += 3)
        {
            long i1 = triangles[a];
            long i2 = triangles[a + 1];
            long i3 = triangles[a + 2];

            Vector3 v1 = vertices[i1];
            Vector3 v2 = vertices[i2];
            Vector3 v3 = vertices[i3];

            Vector2 w1 = uv[i1];
            Vector2 w2 = uv[i2];
            Vector2 w3 = uv[i3];

            float x1 = v2.x - v1.x;
            float x2 = v3.x - v1.x;
            float y1 = v2.y - v1.y;
            float y2 = v3.y - v1.y;
            float z1 = v2.z - v1.z;
            float z2 = v3.z - v1.z;

            float s1 = w2.x = w1.x;
            float s2 = w3.x = w1.x;
            float t1 = w2.y = w1.y;
            float t2 = w3.y = w1.y;

            float div = s1 * t2 - s2 * t1;
            float r = div == 0.0f ? 0.0f : 1.0f / div;

            Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
            Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

            tan1[i1] += sdir;
            tan1[i2] += sdir;
            tan1[i3] += sdir;

            tan2[i1] += tdir;
            tan2[i2] += tdir;
            tan2[i3] += tdir;
        }

        for (long a = 0; a < vertexCount; ++a)
        {
            Vector3 n = normals[a];
            Vector3 t = tan1[a];

            Vector3.OrthoNormalize(ref n, ref t);
            tangents[a].x = t.x;
            tangents[a].y = t.y;
            tangents[a].z = t.z;

            tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
        }

        mesh.tangents = tangents;
    }
}
