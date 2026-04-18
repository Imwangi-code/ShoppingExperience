using UnityEngine;
using UnityEngine.Rendering; // Required for Volume

public class SensoryManager : MonoBehaviour {
    public Volume globalVolume;      // Drag your Global Volume here
    public AudioSource[] chaosSounds; // Drag all your loud sounds here
    public SensoryOverloadLight[] lights; // We will link these below
    
    [Range(0, 1)]
    public float stressLevel = 1.0f; // 1 is Chaos, 0 is Calm

    void Update() {
        // 1. Controls the Blur/Distortion
        globalVolume.weight = stressLevel;

        // 2. Controls the Sound Volume
        foreach (var source in chaosSounds) {
            source.volume = stressLevel * 0.5f; // Adjust max volume here
        }

        // 3. Tells the lights whether to flicker
        foreach (var lightScript in lights) {
            lightScript.isOverloaded = (stressLevel > 0.5f);
        }
    }
}