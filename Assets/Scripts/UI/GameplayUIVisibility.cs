using UnityEngine;

// Этот скрипт отвечает за включение и выключение игрового HUD и мобильных контролов.
// Используется при паузе, победе и поражении.
public class GameplayUIVisibility : MonoBehaviour
{
    [Header("References")]

    [SerializeField] private GameObject gameplayHUDRoot;
    // Корневой объект обычного игрового интерфейса (HP, Wave, Kills, Ammo)

    [SerializeField] private GameObject mobileControlsRoot;
    // Корневой объект мобильных стиков и кнопок

    // Этот метод включает игровой UI обратно
    public void ShowGameplayUI()
    {
        // Включаем основной HUD
        if (gameplayHUDRoot != null)
        {
            gameplayHUDRoot.SetActive(true);
        }

        // Включаем мобильные контролы
        if (mobileControlsRoot != null)
        {
            mobileControlsRoot.SetActive(true);
        }
    }

    // Этот метод скрывает игровой UI
    public void HideGameplayUI()
    {
        // Выключаем основной HUD
        if (gameplayHUDRoot != null)
        {
            gameplayHUDRoot.SetActive(false);
        }

        // Выключаем мобильные контролы
        if (mobileControlsRoot != null)
        {
            mobileControlsRoot.SetActive(false);
        }
    }
}