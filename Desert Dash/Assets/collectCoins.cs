using UnityEngine;

public class collectCoins : MonoBehaviour
{
   public static int score = 0;

    private void OnTriggerEnter2D(Collider2D coll)
    {
        Debug.Log("Triggered with: " + coll.name);

        if (coll.gameObject.CompareTag("coin"))
        {
            Destroy(coll.gameObject);
            score++;
            Debug.Log("Coin collected! Total: " + score);
        }
    }
}
