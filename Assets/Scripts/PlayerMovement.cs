using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class PlayerMovement : MonoBehaviour
{

    public float walkSpeed = 2;
    public float runSpeed = 6;

    public float turnSmoothTime = 0.2f;
    float turnSmoothVelocity;

    public float speedSmoothTime = 0.1f;
    float speedSmoothVelocity;
    float currentSpeed;

    Animator animator;
    Transform cameraT;

    GameObject capsule;

    void Start()
    {
        animator = GetComponent<Animator>();
        cameraT = Camera.main.transform;

        capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
    }

    void Update()
    {

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 inputDir = input.normalized;

        if (inputDir != Vector2.zero)
        {
            float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
        }
        
        bool running = Input.GetKey(KeyCode.LeftShift);
        float timeDelta = Time.deltaTime;
        NetworkManager.Instance.ChangePlayerPosition(transform.forward, inputDir.magnitude, timeDelta, running);

        Vector3 position = transform.position + transform.forward * ((running) ? runSpeed : walkSpeed) * inputDir.magnitude * timeDelta;

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
                transform.position = newPosition;
            }

            // other players
            PlayerSnapshot previousSnapshot = NetworkManager.Instance.previousSnapshot;
            if (previousSnapshot != null && snapshot.otherPlayers.Count > 0)
            {
                float interval = snapshot.timestamp - previousSnapshot.timestamp;
                float elapsed  = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - NetworkManager.Instance.updatedAt;
                float t = elapsed / interval;
                
                Vector3 pos = Vector3.Lerp(previousSnapshot.otherPlayers[0].position, snapshot.otherPlayers[0].position, t);
                capsule.transform.position = pos;
            }

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
