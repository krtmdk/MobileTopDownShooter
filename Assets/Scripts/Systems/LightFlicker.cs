using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [SerializeField] private Light targetLight;

    [SerializeField] private float minIntensity = 1.5f;
    [SerializeField] private float maxIntensity = 3f;

    [SerializeField] private float flickerSpeed = 0.1f;

    private float timer;

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            targetLight.intensity = Random.Range(minIntensity, maxIntensity);
            timer = flickerSpeed;
        }
    }
}