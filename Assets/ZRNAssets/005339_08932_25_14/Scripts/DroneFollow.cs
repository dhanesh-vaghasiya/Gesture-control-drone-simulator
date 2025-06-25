using UnityEngine;

// [RequireComponent(typeof(CharacterController))]
public class DroneFollow : MonoBehaviour
{
    // public float rotationSpeed = 2.0f;
    public GameObject drone;
    public float moveSpeed = 0f;
    public float maxSpeed = 8.0f;
    public float acceleration = 4.0f;
    public float deceleration = 3.0f;
    public float tiltAmount = 30f;
    public GameObject groundCollider;

    // private CharacterController controller;
    // private float currentTilt = 0f;

    // void Start()
    // {
    //     controller = GetComponent<CharacterController>();
    //     Cursor.lockState = CursorLockMode.Locked;
    // }
    void SetChildColliders(bool state)
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders)
        {
            if (col.gameObject != this.gameObject) // skip parent if needed
                col.enabled = state;
        }
    }
    void StopChildRigidbodies()
    {
        Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in rbs)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    void Update()
    {
        HandleMovement();
        // DisplaySpeedGUI(); // Optional visual debug
    }

    void HandleMovement()
    {
        // Get input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 inputDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;

        if (inputDirection.magnitude > 0.1f)
        {
            moveSpeed += acceleration * Time.deltaTime;
            moveSpeed = Mathf.Min(moveSpeed, maxSpeed);
        }
        else
        {
            // Decelerate to zero
            moveSpeed -= deceleration * Time.deltaTime;
            moveSpeed = Mathf.Max(moveSpeed, 0f);
        }
        // Move the drone
        Vector3 moveDirection = drone.transform.forward * moveSpeed * Time.deltaTime;
        transform.position += moveDirection;
        
        if (inputDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(inputDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }


        if (groundCollider.transform.position.y > transform.position.y)
        {
            Vector3 pos = transform.position;
            pos.y = groundCollider.transform.position.y;
            transform.position = pos;
        }
    }
}
