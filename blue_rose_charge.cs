using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class blue_rose_charge : MonoBehaviour
{
    public bool charged;
    private float ChargedTime;
    public float ChargedRequireTime = 4f;
    public Sound Charged;
    public Transform trigger;
    public ParticleSystem ParticleSystem;
    public ParticleSystem FlashDetect;
    public bool prevToggleState;
    public ItemToggle itemToggle;

    void Start()
    {
        itemToggle = GetComponent<ItemToggle>();
        if (trigger == null)
            trigger = transform.Find("Trigger");
        if (trigger == null)
            Debug.LogError("trigger 尚未設定，也無法在子物件中找到 Trigger");
    }

    void Update()
    {
        var grab = GetComponent<PhysGrabObject>();
        if (grab != null && grab.playerGrabbing != null && grab.playerGrabbing.Count >= 1 && ChargedTime < ChargedRequireTime + 1f)
        {
            ChargedTime += Time.deltaTime;
        }

        if (itemToggle != null && itemToggle.toggleState != prevToggleState)
        {
            ChargedTime = 0f;
            prevToggleState = itemToggle.toggleState;
        }

        if (ChargedTime == ChargedRequireTime)
        {
            if (Charged != null && trigger != null)
                Charged.Play(trigger.position);
        }

        if (ChargedTime >= ChargedRequireTime)
        {
            ParticleSystem?.Play(withChildren: false);
            charged = true;
        }
        else
        {
            ParticleSystem?.Stop(withChildren: false);
        }
    }
}
