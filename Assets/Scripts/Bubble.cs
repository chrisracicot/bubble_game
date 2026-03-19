using UnityEngine;

public class Bubble : MonoBehaviour
{
    public float moveSpeed = 5f; // Normal game speed
    private Transform platform;

    private void Start()
    {
        FindPlatform();
        Debug.Log("Bubble instance started at " + transform.position);
    }

    private void FindPlatform()
    {
        GameObject floor = GameObject.Find("Floor");
        if (floor == null) floor = GameObject.Find("Platform");
        if (floor != null) platform = floor.transform;
    }

    private void Update()
    {
        if (platform == null)
        {
            FindPlatform();
            return;
        }

        // Move towards the "back" (from front towards camera/player)
        // Force World Space movement to bypass any local rotation issues
        transform.Translate(Vector3.back * moveSpeed * Time.deltaTime, Space.World);

        // Disappear if behind the platform center on the Z axis (adjustable based on platform length)
        if (platform != null && transform.position.z < platform.position.z - 30f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering object is the player
        if (other.CompareTag("Player") || other.name == "Player")
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                // Snap the player into the bubble
                player.EnterBubble(transform);
            }
        }
    }
}
