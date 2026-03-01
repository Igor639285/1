using UnityEngine;

public class SunTzuMissionLabel : MonoBehaviour
{
    [SerializeField] private SunTzuCampaignDirector director;

    private void Start()
    {
        if (director == null)
        {
            director = FindObjectOfType<SunTzuCampaignDirector>();
        }

        Debug.Log("Сюжет: Сунь У отправлен сдерживать врага до прибытия союзных войск.");
        Debug.Log("Угроза: враг проник в состав чиновников, его необходимо выявить.");

        if (director != null && director.CurrentChapter != null)
        {
            Debug.Log($"Активная глава: {director.CurrentChapter.title}");
            Debug.Log($"Ключевая задача: {director.CurrentChapter.objective}");
        }
    }
}
