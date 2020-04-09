using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

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
        float targetSpeed = ((running) ? runSpeed : walkSpeed) * inputDir.magnitude;

        //transform.Translate(transform.forward * targetSpeed * Time.deltaTime, Space.World);
        
        PlayerSnapshot snapshot;
        if (NetworkManager.Instance.snapshots.TryDequeue(out snapshot))
        {
            transform.position = snapshot.position;

            PlayerRequest request;
            if (NetworkManager.Instance.requests.TryPeek(out request))
            {
                while (request.datagramNumber <= snapshot.lastDatagramNumber)
                {
                    NetworkManager.Instance.requests.TryDequeue(out request);
                }

                transform.Translate(request.direction * targetSpeed * 1 / 30, Space.World);
                float animationSpeedPercent = ((running) ? 1 : .5f) * inputDir.magnitude;
                animator.SetFloat("speedPercent", animationSpeedPercent);
            }

            
        }
        
        NetworkManager.Instance.ChangePlayerPosition(transform.forward, inputDir.magnitude, running);
    }
}
