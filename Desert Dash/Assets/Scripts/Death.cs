using UnityEngine;

public class Death : MonoBehaviour
{
    public Transform Startpoint;
    public Transform player;
    private Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player.transform.position = Startpoint.transform.position;
        }
        if(other.CompareTag("Trap"))
        {
            Die();
        }
    }
    private void Die()
    {
        rb.bodyType = RigidbodyType2D.Static; 

    }
}
