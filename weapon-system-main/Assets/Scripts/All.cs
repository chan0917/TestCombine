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
    public float slideDuration = 0.5f; // 슬라이딩 중 추가된 속도가 유지되는 시간

    public float gravityMultiplier = 2.0f; // 중력 배수

    [Header("TrailRender")]
    public TrailRenderer trailRendererPrefab; // Trail Renderer 프리팹
    private TrailRenderer trailRendererInstance; // Trail Renderer 인스턴스
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked; // 마우스 커서를 중앙에 고정

        rb.drag = 5f; // 물체의 감속을 더 빠르게 함
        rb.angularDrag = 5f; // 회전을 더 빠르게 멈추도록 함
    }

    void Update()
    {
        // 마우스 입력을 받아서 카메라 회전
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // 상하 회전 각도 제한

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // 점프 입력 처리
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // Boost 속도 처리
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
        // 키보드 입력을 받아서 캐릭터 이동
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        Vector3 movement = transform.right * moveHorizontal + transform.forward * moveVertical;
        movement = movement.normalized * speed;

        // Rigidbody의 속도를 설정
        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);

        // 입력이 없을 때 속도를 0으로 설정하여 멈추도록 함
        if (moveHorizontal == 0 && moveVertical == 0)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }

        // 중력 강화 처리
        if (!isGrounded)
        {
            rb.AddForce(Physics.gravity * (gravityMultiplier - 1), ForceMode.Acceleration);
        }
    }

    void OnCollisionStay(Collision collision)
    {
        // 바닥에 닿아 있는지 확인
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
        // 바닥에서 떨어졌을 때 처리
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
        speed += slideBoost; // 슬라이딩 중 속도 증가

        float decreaseRate = (speed - originSpeed) / slideDuration;

        while (speed > originSpeed)
        {
            speed -= decreaseRate * Time.deltaTime;
            yield return null;
        }
        

        // 슬라이딩이 종료되면 원래 속도로 돌아옴
        //speed = originSpeed;

        isSliding = false;
        DisableTrailRenderer();
    }

    void EnableTrailRenderer()
    {
        // Trail Renderer 프리팹을 인스턴스화하여 플레이어에 부착
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
        // Trail Renderer 비활성화 및 삭제
        if (trailRendererInstance != null)
        {
            trailRendererInstance.emitting = false;
            
        }
    }
}
