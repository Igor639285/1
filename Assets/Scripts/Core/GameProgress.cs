using UnityEngine;

public static class GameProgress
{
    private const string UnlockedLevelKey = "UnlockedLevel";

    public static int UnlockedLevel
    {
        get => Mathf.Clamp(PlayerPrefs.GetInt(UnlockedLevelKey, 1), 1, 5);
        set
        {
            int clamped = Mathf.Clamp(value, 1, 5);
            PlayerPrefs.SetInt(UnlockedLevelKey, clamped);
            PlayerPrefs.Save();
        }
    }

    public static void UnlockNextLevel(int completedLevel)
    {
        if (completedLevel >= UnlockedLevel && completedLevel < 5)
        {
            UnlockedLevel = completedLevel + 1;
        }
    }

    public static bool IsLevelUnlocked(int levelIndex)
    {
        return levelIndex <= UnlockedLevel;
    }
}
