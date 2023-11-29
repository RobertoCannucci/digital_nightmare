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
            if (tag.Contains("USB"))
            {
                gameObject.GetComponent<USBScript>().pickedUp = true;
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
        if (gameObject.tag.Contains("Inventory"))
        {
            if (gameObject.tag.Contains("Battery"))
            {
                if (ps.BatteryInventory < 3)
                {
                    GameManager.Instance.AddBattery();
                    Destroy(gameObject);
                }
                else
                {
                    StartCoroutine(GameManager.Instance.DisplayText("Can't hold more than 3 batteries"));
                }
            }
        }
    }
}
