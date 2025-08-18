using UnityEngine;

public class MovingPlatformManager : MonoBehaviour
{
    //* Attach this script to moving platforms.
    //* Make sure that movable objects have a Rigidbody.

    int horizontal, moveDistance = 8, moveSpeed = 6;
    float maxRightMovePosition, maxLeftMovePosition;
    Transform platformTransform;
    Rigidbody2D platformRigidbody;

    void Start()
    {
        platformTransform = transform;
        platformRigidbody = GetComponent<Rigidbody2D>();
        platformRigidbody.bodyType = RigidbodyType2D.Kinematic;
        platformRigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        platformRigidbody.sleepMode = RigidbodySleepMode2D.NeverSleep;
        platformRigidbody.interpolation = RigidbodyInterpolation2D.Interpolate;
        maxRightMovePosition = platformTransform.position.x + moveDistance;
        maxLeftMovePosition = platformTransform.position.x - moveDistance;
    }

    void FixedUpdate()
    {
        // Moving right
        if (horizontal == -1 && platformTransform.position.x <= maxLeftMovePosition)
        {
            platformRigidbody.velocity = 0 * Vector2.right;
            platformTransform.position = new Vector2(maxLeftMovePosition, platformTransform.position.y);
            horizontal = 1;
        }
        // Moving left
        else if (horizontal == 1 && platformTransform.position.x >= maxRightMovePosition)
        {
            platformRigidbody.velocity = 0 * Vector2.right;
            platformTransform.position = new Vector2(maxRightMovePosition, platformTransform.position.y);
            horizontal = -1;
        }
        // Start
        else if (horizontal == 0 && platformTransform.position.x == (maxRightMovePosition + maxLeftMovePosition) / 2)
        {
            horizontal = 1;
        }

        platformRigidbody.velocity = horizontal * moveSpeed * Vector2.right;
    }
}
