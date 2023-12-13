using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementRansform : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 0.0f;
    [SerializeField]
    private Vector3 moveDirection = Vector3.zero;

    /// <sumary>
    /// 이동 방향이 설정되면 알아서 이동하도록 함
    /// <sumary></sumary>
    /// 

    private void Update()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

    }

    public void MoveTo(Vector3 direction)
    {
        moveDirection = direction;
    }

}
