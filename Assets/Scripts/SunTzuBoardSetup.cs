using UnityEngine;

public class SunTzuBoardSetup : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SunTzuCampaignDirector director;
    [SerializeField] private SunTzuWorldBuilder worldBuilder;

    [Header("Hero")]
    [SerializeField] private float heroSpeed = 5f;

    private void Start()
    {
        if (director == null)
        {
            director = FindObjectOfType<SunTzuCampaignDirector>();
        }

        if (worldBuilder == null)
        {
            worldBuilder = FindObjectOfType<SunTzuWorldBuilder>();
        }

        SpawnHeroSunTzu();
        SpawnSupportingCharacters();

        if (worldBuilder != null)
        {
            worldBuilder.BuildCurrentChapterPreview();
        }
    }

    private void SpawnHeroSunTzu()
    {
        var hero = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        hero.name = "Hero_SunTzu";
        hero.transform.SetParent(transform, false);
        hero.transform.position = new Vector3(0f, 1f, -4f);
        hero.transform.localScale = new Vector3(1f, 1.1f, 1f);

        var mover = hero.AddComponent<SunTzuSimpleMover>();
        mover.Speed = heroSpeed;

        hero.GetComponent<Renderer>().material.color = new Color(0.12f, 0.12f, 0.16f);
    }

    private void SpawnSupportingCharacters()
    {
        for (int i = 0; i < 5; i++)
        {
            var ally = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ally.name = $"Ally_{i + 1}";
            ally.transform.SetParent(transform, false);
            ally.transform.position = new Vector3(-6f + i * 3f, 1f, -6f + (i % 2));
            ally.transform.localScale = new Vector3(0.55f, 1f, 0.55f);
            ally.GetComponent<Renderer>().material.color = new Color(0.2f, 0.35f, 0.75f);
        }

        if (director != null && director.CurrentChapterIndex == 2)
        {
            var minister = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            minister.name = "Minister";
            minister.transform.SetParent(transform, false);
            minister.transform.position = new Vector3(-3f, 1f, 1.5f);
            minister.GetComponent<Renderer>().material.color = new Color(0.9f, 0.82f, 0.65f);
        }
    }
}
