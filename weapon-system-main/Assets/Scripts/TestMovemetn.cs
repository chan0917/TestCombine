using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovemetn : MonoBehaviour
{
    public float speed = 10f;
    public float rotationSpeed = 10f;
    public float turnSmoothTime = 0.1f;
    private Rigidbody rb;
    private float turnSmoothVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical).normalized;

        if (movement.magnitude >= 0.1f)
        {
            // 이동 방향에 대한 목표 각도를 계산합니다 based on the input direction
            float targetAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg;

            // 플레이어가 목표 방향으로 부드럽게 회전하도록     turnSmoothTime 변수는 회전의 부드러움을 조절합니다.
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            transform.rotation = Quaternion.Euler(0, angle, 0);

            // Apply velocity in the target direction
            Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            rb.velocity = moveDir * speed;
        }
        else
        {
            // 이동 입력이 없으면 속도를 0으로 설정
            rb.velocity = Vector3.zero;
        }
    }
}
