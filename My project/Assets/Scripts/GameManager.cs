using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;
using NavKeypad;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool GetNoteSetRandomly = true;
    public int NoteSetIdx = 0;
    private SerializableJsonNoteSet noteSet;
    public Text displayTextUI;
    public bool isGamePaused = false;
    public Canvas pauseMenu;
    public PlayerScript ps;
    public RawImage[] batteriesUI;
    private bool displayingText = false;


    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Locked;
        }
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(gameObject);
        }

        //if (SceneManager.GetActiveScene().name == "Level2")
        // if (true) // for testing purposes
        // {
        //     if (GetNoteSetRandomly)
        //     {
        //         NoteSetIdx = Random.Range(0, 3);
        //     }
        //     noteSet = JsonUtility.FromJson<SerializableJsonNoteSet>(File.ReadAllText($"Assets/Notes/noteSet{0}.json"));
        // }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().buildIndex != 0 && SceneManager.GetActiveScene().buildIndex != 5)
        {
            Instance.displayTextUI = GameObject.FindGameObjectWithTag("DisplayText").GetComponent<Text>();
            Instance.pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu").GetComponent<Canvas>();
            Instance.ps = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        }
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            noteSet = JsonUtility.FromJson<SerializableJsonNoteSet>(File.ReadAllText($"Assets/Notes/noteSet0.json"));
            Instance.ps.gameObject.transform.position = new Vector3(-0.15f, 1.37f, 3.29f);
        }
        if (SceneManager.GetActiveScene().buildIndex == 3)
        {
            noteSet = JsonUtility.FromJson<SerializableJsonNoteSet>(File.ReadAllText($"Assets/Notes/noteSet1.json"));
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
        if (SceneManager.GetActiveScene().buildIndex == 4)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

    }

    // https://forum.unity.com/threads/fading-in-out-gui-text-with-c-solved.380822/
    public IEnumerator DisplayText(string textToDisplay)
    {
        displayingText = true;
        displayTextUI.text = textToDisplay;
        displayTextUI.color = new Color(displayTextUI.color.r, displayTextUI.color.g, displayTextUI.color.b, 1);
        System.Threading.Thread.Sleep(100);
        while (displayTextUI.color.a > 0.0f)
        {
            displayTextUI.color = new Color(displayTextUI.color.r, displayTextUI.color.g, displayTextUI.color.b, displayTextUI.color.a - (Time.deltaTime / 2.5f));
            yield return null;
        }
        displayingText = false;
    }
    public void DisplayInteractHint()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        if (!displayingText)
        {
            Debug.DrawRay(ray.origin, ray.direction);
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 playerPosToObjectPos = hit.transform.position - Camera.main.transform.position;
                if (playerPosToObjectPos.magnitude <= ps.pickUpRange)
                {
                    displayTextUI.color = new Color(displayTextUI.color.r, displayTextUI.color.g, displayTextUI.color.b, 1);
                    if (hit.collider.tag.Contains("PickUp"))
                    {
                        displayTextUI.text = "Press F to Pick Up";
                    }
                    else if (hit.collider.tag.Contains("door"))
                    {
                        displayTextUI.text = "Press F to Open";
                    }
                    else if (hit.collider.gameObject.name == "panel" || hit.collider.gameObject.name.Contains("bttn"))
                    {
                        displayTextUI.text = "Press F to Click Key";
                    }
                    else
                    {
                        displayTextUI.text = "";
                    }
                }
                else
                {
                    displayTextUI.text = "";
                }
            }
            else
            {
                displayTextUI.text = "";
            }
        }
    }
    public void TogglePauseGame()
    {
        if (isGamePaused)
        {
            Time.timeScale = 1f;
            AudioListener.pause = false;
            pauseMenu.GetComponent<Canvas>().enabled = false;
            //pauseMenu.gameObject.SetActive(false);
            if (ps.hoveringObj == null)
            {
                Camera.main.GetComponent<CameraRotation>().lockedCamera = false;
            }
            if (SceneManager.GetActiveScene().buildIndex != 0 && SceneManager.GetActiveScene().buildIndex == 5)
            {
                Cursor.visible = false;
            }
        }
        else
        {
            Time.timeScale = 0f;
            AudioListener.pause = true;
            //pauseMenu.gameObject.SetActive(true);
            pauseMenu.GetComponent<Canvas>().enabled = true;
            pauseMenu.GetComponent<PauseMenuManager>().UpdatePages();
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

    public void GoToMainMenu()
    {
        Instance.TogglePauseGame();
        SceneManager.LoadScene(0);
        Destroy(ps.gameObject);
    }
}
