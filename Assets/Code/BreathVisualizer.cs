using UnityEngine;

public class BreathVisualizer : MonoBehaviour
{
    [Header("Scale Settings")]
    public float minScale = 0.3f;
    public float maxScale = 2.0f;
    public float growSpeed = 0.6f; 

    [Header("Color Settings")]
    public Color idleColor = Color.blue;
    public Color breathColor = Color.green;

    private Material _mat;

    void Start()
    {
        _mat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        if (BreathSensor.Instance == null) return;

        float targetSize = BreathSensor.Instance.isInhaling ? maxScale : minScale;
        Vector3 targetScale = new Vector3(targetSize, targetSize, targetSize);

        // Smoothly transition scale
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * growSpeed);

        // Color math based on scale percentage
        float t = (transform.localScale.x - minScale) / (maxScale - minScale);
        Color finalColor = Color.Lerp(idleColor, breathColor, t);
        finalColor.a = Mathf.Lerp(0.1f, 0.8f, t);
        
        if (_mat != null)
        {
            _mat.SetColor("_BaseColor", finalColor);
            _mat.SetColor("_EmissionColor", finalColor * t * 1.5f);
            _mat.EnableKeyword("_EMISSION");
        }
    }
}