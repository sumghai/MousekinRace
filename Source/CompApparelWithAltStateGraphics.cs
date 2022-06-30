using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    public class CompApparelWithAltStateGraphics : ThingComp
    {
        public CompProperties_ApparelWithAltStateGraphics Props => (CompProperties_ApparelWithAltStateGraphics)props;

        public Apparel Apparel => parent as Apparel;

        public Pawn Pawn => Apparel.Wearer;

        public bool isAltState;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref isAltState, "isAltState", defaultValue: false, forceSave: true);
        }

        public override IEnumerable<Gizmo> CompGetWornGizmosExtra()
        {
            foreach (Gizmo item in base.CompGetWornGizmosExtra())
            {
                yield return item;
            }

            if (Pawn != null && Pawn.IsColonistPlayerControlled)
            {
                Command_Toggle command_Toggle = new Command_Toggle
                {
                    defaultLabel = Props.toggleLabel,
                    defaultDesc = Props.toggleDesc,
                    defaultIconColor = Apparel.DrawColor,
                    icon = isAltState ? Apparel.def.uiIcon: Props.altStateDef.uiIcon,
                    isActive = () => isAltState,
                    toggleAction = delegate
                    {
                        isAltState = !isAltState;
                        Pawn.Drawer.renderer.graphics.ResolveAllGraphics();
                    },
                    turnOffSound = null,
                    turnOnSound = null
                };
                yield return command_Toggle;
            }
        }
    }
}