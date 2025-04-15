using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;

// Force anything with this script to require a Rigidbody2D component.
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour, ISomething, ISomethingB
{
    [Header("Movement Fields")]
    [Tooltip("The movement speed of the player in meters per second.")]
    public int moveSpeed;
    [Tooltip("Vector that will be used to store keyboard movement input.")]
    public Vector2 moveInput;
    public Vector2 facingDir;

    [Header("Interact Fields")]
    [Header("Whether the character is trying to interact with something or not.")]
    public bool interactInput;
    public float raycastLength = 3;
    public LayerMask interactableLayerMask;

    [Header("References")]
    [Tooltip("The Rigidbody2D component on this character.")]
    public Rigidbody2D rb;

    public Animator animator;
    public SpriteRenderer spriteRenderer;
    private const string IDLE = "IDLE";
    private const string ATTACK = "ATTACK";
    private const string WALK = "WALK";
    private const string RUN = "RUN";
    public string currentAnim = IDLE;
    public bool lockAnimation = false;

    private void Start()
    {
        // Store a reference to the Rigidbody2D component on this object in rb.
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        HandleAnimation();

        if (moveInput.magnitude != 0.0f)
        {
            facingDir = moveInput.normalized;
        }

        // Check to see if the player is trying to interact.
        if (interactInput)
        {
            // Reset the boolean.
            interactInput = false;
            // Attempt an interaction with something.
            TryInteract();
        }
    }

    private void FixedUpdate()
    {
        //New velocity code for Unity 6
        // Set direction of the player's movement to match the input.
        // Then set the speed in that direction to the moveSpeed.
        rb.linearVelocity = moveInput.normalized * moveSpeed;
    }

    //InputActions parameters
    public void OnMoveInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    //InputActions parameters needs to be held down
    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            /**
            * Some fun stuff here and there
             */
            interactInput = true;
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            moveSpeed = 3;
        }
        else if (context.phase == InputActionPhase.Canceled) 
        {
            moveSpeed = 1;
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed) {
            ChangeAnimation(ATTACK, true);
        }
    }

    // Testing if interacting works.
    private void TryInteract()
    {
        Debug.DrawRay(transform.position, facingDir * raycastLength, Color.red, 1.0f);
        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position, facingDir, raycastLength, interactableLayerMask);

        if (hit)
        {
            Debug.Log(hit.collider.gameObject.name);

            IConversable conversable = hit.collider.gameObject.GetComponent<IConversable>();
            conversable?.StartConversation();
        }
        else
        {
            Debug.Log("Miissed");
        }
    }

    private void ChangeAnimation(string newState, bool isLocked = false)
    {
        if (currentAnim == newState) { return; }

        if (lockAnimation) { return; }

        animator.Play(newState);
        currentAnim = newState;

        if (isLocked)
        {
            lockAnimation = true;
        }
    }

    private void HandleAnimation()
    {
        if(moveInput.x < 0.0f)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }

        if (moveInput.magnitude == 0.0f)
        {
            ChangeAnimation(IDLE);
            /**
             * Some fun stuff here and there
             */
        }
        else if (moveInput.magnitude > 0.0f && moveSpeed == 1)
        {
            ChangeAnimation(WALK);
        }else if(moveInput.magnitude > 0.0f && moveSpeed > 1)
        {
            ChangeAnimation(RUN);
        }
    }

    public void UnlockAnimation()
    {
        lockAnimation = false;
    }

    /**
     * Some wicked cool lines of code
     * that does something totally awesome, that WE WANT TO KEEP
     * ---------------------------------------------------------
     * LIES, the line was not cool and caused bugs
     * _________________________________________________________
     * Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt 
     * ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation 
     * ullamco laboris nisi ut aliquip exea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse 
     * cillum dolore eu fugiat nulla pariatur.Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum
     */
}
