using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using TMPro;
using UnityEngine;

public class NoteScript : MonoBehaviour
{
    public string note;
    public bool collected = false;
    public int noteIdx = 0;
    public int noteSetIdk = 0;
    SerializableJsonNoteSet[] noteSets;
    void Start()
    {
        SerializableJsonNoteSet noteSet = JsonUtility.FromJson<SerializableJsonNoteSet>(File.ReadAllText("Assets/Notes/noteSet0.json"));
        Debug.Log(noteSet.set[0].noteText);
        transform.GetChild(0).gameObject.GetComponent<TextMeshPro>().text = note;
        
    }

    void Update()
    {
        
    }
}
