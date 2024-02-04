using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MousekinRace
{
    public class ScenPart_RequiredFaction : ScenPart
    {
        public FactionDef factionDef;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref factionDef, "factionDef");
        }

        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            if (!Widgets.ButtonText(listing.GetScenPartRect(this, ScenPart.RowHeight), factionDef.LabelCap))
            {
                return;
            }
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
