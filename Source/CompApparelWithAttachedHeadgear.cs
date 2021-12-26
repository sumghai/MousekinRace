using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    public class CompApparelWithAttachedHeadgear : ThingComp
    {
        public CompProperties_ApparelWithAttachedHeadgear Props => (CompProperties_ApparelWithAttachedHeadgear)props;

        public Apparel Apparel => parent as Apparel;

        public Pawn Pawn => Apparel.Wearer;

        public bool isHatOn;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref isHatOn, "isHatOn", defaultValue: false, forceSave: true);
        }

        public override IEnumerable<Gizmo> CompGetWornGizmosExtra()
        {
            foreach (Gizmo item in base.CompGetWornGizmosExtra())
            {
                yield return item;
            }

            if (Pawn != null)
            {
                Command_Toggle command_Toggle = new Command_Toggle
                {
                    defaultLabel = "MousekinRace_CommandToggleAttachedHeadgear_Label".Translate(Props.attachedHeadgearDef.label),
                    defaultDesc = "MousekinRace_CommandToggleAttachedHeadgear_Desc".Translate(Props.attachedHeadgearDef.label),
                    defaultIconColor = Apparel.DrawColor,
                    icon = ContentFinder<Texture2D>.Get(Props.toggleUiIconPath),
                    isActive = () => isHatOn,
                    toggleAction = delegate
                    {
                        isHatOn = !isHatOn;
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