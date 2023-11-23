using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool GetNoteSetRandomly = true;
    public int NoteSetIdx = 0;
    private SerializableJsonNoteSet noteSet;


    private void Awake()
    {
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
}
