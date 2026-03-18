using UnityEngine;

public class Bubble : MonoBehaviour
{
    public float moveSpeed = 5f; // Normal game speed
    private Transform player;

    private void Start()
    {
        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null) player = playerObj.transform;
        Debug.Log("Bubble instance started at " + transform.position);
    }

    private void Update()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.Find("Player");
            if (playerObj != null) player = playerObj.transform;
            return;
        }

        // Move towards the "back" (from front towards camera/player)
        // Force World Space movement to bypass any local rotation issues
        transform.Translate(Vector3.back * moveSpeed * Time.deltaTime, Space.World);

        // Disappear if behind the player on the Z axis
        if (player != null && transform.position.z < player.position.z - 5f)
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
