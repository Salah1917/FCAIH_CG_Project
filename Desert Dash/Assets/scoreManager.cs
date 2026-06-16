using UnityEngine;
using TMPro;

public class scoreManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreValue;
    void Update()
    {
        scoreValue.text = collectCoins.score.ToString();
    }
}
