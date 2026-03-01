using UnityEngine;

public class SunTzuWorldBuilder : MonoBehaviour
{
    [SerializeField] private SunTzuCampaignDirector director;
    [SerializeField] private Material locationMaterial;

    private Transform root;

    private void Start()
    {
        if (director == null)
        {
            director = FindObjectOfType<SunTzuCampaignDirector>();
        }

        BuildCurrentChapterPreview();
    }

    public void BuildCurrentChapterPreview()
    {
        if (director == null || director.CurrentChapter == null)
        {
            return;
        }

        if (root != null)
        {
            Destroy(root.gameObject);
        }

        root = new GameObject("ChapterPreviewRoot").transform;
        root.SetParent(transform, false);

        BuildGround();

        switch (director.CurrentChapterIndex)
        {
            case 0:
                BuildGreatChina();
                break;
            case 1:
                BuildBattlefield();
                break;
            case 2:
                BuildMountains();
                break;
            default:
                BuildPalace();
                break;
        }
    }

    private void BuildGround()
    {
        var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.SetParent(root, false);
        ground.transform.localScale = new Vector3(2f, 1f, 2f);

        ApplyColor(ground, new Color(0.52f, 0.45f, 0.34f));
    }

    private void BuildGreatChina()
    {
        for (int i = 0; i < 6; i++)
        {
            var wallSegment = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wallSegment.name = $"WallSegment_{i}";
            wallSegment.transform.SetParent(root, false);
            wallSegment.transform.position = new Vector3(-12 + i * 4.8f, 1.1f, 7.5f);
            wallSegment.transform.localScale = new Vector3(4.5f, 2.2f, 1.2f);
            ApplyColor(wallSegment, new Color(0.62f, 0.6f, 0.55f));
        }

        CreateObjectiveBeacon("RebellionZone", new Vector3(0f, 1f, -7f), new Color(0.75f, 0.12f, 0.12f));
    }

    private void BuildBattlefield()
    {
        for (int i = 0; i < 12; i++)
        {
            var stake = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            stake.name = $"Spear_{i}";
            stake.transform.SetParent(root, false);
            stake.transform.position = new Vector3(-10 + i * 1.8f, 1f, Random.Range(-3f, 3f));
            stake.transform.localScale = new Vector3(0.2f, 1f, 0.2f);
            ApplyColor(stake, new Color(0.4f, 0.34f, 0.2f));
        }

        CreateObjectiveBeacon("CivilConflictCenter", new Vector3(0f, 1f, 0f), new Color(0.8f, 0.35f, 0.12f));
    }

    private void BuildMountains()
    {
        for (int i = 0; i < 5; i++)
        {
            var mountain = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            mountain.name = $"Mountain_{i}";
            mountain.transform.SetParent(root, false);
            mountain.transform.position = new Vector3(-10 + i * 5f, 2f + i * 0.4f, 5f - i);
            mountain.transform.localScale = new Vector3(3f, 4f + i, 3f);
            ApplyColor(mountain, new Color(0.35f, 0.38f, 0.41f));
        }

        CreateObjectiveBeacon("AmbushPoint", new Vector3(4f, 1f, -2f), new Color(0.7f, 0.08f, 0.08f));
        CreateObjectiveBeacon("MinisterEscort", new Vector3(-3f, 1f, 2f), new Color(0.1f, 0.45f, 0.85f));
    }

    private void BuildPalace()
    {
        var palace = GameObject.CreatePrimitive(PrimitiveType.Cube);
        palace.name = "PalaceCore";
        palace.transform.SetParent(root, false);
        palace.transform.position = new Vector3(0f, 2.5f, 6f);
        palace.transform.localScale = new Vector3(11f, 5f, 7f);
        ApplyColor(palace, new Color(0.65f, 0.2f, 0.14f));

        for (int i = 0; i < 6; i++)
        {
            var suspect = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            suspect.name = $"Suspect_{i}";
            suspect.transform.SetParent(root, false);
            suspect.transform.position = new Vector3(-6f + i * 2.4f, 1f, 0f);
            suspect.transform.localScale = new Vector3(0.5f, 1f, 0.5f);
            ApplyColor(suspect, i == 4 ? new Color(0.08f, 0.08f, 0.08f) : new Color(0.76f, 0.71f, 0.62f));
        }

        CreateObjectiveBeacon("TraitorInvestigationZone", new Vector3(0f, 1f, 2f), new Color(0.92f, 0.82f, 0.2f));
    }

    private void CreateObjectiveBeacon(string beaconName, Vector3 position, Color color)
    {
        var beacon = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        beacon.name = beaconName;
        beacon.transform.SetParent(root, false);
        beacon.transform.position = position;
        beacon.transform.localScale = Vector3.one * 1.2f;

        ApplyColor(beacon, color);
    }

    private void ApplyColor(GameObject go, Color color)
    {
        var renderer = go.GetComponent<Renderer>();

        if (locationMaterial != null)
        {
            renderer.material = locationMaterial;
        }

        renderer.material.color = color;
    }
}
