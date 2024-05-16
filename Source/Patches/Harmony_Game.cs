using HarmonyLib;
using System.Linq;
using Verse;

namespace MousekinRace
{
    // If we are loading an existing savegame that was not originally created with the Biotech DLC,
    // make sure all Mousekin world pawns have the Mousekin xenotype
    [HarmonyPatch(typeof(Game), nameof(Game.FinalizeInit))]
    public static class Harmony_Game_FinalizeInit_BiotechFixIncorrectXenotypeForMousekins
    {
        static void Postfix()
        { 
            if (ModsConfig.BiotechActive) 
            {
                foreach (Pawn pawn in Find.WorldPawns.AllPawnsAliveOrDead.Concat(Find.Maps.SelectMany(m => m.mapPawns.AllPawns)).Where(p => p != null && Utils.IsMousekin(p)).ToArray())
                {
                    if (pawn.genes.xenotype != MousekinDefOf.Mousekin_XenotypeMousekin)
                    {
                        pawn.genes.SetXenotype(MousekinDefOf.Mousekin_XenotypeMousekin);
                    }
                }
            }
        }
    }
}
