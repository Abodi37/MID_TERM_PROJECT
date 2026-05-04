using UnityEngine;

public class AbodiMovements : MonoBehaviour
{
    public CharacterController myPlayer;
    public float speed = 5f; 
    public bool grounded;
    public float gravity = -9.81f;
    public Vector3 velocity;
    public float currentSpeed = 20f;
    public float jumpForce = 10f;
    public LayerMask groundMask;
    public float groundDistance = 1.1f;
    void FixedUpdate()
    {
        float Horizontal = Input.GetAxis("Horizontal");
        float Vertical = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(Horizontal, 0, Vertical);
        myPlayer.Move(move * speed * Time.deltaTime);

        grounded = Physics.Raycast(transform.position, Vector3.down, groundDistance, groundMask);
        Debug.DrawRay(transform.position, Vector3.down * groundDistance, grounded ? Color.green : Color.red);

        if (grounded == false)
        {
            velocity.y = gravity;
            myPlayer.Move(velocity * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            myPlayer.Move(move * currentSpeed * Time.deltaTime);
        }
        if (Input.GetButtonDown("Jump") && grounded == true)
        {
            velocity.y = jumpForce;
            myPlayer.Move(velocity * Time.deltaTime);
        }

    }
}
