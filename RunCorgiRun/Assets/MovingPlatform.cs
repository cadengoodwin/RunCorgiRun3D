using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public bool isVerticalMoving = false;

    public float speed = 3.0f;
    public float moveDistance = 5.0f;
    
    private Vector3 startPosition;
    private Vector3 endPosition;
    private bool movingRight = true;

    private void Start()
    {
        startPosition = transform.position;
        if (!isVerticalMoving)
        {
            endPosition = startPosition + new Vector3(moveDistance, 0, 0);
        }

        else
        {
            endPosition = startPosition + new Vector3(0,moveDistance, 0);

        }
    }

    private void Update()
    {
        if (movingRight)
        {
            MoveRight();
        }
        else
        {
            MoveLeft();
        }
    }

    private void MoveRight()
    {
        transform.position = Vector3.MoveTowards(transform.position, endPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, endPosition) < 0.1f)
        {
            movingRight = false;
        }
    }

    private void MoveLeft()
    {
        transform.position = Vector3.MoveTowards(transform.position, startPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, startPosition) < 0.1f)
        {
            movingRight = true;
        }
    }
}