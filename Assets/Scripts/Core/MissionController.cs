using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionController : MonoBehaviour
{
    [SerializeField] private int levelIndex = 1;
    [SerializeField] private float endDelay = 1.5f;

    private bool finishing;
    private float timer;

    private void Update()
    {
        if (finishing)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                SceneManager.LoadScene("LevelSelect");
            }

            return;
        }

        bool enemiesAlive = HasAlive(Combatant.Faction.Enemy);
        bool alliesAlive = HasAlive(Combatant.Faction.Player) || HasAlive(Combatant.Faction.Ally);

        if (!enemiesAlive)
        {
            GameProgress.UnlockNextLevel(levelIndex);
            Debug.Log("Победа! Следующий уровень разблокирован.");
            finishing = true;
            timer = endDelay;
            return;
        }

        if (!alliesAlive)
        {
            Debug.Log("Поражение! Все ваши силы уничтожены.");
            finishing = true;
            timer = endDelay;
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
