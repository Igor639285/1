using UnityEngine;

public class SunTzuBoardSetup : MonoBehaviour
{
    [Header("Board")]
    [SerializeField] private int boardSize = 8;
    [SerializeField] private float tileSize = 1f;
    [SerializeField] private Color lightTile = new(0.82f, 0.78f, 0.63f);
    [SerializeField] private Color darkTile = new(0.36f, 0.26f, 0.18f);

    [Header("Markers")]
    [SerializeField] private Color strategistColor = new(0.75f, 0.13f, 0.13f);
    [SerializeField] private Color generalColor = new(0.15f, 0.15f, 0.18f);

    private void Start()
    {
        BuildBoard();
        BuildThemeMarkers();
    }

    private void BuildBoard()
    {
        var boardRoot = new GameObject("BoardRoot").transform;
        boardRoot.SetParent(transform, false);

        float offset = (boardSize - 1) * tileSize * 0.5f;

        for (int x = 0; x < boardSize; x++)
        {
            for (int z = 0; z < boardSize; z++)
            {
                var tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
                tile.name = $"Tile_{x}_{z}";
                tile.transform.SetParent(boardRoot, false);
                tile.transform.position = new Vector3(x * tileSize - offset, -0.05f, z * tileSize - offset);
                tile.transform.localScale = new Vector3(tileSize, 0.1f, tileSize);

                var renderer = tile.GetComponent<Renderer>();
                renderer.material.color = (x + z) % 2 == 0 ? lightTile : darkTile;
            }
        }
    }

    private void BuildThemeMarkers()
    {
        float edge = (boardSize - 1) * tileSize * 0.5f;

        CreateMarker("General", PrimitiveType.Capsule, new Vector3(0f, 0.65f, -edge - 1.2f), generalColor, new Vector3(0.75f, 1.2f, 0.75f));
        CreateMarker("Strategist", PrimitiveType.Cylinder, new Vector3(0f, 0.5f, edge + 1.2f), strategistColor, new Vector3(0.8f, 1f, 0.8f));

        var center = GameObject.CreatePrimitive(PrimitiveType.Quad);
        center.name = "DoctrineMarker";
        center.transform.SetParent(transform, false);
        center.transform.position = new Vector3(0f, 0.06f, 0f);
        center.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        center.transform.localScale = Vector3.one * 1.8f;
        center.GetComponent<Renderer>().material.color = new Color(0.85f, 0.65f, 0.2f);
    }

    private void CreateMarker(string markerName, PrimitiveType type, Vector3 position, Color color, Vector3 scale)
    {
        var marker = GameObject.CreatePrimitive(type);
        marker.name = markerName;
        marker.transform.SetParent(transform, false);
        marker.transform.position = position;
        marker.transform.localScale = scale;
        marker.GetComponent<Renderer>().material.color = color;
    }
}
