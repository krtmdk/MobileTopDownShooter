using System.Collections;
using UnityEngine;

// Этот скрипт отвечает за подбираемый предмет.
// Предмет может быть аптечкой, гранатой или клеймором.
// После подбора он временно исчезает и появляется снова.
public class HealthPickup : MonoBehaviour
{
    private enum PickupType
    {
        Health,
        Grenade,
        Claymore
    }

    [Header("Pickup Type")]
    [SerializeField] private PickupType pickupType = PickupType.Health;

    [Header("Health Settings")]
    [SerializeField] private int healAmount = 25;

    [Header("Grenade Settings")]
    [SerializeField] private int grenadeAmount = 1;

    [Header("Claymore Settings")]
    [SerializeField] private int claymoreAmount = 1;

    [Header("Respawn Settings")]
    [SerializeField] private bool canRespawn = true;
    [SerializeField] private float respawnTime = 30f;

    [Header("Visual Settings")]
    [SerializeField] private float rotationSpeed = 100f;

    [Header("Audio")]
    [SerializeField] private AudioClip healthPickupSound;
    // Звук подбора аптечки.

    [SerializeField] private AudioClip itemPickupSound;
    // Звук подбора гранаты или клеймора.

    [SerializeField] private float pickupVolume = 0.8f;
    // Громкость звука подбора.

    private Collider pickupCollider;
    private Renderer[] renderers;
    private bool isAvailable = true;

    private void Awake()
    {
        pickupCollider = GetComponent<Collider>();
        renderers = GetComponentsInChildren<Renderer>();
    }

    private void Update()
    {
        if (!isAvailable)
        {
            return;
        }

        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isAvailable)
        {
            return;
        }

        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

        if (playerHealth == null)
        {
            return;
        }

        bool wasPickedUp = GivePickup(other, playerHealth);

        if (!wasPickedUp)
        {
            return;
        }

        PlayPickupSound();

        if (canRespawn)
        {
            StartCoroutine(RespawnRoutine());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private bool GivePickup(Collider other, PlayerHealth playerHealth)
    {
        if (pickupType == PickupType.Health)
        {
            playerHealth.Heal(healAmount);
            return true;
        }

        if (pickupType == PickupType.Grenade)
        {
            PlayerGrenadeThrower grenadeThrower = other.GetComponent<PlayerGrenadeThrower>();

            if (grenadeThrower == null)
            {
                return false;
            }

            grenadeThrower.AddGrenades(grenadeAmount);
            return true;
        }

        if (pickupType == PickupType.Claymore)
        {
            PlayerClaymorePlacer claymorePlacer = other.GetComponent<PlayerClaymorePlacer>();

            if (claymorePlacer == null)
            {
                return false;
            }

            claymorePlacer.AddClaymores(claymoreAmount);
            return true;
        }

        return false;
    }

    private void PlayPickupSound()
    {
        AudioClip selectedClip = null;

        if (pickupType == PickupType.Health)
        {
            selectedClip = healthPickupSound;
        }
        else
        {
            selectedClip = itemPickupSound;
        }

        if (selectedClip == null)
        {
            return;
        }

        GameObject soundObject = new GameObject("PickupSound");
        soundObject.transform.position = transform.position;

        AudioSource source = soundObject.AddComponent<AudioSource>();
        source.clip = selectedClip;
        source.volume = pickupVolume;
        source.spatialBlend = 0f;
        source.Play();

        Destroy(soundObject, selectedClip.length);
    }

    private IEnumerator RespawnRoutine()
    {
        SetAvailable(false);

        yield return new WaitForSeconds(respawnTime);

        SetAvailable(true);
    }

    private void SetAvailable(bool value)
    {
        isAvailable = value;

        if (pickupCollider != null)
        {
            pickupCollider.enabled = value;
        }

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
            {
                renderers[i].enabled = value;
            }
        }
    }
}