using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_HomeScreen : MonoBehaviour
{
    [SerializeField] GameObject MainMenuPanel;
    [SerializeField] GameObject DescriptionPanel;
    [SerializeField] GameObject CreditsPanel;

    private void Start()
    {
        Reset_MainMenu();
    }
    public void Reset_MainMenu()
    {
        MainMenuPanel.SetActive(true);
        DescriptionPanel.SetActive(false);
        CreditsPanel.SetActive(false);
    }

    public void OnClick_Load()
    {
        SceneManager.LoadScene("Visualization");
    }

    public void OnClick_Description()
    {
        MainMenuPanel.SetActive(false);
        DescriptionPanel.SetActive(true);
        CreditsPanel.SetActive(false);
    }

    public void OnClick_Credits()
    {
        MainMenuPanel.SetActive(false);
        DescriptionPanel.SetActive(false);
        CreditsPanel.SetActive(true);
    }

    public void OnClick_Exit()
    {
        Application.Quit();
    }
}
