using UnityEngine;

public class AbodiMovements : MonoBehaviour
{
    public CharacterController myPlayer;

    [Header("Movements")]
    public float speed = 4f; 
     public float currentSpeed = 8f;
     public float jumpForce = 8f;
     public float gravity = -20f;

     [Header("StaminaLogic")]
    public float stamina = 100f;
    public float maxStamina = 100f; 
    public float drainRate = 20f; 
    public float refillRate = 15f;
    public bool canSprint = true;

     [Header("CheckGround")]
    public bool grounded;
    public Vector3 velocity;
    public LayerMask groundMask;
    public float groundDistance = 1.1f;

    public float liftDuration = 0.2f;
    public float liftTimer; 

    void FixedUpdate()
    {
        float Horizontal = Input.GetAxis("Horizontal");
        float Vertical = Input.GetAxis("Vertical");

        Vector3 move = (transform.right * Horizontal) + (transform.forward * Vertical); 

        grounded = Physics.Raycast(transform.position, Vector3.down, groundDistance, groundMask);
        Debug.DrawRay(transform.position, Vector3.down * groundDistance, grounded ? Color.green : Color.red);

        bool isTryingToSprint = Input.GetKey(KeyCode.LeftShift) && Vertical > 0; 

        if (isTryingToSprint && canSprint && stamina > 0)
        {
            move *= currentSpeed;
            stamina -= drainRate * Time.deltaTime;
            if (stamina <= 0) canSprint = false;
        }
        else
        {
            move *= speed; 
            if (stamina < maxStamina)
                stamina += refillRate * Time.deltaTime;

            if (stamina >= 20f) canSprint = true;
        }

        if (grounded && velocity.y < 0)
        {
            velocity.y = -1f;
        }

        if (Input.GetButtonDown("Jump") && grounded == true)
        {
            liftTimer = liftDuration;
        }

        if (liftTimer > 0)
        {
            velocity.y += jumpForce * (liftTimer / liftDuration) * Time.deltaTime * 10f;
            liftTimer -= Time.deltaTime;
        }

        velocity.y += gravity * Time.deltaTime;
        myPlayer.Move(velocity * Time.deltaTime);
       
        myPlayer.Move(move * Time.deltaTime);
    }
}
