using UnityEngine;

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

    private void OnMouseDown()
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
