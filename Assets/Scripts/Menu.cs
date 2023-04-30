using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Menu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject rulesMenu;
    public GameObject startMenu;
    public TMP_Dropdown minesDropdown;

    public void StartGameVSAI()
    {
        PlayerPrefs.SetInt("minecount", minesDropdown.value);
        SceneManager.LoadScene(1); //load scene 1 (the board)
    }

    public void RulesMenu()
    {
        mainMenu.SetActive(false);
        rulesMenu.SetActive(true);
    }

    public void StartMenu()
    {
        mainMenu.SetActive(false);
        startMenu.SetActive(true);
    }

    public void GoBack()
    {
        rulesMenu.SetActive(false);
        startMenu.SetActive(false);
        mainMenu.SetActive(true);
    }   

    public void QuitGame()
    {
        Application.Quit();
    }

}
