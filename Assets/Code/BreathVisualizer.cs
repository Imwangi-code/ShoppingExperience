using UnityEngine;

public class BreathVisualizer : MonoBehaviour
{
    [Header("Visual Bounds")]
    public float minScale = 0.3f;
    public float maxScale = 1.0f;
    
    [Header("Fluidity")]
    [Tooltip("6 is smooth; 10+ is very snappy.")]
    public float transitionSpeed = 6.0f; 

    private Material _mat;
    private float _currentT; 

    void Start()
    {
        // Get the material once at the start to save performance
        _mat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        if (BreathSensor.Instance == null) return;

        // 1. Determine target scale based on breath state
        bool inhaling = BreathSensor.Instance.isInhaling;
        float targetS = inhaling ? maxScale : minScale;

        // 2. Smoothly Scale (No position changes here)
        float lerpStep = Time.deltaTime * transitionSpeed;
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * targetS, lerpStep);

        // 3. Color Shift (Synced to current scale)
        // This ensures the color transition feels "physical"
        _currentT = Mathf.InverseLerp(minScale, maxScale, transform.localScale.x);
        
        Color finalColor = Color.Lerp(Color.blue, Color.green, _currentT);
        _mat.color = finalColor;
        
        // Optional: Adds a slight glow that gets stronger as you inhale
        _mat.SetColor("_EmissionColor", finalColor * (_currentT * 0.5f));
    }
}