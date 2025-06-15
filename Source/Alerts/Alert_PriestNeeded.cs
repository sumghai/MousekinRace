using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MousekinRace
{
    public class Alert_PriestNeeded : Alert
    {
        public TaggedString PriestPawnkindLabel = Find.ActiveLanguageWorker.ToTitleCase(MousekinDefOf.MousekinPriest.LabelCap);

        public Alert_PriestNeeded()
        {
            defaultLabel = "AlertNeedPriestLabel".Translate(PriestPawnkindLabel);
            defaultPriority = AlertPriority.Medium;
        }

        public override TaggedString GetExplanation()
        {
            return "AlertNeedPriestDesc".Translate(PriestPawnkindLabel.Colorize(ColoredText.NameColor), MousekinDefOf.Mousekin_ChurchAltar.LabelCap.Colorize(ColoredText.NameColor));
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

                // Show the alert if we have a church built, but no priest
                if (ChurchService_Utils.ValidChurchFound(map, out Building altarOnMap) && ChurchService_Utils.GetRandomMousekinPriest(map) == null)
                {
                    return AlertReport.CulpritIs(altarOnMap);
                }
            }
            return false;
        }
    }
}
