using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 300f;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private Transform cameraLookAt;
    [SerializeField] private Transform playerMesh;

    private Rigidbody rb;

    private Vector2 moveDirection;
    private Vector2 mouseDelta;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<Vector2>();

        Movement();
    }

    public void OnCameraMove(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();

        LookAround();
    }

    private void Movement()
    {
        var moveDir = new Vector3(moveDirection.x, 0, moveDirection.y).normalized;

        if(moveDir.magnitude >= 0.1f)
        {
            var MoveAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg + cameraLookAt.transform.eulerAngles.y;
            var dir = Quaternion.Euler(0, MoveAngle, 0) * Vector3.forward;

            playerMesh.rotation = Quaternion.Euler(0, MoveAngle, 0);

            rb.velocity = dir * moveSpeed;
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }

    private void LookAround()
    {
        Vector2 mouseDelta = this.mouseDelta * mouseSensitivity;

        var CameraAngle = cameraLookAt.rotation.eulerAngles;
        float x = CameraAngle.x - mouseDelta.y;

        if (x < 180f)
            x = Mathf.Clamp(x, -1f, 60f);
        else
            x = Mathf.Clamp(x, 320f, 361f);

        cameraLookAt.rotation = Quaternion.Euler(x, CameraAngle.y + mouseDelta.x, CameraAngle.z);
    }

    private void FixedUpdate()
    {
        //LookAround();
        //Movement();
    }
}
