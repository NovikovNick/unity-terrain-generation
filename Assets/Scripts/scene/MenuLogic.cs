using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.UIElements.Runtime;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;


public class MenuLogic : MonoBehaviour
{

    public PanelRenderer menuScreen;


    // Start is called before the first frame update
    void Start()
    {

        TerrainChunk chunk = new TerrainChunk();

        List<Vector3> data = new List<Vector3>();

        for(int x = -3; x < 3; x++)
        {
            for (int z = 0; z < 5; z++)
            {
                for (int y = 0; y < 4; y++)
                {
                    if(y == 0 || x == -3 || x == 2 || z == 4)
                    {
                        data.Add(new Vector3(x, y, z));
                    }

                }
            }
        }

        chunk.children = data;
        TerrainManager.Instance.updateChunk(chunk);

        BindMainMenuScreen();

    }

    private IEnumerable<Object> BindMainMenuScreen()
    {
        var root = menuScreen.visualTree;

        var startButton = root.Q<Button>("start-button");
        if (startButton != null)
        {
            startButton.clickable.clicked += () =>
            {
                SceneManager.LoadScene(1);
            };
        }

        var backButton = root.Q<Button>("back-button");
        if (backButton != null)
        {
            backButton.clickable.clicked += () =>
            {
                SceneManager.LoadScene(0);
            };
        }

        var exitButton = root.Q<Button>("exit-button");
        if (exitButton != null)
        {
            exitButton.clickable.clicked += () =>
            {
                Application.Quit();
            };
        }

        return null;
    }
}
