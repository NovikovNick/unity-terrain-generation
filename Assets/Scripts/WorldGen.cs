using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGen : MonoBehaviour
{

    Voxel voxel;
    // Start is called before the first frame update
    void Start()
    {
        //int size = 32;

        /*for(int x = 0; x < size; x++)
        {
            for (int y = 0; y < 1; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    voxel = new Voxel();
                    voxel.Start("Voxel_" + x + "_" + y + "_" + z, new Vector3(x, y, z));
                }
            }
        }*/


        Chunk chunk = new Chunk();
        List<Vector3> data = new List<Vector3>();
        data.Add(new Vector3(0, 0, 0));
        data.Add(new Vector3(1, 0, 0));
        data.Add(new Vector3(2, 0, 0));

        int height = 8;
        int width = 16;
        int length = 24;


        for (int x = 0; x <= width; x++)
        {
            for (int y = 0; y <= height; y++)
            {
                for (int z = 0; z <= length; z++)
                {

                    if(x == 0 || y == 0 || z == 0)
                    {
                        data.Add(new Vector3(x, y, z));
                    }
                    else if (x == width || z == length)
                    {
                        data.Add(new Vector3(x, y, z));
                    }

                }
            }
        }
        

        chunk.Start(data);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
