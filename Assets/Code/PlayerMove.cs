using System;
using UnityEngine;
using UnityEngine.XR;

public class PlayerMove : MonoBehaviour
{
    public float speed = 5.0f;
    public XRNode moveStick = XRNode.LeftHand;
    public XRNode rotStick = XRNode.RightHand;

    public Transform body;
    private Rigidbody _rigidbody;
    private Transform camTrans;
    private bool justClicked = false;
    private float rot = 0;

    void Start()
    {
        // Get the main camera reference
        if (Camera.main != null)
        {
            camTrans = Camera.main.transform;
        }
        else
        {
            Debug.LogError("No Main Camera found! Please tag your XR Camera as 'MainCamera'.");
        }

        _rigidbody = GetComponent<Rigidbody>();

        // Align the body to the camera's starting position (ignoring height)
        if (body != null && camTrans != null)
        {
            body.position = new Vector3(camTrans.position.x, body.position.y, camTrans.position.z);
        }
    }

    void Update()
    {
        // Snap Turning Logic
        InputDevices.GetDeviceAtXRNode(rotStick).TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 joyStick);
        
        if (!justClicked)
        {
            if (joyStick.x > 0.8f)
            {
                rot += 90;
                _rigidbody.MoveRotation(Quaternion.Euler(0, rot, 0));
                justClicked = true;
            }
            else if (joyStick.x < -0.8f)
            {
                rot -= 90;
                _rigidbody.MoveRotation(Quaternion.Euler(0, rot, 0));
                justClicked = true;
            }
        }
        else if (Mathf.Abs(joyStick.x) < 0.2f)
        {
            justClicked = false;
        }
    }

    void FixedUpdate()
    {
        if (camTrans == null) return;

        // Get Input from movement stick
        InputDevices.GetDeviceAtXRNode(moveStick).TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 direction);

        // 1. Get Camera Direction
        Vector3 forward = camTrans.forward;
        Vector3 right = camTrans.right;

        // 2. Flatten Direction (Y = 0) so looking up/down doesn't affect movement speed or direction
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        // 3. Calculate Final Move Vector
        Vector3 moveDir = (forward * direction.y + right * direction.x) * speed;

        // 4. Apply movement while preserving gravity
        moveDir.y = _rigidbody.linearVelocity.y;
        _rigidbody.linearVelocity = moveDir;
    }


/*
public class PlayerMove : MonoBehaviour
{

    int speed = 5;
    public XRNode handRole = XRNode.LeftHand;
    Rigidbody _rigidbody;
    Transform camTrans;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        camTrans = Camera.main.transform;
        _rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        InputDevices.GetDeviceAtXRNode(node: handRole).TryGetFeatureValue(usage: CommonUsages.primary2DAxis, value: out Vector2 direction);
        Vector3 moveDir = camTrans.forward * direction.y + camTrans.right * direction.x;
        moveDir = moveDir.normalized * speed;
        moveDir.y = _rigidbody.linearVelocity.y;
        _rigidbody.linearVelocity = moveDir;
    }
}

*/
}