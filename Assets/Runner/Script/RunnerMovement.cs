using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerMovement : MonoBehaviour
{
    public float speed;
    public float jumpSpeed;
    private float xAxis;
    private float zAxis;
    private float ySpeed;
    private bool isJumping;
    //private bool isGrounded;
    private bool isCrouched;

    private Animator anim;
    private CharacterController controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        isCrouched = false;
    }

    // Update is called once per frame
    void Update()
    {
        // x, z Axis Movement Input
        xAxis = Input.GetAxisRaw("Horizontal");
        zAxis = Input.GetAxisRaw("Vertical");
        Vector3 movementDirection = new Vector3(xAxis, 0, zAxis).normalized;

        // Standing <-> Crunching Input
        if (Input.GetMouseButtonDown(0))
        {
            anim.SetBool("IsCrouched", !isCrouched);
            isCrouched = !isCrouched;
        }

        // y Axis Movement Decision
        ySpeed += Physics.gravity.y * Time.deltaTime;
        if(controller.isGrounded)
        {
            ySpeed = -2.0f;
            anim.SetBool("IsGrounded", true);
            //isGrounded = true;
            anim.SetBool("IsJumping", false);
            isJumping = false;
            anim.SetBool("IsFalling", false);

            // Jumping Input
            if (Input.GetButtonDown("Jump") && !isCrouched)
            {
                ySpeed = jumpSpeed;
                anim.SetBool("IsJumping", true);
                isJumping = true;
            }
        }
        else
        {
            anim.SetBool("IsGrounded", false);
            //isGrounded = false;
             
            if ((isJumping && ySpeed < 0) || ySpeed < -2.5f)
            {
                anim.SetBool("IsFalling", true);
            }
        }
        
        // Integrating Movements
        Vector3 velocity = movementDirection * speed * (isCrouched ? 0.3f : 1.0f);
        velocity.y = ySpeed;
        // Move Character
        controller.Move(velocity * Time.deltaTime);

        // Rotating Character
        if (movementDirection != Vector3.zero)
        {
            transform.forward = movementDirection;
            anim.SetBool("IsRunning", true);
        }
        else
        {
            anim.SetBool("IsRunning", false);
        }

        Debug.Log("Direction : " + transform.forward);
    }
}
