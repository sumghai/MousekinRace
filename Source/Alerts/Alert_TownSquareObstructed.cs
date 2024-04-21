using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MousekinRace
{
    // Alerts the player that they need to keep any town squares clear of all obstructions within their bounded areas
    public class Alert_TownSquareObstructed : Alert
    {
        public List<Thing> allTownSquares = new List<Thing>();

        public List<Thing> obstructedTownSquares = new List<Thing>();

        public Alert_TownSquareObstructed() 
        {
            defaultLabel = "AlertObstructedTownSquareSingularLabel".Translate();
            defaultExplanation = "AlertObstructedTownSquareSingularDesc".Translate(MousekinDefOf.Mousekin_TownSquare.LabelCap, "Possess_Singular".Translate()) + "AlertObstructedTownSquareDescEnding".Translate();
            defaultPriority = AlertPriority.Critical;
        }
        
        public override AlertReport GetReport()
        {
            // Clear existing lists
            allTownSquares.Clear();
            obstructedTownSquares.Clear();

            foreach (Map map in Find.Maps)
            {
                // Only for town squares on player-owned settlements
                if (map.IsPlayerHome && map.listerThings.ThingsOfDef(MousekinDefOf.Mousekin_TownSquare).FirstOrDefault() is Thing townSquare)
                {
                    // Add current town square to list
                    allTownSquares.Add(townSquare);

                    // If current town square is obstructed, add it to another list
                    if (CompTownSquare.HasObstructedCellsWithinSquare(townSquare.positionInt, map, townSquare.def.GetCompProperties<CompProperties_TownSquare>()))
                    {
                        obstructedTownSquares.Add(townSquare);
                    }
                }
            }

            if (obstructedTownSquares.Count > 0)
            {
                // If the player only has one town square, which is also obstructed
                if (allTownSquares.Count == 1)
                {
                    // Use the (default) singular alert label and desc
                }
                // If the player has more than one town square
                else if (allTownSquares.Count > 1)
                {
                    string quantifier = "";
                    string possess = "Possess_Plural".Translate();
                    // If only one of multiple town squares is obstructed
                    if (obstructedTownSquares.Count == 1)
                    {
                        // Use the (default) singular alert label and plural desc with "One" and "has"
                        quantifier = "Quantifier_One".Translate().CapitalizeFirst();
                        possess = "Possess_Singular".Translate();
                    }
                    // If all town squares are obstructed
                    else if (obstructedTownSquares.Count == allTownSquares.Count)
                    {
                        // Use the plural alert label and desc with "all"
                        defaultLabel = "AlertObstructedTownSquarePluralLabel".Translate();
                        quantifier = "Quantifier_All".Translate().CapitalizeFirst();
                    }
                    // Default: only some town squares are obstructed
                    else
                    {
                        // Use the plural alert label and desc with "some"
                        defaultLabel = "AlertObstructedTownSquarePluralLabel".Translate();
                        quantifier = "Quantifier_Some".Translate().CapitalizeFirst();
                    }
                    defaultExplanation = "AlertObstructedTownSquarePluralDesc".Translate(quantifier, MousekinDefOf.Mousekin_TownSquare.LabelCap, possess) + "AlertObstructedTownSquareDescEnding".Translate();
                }
                // Enable the alert
                return AlertReport.CulpritsAre(obstructedTownSquares);
            }

            return false;
        }
    }
}
