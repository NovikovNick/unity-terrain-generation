using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

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

    void Start()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        player = Instantiate(characterPrefab, new Vector3(0,0,0), Quaternion.identity);
        animator = player.GetComponent<Animator>();
        cameraT = Camera.main.transform;

       
    }

    void Update()
    {
        
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 inputDir = input.normalized;

        if (inputDir != Vector2.zero)
        {
            float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
            player.transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(player.transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
        }
        
        bool running = Input.GetKey(KeyCode.LeftShift);
        float timeDelta = Time.deltaTime;
        NetworkManager.Instance.ChangePlayerPosition(player.transform.forward, inputDir.magnitude, timeDelta, running);

        Vector3 position = player.transform.position + player.transform.forward * ((running) ? runSpeed : walkSpeed) * inputDir.magnitude * timeDelta;

        animator.SetFloat("speedPercent", ((running) ? 1 : .5f) * inputDir.magnitude);

       
        PlayerSnapshot snapshot = NetworkManager.Instance.snapshot;
        if (snapshot != null)
        {
            // Client-side prediction

            Vector3 newPosition = new Vector3(snapshot.player.position.x, snapshot.player.position.y, snapshot.player.position.z);
            PlayerRequest request;
            if (NetworkManager.Instance.requests.TryPeek(out request))
            {
                
                while (request.sequenceNumber < snapshot.acknowledgmentNumber)
                {
                    NetworkManager.Instance.requests.TryDequeue(out request);
                }

                foreach (PlayerRequest r in new List<PlayerRequest>(NetworkManager.Instance.requests))
                {
                    float speed = ((r.isRunning) ? runSpeed : walkSpeed);
                    float multiplier = round(speed * r.magnitude * r.timeDelta);

                    newPosition = new Vector3(
                        newPosition.x + round(r.direction.x * multiplier), 
                        newPosition.y + round(r.direction.y * multiplier), 
                        newPosition.z + round(r.direction.z * multiplier)
                        );
                }
                player.transform.position = newPosition;
            }

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

    float round(float value)
    {
        return Mathf.Round(value * 10000) / 10000;
    }
}
