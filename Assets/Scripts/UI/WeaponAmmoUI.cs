using TMPro;
using UnityEngine;

public class WeaponAmmoUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerShooter playerShooter;
    // Ссылка на скрипт стрельбы игрока.

    [SerializeField] private TextMeshProUGUI weaponAmmoText;
    // Текст HUD, который показывает оружие и патроны.

    private void Start()
    {
        // Если ссылка на игрока не назначена вручную,
        // пробуем найти его по тегу.
        if (playerShooter == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                playerShooter = playerObject.GetComponent<PlayerShooter>();
            }
        }

        UpdateWeaponAmmoText();
    }

    private void Update()
    {
        UpdateWeaponAmmoText();
    }

    private void UpdateWeaponAmmoText()
    {
        if (playerShooter == null || weaponAmmoText == null)
        {
            return;
        }

        string weaponName = playerShooter.GetCurrentWeaponName();

        if (playerShooter.IsReloading())
        {
            weaponAmmoText.text = weaponName + " | Reloading...";
            return;
        }

        int currentAmmo = playerShooter.GetCurrentAmmo();
        int maxAmmo = playerShooter.GetCurrentWeaponMaxAmmoForUI();

        weaponAmmoText.text = weaponName + " | Ammo: " + currentAmmo + " / " + maxAmmo;
    }
}