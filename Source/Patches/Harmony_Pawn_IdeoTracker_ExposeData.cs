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
                    // Only for player Mousekin colonists
                    if (__instance.pawn.IsColonist && __instance.pawn.IsMousekin())
                    {
                        GameComponent_Allegiance gameComp_Allegiance_Instance = GameComponent_Allegiance.Instance;
                        Faction alignedFaction = gameComp_Allegiance_Instance.alignedFaction;
                        List<int> availableAllegianceFactionIdeoIDs = gameComp_Allegiance_Instance.alliableFactions.ConvertAll(f => f.ideos.primaryIdeo.id);

                        // If the player has already pledged allegiance to a faction, use that faction's ideo
                        // Otherwise, default to Mousekin Kingdom ideo
                        Ideo tgtIdeo = (alignedFaction != null) ? alignedFaction.ideos.primaryIdeo : Page_IdeoFixedByScenario.GetFixedIdeo();

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
