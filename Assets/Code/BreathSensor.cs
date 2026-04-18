using UnityEngine;

public class BreathSensor : MonoBehaviour
{
    public static BreathSensor Instance;

    [Header("Detection Settings")]
    public float motionThreshold = 0.0005f; 
    
    public bool isInhaling { get; private set; }
    private float _lastY;

    void Awake() => Instance = this;

    void Start()
    {
        _lastY = transform.localPosition.y;
    }

    void Update()
    {
        float currentY = transform.localPosition.y;
        float deltaY = currentY - _lastY;
        _lastY = currentY;

        // If head is moving up, state is 'True'
        isInhaling = (deltaY > motionThreshold);
    }
}