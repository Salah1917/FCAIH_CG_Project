using UnityEngine;

public class rotate : MonoBehaviour
{
    [SerializeField] private float speed = 2f; 
    private void Update()
    {
        transform.Rotate(0, 360 * speed * Time.deltaTime, 0);
    }
}
