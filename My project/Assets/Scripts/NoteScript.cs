using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class NoteScript : MonoBehaviour
{
    public string note;
    public bool collected = false;
    public int noteIdx = 0;
    private SerializableNote NoteFromNoteSet;
    void Start()
    {
        NoteFromNoteSet = GameManager.Instance.GetNote(noteIdx);
        note = NoteFromNoteSet.noteText;
        transform.GetChild(0).gameObject.GetComponent<TextMeshPro>().text = note;
        transform.rotation = Quaternion.Euler(NoteFromNoteSet.noteRotX, NoteFromNoteSet.noteRotY, NoteFromNoteSet.noteRotZ);
        transform.position = new Vector3(NoteFromNoteSet.notePosX, NoteFromNoteSet.notePosY, NoteFromNoteSet.notePosZ);
        
    }

    void Update()
    {
        
    }
}
