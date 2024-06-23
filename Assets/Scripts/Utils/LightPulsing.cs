using UnityEngine.Rendering.Universal;
using UnityEngine;

public class LightPulsing : MonoBehaviour
{
    [SerializeField] private float pulseFrequency = 1f;
    [SerializeField] private float minIntensity = 0.1f;
    [SerializeField] private float maxIntensity = 1f;

    private Light2D light2D;
    private float timer;
    private bool increasingIntensity = true;

    private void Start()
    {
        timer = 0f;
        light2D = GetComponent<Light2D>();
        light2D.intensity = minIntensity;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        float intensityRange = maxIntensity - minIntensity;
        float intensityDelta = Mathf.Sin(timer * 2 * Mathf.PI * pulseFrequency) * intensityRange / 2f;
        light2D.intensity = minIntensity + intensityRange / 2f + intensityDelta;
    }
}
