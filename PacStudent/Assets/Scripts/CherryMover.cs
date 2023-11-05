using System;
using UnityEngine;

public class CherryMover : MonoBehaviour
{
    public event Action OnDestroyed;
    public Vector2 targetPosition;
    public float speed = 1.0f;

    private void Start()
    {
        targetPosition = GetOppositePosition(transform.position);
    }

    private void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if ((Vector2)transform.position == targetPosition)
        {
            OnDestroyed?.Invoke(); // Notify any subscribers that the cherry is destroyed.
            Destroy(gameObject);
        }
    }

    private Vector2 GetOppositePosition(Vector2 originalPosition)
    {
        Vector2 opposite = originalPosition * -1;
        return opposite;
    }
}
