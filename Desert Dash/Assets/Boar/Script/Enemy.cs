using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform startpos; // Right
    public Transform endpos;   // Left
    public float speed = 5f;
    private Rigidbody2D rb;
    private bool movingToEnd = true;// Start by moving left toward endpos
    private SpriteRenderer sprite;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (movingToEnd)
        {
            if (transform.position.x > endpos.position.x)
            {
                rb.linearVelocity = new Vector2(-speed, rb.linearVelocity.y);  // Move left
            }
            else
            {
                movingToEnd = false;  // Reached the left point, start moving right
                sprite.flipX = true;
            }
        }
        else
        {
            if (transform.position.x < startpos.position.x)
            {
                rb.linearVelocity = new Vector2(speed, rb.linearVelocity.y);  // Move right
            }
            else
            {
                movingToEnd = true;  // Reached the right point, start moving left
                sprite.flipX = false;
            }
        }
    }
}
