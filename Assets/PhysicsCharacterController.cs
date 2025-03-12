using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsCharacterController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float mouseSensitivity = 2f;
    public float extraGravity = 20f; // Increases gravity for better fall speed
    public LayerMask groundLayer; // Assign this to the "Ground" layer in Inspector


    [SerializeField] private Transform _pointToRayCast;
    private Rigidbody rb;
    private float X, Y;
    [SerializeField] private float ThresholdY = 0.01f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // Disable default gravity to apply custom one
        rb.drag = 0f; // Remove drag for better control
        Cursor.lockState = CursorLockMode.Locked; // Lock cursor
    }

    void Update()
    {
        RotateCharacter();
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        MoveCharacter();
        ApplyCustomGravity();
    }

    void MoveCharacter()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.forward * v + transform.right * h;
        moveDirection.Normalize();
        
        Debug.Log(rb.velocity);
        
        if (moveDirection.magnitude > ThresholdY)
        {
            // Directly set velocity instead of using forces (ensures instant stop)
            
            rb.velocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.z * moveSpeed);
        }
        else
        {
            // Stop movement instantly when no input
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }

    void ApplyCustomGravity()
    {
        if (!IsGrounded())
        {
            rb.velocity += Vector3.down * extraGravity * Time.fixedDeltaTime;
            if ((Mathf.Abs(rb.velocity.y) < ThresholdY)) rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        }
    }

    void RotateCharacter()
    {
        #region Camera Limitation Calculator
        //Camera limitation variables
        const float MIN_Y = -60.0f;
        const float MAX_Y = 70.0f;

        X += Input.GetAxis("Mouse X") * (mouseSensitivity * Time.deltaTime);
        Y -= Input.GetAxis("Mouse Y") * (mouseSensitivity * Time.deltaTime);

        if (Y < MIN_Y)
            Y = MIN_Y;
        else if (Y > MAX_Y)
            Y = MAX_Y;
        #endregion
        transform.localRotation = Quaternion.Euler(Y, X, 0.0f);
    }

    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
    }

    bool IsGrounded()
    {
        float rayLength = 0.2f; // Distance to check below the character
        return Physics.Raycast(_pointToRayCast.position, Vector3.down, rayLength, groundLayer);
    }
}
