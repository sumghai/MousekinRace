using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    // Alerts the player that they need to build a church if their current map population has 5 or more non-Apostate Mousekins
    // (Checked for each player-owned colony map)
    public class Alert_ChurchNeeded : Alert
    {
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

                // Only check for a Church if we have enough viable worshippers
                if (ChurchService_Utils.EnoughPlayerMousekinWorshippers(map))
                {
                    bool validChurch = ChurchService_Utils.ValidChurchFound(map, out Building altarOnMap, out bool lecternFound, out bool enoughPewsFound);

                    requirementsList = "- " + MousekinDefOf.Mousekin_ChurchAltar.label.Colorize((altarOnMap != null) ? Color.green : ColoredText.NameColor) + "\n"
                            + "- " + MousekinDefOf.Mousekin_ChurchLectern.label.Colorize(lecternFound ? Color.green : ColoredText.NameColor) + "\n"
                            + "- " + (MousekinDefOf.Mousekin_ChurchPew.label + " x3 (" + "MousekinRace_Minimum".Translate() + ")").Colorize(enoughPewsFound ? Color.green : ColoredText.NameColor);

                    if (validChurch)
                    {
                        return false;
                    }
                    else
                    {                     
                        return altarOnMap != null ? AlertReport.CulpritIs(altarOnMap) : true;
                    }
                }
            }
            return false;
        }
    }
}
