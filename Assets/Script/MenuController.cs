using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class menuController : MonoBehaviour
{
    [Header("levels to load")]
    public string _newgameLevel;
    private string levelToLoad;
    [SerializeField] private GameObject noSaveGameDialog = null;

    public void NewGameDialogYes()
    {
        SceneManager.LoadScene(_newgameLevel);
    }

    public void LoadGameDialogYes()
    {
        if (PlayerPrefs.HasKey("SavedLevel"))
        {
            levelToLoad = PlayerPrefs.GetString("SavedLevel");
            SceneManager.LoadScene(levelToLoad);
        }
        else
        {
            noSaveGameDialog.SetActive(true);
        }
    }

    public void Exitbutton()
    {
        Application.Quit();
    }
}