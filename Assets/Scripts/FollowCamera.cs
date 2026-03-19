using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 6f, -8f);
    public float smoothSpeed = 8f;

    private void LateUpdate()
    {
        if (target == null)
        {
            GameObject playerObj = GameObject.Find("Player");
            if (playerObj != null)
            {
                target = playerObj.transform;
                Debug.Log("[FollowCamera] Auto-assigned target to GameObject named 'Player'.");
            }
            else
            {
                return;
            }
        }

        Vector3 desiredPosition = target.position + offset;
        
        // If the camera is accidentally inside the player (bug), snap back to offset
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            transform.position = desiredPosition;
        }

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        Vector3 lookTarget = target.position + new Vector3(0f, 1f, 0f);
        transform.LookAt(lookTarget);
    }
}