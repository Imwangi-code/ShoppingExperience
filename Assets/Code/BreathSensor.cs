using UnityEngine;

public class BreathSensor : MonoBehaviour
{
    public static BreathSensor Instance;

    [Header("Sensitivity")]
    [Tooltip("Lower = more instant. 0.01 is very sensitive.")]
    public float sensitivity = 0.01f; 

    public bool isInhaling { get; private set; }
    private float _lastPitch;

    void Awake() => Instance = this;

    void Update()
    {
        float currentPitch = transform.localEulerAngles.x;
        if (currentPitch > 180) currentPitch -= 360; 

        float delta = currentPitch - _lastPitch;
        _lastPitch = currentPitch;

        // Instant switch: No timers, no lists. 
        // As soon as the delta exceeds sensitivity, the state flips.
        if (Mathf.Abs(delta) > sensitivity)
        {
            isInhaling = delta > 0;
        }
    }
}