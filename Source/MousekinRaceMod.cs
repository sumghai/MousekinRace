using AlienRace;
using HarmonyLib;
using System;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    public class MousekinRaceMod : Mod
    {
        private static readonly Type patchType = typeof(HarmonyPatches);

        public MousekinRaceMod(ModContentPack content) : base(content)
        {
            var harmony = new Harmony("com.MousekinRace.patches");
            harmony.PatchAll();
        }
    }
}