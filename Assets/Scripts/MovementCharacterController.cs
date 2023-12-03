using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovementCharacterController : MonoBehaviour
{
    [SerializeField]
    // Start is called before the first frame update
    private float   moveSpeed;
    private Vector3 moveForce;

    [SerializeField]
    private float jumpForce; // 점프 힘
    [SerializeField]
    private float gravity;  // 중력 개수

    public float MoveSpeed
    {
        set => moveSpeed = Mathf.Max(0, value);
        get => moveSpeed;
    }

    private CharacterController characterController;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // 허공에 떠있으면 중력만큼 y축 이동속도 감소
        if ( !characterController.isGrounded )
        {
            moveForce.y += gravity * Time.deltaTime;
        }

        // 1초당 moveForce 속력으로 이동
        characterController.Move(moveForce * Time.deltaTime);
    }

    

    public void MoveTo(Vector3 direction)
    {
        direction = transform.rotation * new Vector3(direction.x, 0, direction.z);

        moveForce = new Vector3(direction.x * moveSpeed, moveForce.y, direction.z * moveSpeed);
    }

    public void Jump()
    {
        // 플레이어가 바닥에 있을 때만 점프 가능
        if ( characterController.isGrounded )
        {
            moveForce.y = jumpForce;
        }
    }

}
