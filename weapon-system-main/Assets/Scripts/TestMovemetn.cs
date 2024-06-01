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
            // �̵� ���⿡ ���� ��ǥ ������ ����մϴ� based on the input direction
            float targetAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg;

            // �÷��̾ ��ǥ �������� �ε巴�� ȸ���ϵ���     turnSmoothTime ������ ȸ���� �ε巯���� �����մϴ�.
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            transform.rotation = Quaternion.Euler(0, angle, 0);

            // Apply velocity in the target direction
            Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            rb.velocity = moveDir * speed;
        }
        else
        {
            // �̵� �Է��� ������ �ӵ��� 0���� ����
            rb.velocity = Vector3.zero;
        }
    }
}
