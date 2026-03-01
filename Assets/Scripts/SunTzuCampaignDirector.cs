using System;
using System.Collections.Generic;
using UnityEngine;

public class SunTzuCampaignDirector : MonoBehaviour
{
    [Serializable]
    public class ChapterData
    {
        public string title;
        [TextArea(2, 5)] public string location;
        [TextArea(2, 5)] public string objective;
        [TextArea(2, 5)] public string heroes;
        [TextArea(2, 5)] public string gameplayGoal;
    }

    [Header("Campaign")]
    [SerializeField] private List<ChapterData> chapters = new();
    [SerializeField] private int currentChapterIndex;

    [Header("Progress")]
    [SerializeField] private int gold;
    [SerializeField] private int goldenBricks;
    [SerializeField] private int emperorRank = 1;

    public ChapterData CurrentChapter =>
        chapters.Count == 0 ? null : chapters[Mathf.Clamp(currentChapterIndex, 0, chapters.Count - 1)];

    public int CurrentChapterIndex => currentChapterIndex;

    private void Awake()
    {
        if (chapters.Count == 0)
        {
            LoadDefaultChapters();
        }
    }

    private void Start()
    {
        PrintCurrentChapterBrief();
        PrintGlobalVision();
    }

    public void CompleteMission(int enemiesDefeated, bool minimumLosses)
    {
        gold += Mathf.Max(0, enemiesDefeated);

        if (minimumLosses)
        {
            goldenBricks += 1;
            Debug.Log("Награда: вы получили золотой кирпич за минимальные потери.");
        }

        emperorRank += 1;
        Debug.Log($"Император повысил ваш ранг до уровня {emperorRank}.");
        Debug.Log($"Всего золота: {gold}. Золотых кирпичей: {goldenBricks}.");

        AdvanceChapter();
    }

    public void AdvanceChapter()
    {
        if (currentChapterIndex < chapters.Count - 1)
        {
            currentChapterIndex++;
            Debug.Log($"Переход к главе {currentChapterIndex + 1}: {CurrentChapter.title}");
            PrintCurrentChapterBrief();
        }
        else
        {
            Debug.Log("Кампания завершена: начинается этап строительства Великой стены.");
        }
    }

    private void PrintCurrentChapterBrief()
    {
        if (CurrentChapter == null)
        {
            Debug.LogWarning("Главы кампании не настроены.");
            return;
        }

        Debug.Log($"Глава {currentChapterIndex + 1}: {CurrentChapter.title}");
        Debug.Log($"Локация: {CurrentChapter.location}");
        Debug.Log($"Герои: {CurrentChapter.heroes}");
        Debug.Log($"Задача: {CurrentChapter.objective}");
        Debug.Log($"RPG-цель: {CurrentChapter.gameplayGoal}");
    }

    private static void PrintGlobalVision()
    {
        Debug.Log("Жанр: RPG от 3-го лица + стратегия + головоломка.");
        Debug.Log("Основная идея: тяжёлое военное приключение, где одна ошибка может стоить жизни.");
        Debug.Log("Финальная цель: завершить строительство стены для защиты от врага.");
    }

    private void LoadDefaultChapters()
    {
        chapters = new List<ChapterData>
        {
            new()
            {
                title = "Глава 1 — Пламя Великого Китая",
                location = "Великий Китай",
                objective = "Остановить вражеское наступление, используя честные и хитрые методы.",
                heroes = "Сунь Цзы",
                gameplayGoal = "Выжить и закончить войну как главнокомандующий."
            },
            new()
            {
                title = "Глава 2 — Разорванное поле боя",
                location = "Поле боя",
                objective = "Устранить междуусобицу в государстве.",
                heroes = "Сунь Цзы и отряд воинов",
                gameplayGoal = "Выжить и получить более высокий ранг от императора."
            },
            new()
            {
                title = "Глава 3 — Засада в горах",
                location = "Горы Китая",
                objective = "Спасти министра от внезапной атаки врага из засады.",
                heroes = "Министр и воины Китая",
                gameplayGoal = "Сопроводить министра и не допустить его гибели."
            },
            new()
            {
                title = "Глава 4 — Тени дворца",
                location = "Дворец",
                objective = "Раскрыть предателя среди министров и прислуги.",
                heroes = "Сунь Цзы",
                gameplayGoal = "Провести расследование и устранить заговор."
            }
        };
    }
}
