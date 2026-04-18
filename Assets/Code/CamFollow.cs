using UnityEngine;

public class CamFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform followTarget; // Drag your 'SphereTarget' (child of Camera) here
    [Range(0.001f, 0.1f)] 
    public float followSpeed = 0.02f; // Lower = more "laggy" and dreamy
    
    private Transform _camTrans;

    void Start()
    {
        _camTrans = Camera.main.transform;
    }

    void Update()
    {
        if (followTarget == null) return;

        // 1. Move toward the target with lag
        // Using a fixed small number like 0.02f creates that HUD-style delay
        transform.position = Vector3.Lerp(transform.position, followTarget.position, followSpeed);

        // 2. Rotate to face the user
        // We use the camera's X and Z but keep the sphere's Y so it doesn't tilt up/down weirdly
        Vector3 targetLookPos = new Vector3(_camTrans.position.x, transform.position.y, _camTrans.position.z);
        transform.LookAt(targetLookPos);
    }
}