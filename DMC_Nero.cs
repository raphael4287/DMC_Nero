using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace DMC_Nero;

[BepInPlugin("Raphael.DMC_Nero", "DMC_Nero", "1.0")]
public class DMC_Nero : BaseUnityPlugin
{
    internal static DMC_Nero Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger => Instance._logger;
    private ManualLogSource _logger => base.Logger;
    internal Harmony? Harmony { get; set; }
    internal EX_mode_charge? mycustomscriptInstance{ get; private set; }
    internal blue_rose_charge? mycustomscriptInstance1{ get; private set; }
    internal ExplosionBullet? mycustomscriptInstance2{ get; private set; }

    private void Awake()
    {
        Instance = this;
        
        // Prevent the plugin from being deleted
        this.gameObject.transform.parent = null;
        this.gameObject.hideFlags = HideFlags.HideAndDontSave;

        mycustomscriptInstance = this.gameObject.AddComponent<EX_mode_charge>();

        mycustomscriptInstance1 = this.gameObject.AddComponent<blue_rose_charge>();
        
        mycustomscriptInstance2 = this.gameObject.AddComponent<ExplosionBullet>();
        Patch();

        Logger.LogInfo($"{Info.Metadata.GUID} v{Info.Metadata.Version} has loaded!");
    }

    internal void Patch()
    {
        Harmony ??= new Harmony(Info.Metadata.GUID);
        Harmony.PatchAll();
    }

    internal void Unpatch()
    {
        Harmony?.UnpatchSelf();
    }

    private void Update()
    {
        // Code that runs every frame goes here
    }
}