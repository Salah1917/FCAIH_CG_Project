using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevel : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            UnLockNewLevel();

            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                // ?? ??? ?????? ??? ????? ?????
                Debug.Log("Congratulations! You finished all levels!");
                SceneManager.LoadScene("WinScene"); // ???? ??? ???? ?? Build Settings
            }
        }
    }

    void UnLockNewLevel()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;

        // ??? ??????? ???? ???? ?? ?? ?????? ??? ???
        if (currentIndex >= PlayerPrefs.GetInt("ReachedIndex", 1))
        {
            PlayerPrefs.SetInt("ReachedIndex", currentIndex + 1);
            PlayerPrefs.SetInt("UnLockLevel", PlayerPrefs.GetInt("UnLockLevel", 1) + 1);
            PlayerPrefs.Save();
        }
    }
}
