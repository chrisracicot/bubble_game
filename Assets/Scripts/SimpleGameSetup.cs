using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SimpleGameSetup : MonoBehaviour
{
    [Header("Materials")]
    public Material playerMaterial;
    public Material floorMaterial;

    private void Start()
    {
        Debug.Log("[SimpleGameSetup] Initializing UI...");
        CreateUI();
        Debug.Log("[SimpleGameSetup] UI Initialization complete.");
    }

    private void CreateUI()
    {
        if (FindFirstObjectByType<EventSystem>() == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<EventSystem>();
#if ENABLE_INPUT_SYSTEM
            var inputModuleType = System.Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
            if (inputModuleType != null)
            {
                eventSystemObj.AddComponent(inputModuleType);
            }
            else
            {
                eventSystemObj.AddComponent<StandaloneInputModule>();
            }
#else
            eventSystemObj.AddComponent<StandaloneInputModule>();
#endif
        }

        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();

        CreateControlPanel(canvas.transform);
        CreateScoreUI(canvas.transform);
        CreateGameOverUI(canvas.transform);
        
        Debug.Log("[SimpleGameSetup] UI setup complete.");
    }

    private void CreateControlPanel(Transform parent)
    {
        GameObject panelObj = new GameObject("ControlPanel");
        panelObj.transform.SetParent(parent, false);

        RectTransform rect = panelObj.AddComponent<RectTransform>();
        // Anchor to bottom 50% of screen
        rect.anchorMin = new Vector2(0f, 0f);
        rect.anchorMax = new Vector2(1f, 0.5f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.pivot = new Vector2(0.5f, 0.5f); // Center pivot for easy math

        Image image = panelObj.AddComponent<Image>();
        image.color = new Color(1f, 1f, 1f, 0.1f); // Slightly visible so player knows where to tap

        GameObject textObj = new GameObject("HelpText");
        textObj.transform.SetParent(panelObj.transform, false);
        Text text = textObj.AddComponent<Text>();
        text.text = "TAP HERE TO JUMP\nCenter = Jump Forward\nSides = Jump Sideways\nTop = Higher Jump\nBottom = Longer Jump";
        text.alignment = TextAnchor.MiddleCenter;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 40;
        text.color = new Color(1f, 1f, 1f, 0.5f);
        
        RectTransform textRect = text.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        panelObj.AddComponent<ControlPanelHandler>();
    }

    // Keep Score UI logic intact
    private void CreateScoreUI(Transform parent)
    {
        GameObject scoreObj = new GameObject("ScoreText");
        scoreObj.transform.SetParent(parent, false);

        RectTransform rect = scoreObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0f, -50f);
        rect.sizeDelta = new Vector2(400f, 100f);

        Text scoreText = scoreObj.AddComponent<Text>();
        scoreText.text = "SCORE: 0";
        scoreText.alignment = TextAnchor.MiddleCenter;
        scoreText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        scoreText.fontSize = 70;
        scoreText.color = Color.white;
        
        Shadow shadow = scoreObj.AddComponent<Shadow>();
        shadow.effectColor = new Color(0f, 0f, 0f, 0.5f);
        shadow.effectDistance = new Vector2(4f, -4f);

        ScoreManager.Instance.OnScoreChanged.AddListener((score) => {
            scoreText.text = "SCORE: " + score;
        });
    }

    // Keep Game Over UI logic intact
    private void CreateGameOverUI(Transform parent)
    {
        GameObject panelObj = new GameObject("GameOverPanel");
        panelObj.transform.SetParent(parent, false);
        panelObj.SetActive(false);

        RectTransform panelRect = panelObj.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image panelImg = panelObj.AddComponent<Image>();
        panelImg.color = new Color(0f, 0f, 0f, 0.8f);

        GameObject textObj = new GameObject("GameOverText");
        textObj.transform.SetParent(panelObj.transform, false);
        Text goText = textObj.AddComponent<Text>();
        goText.text = "GAME OVER";
        goText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        goText.fontSize = 120;
        goText.color = Color.red;
        goText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchoredPosition = new Vector2(0f, 100f);
        textRect.sizeDelta = new Vector2(800f, 200f);

        GameObject btnObj = new GameObject("RestartButton");
        btnObj.transform.SetParent(panelObj.transform, false);
        
        RectTransform btnRect = btnObj.AddComponent<RectTransform>();
        btnRect.sizeDelta = new Vector2(400f, 120f);
        btnRect.anchoredPosition = new Vector2(0f, -100f);

        Image btnImg = btnObj.AddComponent<Image>();
        btnImg.color = new Color(0.2f, 0.6f, 0.2f, 1f);
        
        Button btn = btnObj.AddComponent<Button>();
        btn.onClick.AddListener(() => {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        });

        GameObject btnTextObj = new GameObject("Text");
        btnTextObj.transform.SetParent(btnObj.transform, false);
        Text btnText = btnTextObj.AddComponent<Text>();
        btnText.text = "RETRY";
        btnText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        btnText.fontSize = 50;
        btnText.color = Color.white;
        btnText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform btnTextRect = btnTextObj.GetComponent<RectTransform>();
        btnTextRect.anchorMin = Vector2.zero;
        btnTextRect.anchorMax = Vector2.one;
        btnTextRect.sizeDelta = Vector2.zero;

        ScoreManager.Instance.OnGameOver.AddListener(() => {
            panelObj.SetActive(true);
        });
    }
}

public class ControlPanelHandler : MonoBehaviour, IPointerDownHandler
{
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector2 screenPos = eventData.position;
        
        // The panel is the bottom 50% of the screen.
        // Screen width center is Screen.width * 0.5f.
        float normalizedX = Mathf.Clamp((screenPos.x - Screen.width * 0.5f) / (Screen.width * 0.5f), -1f, 1f);
        
        // Panel center Y is Screen.height * 0.25f. Max travel from center is Screen.height * 0.25f.
        float normalizedY = Mathf.Clamp((screenPos.y - Screen.height * 0.25f) / (Screen.height * 0.25f), -1f, 1f);

        MobileInput.SetTrajectoryJump(new Vector2(normalizedX, normalizedY));
        Debug.Log($"[ControlPanel] Raw {screenPos}, Jump requested at mapping ({normalizedX:F2}, {normalizedY:F2})");
    }
}