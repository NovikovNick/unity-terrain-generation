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

    void Start()
    {
        animator = GetComponent<Animator>();
        cameraT = Camera.main.transform;
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
            Vector3 newPosition = new Vector3(snapshot.position.x, snapshot.position.y, snapshot.position.z);

            PlayerRequest request;
            if (NetworkManager.Instance.requests.TryPeek(out request))
            {
                
                while (request.datagramNumber < snapshot.lastDatagramNumber)
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
        }
    }

    float round(float value)
    {
        return Mathf.Round(value * 10000) / 10000;
    }
}
