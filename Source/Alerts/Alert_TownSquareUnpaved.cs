using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MousekinRace
{
    // Alerts the player that they need to fully pave any town squares from a list of allowed floor defs
    public class Alert_TownSquareUnpaved : Alert
    {
        public List<Thing> allTownSquares = new List<Thing>();

        public List<Thing> unpavedTownSquares = new List<Thing>();

        private string floorList
        {
            get
            {
                return "- " + string.Join("\n- ", MousekinDefOf.Mousekin_TownSquare.GetCompProperties<CompProperties_TownSquare>().acceptablePavedTerrainDefs.Select(d => d.LabelCap).OrderBy(d => d.RawText).ToList());
            }
        }
        
        public Alert_TownSquareUnpaved()
        {
            defaultLabel = "AlertUnpavedTownSquareSingularLabel".Translate();
            defaultExplanation = "AlertUnpavedTownSquareSingularDesc".Translate(MousekinDefOf.Mousekin_TownSquare.LabelCap, floorList);
            defaultPriority = AlertPriority.Medium;
        }

        public override AlertReport GetReport()
        {
            // Clear existing lists
            allTownSquares.Clear();
            unpavedTownSquares.Clear();

            foreach (Map map in Find.Maps)
            {
                // Only for town squares on player-owned settlements
                if (map.IsPlayerHome && map.listerThings.ThingsOfDef(MousekinDefOf.Mousekin_TownSquare).FirstOrDefault() is Thing townSquare)
                {
                    // Add current town square to list
                    allTownSquares.Add(townSquare);

                    // If current town square is not fully paved, add it to another list
                    if (CompTownSquare.HasUnpavedCellsWithinSquare(townSquare.positionInt, map, townSquare.def.GetCompProperties<CompProperties_TownSquare>()))
                    { 
                        unpavedTownSquares.Add(townSquare);
                    }
                }
            }

            if (unpavedTownSquares.Count > 0) 
            {
                // If the player only has one town square, which is also unpaved
                if (allTownSquares.Count == 1)
                {
                    // Use the (default) singular alert label and desc
                }
                // If the player has more than one town square
                else if (allTownSquares.Count > 1)
                {
                    string quantifier = "";
                    // If only one of multiple town squares is unpaved
                    if (unpavedTownSquares.Count == 1)
                    {
                        // Use the (default) singular alert label and plural desc with "One"
                        quantifier = "Quantifier_One".Translate().CapitalizeFirst();
                    }
                    // If all town squares are unpaved
                    else if (unpavedTownSquares.Count == allTownSquares.Count)
                    {
                        // Use the plural alert label and desc with "all"
                        defaultLabel = "AlertUnpavedTownSquarePluralLabel".Translate();
                        quantifier = "Quantifier_All".Translate().CapitalizeFirst();
                    }
                    // Default: only some town squares are unpaved
                    else
                    {
                        // Use the plural alert label and desc with "some"
                        defaultLabel = "AlertUnpavedTownSquarePluralLabel".Translate();
                        quantifier = "Quantifier_Some".Translate().CapitalizeFirst();
                    }
                    defaultExplanation = "AlertUnpavedTownSquarePluralDesc".Translate(quantifier, MousekinDefOf.Mousekin_TownSquare.LabelCap, floorList);
                }
                // Enable the alert
                return AlertReport.CulpritsAre(unpavedTownSquares);
            }

            return false;
        }
    }
}
