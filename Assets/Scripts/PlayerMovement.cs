using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Unity.UIElements.Runtime;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class PlayerMovement : MonoBehaviour
{

    #region Camera

    public bool lockCursor;
    public float mouseSensitivity = 10;
    public float dstFromTarget = 2;
    public Vector2 pitchMinMax = new Vector2(-40, 85);

    float yaw;
    float pitch;

    public float rotationSmoothTime = .12f;
    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;
    #endregion

    #region Laser
    public LineRenderer lineRenderer;
    #endregion


    Transform cameraT;
    GameObject player;
    Dictionary<int, Transform> otherPlayers = new Dictionary<int, Transform>();

    public GameObject characterPrefab;
    public float walkSpeed = 2;
    public float runSpeed = 6;
    public float turnSmoothTime = 0.2f;
    public float speedSmoothTime = 0.1f;

    float turnSmoothVelocity;
    float speedSmoothVelocity;
    float currentSpeed;


    Animator animator;
    Rigidbody rb;

    #region UI
    public PanelRenderer menuScreen;
    #endregion

    private void OnEnable()
    {
        Debug.Log("OnEnable");
        // UI 
        menuScreen.postUxmlReload = BindMainMenuScreen;

    }
    private void GoToMainMenu()
    {
        SetScreenEnableState(menuScreen, true);
        //SetScreenEnableState(m_GameScreen, false);
        //SetScreenEnableState(m_EndScreen, false);
    }

    void SetScreenEnableState(PanelRenderer screen, bool state)
    {
        if (state)
        {
            screen.visualTree.style.display = DisplayStyle.Flex;
            screen.enabled = true;
            screen.gameObject.GetComponent<UIElementsEventSystem>().enabled = true;
            BindMainMenuScreen();
        }
        else
        {
            screen.visualTree.style.display = DisplayStyle.None;
            screen.enabled = false;
            screen.gameObject.GetComponent<UIElementsEventSystem>().enabled = false;
        }
    }

    private IEnumerable<Object> BindMainMenuScreen()
    {
        Debug.Log("BindMainMenuScreen");
        var root = menuScreen.visualTree;

        var startButton = root.Q<Button>("start-button");
        if (startButton != null)
        {
            startButton.clickable.clicked += () =>
            {
                StartRound();
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

    private void StartRound()
    {
        SceneManager.LoadScene(1);
        Debug.Log("it works...");
    }

    void Start()
    {
       
        GoToMainMenu();

        if (lockCursor)
        {
           // Cursor.lockState = CursorLockMode.Locked;
          //  Cursor.visible = false;
        }

        player = Instantiate(characterPrefab, new Vector3(0,0,0), Quaternion.identity);
        animator = player.GetComponent<Animator>();
        rb = player.GetComponent<Rigidbody>();
        cameraT = Camera.main.transform;
    }

    void Update()
    {
        /*
        RaycastHit hit;

        if (Physics.Raycast(player.transform.position, player.transform.forward, out hit))
        {
            // hit.point
            lineRenderer.SetPosition(0, player.transform.position + new Vector3(0, 1.7f, 0));
            lineRenderer.SetPosition(1, hit.point + new Vector3(0, 1.7f, 0));
        } else
        {
            lineRenderer.SetPosition(0, player.transform.position + new Vector3(0, 1.7f, 0));
            lineRenderer.SetPosition(1, player.transform.position + new Vector3(0, 1.7f, 0) + player.transform.forward * 2) ;
        }
        */

        /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
           
            lineRenderer.enabled = true;
        }
        else
        {
            lineRenderer.enabled = false;
        }
        */


        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 inputDir = input.normalized;
        Vector3 euler = player.transform.eulerAngles;

        if (inputDir != Vector2.zero)
        {
            float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
            euler = Vector3.up * Mathf.SmoothDampAngle(player.transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
        }
        
        bool running = Input.GetKey(KeyCode.LeftShift);
        float timeDelta = Time.deltaTime;
        NetworkManager.Instance.ChangePlayerPosition(player.transform.forward, inputDir.magnitude, timeDelta, running);
        
        animator.SetFloat("speedPercent", ((running) ? 1 : .5f) * inputDir.magnitude);

       
        PlayerSnapshot snapshot = NetworkManager.Instance.snapshot;
        if (snapshot != null)
        {
            

            // other players
            PlayerSnapshot previousSnapshot = NetworkManager.Instance.previousSnapshot;
            if (previousSnapshot != null && snapshot.otherPlayers.Count > 0)
            {
                float interval = snapshot.timestamp - previousSnapshot.timestamp;
                float elapsed  = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - NetworkManager.Instance.updatedAt;
                float t = elapsed / interval;
                
                for(int i = 0; i < snapshot.otherPlayers.Count; i++)
                {
                    if(!otherPlayers.ContainsKey(i))
                    {
                        otherPlayers.Add(i, Instantiate(characterPrefab, previousSnapshot.otherPlayers[i].position, Quaternion.identity).GetComponent<Transform>());
                    }
                    Vector3 pos = Vector3.Lerp(previousSnapshot.otherPlayers[i].position, snapshot.otherPlayers[i].position, t);
                    otherPlayers[i].transform.position = pos;
                    otherPlayers[i].transform.LookAt(pos + snapshot.otherPlayers[i].rotation, Vector3.up);

                    if (snapshot.otherPlayers[i].speed > 0)
                    {
                        otherPlayers[i].transform.GetComponent<Animator>().SetFloat("speedPercent", ((snapshot.otherPlayers[i].speed > 3) ? 1 : .5f));
                    }
                    else
                    {
                        otherPlayers[i].transform.GetComponent<Animator>().SetFloat("speedPercent", 0);
                    }
                }
            }

            // camera
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);


            Vector3 targetRotation = new Vector3(pitch, yaw);
            currentRotation = Vector3.SmoothDamp(currentRotation, targetRotation, ref rotationSmoothVelocity, rotationSmoothTime);
            cameraT.transform.eulerAngles = currentRotation;

            cameraT.transform.position = player.transform.position + new Vector3(0, 1.8f, 0) - cameraT.transform.forward * dstFromTarget;

            // terrain
            foreach (TerrainChunk chunk in new List<TerrainChunk>(NetworkManager.Instance.snapshot.terrainChunks))
            {
                TerrainManager.Instance.updateChunk(chunk);
            }
        }
    }

    private void FixedUpdate()
    {
        PlayerSnapshot snapshot = NetworkManager.Instance.snapshot;
        if (snapshot != null)
        {
            // rotation

            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            Vector2 inputDir = input.normalized;
            Vector3 euler = player.transform.eulerAngles;

            if (inputDir != Vector2.zero)
            {
                float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
                euler = Vector3.up * Mathf.SmoothDampAngle(player.transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
            }


            // Client-side prediction
            
            rb.position = snapshot.player.position;

            Vector3 newPosition = new Vector3(0,0,0);
            PlayerRequest request;
            if (NetworkManager.Instance.requests.TryPeek(out request))
            {

                while (request.sequenceNumber < snapshot.acknowledgmentNumber)
                {
                    NetworkManager.Instance.requests.TryDequeue(out request);
                }
                float speed = 0;
                float magnitude = 0;
                float timeDelta = 0;
                Vector3 direction = new Vector3(0,0,0);

                foreach (PlayerRequest r in new List<PlayerRequest>(NetworkManager.Instance.requests))
                {
                    speed = ((r.isRunning) ? runSpeed : walkSpeed);
                    magnitude = r.magnitude;
                    timeDelta += r.timeDelta;
                    direction = r.direction;
                    newPosition = rb.position + r.direction * speed * r.magnitude * r.timeDelta;
                    
                    Ray ray = new Ray(rb.position + new Vector3(0, 1.7f, 0), r.direction);
                    RaycastHit hit;
                    if (!Physics.Raycast(ray, out hit, 0.5f + speed * r.magnitude * r.timeDelta))
                    {
                        lineRenderer.enabled = false;

                        rb.MovePosition(newPosition);
                       
                    } 
                }
                
                rb.MoveRotation(Quaternion.Euler(euler));
            }
        }
    }
}
