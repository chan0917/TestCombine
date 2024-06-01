using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class All : MonoBehaviour
{
    [Header("Basic Movement")]
    private Rigidbody rb;
    public float originSpeed = 6f;
    public float speed = 6f;
    public float rotationSpeed = 700f;

    [Header("Camera")]
    public float mouseSensitivity = 100f;
    public Transform cameraTransform;
    private float xRotation = 0f;

    [Header("Jump and Boost")]
    public float jumpForce = 5f;
    private bool isGrounded;
    public float boostDuration = 0.25f;
    public float boostedSpeed = 13f;
    private Coroutine boostCoroutine;

    [Header("Sliding")]
    public float slideBoost = 7f;
    private bool isSliding = false;
    public float slideDuration = 0.5f; // �����̵� �� �߰��� �ӵ��� �����Ǵ� �ð�

    public float gravityMultiplier = 2.0f; // �߷� ���

    [Header("TrailRender")]
    public TrailRenderer trailRendererPrefab; // Trail Renderer ������
    private TrailRenderer trailRendererInstance; // Trail Renderer �ν��Ͻ�
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked; // ���콺 Ŀ���� �߾ӿ� ����

        rb.drag = 5f; // ��ü�� ������ �� ������ ��
        rb.angularDrag = 5f; // ȸ���� �� ������ ���ߵ��� ��
    }

    void Update()
    {
        // ���콺 �Է��� �޾Ƽ� ī�޶� ȸ��
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // ���� ȸ�� ���� ����

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // ���� �Է� ó��
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // Boost �ӵ� ó��
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(SpeedBoost());

            EnableTrailRenderer();
        }

        if (Input.GetKeyDown(KeyCode.C) && isGrounded)
        {
            StartCoroutine(SlideBoostCoroutine());

            EnableTrailRenderer();
        
        }
    }

    private void FixedUpdate()
    {
        // Ű���� �Է��� �޾Ƽ� ĳ���� �̵�
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        Vector3 movement = transform.right * moveHorizontal + transform.forward * moveVertical;
        movement = movement.normalized * speed;

        // Rigidbody�� �ӵ��� ����
        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);

        // �Է��� ���� �� �ӵ��� 0���� �����Ͽ� ���ߵ��� ��
        if (moveHorizontal == 0 && moveVertical == 0)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }

        // �߷� ��ȭ ó��
        if (!isGrounded)
        {
            rb.AddForce(Physics.gravity * (gravityMultiplier - 1), ForceMode.Acceleration);
        }
    }

    void OnCollisionStay(Collision collision)
    {
        // �ٴڿ� ��� �ִ��� Ȯ��
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.point.y < transform.position.y - 0.5f)
            {
                isGrounded = true;
                return;
            }
        }
        isGrounded = false;
    }

    void OnCollisionExit(Collision collision)
    {
        // �ٴڿ��� �������� �� ó��
        isGrounded = false;
    }

    IEnumerator SpeedBoost()
    {
        speed += boostedSpeed;
        float decreaseRate = (speed - originSpeed) / boostDuration;

        while (speed > originSpeed)
        {
            speed -= decreaseRate * Time.deltaTime;
            yield return null;
        }

        DisableTrailRenderer();
    }

    IEnumerator SlideBoostCoroutine()
    {
        isSliding = true;
        speed += slideBoost; // �����̵� �� �ӵ� ����

        float decreaseRate = (speed - originSpeed) / slideDuration;

        while (speed > originSpeed)
        {
            speed -= decreaseRate * Time.deltaTime;
            yield return null;
        }
        

        // �����̵��� ����Ǹ� ���� �ӵ��� ���ƿ�
        //speed = originSpeed;

        isSliding = false;
        DisableTrailRenderer();
    }

    void EnableTrailRenderer()
    {
        // Trail Renderer �������� �ν��Ͻ�ȭ�Ͽ� �÷��̾ ����
        if(trailRendererInstance != null)
        {
            trailRendererInstance.emitting = false;
        }

        trailRendererInstance = Instantiate(trailRendererPrefab, transform.position, Quaternion.identity);
        trailRendererInstance.transform.SetParent(transform);
        Destroy(trailRendererInstance.gameObject, trailRendererInstance.time);
    }

    void DisableTrailRenderer()
    {
        // Trail Renderer ��Ȱ��ȭ �� ����
        if (trailRendererInstance != null)
        {
            trailRendererInstance.emitting = false;
            
        }
    }
}
