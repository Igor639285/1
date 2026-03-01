using UnityEngine;
using UnityEngine.UI;

public class SceneButton : MonoBehaviour
{
    public enum ActionType
    {
        OpenLevels,
        OpenSettings,
        Exit,
        LoadLevel
    }

    [SerializeField] private ActionType actionType;
    [SerializeField] private int levelIndex = 1;
    [SerializeField] private MainMenuController mainMenu;
    [SerializeField] private LevelSelectController levelSelect;
    [SerializeField] private Button uiButton;

    private void Awake()
    {
        if (uiButton != null)
        {
            uiButton.onClick.AddListener(HandleAction);
        }
    }

    private void OnMouseDown()
    {
        HandleAction();
    }

    private void HandleAction()
    {
        switch (actionType)
        {
            case ActionType.OpenLevels:
                mainMenu?.OpenLevels();
                break;
            case ActionType.OpenSettings:
                mainMenu?.OpenSettings();
                break;
            case ActionType.Exit:
                mainMenu?.ExitGame();
                break;
            case ActionType.LoadLevel:
                levelSelect?.TryLoadLevel(levelIndex);
                break;
        }
    }
}
