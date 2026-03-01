using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RuntimeUiBuilder : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Build()
    {
        string scene = SceneManager.GetActiveScene().name;
        if (scene == "MainMenu")
        {
            BuildMainMenu();
        }
        else if (scene == "LevelSelect")
        {
            BuildLevelSelect();
        }
        else if (scene == "Level1_Ambush")
        {
            BuildBattleHud();
        }
    }

    private static Canvas CreateCanvas(string name)
    {
        GameObject canvasGo = new(name);
        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasGo.AddComponent<GraphicRaycaster>();

        if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject evt = new("EventSystem");
            evt.AddComponent<UnityEngine.EventSystems.EventSystem>();
            evt.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        return canvas;
    }

    private static Button CreateButton(Transform parent, string text, Vector2 anchored, Vector2 size)
    {
        GameObject go = new(text + "Btn");
        go.transform.SetParent(parent, false);

        Image image = go.AddComponent<Image>();
        image.color = new Color(0.12f, 0.12f, 0.16f, 0.86f);
        Button button = go.AddComponent<Button>();

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = anchored;
        rt.sizeDelta = size;

        GameObject label = new(text + "Label");
        label.transform.SetParent(go.transform, false);
        Text txt = label.AddComponent<Text>();
        txt.text = text;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        txt.color = Color.white;
        txt.resizeTextForBestFit = true;

        RectTransform lrt = label.GetComponent<RectTransform>();
        lrt.anchorMin = Vector2.zero;
        lrt.anchorMax = Vector2.one;
        lrt.offsetMin = Vector2.zero;
        lrt.offsetMax = Vector2.zero;

        return button;
    }

    private static void BuildMainMenu()
    {
        MainMenuController menu = Object.FindObjectOfType<MainMenuController>();
        if (menu == null)
        {
            menu = new GameObject("MainMenuControllerUI").AddComponent<MainMenuController>();
        }

        Canvas canvas = CreateCanvas("MainMenuCanvas");
        Button levels = CreateButton(canvas.transform, "Уровни", new Vector2(0, 80), new Vector2(300, 70));
        Button settings = CreateButton(canvas.transform, "Настройки", new Vector2(0, 0), new Vector2(300, 70));
        Button exit = CreateButton(canvas.transform, "Выход", new Vector2(0, -80), new Vector2(300, 70));

        levels.onClick.AddListener(menu.OpenLevels);
        settings.onClick.AddListener(menu.OpenSettings);
        exit.onClick.AddListener(menu.ExitGame);
    }

    private static void BuildLevelSelect()
    {
        LevelSelectController select = Object.FindObjectOfType<LevelSelectController>();
        if (select == null)
        {
            select = new GameObject("LevelSelectControllerUI").AddComponent<LevelSelectController>();
        }

        Canvas canvas = CreateCanvas("LevelSelectCanvas");

        for (int i = 1; i <= 5; i++)
        {
            int level = i;
            Button button = CreateButton(canvas.transform, $"Уровень {i}", new Vector2(0, 180 - i * 70), new Vector2(320, 56));
            bool unlocked = GameProgress.IsLevelUnlocked(i);
            button.interactable = unlocked;
            ColorBlock colors = button.colors;
            colors.disabledColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            button.colors = colors;
            button.onClick.AddListener(() => select.TryLoadLevel(level));
        }
    }

    private static void BuildBattleHud()
    {
        if (Camera.main != null && Camera.main.GetComponent<ThirdPersonCameraController>() == null)
        {
            Camera.main.gameObject.AddComponent<ThirdPersonCameraController>();
        }

        SquadCommandController squad = Object.FindObjectOfType<SquadCommandController>();
        if (squad == null)
        {
            squad = new GameObject("SquadCommandController").AddComponent<SquadCommandController>();
        }

        Canvas canvas = CreateCanvas("BattleHudCanvas");

        Text title = new GameObject("Title").AddComponent<Text>();
        title.transform.SetParent(canvas.transform, false);
        title.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        title.text = "Tab: переключить героя | ПКМ: переместить союзников | T: фокус цель";
        title.alignment = TextAnchor.MiddleCenter;
        title.color = Color.white;
        title.resizeTextForBestFit = true;

        RectTransform trt = title.GetComponent<RectTransform>();
        trt.anchorMin = new Vector2(0.5f, 1f);
        trt.anchorMax = new Vector2(0.5f, 1f);
        trt.anchoredPosition = new Vector2(0, -25);
        trt.sizeDelta = new Vector2(900, 40);
    }
}
