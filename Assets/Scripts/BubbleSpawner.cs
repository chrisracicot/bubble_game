using UnityEngine;

public class BubbleSpawner : MonoBehaviour
{
    public GameObject bubblePrefab;
    public float spawnInterval = 2f;
    public float minHeight = 1f;
    public float maxHeight = 5f;
    public float spawnDistanceAhead = 15f;

    private float timer;
    private Transform player;

    private void Start()
    {
        timer = spawnInterval;
        FindPlayer();
    }

    private void FindPlayer()
    {
        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    private void Update()
    {
        if (player == null) 
        {
            FindPlayer();
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
        Vector3 spawnPos = new Vector3(randomX, randomHeight, player.position.z + 20f); // Spawn far ahead
        
        GameObject bubbleObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bubbleObj.name = "MovingBubble";
        bubbleObj.transform.position = spawnPos;
        bubbleObj.transform.localScale = new Vector3(1.8f, 1.8f, 1.8f);
        
        bubbleObj.AddComponent<Bubble>();
        
        // Setup Trigger
        Collider col = bubbleObj.GetComponent<Collider>();
        if (col != null) col.isTrigger = true;

        // Setup material - direct color modification of the default material
        Renderer rend = bubbleObj.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = new Color(0f, 1f, 1f, 0.8f); // High-visibility Cyan
        }

        Debug.Log($"[Spawner] Spawned bubble at {spawnPos}");
    }
}
