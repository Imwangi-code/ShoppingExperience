using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;


public class GrabScript : MonoBehaviour
{
    private float launchForce = 20; //how moch force to apply to a throw
    private float raycastDist = 50; //how far away can an object be grabbed

    public Image reticle; //the dot where you aim
    public Transform holdPoint; // where the players hand would be
    Transform camTrans; // reference to the camera's transform
    private bool reticleTarget = false; // this check is used to track if the reticle is over a valid target
    public LayerMask grabbableLayers; // What layers can be grabbed
    private Transform heldObject = null; // The held object's transform if a object is held
    private Rigidbody heldRigidbody = null; // The held object's Rigidbody

    InputAction GrabAction;

    private void Start()
    {
        camTrans = Camera.main.transform;
        GrabAction = InputSystem.actions.FindAction("Attack"); //Left Mouse Click
    }

    void Update()
    {
        if (GrabAction.WasPressedThisFrame())
        {
            //On mouse click, check for an object to pick up. If an object is already held throw it.
            if (heldObject == null)
            {
                CheckForPickUp();
            }
            else
            {
                LaunchObject();
            }
        }
    }

    void CheckForPickUp()
    {
        //Cast a ray from the camera position in the direction the camera is facing for a distance of raycastDist.
        //Return a collision with an object on a grabbable layer if one is detected.

        if (Physics.Raycast(camTrans.position, camTrans.forward, out RaycastHit hit, raycastDist, grabbableLayers))
        {
            StartCoroutine(PickUpObject(hit.transform)); //pass in the transform of the collider that was hit
        }
    }

    IEnumerator PickUpObject(Transform _trans)
    {
        heldObject = _trans;
        heldRigidbody = heldObject.GetComponent<Rigidbody>();
        heldRigidbody.isKinematic = true; //ignore gravity and other physics while held
        //heldObject.parent = holdPoint; // make it a child of the hold position so it inharits the position and rotation
        heldRigidbody.detectCollisions = false; 

        float t = 0;
        while (t < .4f)
        {
            //lerp the position of the object to the held position for .4 sec
            //heldRigidbody.position = Vector3.Lerp(heldRigidbody.position, holdPoint.position, t);
            heldRigidbody.MovePosition(Vector3.Lerp(heldRigidbody.position, holdPoint.position + camTrans.forward * 0.5f, t / 0.4f));
            t += Time.deltaTime;
            yield return null;
        }
        SnapToHand(); //When it is close snap it into place
    }

    void SnapToHand()
    {
        heldObject.position = holdPoint.position + camTrans.forward * 0.5f;
    }

    void LaunchObject()
    {
        StopAllCoroutines(); //if the grab coroutine is still running, stop it and skip to the end
        SnapToHand();

        heldRigidbody.isKinematic = false; //regular physics like gravity is active again
        heldRigidbody.detectCollisions = true;
        heldRigidbody.position = camTrans.position + camTrans.forward * 1f; // move slightly in front of camera
        heldRigidbody.linearVelocity = Vector3.zero; // reset any motion
        //heldObject.position = camTrans.position; //jump to the center camera position so it is more lined up with the retical
        heldRigidbody.AddForce(camTrans.forward * launchForce, ForceMode.VelocityChange);  //throw in the direction the camera is facing
        //ForceMode.VelocityChange means add an instant velocity, and the same for any object regardless of mass

        heldObject.parent = null; //remove it as a child and set it back on the root level of the hierarchy
        StartCoroutine(LetGo());
    }

    IEnumerator LetGo()
    {
        yield return new WaitForSeconds(.1f);
        heldObject = null; //remove the reference to the object 
    }


    private void FixedUpdate()
    {
        if (Physics.Raycast(camTrans.position, camTrans.forward, out RaycastHit hit, raycastDist, grabbableLayers))
        {
            //Cast a ray and if the retical is not already red change its color
            if (!reticleTarget)
            {
                reticle.color = Color.red;
                reticleTarget = true; //This bool keeps the color from updatiing if there is no change
            }
        }
        else if (reticleTarget) //if no target is hit and the reticle is active then change it back to white
        {
            reticle.color = Color.white;
            reticleTarget = false;
        }
    }
}