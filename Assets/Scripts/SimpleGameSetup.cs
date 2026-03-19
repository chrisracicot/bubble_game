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
        CreateJumpButton(canvas.transform, "JumpButton", new Vector2(180f, 180f), new Vector2(-160f, 320f), "JUMP");
        CreateJumpForwardButton(canvas.transform, "JumpForwardButton", new Vector2(180f, 140f), new Vector2(-160f, 140f), "JUMP FW");
        
        Debug.Log("[SimpleGameSetup] UI setup complete.");
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

    private void CreateJumpForwardButton(Transform parent, string buttonName, Vector2 size, Vector2 anchoredPos, string label)
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
        image.color = new Color(0.2f, 0.4f, 1f, 0.5f); // Distinct blue

        Button button = buttonObj.AddComponent<Button>();

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        Text text = textObj.AddComponent<Text>();
        text.text = label;
        text.alignment = TextAnchor.MiddleCenter;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 30;
        text.color = Color.white;

        JumpForwardButton jumpFwd = buttonObj.AddComponent<JumpForwardButton>();
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

public class JumpButton : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        MobileInput.PressJump();
    }
}

public class JumpForwardButton : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        MobileInput.PressJumpForward();
    }
}