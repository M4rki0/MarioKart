using UnityEngine;

public class KartController : MonoBehaviour
{
    [Header("Speed Settings")] 
    public float acceleration = 50f;
    public float maxSpeed = 20f;
    public float reverseSpeed = 5f;
    public float dragOnGround = 3f;

    [Header("Steering")] 
    public float steerStrength = 100f;
    public float minSteerAtMaxStrength = 0.5f;

    [Header("Drift Settings")] 
    public float driftSteerMultiplier = 1.5f;
    public float driftGrip = 0.95f;
    public KeyCode driftKey = KeyCode.RightShift;
    public float boostMultiplier = 1.5f;
    public float boostDuration = 1.5f;
    public float driftThreshold = 0.5f;
    
    [Header("Ground Check")] 
    public float groundRayLength = 0.5f;
    public LayerMask groundLayers;

    private Rigidbody rb;
    private bool isGrounded;
    private bool isDrifting;
    private float driftDirection = 0;
    private float driftTime = 0;
    private bool isBoosting = false;
    private float boostTime = 0f;
    private KartStatusEffects statusEffects;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        statusEffects = GetComponent<KartStatusEffects>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }
    
    void FixedUpdate()
    {
        if (statusEffects != null && statusEffects.IsStunned()) return;
        
        float inputForward = Input.GetAxis("Vertical");
        float inputTurn = Input.GetAxis("Horizontal");
        bool driftInput = Input.GetKey(driftKey);
        
        isGrounded = Physics.Raycast(transform.position, -transform.up, groundRayLength, groundLayers);


        if (isGrounded)
        {
            Vector3 localVelocity = transform.InverseTransformDirection(rb.linearVelocity);
            float forwardSpeed = localVelocity.z;

            if (driftInput && inputForward > 0 && Mathf.Abs(inputTurn) > 0.1f && !isDrifting)
            {
                isDrifting = true;
                driftDirection = Mathf.Sign(inputTurn);
                driftTime = 0f;
            }

            if (!driftInput && isDrifting)
            {
                isDrifting = false;

                if (driftTime >= driftThreshold)
                {
                    isBoosting = true;
                    boostTime = 0f;
                }
            }
            
            if (inputForward != 0)
            {
                float speedLimit = inputForward > 0 ? maxSpeed : reverseSpeed;
                if (Mathf.Abs(forwardSpeed) < speedLimit)
                {
                    rb.AddForce(transform.forward * inputForward * acceleration);
                }
            }

            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity,Vector3.zero, dragOnGround * Time.fixedDeltaTime);

            float speedFactor = Mathf.InverseLerp(0, maxSpeed, rb.linearVelocity.magnitude);
            float steerMultiplier = isDrifting ? driftSteerMultiplier : 1f;
            float steerAmount = inputTurn * steerStrength * steerMultiplier * (1 - speedFactor * (1 - minSteerAtMaxStrength));
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0, steerAmount * Time.fixedDeltaTime, 0));

            if (isDrifting)
            {
                Vector3 velocity = rb.linearVelocity;
                Vector3 sideways = Vector3.Dot(velocity, transform.right) * transform.right;
                Vector3 forward = Vector3.Dot(velocity, transform.forward) * transform.forward;
                rb.linearVelocity = forward + sideways * driftGrip;

                driftTime += Time.fixedDeltaTime;
            }

            if (isBoosting)
            {
                if (boostTime < boostDuration)
                {
                    rb.AddForce(transform.forward * acceleration * boostMultiplier, ForceMode.Acceleration);
                    boostTime += Time.fixedDeltaTime;
                }
                else
                {
                    isBoosting = false;
                }
            }
        }

        GetComponent<Renderer>().material.color = isDrifting ? Color.red : (isBoosting ? Color.green : Color.white);
    }
}
