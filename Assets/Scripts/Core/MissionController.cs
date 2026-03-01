using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionController : MonoBehaviour
{
    [SerializeField] private int levelIndex = 1;

    private void Update()
    {
        bool enemiesAlive = HasAlive(Combatant.Faction.Enemy);
        bool alliesAlive = HasAlive(Combatant.Faction.Player) || HasAlive(Combatant.Faction.Ally);

        if (!enemiesAlive)
        {
            GameProgress.UnlockNextLevel(levelIndex);
            Debug.Log("Победа! Следующий уровень разблокирован.");
            SceneManager.LoadScene("LevelSelect");
            enabled = false;
            return;
        }

        if (!alliesAlive)
        {
            Debug.Log("Поражение! Все ваши силы уничтожены.");
            SceneManager.LoadScene("LevelSelect");
            enabled = false;
        }
    }

    private static bool HasAlive(Combatant.Faction faction)
    {
        Combatant[] all = FindObjectsOfType<Combatant>();
        for (int i = 0; i < all.Length; i++)
        {
            if (all[i].Team == faction && all[i].IsAlive)
            {
                return true;
            }
        }

        return false;
    }
}
