using UnityEngine;
using TMPro;

public class BreathingManager : MonoBehaviour
{
    public TextMeshProUGUI countdownText;
    public SensoryManager sensoryManager; // Drag your SensoryManager here
    
    private int successfulBreaths = 0;
    private int targetBreaths = 5;
    private bool hasCalmedDown = false;
    private bool wasInhaling = false;

    void Start()
    {
        countdownText.text = "Breathe to focus...";
    }

    void Update()
    {
        if (hasCalmedDown || BreathSensor.Instance == null) return;

        // Detect a completed breath (Inhale then Exhale)
        if (BreathSensor.Instance.isInhaling)
        {
            wasInhaling = true;
        }
        else if (wasInhaling && !BreathSensor.Instance.isInhaling)
        {
            // Just finished an inhale, now exhaling
            OnSuccessfulBreath();
            wasInhaling = false;
        }
    }

    void OnSuccessfulBreath()
    {
        successfulBreaths++;
        int remaining = targetBreaths - successfulBreaths;
        
        if (remaining > 0)
        {
            countdownText.text = remaining.ToString();
        }
        else
        {
            StartCalmState();
        }
    }

    void StartCalmState()
    {
        hasCalmedDown = true;
        countdownText.text = "Calm achieved.";
        
        // This is the link! We tell the manager to fade everything out.
        // We'll use a simple Lerp in the SensoryManager to make this pretty.
        StartCoroutine(FadeOutStress());
    }

    System.Collections.IEnumerator FadeOutStress()
    {
        float duration = 3f; // 3 seconds to fade to calm
        float elapsed = 0;
        float startStress = sensoryManager.stressLevel;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            sensoryManager.stressLevel = Mathf.Lerp(startStress, 0f, elapsed / duration);
            yield return null;
        }
        
        sensoryManager.stressLevel = 0f;
        countdownText.gameObject.SetActive(false); // Hide UI when done
    }
}