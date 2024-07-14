using AlienRace;
using HarmonyLib;
using System;
using System.Collections.Generic;
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
            var harmony = new Harmony("com.MousekinRace");
            harmony.PatchAll();

            if (ModCompatibility.LwmDeepStorageIsActive)
            {
                Log.Message("MousekinRace :: LWM's Deep Storage detected!");

                var LwmDeepStorage_PatchPostfix = AccessTools.Method("LWM.DeepStorage.Open_DS_Tab_On_Select:Postfix");
                var MousekinRace_LwmDeepStorage_PatchPrefix = AccessTools.Method(typeof(Harmony_LWM_DeepStorage_Open_DS_Tab_On_Select_IgnoreRootCellars), nameof(Harmony_LWM_DeepStorage_Open_DS_Tab_On_Select_IgnoreRootCellars.Prefix));
                harmony.Patch(LwmDeepStorage_PatchPostfix, prefix: new HarmonyMethod(MousekinRace_LwmDeepStorage_PatchPrefix));
            }

            if (ModCompatibility.VanillaCookingExpandedIsActive)
            {
                Log.Message("MousekinRace :: Vanilla Cooking Expanded detected!");
            }

            if (ModCompatibility.MedievalOverhaulIsActive)
            {
                Log.Message("MousekinRace :: Medieval Overhaul detected!");
            }

            List<BackCompatibilityConverter> compatibilityConverters =
                AccessTools.StaticFieldRefAccess<List<BackCompatibilityConverter>>(typeof(BackCompatibility), "conversionChain");

            compatibilityConverters.Add(new BackCompatibilityConverter_Mousekin());
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