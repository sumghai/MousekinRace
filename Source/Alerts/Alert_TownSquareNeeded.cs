using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MousekinRace
{
    // Alerts the player that they need to build a town square for each settlement where 50% or more of the population are Mousekins
    public class Alert_TownSquareNeeded : Alert
    {
        public List<string> mapsNeedingTownSquares = new List<string>();

        public Alert_TownSquareNeeded()
        {
            defaultLabel = "AlertNeedTownSquareSingularLabel".Translate();
            defaultExplanation = "AlertNeedTownSquareSingularDesc".Translate() + "AlertNeedTownSquareDescEnding".Translate(MousekinDefOf.Mousekin_TownSquare.LabelCap);
            defaultPriority = AlertPriority.Medium;
        }
        
        public override AlertReport GetReport()
        {
            // Clear existing lists
            mapsNeedingTownSquares.Clear();

            int playerSettlementsCount = Find.Maps.Where(m => m.IsPlayerHome).Count();

            foreach (Map map in Find.Maps) 
            { 
                // Only for player-owned settlements
                if(map.IsPlayerHome) 
                {
                    // Get the ratio/percentage of Mousekin colonists
                    int curMapTotalColonistCount = map.mapPawns.ColonistCount;
                    int curMapTotalMousekinColonistCount = map.mapPawns.AllPawns.Where(p => p.IsColonist && Utils.IsMousekin(p)).Count();
                    float curMapMousekinColonistPercent = (curMapTotalColonistCount == 0) ? 0 : curMapTotalMousekinColonistCount / curMapTotalColonistCount;

                    // If the current settlement has 50% or more Mousekin colonists but does not have a Town Square, add it to a list of settlements that need remediation
                    if (curMapMousekinColonistPercent >= 0.5 && map.listerThings.ThingsOfDef(MousekinDefOf.Mousekin_TownSquare).Count == 0)
                    { 
                        mapsNeedingTownSquares.Add(map.info.parent.LabelCap);
                    }
                }
            }

            int mapsNeedingTownSquaresCount = mapsNeedingTownSquares.Count();

            if (mapsNeedingTownSquaresCount > 0) 
            {
                // If the player owns only one settlement, and that settlement needs a town square, 
                if (playerSettlementsCount == 1)
                {
                    // Use the (default) singular alert label and desc
                }
                // If the player owns more than one settlement
                else if (playerSettlementsCount > 1)
                {
                    string quantifier = "";
                    //If only one of multiple settlements needs a town square
                    if (mapsNeedingTownSquaresCount == 1)
                    {
                        // Use the (default) singular alert label and plural desc with "one"
                        quantifier = "Quantifier_One".Translate();
                    }
                    // If all the settlements need town squares
                    else if (mapsNeedingTownSquaresCount == playerSettlementsCount)
                    {
                        // Use the plural alert label and desc with "all"
                        defaultLabel = "AlertNeedTownSquarePluralLabel".Translate();
                        quantifier = "Quantifier_All".Translate();
                    }
                    // Default: only some settlements need town squares
                    else
                    {
                        // Use the plural alert label and desc with "some"
                        defaultLabel = "AlertNeedTownSquarePluralLabel".Translate();
                        quantifier = "Quantifier_Some".Translate();
                    }
                    defaultExplanation = "AlertNeedTownSquarePluralDesc".Translate(quantifier, "\n\n- " + string.Join("\n- ", mapsNeedingTownSquares)) + "AlertNeedTownSquareDescEnding".Translate(MousekinDefOf.Mousekin_TownSquare.LabelCap);
                }
                // Enable the alert
                return true;
            }

            return false;
        }
    }
}
