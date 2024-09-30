using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cart : MonoBehaviour
{
    [SerializeField] private bool isMoving = false;
    [SerializeField] private float moveSpeed = 10.0f;
    private Vector3 moveDir;
    public bool IsMoving
    {
        get { return isMoving; }
        set
        {
            if(isMoving != value)
            {
                isMoving = value;
                if (value == false)
                    moveDir *= -1;
            }
        }
    }
    private void Start()
    {
        moveDir = -transform.forward;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            IsMoving = true;
        }
        if (isMoving)
        {
            transform.Translate(moveDir * Time.deltaTime * moveSpeed);
        }
    }



    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Rail"))
        {
            IsMoving = false;
        }
    }
}
