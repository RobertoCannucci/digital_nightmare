using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItemScript : MonoBehaviour
{
    PlayerScript ps;
    void Start()
    {
        ps = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
    }

    void Update()
    {

    }

    public void PickUpObject()
    {
        if (tag.Contains("RightHand"))
        {
            ps.rightHandInventory.Add(gameObject);
            ps.CycleRightHand();

            if (tag.Contains("FlashLight"))
            {
                ps.ToggleFlashLight();
            }
        }
        if (tag.Contains("LeftHand"))
        {
            ps.leftHandObj = gameObject;
            ps.AttachItemToHand(ps.leftHandObj, ps.leftHandIK, ps.leftHandObjPos, ps.leftHandObjRot);
        }
        if (gameObject.tag.Contains("Hovering"))
        {
            GetComponent<HoveringItemScript>().PlayerInput();

            if (gameObject.tag.Contains("Note"))
            {
                NoteScript ns = gameObject.GetComponent<NoteScript>();
                Debug.Log(ns);
                if (!ns.collected)
                {
                    ps.collectedNotes.Add(ns.note);
                    ns.collected = true;
                }
            }
        }
    }
}
