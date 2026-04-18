using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{

    int moveSpeed = 5; //how fast the player moves
    float lookSpeedX = .2f; //left/right muse sensitivity
    float lookSpeedY = .2f; //up/down mouse sensitivity
    int jumpForce = 300; //amount of force applied to create a jump

    Transform camTrans; //a reference to the camera's transform
    float xRotation;
    float yRotation;
    Rigidbody _rigidbody; //a reference to the player's rigidbody, _ is a naming convention for private variables

    InputAction moveAction; //reference to the move input action
    InputAction lookAction; //reference to the look input action
    InputAction jumpAction; //reference to the jump input action


    //The physics layers you want the player to be able to jump off of. Just dont include the layer the palyer is on.
    public LayerMask groundLayer;
    //public Transform feetTrans; //Position of where the players feet touch the ground
    float groundCheckDist = .35f; //How far down to check for the ground. The radius of Physics.CheckSphere
    public bool grounded = false; //Is the player on the ground


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        Cursor.lockState = CursorLockMode.Locked; //Hides the mouse and locks it ot the center of the screen

        camTrans = Camera.main.transform; //get the main camera's transform
        _rigidbody = GetComponent<Rigidbody>(); //get the player's rigidbody... get component is expensive, so we only do it once here in start

        //get references to the input actions
        moveAction = InputSystem.actions.FindAction("Move"); //Find the set of actios in the input system called "Move"
        lookAction = InputSystem.actions.FindAction("Look");
        jumpAction = InputSystem.actions.FindAction("Jump");
        
    }

    void FixedUpdate()
    {
        //Creates a movement vector local to the direction the player is facing.
        Vector3 moveDir = transform.forward * moveAction.ReadValue<Vector2>().y + transform.right * moveAction.ReadValue<Vector2>().x; //Use Move X and Y for forward and strafing movement
        moveDir *= moveSpeed;
        moveDir.y = _rigidbody.linearVelocity.y; // We dont want y so we replace y with that the _rigidbody.velocity.y already is.
        _rigidbody.linearVelocity = moveDir; // Set the velocity to our movement vector

         //The sphere check draws a sphere like a ray cast and returns true if any collider is withing its radius.
        //grounded is set to true if a sphere at feetTrans.position with a radius of groundCheckDist detects any objects on groundLayer within it
        grounded = Physics.CheckSphere(transform.position, groundCheckDist, groundLayer);    
        
    }

    // Update is called once per frame
    void Update()
    {
        yRotation += lookAction.ReadValue<Vector2>().x * lookSpeedX; //get the x value of the look input and multiply it by the look speed
        xRotation -= lookAction.ReadValue<Vector2>().y * lookSpeedY; //get  the y value of the look input and multiply it by the look speed, this is negative bc moving the mouse up gives a negative y value (inverted)
        xRotation = Mathf.Clamp(xRotation, -90, 90); //keeps the up/down head rotation realistic btwn -90 and 90 degrees

        camTrans.localEulerAngles = new Vector3(xRotation, 0, 0); //apply the x rotation to the camera's local rotation, rotate head up/down
        transform.eulerAngles = new Vector3(0, yRotation, 0); //apply the y rotation to the player's rotation, rotate body left/right

          if (grounded && jumpAction.WasPressedThisFrame()) //if the player is on the ground and press Spacebar
        {
            _rigidbody.linearVelocity = Vector3.zero; //Zero out gravity before applying the jumpForce
            _rigidbody.AddForce(new Vector3(0, jumpForce, 0)); // Add a force jumpForce in the Y direction
        }
    }


}
