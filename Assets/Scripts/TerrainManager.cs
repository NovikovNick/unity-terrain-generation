using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : Singleton<TerrainManager>
{
    Dictionary<Vector3, Chunk> map = new Dictionary<Vector3, Chunk>();

    public void updateChunk(TerrainChunk chunk)
    {
        // Debug.Log("create " + chunk.position);
        if (map.ContainsKey(chunk.position)) {
            // Debug.Log("remove " + chunk.position);
            Destroy(map[chunk.position].gameObject);
            map.Remove(chunk.position);
        } 

        List<Vector3> data = new List<Vector3>();

        foreach (Vector3 voxel in chunk.children)
        {
            data.Add(new Vector3(voxel.x + 10 * chunk.position.x, voxel.y + 10 * chunk.position.y, voxel.z + 10 * chunk.position.z));
        }
        Chunk ch = new Chunk(chunk.position, data);
        map.Add(chunk.position, ch);
    }
}
