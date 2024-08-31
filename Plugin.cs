using BepInEx;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using Utilla;
using TMPro;

namespace Clock
{
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public AssetBundle assetbundle = null;
        public GameObject clock;
        public TextMeshPro clocktime;
        public TextMeshPro clocktimepm;
        public bool Enabled = true;

        void Start() => Utilla.Events.GameInitialized += OnGameInitialized;

        void OnEnable() { Enabled = true; HarmonyPatches.ApplyHarmonyPatches(); clock.SetActive(true); }

        void OnDisable() { Enabled = false; HarmonyPatches.RemoveHarmonyPatches(); clock.SetActive(false); }

        void OnGameInitialized(object sender, EventArgs e) => Setup();

        public void Setup()
        {
            assetbundle = LoadAssetBundle("Clock.Resources.clock");

            if (assetbundle == null)
            {
                Debug.LogError("Failed to load AssetBundle.");
                return;
            }

            GameObject stupid = assetbundle.LoadAsset<GameObject>("clock");

            if (stupid == null)
            {
                Debug.LogError("Failed to load the 'clock' GameObject from the AssetBundle.");
                return;
            }

            clock = Instantiate(stupid);

            if (clock != null)
            {
                clocktime = clock.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>();
                clocktimepm = clock.transform.GetChild(1).gameObject.GetComponent<TextMeshPro>();

                clock.transform.position = new Vector3(-68.819f, 11.8992f, -82.64f);
                clock.transform.rotation = Quaternion.Euler(270, 83.2288f, 0);

                if (clocktime == null || clocktimepm == null)
                {
                    Debug.LogError("Failed to find TextMeshPro components on the clock object.");
                }
            }
            else
            {
                Debug.LogError("Failed to instantiate the clock GameObject.");
            }
        }

        public void Update()
        {
            if (Enabled)
            {
                DateTime currentTime = DateTime.Now;

                if (clocktime != null && clocktimepm != null)
                {
                    clocktime.text = currentTime.ToString("hh:mm");
                    clocktimepm.text = currentTime.ToString("tt");
                }
            }
        }

        public AssetBundle LoadAssetBundle(string path)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);

            if (stream == null)
            {
                Debug.LogError("Failed to find the resource stream at: " + path);
                return null;
            }

            AssetBundle bundle = AssetBundle.LoadFromStream(stream);
            stream.Close();
            return bundle;
        }
    }
}
