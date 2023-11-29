using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool GetNoteSetRandomly = true;
    public int NoteSetIdx = 0;
    private SerializableJsonNoteSet noteSet;
    public Text displayTextUI;
    public bool isGamePaused = false;
    public Canvas pauseMenu;
    private PlayerScript ps;
    public RawImage[] batteriesUI;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
            ps = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        }
        else
        {
            Destroy(gameObject);
        }

        //if (SceneManager.GetActiveScene().name == "Level2")
        if (true) // for testing purposes
        {
            if (GetNoteSetRandomly)
            {
                NoteSetIdx = Random.Range(0, 3);
                noteSet = JsonUtility.FromJson<SerializableJsonNoteSet>(File.ReadAllText($"Assets/Notes/noteSet{NoteSetIdx}.json"));
            }
        }

    }

    public SerializableNote GetNote(int noteIdx)
    {
        return noteSet.set[noteIdx];
    }
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // https://forum.unity.com/threads/fading-in-out-gui-text-with-c-solved.380822/
    public IEnumerator DisplayText(string textToDisplay)
    {
        displayTextUI.text = textToDisplay;
        displayTextUI.color = new Color(displayTextUI.color.r, displayTextUI.color.g, displayTextUI.color.b, 1);
        System.Threading.Thread.Sleep(200);
        while (displayTextUI.color.a > 0.0f)
        {
            displayTextUI.color = new Color(displayTextUI.color.r, displayTextUI.color.g, displayTextUI.color.b, displayTextUI.color.a - (Time.deltaTime / 2.5f));
            yield return null;
        }
    }
    public void TogglePauseGame()
    {
        if (isGamePaused)
        {
            Time.timeScale = 1f;
            AudioListener.pause = false;
            pauseMenu.gameObject.SetActive(false);
            if (ps.hoveringObj == null)
            {
                Camera.main.GetComponent<CameraRotation>().lockedCamera = false;
            }
            Cursor.visible = false;
        }
        else
        {
            Time.timeScale = 0f;
            AudioListener.pause = true;
            pauseMenu.gameObject.SetActive(true);
            Camera.main.GetComponent<CameraRotation>().lockedCamera = true;
            Cursor.visible = true;
        }
        isGamePaused = !isGamePaused;
    }
    public void AddBattery()
    {
        ps.BatteryInventory++;
        UpdateBatteryHUD();
    }
    public void RemoveBattery()
    {
        ps.BatteryInventory--;
        UpdateBatteryHUD();
    }
    private void UpdateBatteryHUD()
    {
        for (int i = 0; i < 3; i++)
        {
            if (i < ps.BatteryInventory)
            {
                batteriesUI[i].gameObject.SetActive(true);
            }
            else
            {
                batteriesUI[i].gameObject.SetActive(false);
            }

        }
    }
}
