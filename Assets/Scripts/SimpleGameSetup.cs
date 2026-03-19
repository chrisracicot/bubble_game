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

        CreateMoveButton(canvas.transform, "LeftButton", new Vector2(140f, 140f), new Vector2(120f, 140f), "<", -1f);
        CreateMoveButton(canvas.transform, "RightButton", new Vector2(140f, 140f), new Vector2(300f, 140f), ">", 1f);
        CreateJumpButton(canvas.transform, "JumpButton", new Vector2(180f, 180f), new Vector2(-160f, 160f), "JUMP");
        
        Debug.Log("[SimpleGameSetup] UI setup complete.");
    }

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
        
        // Add a shadow effect
        Shadow shadow = scoreObj.AddComponent<Shadow>();
        shadow.effectColor = new Color(0f, 0f, 0f, 0.5f);
        shadow.effectDistance = new Vector2(4f, -4f);

        // Simple listener to update score
        ScoreManager.Instance.OnScoreChanged.AddListener((score) => {
            scoreText.text = "SCORE: " + score;
        });
    }

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

        // Game Over Text
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

        // Restart Button
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

        // Show panel on Game Over
        ScoreManager.Instance.OnGameOver.AddListener(() => {
            panelObj.SetActive(true);
        });
    }

    private void CreateMoveButton(Transform parent, string buttonName, Vector2 size, Vector2 anchoredPos, string label, float direction)
    {
        GameObject buttonObj = new GameObject(buttonName);
        buttonObj.transform.SetParent(parent, false);

        RectTransform rect = buttonObj.AddComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchorMin = new Vector2(0f, 0f);
        rect.anchorMax = new Vector2(0f, 0f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPos;

        Image image = buttonObj.AddComponent<Image>();
        image.color = new Color(1f, 1f, 1f, 0.35f);

        Button button = buttonObj.AddComponent<Button>();

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        Text text = textObj.AddComponent<Text>();
        text.text = label;
        text.alignment = TextAnchor.MiddleCenter;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 60;
        text.color = Color.black;

        HoldButton holdButton = buttonObj.AddComponent<HoldButton>();
        holdButton.direction = direction;
    }

    private void CreateJumpButton(Transform parent, string buttonName, Vector2 size, Vector2 anchoredPos, string label)
    {
        GameObject buttonObj = new GameObject(buttonName);
        buttonObj.transform.SetParent(parent, false);

        RectTransform rect = buttonObj.AddComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchorMin = new Vector2(1f, 0f);
        rect.anchorMax = new Vector2(1f, 0f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPos;

        Image image = buttonObj.AddComponent<Image>();
        image.color = new Color(1f, 1f, 1f, 0.35f);

        Button button = buttonObj.AddComponent<Button>();

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        Text text = textObj.AddComponent<Text>();
        text.text = label;
        text.alignment = TextAnchor.MiddleCenter;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 40;
        text.color = Color.black;

        JumpButton jumpButton = buttonObj.AddComponent<JumpButton>();
    }
}

public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public float direction;

    public void OnPointerDown(PointerEventData eventData)
    {
        MobileInput.SetHorizontal(direction);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        MobileInput.StopHorizontal();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MobileInput.StopHorizontal();
    }
}

public class JumpButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        MobileInput.PressJump();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // No action needed on up for current simple logic, 
        // but we could use it to reset the jump state if we wanted to prevent holding jump.
    }
}