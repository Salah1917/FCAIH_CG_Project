using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class FinshPoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.CompareTag("Player"))
        {     

            UnLockNewLevel();
            SceneController.instance.NextLevel();

        }
    }
    void UnLockNewLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex >= PlayerPrefs.GetInt("ReachedIndex"))
        {
            PlayerPrefs.SetInt("RrachedIndex", SceneManager.GetActiveScene().buildIndex + 1);
            PlayerPrefs.SetInt("UnLOCKLevel1", PlayerPrefs.GetInt("UnLockedLevel", 1) + 1);
            PlayerPrefs.Save();
        }
    }
}