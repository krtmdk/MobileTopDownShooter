using UnityEngine;

public class LightRandomFlicker : MonoBehaviour
{
    [SerializeField] private Light targetLight;

    [Header("On State")]
    [SerializeField] private float minOnTime = 1f;
    [SerializeField] private float maxOnTime = 5f;

    [Header("Off State")]
    [SerializeField] private float minOffTime = 0.05f;
    [SerializeField] private float maxOffTime = 0.5f;

    [Header("Flicker Burst")]
    [SerializeField] private bool useFlickerBurst = true;
    [SerializeField] private int burstCountMin = 2;
    [SerializeField] private int burstCountMax = 6;

    private float timer;
    private bool isOn = true;

    private int burstRemaining;
    private bool inBurst;

    private void Start()
    {
        if (targetLight == null)
        {
            targetLight = GetComponent<Light>();
        }

        SetOnState(true);
        timer = Random.Range(minOnTime, maxOnTime);
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer > 0f)
            return;

        if (useFlickerBurst && !inBurst && Random.value < 0.3f)
        {
            // запускаем серию быстрых миганий
            inBurst = true;
            burstRemaining = Random.Range(burstCountMin, burstCountMax);
        }

        if (inBurst)
        {
            ToggleLight();

            burstRemaining--;

            if (burstRemaining <= 0)
            {
                inBurst = false;
                SetOnState(true);
                timer = Random.Range(minOnTime, maxOnTime);
            }
            else
            {
                timer = Random.Range(0.05f, 0.15f);
            }

            return;
        }

        // обычный режим
        ToggleLight();

        if (isOn)
        {
            timer = Random.Range(minOnTime, maxOnTime);
        }
        else
        {
            timer = Random.Range(minOffTime, maxOffTime);
        }
    }

    private void ToggleLight()
    {
        SetOnState(!isOn);
    }

    private void SetOnState(bool value)
    {
        isOn = value;

        if (targetLight != null)
        {
            targetLight.enabled = isOn;
        }
    }
}