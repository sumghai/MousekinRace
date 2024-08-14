using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    // Alerts the player that they need to build a church if their current map population has 5 or more non-Apostate Mousekins
    // (Checked for each player-owned colony map)
    public class Alert_ChurchNeeded : Alert
    {
        public const int
            MinNumWorshippers = 5,
            MinNumPews = 3;

        private TaggedString requirementsList;
        
        public Alert_ChurchNeeded() 
        {
            defaultLabel = "AlertNeedChurchLabel".Translate();
            defaultExplanation = "AlertNeedChurchDesc".Translate(MousekinDefOf.Mousekin_ChurchAltar.LabelCap);
            defaultPriority = AlertPriority.Medium;
        }

        public override TaggedString GetExplanation()
        {
            return base.GetExplanation() + "\n\n" + requirementsList;
        }

        public override AlertReport GetReport()
        {
            List<Map> maps = Find.Maps;
            for (int i = 0; i < maps.Count; i++)
            {
                Map map = maps[i];
                // Skip non-player colony maps
                if (!map.IsPlayerHome)
                {
                    continue;
                }

                // Determine if there are enough (i.e. 5 or more) non-Apostate Mousekin player colonists on a given map
                // - Apostates have trait degree = 1
                // - Mousekins with no religious affinity are also considered worshippers
                int mousekinWorshippers = map.mapPawns.PawnsInFaction(Faction.OfPlayer).Where(p => p.IsMousekin() && p.story.traits.DegreeOfTrait(MousekinDefOf.Mousekin_TraitSpectrum_Faith) != 1).Count();

                // Only check for a Church if we have enough viable worshippers
                if (mousekinWorshippers > MinNumWorshippers)
                {
                    AlertReport outputReport = false;
                    
                    // Get the first (usually only) church altar on the map
                    Building altarOnMap = map.listerBuildings.AllBuildingsColonistOfDef(MousekinDefOf.Mousekin_ChurchAltar).FirstOrDefault();

                    bool lecternFound = false;
                    int pewsFound = 0;

                    // If an altar exists, and is enclosed in a room (and is thus a Church)
                    if (altarOnMap != null && RoomRoleWorker_Church.IsValidRoom(altarOnMap.GetRoom()))
                    {
                        // Check if church furniture requirements have been met
                        List<Thing> containedAndAdjacentThings = altarOnMap.GetRoom().ContainedAndAdjacentThings;
                        for (int j = 0; j < containedAndAdjacentThings.Count; j++)
                        {
                            // Look for any lecterns
                            if (containedAndAdjacentThings[j].def == MousekinDefOf.Mousekin_ChurchLectern)
                            {
                                lecternFound = true;
                            }
                            // Count number of pews
                            if (containedAndAdjacentThings[j].def == MousekinDefOf.Mousekin_ChurchPew)
                            {
                                pewsFound++;
                            }
                            // Stop checking if requirements are already met
                            if (lecternFound && (pewsFound >= MinNumPews))
                            {
                                break;
                            }
                        }

                        if (lecternFound && (pewsFound >= MinNumPews))
                        {
                            outputReport = false;
                        }
                        else
                        {
                            outputReport = AlertReport.CulpritIs(altarOnMap);
                        }
                    }
                    // No altar, or the altar was in a room that is now exposed to the outside
                    else
                    {
                        outputReport = true;
                    }

                    requirementsList = "- " + MousekinDefOf.Mousekin_ChurchAltar.label.Colorize((altarOnMap != null) ? Color.green : ColoredText.NameColor) + "\n"
                            + "- " + MousekinDefOf.Mousekin_ChurchLectern.label.Colorize(lecternFound ? Color.green : ColoredText.NameColor) + "\n"
                            + "- " + (MousekinDefOf.Mousekin_ChurchPew.label + " x3 (" + "MousekinRace_Minimum".Translate() + ")").Colorize((pewsFound >= MinNumPews) ? Color.green : ColoredText.NameColor);
                    return outputReport;
                }
            }
            return false;
        }
    }
}
