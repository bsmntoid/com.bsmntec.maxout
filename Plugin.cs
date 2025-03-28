﻿using BepInEx;
using UnityEngine;
using UnityEngine.EventSystems;

// (c) bsmntoid 2025

namespace com.bsmntec.maxout
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private GTTOD_UpgradesManager UpgradesManager;
        private GameObject MiddlePlate;
        private GameObject MaxOutButton;
        private GameObject RespecButton;
        private void Awake()
        {
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
        private void Update()
        {
            ButtonUpdate();
        }
        private void ButtonUpdate()
        {
            if (MaxOutButton != null) return;
            if (GameManager.GM == null) return;
            if (GameObject.Find("MiddlePlate") == null) return;

            MiddlePlate = GameObject.Find("MiddlePlate");
            MaxOutButton = GameObject.Instantiate(MiddlePlate, MiddlePlate.transform.GetParent().transform);
            RespecButton = GameObject.Instantiate(MiddlePlate, MiddlePlate.transform.GetParent().transform);
            MaxOutButton.transform.Translate(new Vector3(0, -90, 0));
            RespecButton.transform.Translate(new Vector3(0, -180, 0));

            EventTrigger MaxOutEvent = MaxOutButton.GetComponent<EventTrigger>();
            EventTrigger.Entry maxEntry = new EventTrigger.Entry();
            EventTrigger RespecEvent = RespecButton.GetComponent<EventTrigger>();
            EventTrigger.Entry respecEntry = new EventTrigger.Entry();

            maxEntry.eventID = EventTriggerType.PointerClick;
            maxEntry.callback.AddListener((data)=>{MaxOut();});

            respecEntry.eventID = EventTriggerType.PointerClick;
            respecEntry.callback.AddListener((data) => {Respec();});

            MaxOutEvent.triggers.RemoveAt(2);
            MaxOutEvent.triggers.Add(maxEntry);
            RespecEvent.triggers.RemoveAt(2);
            RespecEvent.triggers.Add(respecEntry);

        }
        private void MaxOut()
        {
            UpgradesManager = GameObject.Find("GTTOD").GetComponent<GTTOD_UpgradesManager>();
            foreach (Aspect a in UpgradesManager.Aspects)
            {
                foreach (Potential p in a.AspectPotential)
                {
                    p.PotentialLevel = p.PotentialProgression.Count;
                    p.MaxedOut = true;
                    UpgradesManager.AdjustAspects(Save: true);
                }
            }
        }
        private void Respec()
        {
            UpgradesManager = GameObject.FindObjectOfType<GTTOD_UpgradesManager>();;
            foreach (Aspect a in UpgradesManager.Aspects)
            {
                foreach (Potential p in a.AspectPotential)
                {
                    p.PotentialLevel = 0;
                    p.MaxedOut = false;
                    UpgradesManager.AdjustAspects(Save: true);
                }
            }
            ac_CharacterController Player = GameObject.Find("Player").GetComponent<ac_CharacterController>();
            Player.DashCount = 0;
        }
    }
}
