using UnityEngine;

public class MovingPlatformLogic : MonoBehaviour
{
    [Header("Horizontal Movement")]
    public float moveDistance = 2f;
    public float moveSpeed = 2f;

    [Header("Vertical Movement")]
    public float verticalSpeed = 2f;
    public float upperBoundY = 6f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Horizontal oscillation
        float offsetX = Mathf.Sin(Time.time * moveSpeed) * moveDistance;
        float newY = transform.position.y + verticalSpeed * Time.deltaTime;

        transform.position = new Vector3(startPos.x + offsetX, newY, startPos.z);

        if (newY >= upperBoundY)
        {
            gameObject.SetActive(false);
        }
    }
}
