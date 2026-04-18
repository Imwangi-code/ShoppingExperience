using UnityEngine;

public class BreathMotionTracker : MonoBehaviour
{
    [Header("Calibration")]
    public float sensitivity = 100f; // Quest 3 movement is tiny, so we boost it
    public float smoothSpeed = 2f;  // Smooths out micro-jitters from tracking

    [Header("Live Data")]
    [Range(0, 1)] public float breathCycle; // 0 = Exhale floor, 1 = Inhale peak
    
    private float _previousY;
    private float _velocity;

    void Start()
    {
        _previousY = transform.localPosition.y;
    }

    void Update()
    {
        // 1. Get the vertical delta (how much the head moved up/down this frame)
        float currentY = transform.localPosition.y;
        float deltaY = currentY - _previousY;

        // 2. Map movement to a 0-1 scale
        // We use a target value based on whether deltaY is positive (inhale) or negative (exhale)
        float targetBreath = Mathf.Clamp01(deltaY * sensitivity + 0.5f);

        // 3. Smooth the transition so visuals don't "snap"
        breathCycle = Mathf.Lerp(breathCycle, targetBreath, Time.deltaTime * smoothSpeed);

        _previousY = currentY;
    }
}