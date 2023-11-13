using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    public Button previousPageBtn;
    public Button nextPageBtn;
    public Text page1Text;
    public Text page2Text;
    public GameObject player;
    private int currentPageIdx = 0;
    void Start()
    {
        previousPageBtn.GetComponent<Button>().onClick.AddListener(PreviousPage);
        nextPageBtn.GetComponent<Button>().onClick.AddListener(NextPage);
    }
    private void OnEnable()
    {
        UpdatePages();
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
        if ((currentPageIdx + 1) * 2 < player.GetComponent<PlayerScript>().collectedNotes.Count)
        {
            currentPageIdx++;
            UpdatePages();
        }
    }
    void UpdatePages()
    {
        try
        {
            page1Text.text = player.GetComponent<PlayerScript>().collectedNotes[currentPageIdx * 2];
        }
        catch
        {
            page1Text.text = "";
        }
        

        try
        {
            page2Text.text = player.GetComponent<PlayerScript>().collectedNotes[currentPageIdx * 2 + 1];
        }
        catch {
            page2Text.text = "";
        }
        
    }
}
