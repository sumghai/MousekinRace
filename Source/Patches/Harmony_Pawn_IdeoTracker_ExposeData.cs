using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MousekinRace
{
    // If a savegame originally created without the Ideology DLC is loaded with the DLC activated,
    // and the player faction colonists don't already have an "approved" Mousekin preset ideology,
    // default to the same ideology as the Kingdom NPC faction
    [HarmonyPatch(typeof(Pawn_IdeoTracker), nameof(Pawn_IdeoTracker.ExposeData))]
    public static class Harmony_Pawn_IdeoTracker_ExposeData_SetDefaultIdeo
    {
        static void Postfix(Pawn_IdeoTracker __instance)
        {
            // Only check when loading an existing savegame and Ideo DLC is active
            if (Scribe.mode == LoadSaveMode.PostLoadInit && ModsConfig.ideologyActive)
            {
                IEnumerable<ScenPart_RequiredFaction> scenParts = Current.Game.Scenario.AllParts.OfType<ScenPart_RequiredFaction>();

                // Only for savegames running scenarios with a preset ideo forced on the player
                if (scenParts.Any() && scenParts.FirstOrDefault(scenPart => scenPart.useFactionIdeoForPlayer == true) != null)
                {                   
                    // Only for player colonists
                    if (__instance.pawn.IsColonist)
                    {
                        // Only for those whose personal or faction ideo IDs match neither the Kingdom or Independent NPC factions
                        //
                        // (TODO - future support for faction allegiance system)
                        // (TODO - change to a startup-generated list to support more than two allegiance choices in the future)
                        if (__instance.pawn.ideo.ideo.id != Find.FactionManager.allFactions.FirstOrDefault(x => x.def == MousekinDefOf.Mousekin_FactionKingdom || x.def == MousekinDefOf.Mousekin_FactionIndyTown).ideos.primaryIdeo.id || __instance.pawn.Faction.ideos.primaryIdeo.id != Find.FactionManager.allFactions.FirstOrDefault(x => x.def == MousekinDefOf.Mousekin_FactionKingdom || x.def == MousekinDefOf.Mousekin_FactionIndyTown).ideos.primaryIdeo.id)
                        {
                            Ideo tgtIdeo = Page_IdeoFixedByScenario.GetFixedIdeo();
                            __instance.pawn.ideo.SetIdeo(tgtIdeo);
                            __instance.pawn.ideo.previousIdeos.Clear();
                            __instance.pawn.Faction.ideos.SetPrimary(tgtIdeo);
                            __instance.pawn.Faction.ideos.ideosMinor.Clear();
                            Find.IdeoManager.RemoveUnusedStartingIdeos();
                        }
                    }
                }
            }
        }
    }
}
