using UnityEngine;

public class SensoryOverloadLight : MonoBehaviour {
    private Light _light;
    
    [Header("Settings")]
    public bool isOverloaded = true; 
    public float maxChaosIntensity = 1000f;
    public float calmIntensity = 20f;
    
    [Range(0, 1)]
    public float currentStress = 1.0f; // Controlled by SensoryManager

    void Awake() {
        _light = GetComponent<Light>();
        if (_light == null) {
            Debug.LogError($"No Light component found on {gameObject.name}!");
        }
    }

    void Update() {
        if (_light == null) return;

        if (isOverloaded && currentStress > 0.1f) {
            // The flicker intensity now scales with the stress level
            // This creates the "escalation" feel from the video
            float minFlicker = Mathf.Lerp(calmIntensity, 200f, currentStress);
            float maxFlicker = Mathf.Lerp(calmIntensity, maxChaosIntensity, currentStress);
            
            _light.intensity = Random.Range(minFlicker, maxFlicker);
        } else {
            // Smoothly fade to calm when isOverloaded is false OR stress is low
            _light.intensity = Mathf.Lerp(_light.intensity, calmIntensity, Time.deltaTime * 3f);
        }
    }
}