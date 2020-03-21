using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainScript : MonoBehaviour
{
    public Terrain terrain;
    private TerrainData m_tData;

    // Start is called before the first frame update
    void Start()
    {
        m_tData = terrain.terrainData;
        int x = m_tData.heightmapResolution;
        int z = m_tData.heightmapResolution;
        float[,] heights = m_tData.GetHeights(0, 0, x, z);

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < x; j++)
            {
                heights[i, j] = (Mathf.Cos(i) + Mathf.Sin(j)) / 100;
            }
        }

        m_tData.SetHeights(0, 0, heights);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
