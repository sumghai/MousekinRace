using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    public class ScenPart_RequiredFaction : ScenPart
    {
        public FactionDef factionDef;

        public bool useFactionIdeoForPlayer = false;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref factionDef, "factionDef");
            Scribe_Values.Look(ref useFactionIdeoForPlayer, "useFactionIdeoForPlayer", false);
        }

        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            Rect scenPartRect = listing.GetScenPartRect(this, ScenPart.RowHeight * 2f);
            Rect factionRect = new Rect(scenPartRect.x, scenPartRect.y, scenPartRect.width, RowHeight);
            if (Widgets.ButtonText(factionRect, factionDef.LabelCap))
            {
                List<FloatMenuOption> list = new List<FloatMenuOption>();
                foreach (FactionDef item in PossibleFactionDefs())
                {
                    FactionDef localFd = item;
                    list.Add(new FloatMenuOption(localFd.LabelCap, delegate
                    {
                        factionDef = localFd;
                    }));
                }
                Find.WindowStack.Add(new FloatMenu(list));
            }
            Rect useFactionIdeoForPlayerCheckboxRect = new Rect(scenPartRect.x, scenPartRect.y + RowHeight, scenPartRect.width, RowHeight);
            Widgets.CheckboxLabeled(useFactionIdeoForPlayerCheckboxRect, "MousekinRace_ScenPart_RequiredFaction_UseIdeoForPlayer".Translate(), ref useFactionIdeoForPlayer);
        }

        public override void Randomize()
        {
            factionDef = PossibleFactionDefs().RandomElement();
        }

        public virtual IEnumerable<FactionDef> PossibleFactionDefs()
        {
            return DefDatabase<FactionDef>.AllDefs.Where((FactionDef fd) => !fd.isPlayer);
        }
    }
}
