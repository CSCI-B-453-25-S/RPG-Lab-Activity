using System.Collections;
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
    public SpriteRenderer sprite;
    //Animator Constants
    private const string IDLE = "IDLE";
    private const string WALK = "WALK";
    private const string RUN = "RUN";
    private const string ATTACK = "ATTACK";
    public string currentAnimationState = IDLE;
    public bool lockAnimation = false;
    
    private void Start()
    {
        // Store a reference to the Rigidbody2D component on this object in rb.
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if(moveInput.magnitude != 0.0f)
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

        // Call the animation handler
        HandleAnimation();
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

    public void OnAttackInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            ChangeAnimationState(ATTACK, true);
        }
    }

    public void OnSprintInput(InputAction.CallbackContext context)
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

    //InputActions parameters needs to be held down
    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            interactInput = true;
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

    private void HandleAnimation(){
        //Handkes the spriteflipling
        if(moveInput.x < 0.0f){
            sprite.flipX = true;
        }
        else if(moveInput.x > 0.0f){
            sprite.flipX = false;
        }

        //Handles the animation states
        if(moveInput.magnitude == 0.0f)
        {
            ChangeAnimationState(IDLE);
        }
        else if (moveInput.magnitude > 0.0f && moveSpeed == 1)
        {
            ChangeAnimationState(WALK);
        }
        else if (moveInput.magnitude > 0.0f && moveSpeed == 3)
        {
            ChangeAnimationState(RUN);
        }
    }

    private void ChangeAnimationState(string newState, bool isLocked = false)
    {
        // Prevent same animation from restarting.
        if (currentAnimationState == newState) return;

        // If locked and trying to change to something else, do nothing.
        if (lockAnimation) return;

        // Play the animation
        animator.Play(newState);
        currentAnimationState = newState;

        // Lock if it's a locking animation
        if (isLocked)
        {
            lockAnimation = true;
        }
    }

    //Animation for Unlocking the animation
    public void AnimationUnlock()
    {
        lockAnimation = false;
    }
}
