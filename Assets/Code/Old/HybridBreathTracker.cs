using UnityEngine;
using UnityEngine.Android; // Required for Quest microphone permissions

public class HybridBreathTracker : MonoBehaviour
{
    [Header("Microphone Settings")]
    public float micSensitivity = 100f;
    public float micThreshold = 0.02f; // Ignore background hum
    
    [Header("Motion Settings")]
    public float motionSensitivity = 80f;

    [Header("Output (0 to 1)")]
    public float breathCombinedValue;

    private AudioClip _micClip;
    private string _deviceName;
    private float[] _sampleBuffer = new float[128];
    private float _prevY;

    void Start()
    {
        // 1. Request Microphone Permission for Android (Quest)
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }

        // 2. Start Microphone
        _deviceName = Microphone.devices[0];
        _micClip = Microphone.Start(_deviceName, true, 10, 44100);
        _prevY = transform.localPosition.y;
    }

    void Update()
    {
        // --- Part A: Audio Detection ---
        float currentMaxVolume = 0;
        int micPosition = Microphone.GetPosition(_deviceName);
        
        if (micPosition > 128) 
        {
            _micClip.GetData(_sampleBuffer, micPosition - 128);
            foreach (var sample in _sampleBuffer)
            {
                currentMaxVolume = Mathf.Max(currentMaxVolume, Mathf.Abs(sample));
            }
        }

        float audioInput = (currentMaxVolume > micThreshold) ? currentMaxVolume * micSensitivity : 0;

        // --- Part B: Motion Detection ---
        float currentY = transform.localPosition.y;
        float motionInput = Mathf.Abs(currentY - _prevY) * motionSensitivity;
        _prevY = currentY;

        // --- Part C: Combine & Smooth ---
        float rawCombined = Mathf.Clamp01(audioInput + motionInput);
        breathCombinedValue = Mathf.Lerp(breathCombinedValue, rawCombined, Time.deltaTime * 5f);
    }
}