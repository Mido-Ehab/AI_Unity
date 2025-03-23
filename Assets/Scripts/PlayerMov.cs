using UnityEngine;

// This class handles player movement and inherits from MonoBehaviour (Unity's base class for scripts)
public class PlayerMov : MonoBehaviour
{           
    // SerializeField attribute makes this private variable visible in Unity Inspector
    // Movement speed of the player, defaulted to 5 units per second
    [SerializeField]
    private float moveSpeed = 5f;

    // SerializeField attribute makes this private variable visible in Unity Inspector
    // Reference to the Rigidbody component for physics-based movement
    [SerializeField]
    private Rigidbody rb;

    // Stores the current movement direction of the player
    private Vector3 moveDirection;

    // Called once when the script instance is being loaded
    private void Start()
    {
        // Get and store the Rigidbody component attached to this GameObject
        rb = GetComponent<Rigidbody>();
    }

    // Called every frame
    private void Update()
    {
        // Get horizontal input (A/D keys or left/right arrows) between -1 and 1
        float moveX = Input.GetAxis("Horizontal");
        // Get vertical input (W/S keys or up/down arrows) between -1 and 1
        float moveZ = Input.GetAxis("Vertical");

        // Create a normalized direction vector from input
        // normalized ensures consistent movement speed in all directions
        moveDirection = new Vector3(moveX, 0f, moveZ).normalized;
    }

    // Called at fixed time intervals (better for physics calculations)
    private void FixedUpdate()
    {
        // Apply velocity to the Rigidbody based on movement direction and speed
        rb.linearVelocity = moveDirection * moveSpeed;
    }
}


