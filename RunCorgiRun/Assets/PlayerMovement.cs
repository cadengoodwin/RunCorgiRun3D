using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public Renderer checkpointRenderer;
    public GameObject deathEffectPrefab;
    public SkinnedMeshRenderer playerModelRender;

    private bool wasFacingRightAtDeath;
    public GameObject playerModel;
    public GameObject checkpointEffectPrefab;
    private bool checkpointEffectSpawned = false;
    private bool isDead = false;

    public Slider staminaSlider; // Assign this in the Unity Editor
    private float stamina = 3.0f;
    private float maxStamina = 3.0f;
    private float staminaRechargeRate = 3.0f / 2.0f; // Recharges in 3 seconds
    public Image fillImage; // Assign this in the Unity Editor
    public Image backgroundImage; // Assign this in the Unity Editor
    
    public float walkSpeed = 3.0f;
    public float runSpeed = 6.0f;
    public float jumpForce = 5.0f;
    public Transform groundCheck;
    public float checkRadius = 0.2f;
    public int coinCount = 0;

    public LayerMask whatIsGround;
    public string nextSceneName;
    public Transform checkpointTransform;

    private Vector3 checkpointPosition;
    private bool hasCheckpoint = false;

    private Rigidbody rb;
    private Animator animator;
    private bool facingRight = true;
    private bool isGrounded;
    private float currentSpeed;
    private float fixedXPosition; // Added variable to store the X position

    private Vector3 startPosition;
    private Quaternion startRotation;
    private Vector3 startLocalEulerAngles;
    
    
    public float targetXPosition; // Set this in the Unity Editor
    public bool shouldLerpToPosition = false;
    public float lerpSpeed = 1.0f; // Speed of the lerp
    private Color normalBackgroundColor; // Store the normal background color

    [HideInInspector]
    public bool isDrunk = false;

    private void Start()
    {
        coinCount = PlayerPrefs.GetInt("CoinCount", 0); // Load saved coin count, default to 0

        if (backgroundImage != null)
        {
            normalBackgroundColor = backgroundImage.color; // Save the original background color
        }
        
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        currentSpeed = walkSpeed;

        startPosition = transform.position;
        startRotation = transform.rotation;
        startLocalEulerAngles = transform.localEulerAngles;
        fixedXPosition = transform.position.x; // Initialize the fixed X position
    }

    private void Update()
    {
        
        UpdateStamina();
        UpdateUI();
        
        isGrounded = Physics.CheckSphere(groundCheck.position, checkRadius, whatIsGround);
        float moveHorizontal = Input.GetAxis("Horizontal");

        if (isDrunk)
        {
            moveHorizontal *= -1;
        }
    
        HandleMovement(moveHorizontal);
        CheckIfPlayerFellOutOfWorld();

        // Update fixedXPosition only if player is grounded and not lerping
        if (isGrounded && !shouldLerpToPosition)
        {
            fixedXPosition = transform.position.x;
        }
    
        if (shouldLerpToPosition)
        {
            float newX = Mathf.Lerp(transform.position.x, targetXPosition, lerpSpeed * Time.deltaTime);
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);

            // Optional: Stop lerp when close enough to the target
            if (Mathf.Abs(transform.position.x - targetXPosition) < 0.01f)
            {
                shouldLerpToPosition = false;
                fixedXPosition = transform.position.x; // Update fixedXPosition after lerp is complete
            }
        }
    }

    private void UpdateStamina()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            if (stamina > 0)
            {
                currentSpeed = runSpeed;
                stamina -= Time.deltaTime; // Decrease stamina when running
            }
            else
            {
                currentSpeed = walkSpeed; // Switch to walking speed when stamina is depleted
            }
        }
        else
        {
            currentSpeed = walkSpeed;
            stamina += staminaRechargeRate * Time.deltaTime; // Recharge stamina
        }
        stamina = Mathf.Clamp(stamina, 0, maxStamina); // Ensure stamina stays within bounds
    }

    private void UpdateUI()
    {
        if (staminaSlider != null)
        {
            staminaSlider.value = stamina / maxStamina;

            // Hide the stamina slider when stamina is full and the player is not running
            bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            staminaSlider.gameObject.SetActive(!(stamina >= maxStamina && !isRunning));
        }

        // Hide fill image and change background color when stamina is depleted
        if (fillImage != null && backgroundImage != null)
        {
            if (stamina <= 0)
            {
                fillImage.enabled = false;
                backgroundImage.color = Color.red;
            }
            else
            {
                fillImage.enabled = true;
                backgroundImage.color = normalBackgroundColor;
            }
        }
    }
    private void LateUpdate()
    {
        // If the player is grounded, enforce the fixed X position.
        if (isGrounded && !shouldLerpToPosition)
        {
            transform.position = new Vector3(fixedXPosition, transform.position.y, transform.position.z);
        }
    }

    private void HandleMovement(float moveHorizontal)
    {
        Vector3 movement = new Vector3(0.0f, 0.0f, moveHorizontal);
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, movement.z * currentSpeed);

        if (moveHorizontal > 0 && !facingRight)
        {
            Flip();
        }
        else if (moveHorizontal < 0 && facingRight)
        {
            Flip();
        }

        if (isGrounded)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                currentSpeed = runSpeed;
                animator.Play("Run");
            }
            else
            {
                currentSpeed = walkSpeed;
                if (moveHorizontal != 0) 
                {
                    animator.Play("Walk");
                }
            }
        }
        else
        {
            currentSpeed = walkSpeed;
            if (moveHorizontal != 0) 
            {
                animator.Play("Walk");
            }
        }

        if (moveHorizontal == 0 && isGrounded)
        {
            animator.Play("Idle");
        }

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.Play("Jump");
        }
    }

private void Flip()
{
    facingRight = !facingRight;
    Vector3 theScale = transform.localScale;
    theScale.z *= -1;
    transform.localScale = theScale;
}


    
    

    private void CheckIfPlayerFellOutOfWorld()
    {
        if (transform.position.y < -10)
        {
            PlayerDied();
        }
    }

    private void PlayerDied()
    {
        // Check if the player is already dead
        if (isDead) return;

        isDead = true; // Set the player to dead state

        if (staminaSlider != null)
        {
            staminaSlider.gameObject.SetActive(false);
        }
        
        // Instantiate the death effect at the player's position
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        // Disable player model render, movement, and inputs
        if (playerModelRender != null)
        {
            playerModelRender.enabled = false;
        }
        DisableMovementAndInputs();

        // Wait for 3 seconds before respawning
        Invoke(nameof(Respawn), 3f);
    }


    
    private void DisableMovementAndInputs()
    {
        rb.isKinematic = true; // Make the Rigidbody kinematic to disable physics-based movement
        animator.enabled = false; // Disable the Animator
        this.enabled = false; // Disable this script to prevent inputs being processed
    }
    
    private void DisableMovement()
    {
        rb.isKinematic = true; // Make the Rigidbody kinematic to disable movement
        animator.enabled = false; // Disable the Animator
    }

    private void Respawn()
    {
        isDead = false; // Reset the dead state
        
        stamina = maxStamina;
        if (staminaSlider != null)
        {
            staminaSlider.gameObject.SetActive(true);
            staminaSlider.value = stamina / maxStamina;
        }
        // Enable player model render, movement, and inputs
        if (playerModelRender != null)
        {
            playerModelRender.enabled = true; // Enable the Skinned Mesh Renderer
        }
        rb.isKinematic = false; // Make the Rigidbody non-kinematic to enable movement
        animator.enabled = true; // Enable the Animator
        this.enabled = true; // Re-enable this script to process inputs again
        
        // Respawn logic
        transform.SetParent(null); // Ensure player isn't childed to anything upon respawn
        if (hasCheckpoint)
        {
            transform.position = checkpointPosition;
        }
        else
        {
            transform.position = startPosition;
        }
        transform.rotation = startRotation;
        transform.localEulerAngles = startLocalEulerAngles;
        rb.velocity = Vector3.zero;

        // Restore the facing direction
        facingRight = wasFacingRightAtDeath;
        UpdatePlayerOrientation(); // Ensure player's orientation is updated
    }
    
    
private void UpdatePlayerOrientation()
{
    if ((facingRight && transform.localScale.z < 0) || (!facingRight && transform.localScale.z > 0))
    {
        Vector3 theScale = transform.localScale;
        theScale.z *= -1;
        transform.localScale = theScale;
    }
}





    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("XReturn")) // Replace with your trigger's tag
        {
            shouldLerpToPosition = true;
        }
        
        
        if (other.CompareTag("saw"))
        {
            PlayerDied();
        }
        else if (other.CompareTag("Checkpoint") && !checkpointEffectSpawned)
        {
            if (checkpointTransform != null)
            {
                checkpointPosition = checkpointTransform.position;
            }
            else
            {
                checkpointPosition = transform.position; // Fallback to current position
            }
            hasCheckpoint = true;

            // Change checkpoint material color to green
            if (checkpointRenderer != null)
            {
                checkpointRenderer.material.color = new Color(0f, 0.5f, 0f, 1f); // Dark green color
            }

            // Instantiate the checkpoint effect only once
            if (checkpointEffectPrefab != null)
            {
                Quaternion rotation = Quaternion.Euler(-90, 0, 0);
                Instantiate(checkpointEffectPrefab, checkpointTransform.position, rotation);
                checkpointEffectSpawned = true; // Set the flag to true
            }
        }
        
        else if (other.CompareTag("FinalCheckpoint"))
        {
            // Load the next scene
            SceneManager.LoadScene(nextSceneName);
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            transform.SetParent(collision.transform);
            // No need to update fixedXPosition here since it's updated in Update() when grounded
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            transform.SetParent(null);
        }
    }
    
    
    
}