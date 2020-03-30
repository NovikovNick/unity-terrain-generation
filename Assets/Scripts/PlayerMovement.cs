using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{


    public Text text;

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
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

        transform.Translate(transform.forward * currentSpeed * Time.deltaTime, Space.World);

        float animationSpeedPercent = ((running) ? 1 : .5f) * inputDir.magnitude;
        animator.SetFloat("speedPercent", animationSpeedPercent, speedSmoothTime, Time.deltaTime);


        text.text =
              "input: " + input.ToString() + "\n"
            + "inputDir: " + inputDir.ToString() + "\n"
            + "speedPercent: " + animationSpeedPercent.ToString() + "\n"
            + "speedSmoothTime: " + speedSmoothTime.ToString() + "\n"
            + "currentSpeed: " + currentSpeed.ToString() + "\n"
            + "transform.up: " + transform.up + "\n"
            + "transform.forward: " + transform.forward + "\n"
            ;
    }
}
