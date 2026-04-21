using UnityEngine;

public class KillCounter : MonoBehaviour
{
    private int killCount;
    // Текущее количество убийств.

    public void RegisterKill()
    {
        // Увеличиваем число убийств на 1.
        killCount++;
    }

    public int GetKillCount()
    {
        // Возвращаем текущее количество убийств.
        return killCount;
    }

    public void ResetKills()
    {
        // Сбрасываем счётчик.
        // Сейчас почти не нужен, потому что Restart перезагружает сцену,
        // но метод полезен на будущее.
        killCount = 0;
    }
}