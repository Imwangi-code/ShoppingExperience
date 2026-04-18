using UnityEngine;
using UnityEngine.Android;

public class BreathingSphereController : MonoBehaviour
{
    public enum TrackingMode { Both, MotionOnly, MicrophoneOnly }

    [Header("Mode Selection")]
    public TrackingMode trackingMode = TrackingMode.Both;

    [Header("References")]
    public Transform sphere; 
    public Color sphereColor = Color.cyan;

    [Header("Sensitivity")]
    public float motionSens = 100f;
    public float micSens = 20f;
    public float micThreshold = 0.02f;

    [Header("Scale Limits")]
    public float minScale = 0.3f;
    public float maxScale = 2.0f;

    private float targetScale = 0.5f;
    private float lastY;
    private AudioClip micClip;
    private string micName;
    private Material sphereMaterial;

    void Start()
    {
        if (sphere != null)
        {
            sphereMaterial = sphere.GetComponent<Renderer>().material;
            sphereMaterial.SetColor("_BaseColor", sphereColor);
        }

        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            Permission.RequestUserPermission(Permission.Microphone);

        lastY = transform.localPosition.y;

        if (Microphone.devices.Length > 0)
        {
            micName = Microphone.devices[0];
            micClip = Microphone.Start(micName, true, 10, 44100);
        }
    }

    void Update()
    {
        float deltaY = 0;
        float vol = 0;

        // 1. Motion Logic (Inhale = Upward movement)
        if (trackingMode != TrackingMode.MicrophoneOnly)
        {
            float currentY = transform.localPosition.y;
            deltaY = currentY - lastY;
            lastY = currentY;

            // Grow if head moves UP
            if (deltaY > 0.0005f) 
                targetScale += deltaY * motionSens;
            // Shrink if head moves DOWN
            else if (deltaY < -0.0005f)
                targetScale -= Mathf.Abs(deltaY) * motionSens;
        }

        // 2. Microphone Logic (Exhale = Noise)
        if (trackingMode != TrackingMode.MotionOnly)
        {
            vol = GetVolume();
            if (vol > micThreshold)
            {
                // Mic almost always indicates an EXHALE, so we shrink
                targetScale -= vol * micSens * Time.deltaTime * 50f;
            }
        }

        // 3. Apply Smooth Scaling
        targetScale = Mathf.Clamp(targetScale, minScale, maxScale);
        float s = Mathf.Lerp(sphere.localScale.x, targetScale, Time.deltaTime * 3f);
        sphere.localScale = Vector3.one * s;

        // 4. Update Opacity/Color
        float t = Mathf.InverseLerp(minScale, maxScale, s);
        Color newColor = sphereColor;
        newColor.a = Mathf.Lerp(0.1f, 0.8f, t);
        
        if (sphereMaterial != null)
            sphereMaterial.SetColor("_BaseColor", newColor);
    }

    float GetVolume()
    {
        if (string.IsNullOrEmpty(micName)) return 0;
        float[] data = new float[128];
        int pos = Microphone.GetPosition(micName);
        if (pos < 128) return 0;
        micClip.GetData(data, pos - 128);
        float peak = 0;
        foreach (var sample in data) peak = Mathf.Max(peak, Mathf.Abs(sample));
        return peak;
    }
}