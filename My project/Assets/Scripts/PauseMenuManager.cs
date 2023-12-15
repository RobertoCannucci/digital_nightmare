using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    public Button previousPageBtn;
    public Button nextPageBtn;
    public Button resumeBtn;
    public Button controlsBtn;
    public Button notesBtn;
    public Button quitBtn;
    public TextMeshProUGUI page1Text;
    public TextMeshProUGUI page2Text;
    public GameObject controlsPage;
    public GameObject notesPage;
    public int currentPageIdx = 0;
    void Start()
    {
        previousPageBtn.GetComponent<Button>().onClick.AddListener(PreviousPage);
        nextPageBtn.GetComponent<Button>().onClick.AddListener(NextPage);
        resumeBtn.GetComponent<Button>().onClick.AddListener(Resume);
        controlsBtn.GetComponent<Button>().onClick.AddListener(Controls);
        notesBtn.GetComponent<Button>().onClick.AddListener(Notes);
        quitBtn.GetComponent<Button>().onClick.AddListener(Quit);
    }
    public void PreviousPage()
    {
        if (currentPageIdx != 0)
        {
            currentPageIdx--;
            UpdatePages();
        }
    }
    public void NextPage()
    {
        if ((currentPageIdx + 1) * 2 < GameManager.Instance.ps.collectedNotes.Count)
        {
            currentPageIdx++;
            UpdatePages();
        }
    }
    public void UpdatePages()
    {
        try
        {
            page1Text.text = GameManager.Instance.ps.collectedNotes[currentPageIdx * 2];
        }
        catch
        {
            page1Text.text = "";
        }
        

        try
        {
            page2Text.text = GameManager.Instance.ps.collectedNotes[currentPageIdx * 2 + 1];
        }
        catch {
            page2Text.text = "";
        }
        
    }

    void Resume()
    {
        GameManager.Instance.TogglePauseGame();
    }

    void Controls()
    {
        controlsPage.SetActive(true);
        controlsBtn.gameObject.SetActive(false);

        notesPage.SetActive(false);
        notesBtn.gameObject.SetActive(true);
    }

    void Notes()
    {
        controlsPage.SetActive(false);
        controlsBtn.gameObject.SetActive(true);

        notesPage.SetActive(true);
        notesBtn.gameObject.SetActive(false);
    }

    void Quit()
    {
        GameManager.Instance.GoToMainMenu();
    }
}
