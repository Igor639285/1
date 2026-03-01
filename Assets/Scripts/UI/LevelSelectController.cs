using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectController : MonoBehaviour
{
    [SerializeField] private Renderer[] levelRenderers;

    private void Start()
    {
        RefreshView();
    }

    public void TryLoadLevel(int levelIndex)
    {
        if (!GameProgress.IsLevelUnlocked(levelIndex))
        {
            Debug.Log($"Уровень {levelIndex} пока заблокирован.");
            return;
        }

        if (levelIndex == 1)
        {
            SceneManager.LoadScene("Level1_Ambush");
            return;
        }

        Debug.Log($"Уровень {levelIndex} пока не собран. Возврат в выбор уровней.");
    }

    private void RefreshView()
    {
        if (levelRenderers == null)
        {
            return;
        }

        for (int i = 0; i < levelRenderers.Length; i++)
        {
            if (levelRenderers[i] == null)
            {
                continue;
            }

            int level = i + 1;
            bool unlocked = GameProgress.IsLevelUnlocked(level);
            levelRenderers[i].material.color = unlocked ? new Color(0.2f, 0.7f, 0.25f) : new Color(0.35f, 0.35f, 0.35f);
        }
    }
}
