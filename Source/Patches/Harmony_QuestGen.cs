using HarmonyLib;
using RimWorld;
using RimWorld.QuestGen;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MousekinRace
{
    // Skip generating quests from the Shattered Empire if the player has pledged allegiance to a Mousekin faction
    [HarmonyPatch(typeof(QuestGen), nameof(QuestGen.Generate))]
    public static class Harmony_QuestGen_Generate_SkipEmpireIfAllegianceSet
    {
        static int iterationNumber = 0;
        
        public static void Postfix(QuestScriptDef root, Slate initialVars, ref Quest __result)
        {
            if (GameComponent_Allegiance.Instance.alignedFaction != null) 
            {                
                if (RealInvolvedFactions(__result).Any(f => f == Faction.OfEmpire))
                {
                    Quest newResult = null;

                    while (newResult == null || RealInvolvedFactions(newResult).Any(f => f == Faction.OfEmpire))
                    {
                        newResult = QuestGen.Generate(root, initialVars);
                        iterationNumber++;
                    }

                    __result = newResult;
                }
                if (iterationNumber > 0)
                {
                    iterationNumber = 0;
                }
            }
        }

        private static List<Faction> RealInvolvedFactions(Quest quest)
        {
            List<Faction> factions = new List<Faction>();

            if (quest != null && quest.PartsListForReading != null)
            {
                foreach (QuestPart part in quest.PartsListForReading)
                {
                    if (part != null && part.InvolvedFactions != null)
                    {
                        foreach (Faction faction in part.InvolvedFactions)
                        {
                            if (!factions.Contains(faction))
                            {
                                factions.Add(faction);
                            }
                        }
                    }
                    if (part is QuestPart_SpawnWorldObject qpswo && qpswo != null && qpswo.worldObject?.Faction != null)
                    {
                        if (!factions.Contains(qpswo.worldObject.Faction))
                        {
                            factions.Add(qpswo.worldObject.Faction);
                        }
                    }
                }
            }
            return factions;
        }
    }
}
