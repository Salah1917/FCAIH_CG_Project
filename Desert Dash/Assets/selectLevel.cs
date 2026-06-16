using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class selectLevel : MonoBehaviour
{
    public Button[] buttons;
    private void Awake()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnLockLevel", 1);

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
        }

        int levelsToUnlock = Mathf.Min(unlockedLevel, buttons.Length);
        for (int i = 0; i < levelsToUnlock; i++)
        {
            buttons[i].interactable = true;
        }
    }

    public void OpenLevels(int levelId)
    {
        string LevelName = "Level"+levelId;
        SceneManager.LoadSceneAsync(LevelName);
    }
}

