using AlienRace;
using HarmonyLib;
using System;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    public class MousekinRaceMod : Mod
    {
        public static Settings Settings;
        
        private static readonly Type patchType = typeof(HarmonyPatches);

        public MousekinRaceMod(ModContentPack content) : base(content)
        {
            Settings = GetSettings<Settings>();
            var harmony = new Harmony("com.MousekinRace.patches");
            harmony.PatchAll();
        }
        public override void DoSettingsWindowContents(Rect canvas)
        {
            Settings.Draw(canvas);
            base.DoSettingsWindowContents(canvas);
        }

        public override string SettingsCategory()
        {
            return "MousekinRace_SettingsCategory_Heading".Translate();
        }

    }
}