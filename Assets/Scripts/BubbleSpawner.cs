using UnityEngine;

public class BubbleSpawner : MonoBehaviour
{
    public GameObject bubblePrefab;
    public float spawnInterval = 2f;
    public float minHeight = 1f;
    public float maxHeight = 5f;
    public float spawnDistanceAhead = 50f; // Fixed distance from platform center

    private float timer;
    private Transform platform;

    private void Start()
    {
        timer = spawnInterval;
        FindPlatform();
    }

    private void FindPlatform()
    {
        // Try to find the platform/floor
        GameObject floor = GameObject.Find("Floor");
        if (floor == null) floor = GameObject.Find("Platform");
        
        if (floor != null)
        {
            platform = floor.transform;
            Debug.Log("[BubbleSpawner] Found platform: " + floor.name);
        }
    }

    private void Update()
    {
        if (platform == null) 
        {
            FindPlatform();
            return;
        }

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            SpawnBubble();
            timer = spawnInterval;
        }
    }

    private void SpawnBubble()
    {
        float randomX = Random.Range(-4f, 4f); // Spread across lane
        float randomHeight = Random.Range(minHeight, maxHeight);
        
        // Spawn relative to the platform's Z position
        Vector3 spawnPos = new Vector3(randomX, randomHeight, platform.position.z + spawnDistanceAhead);
        
        GameObject bubbleObj;

        if (bubblePrefab != null)
        {
            bubbleObj = Instantiate(bubblePrefab, spawnPos, Quaternion.identity);
            bubbleObj.name = "MovingBubble (Prefab)";
        }
        else
        {
            bubbleObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bubbleObj.name = "MovingBubble (Primitive)";
            bubbleObj.transform.position = spawnPos;
            bubbleObj.transform.localScale = new Vector3(1.8f, 1.8f, 1.8f);
            
            // Setup Trigger for primitive
            Collider col = bubbleObj.GetComponent<Collider>();
            if (col != null) col.isTrigger = true;

            // Setup material for primitive
            Renderer rend = bubbleObj.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material.color = new Color(0f, 1f, 1f, 0.8f); // High-visibility Cyan
            }
        }
        
        // Ensure the Bubble script is attached
        if (bubbleObj.GetComponent<Bubble>() == null)
        {
            bubbleObj.AddComponent<Bubble>();
        }

        Debug.Log($"[Spawner] Spawned bubble relative to platform at {spawnPos}");
    }
}
