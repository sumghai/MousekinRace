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

        public override List<PawnRenderNode> CompRenderNodes()
        {
            List<PawnRenderNode> hoodRenderNode = new();
            PawnRenderNodeProperties properties = new();

            properties.texPath = Props.attachedHeadgearGraphicPath;
            properties.color = Apparel.DrawColor;
            properties.shaderTypeDef = Apparel.def.graphicData.shaderType;
            properties.workerClass = typeof(PawnRenderNodeWorker_Apparel_Head_ToggleableHood);
            properties.parentTagDef = PawnRenderNodeTagDefOf.ApparelHead;
            properties.baseLayer = 80f; // todo - replace with dynamically-fetched value

            PawnRenderNode_ApparelToggleableHood hoodApparelNode = new(Pawn, properties, Pawn.drawer.renderer.renderTree);
            hoodApparelNode.attachedHeadgearComp = this;
            hoodRenderNode.Add(hoodApparelNode);

            return hoodRenderNode;
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
                    defaultLabel = "MousekinRace_CommandToggleAttachedHeadgearLabel".Translate(Props.attachedHeadgearLabel),
                    defaultDesc = "MousekinRace_CommandToggleAttachedHeadgearDesc".Translate(Props.attachedHeadgearLabel),
                    defaultIconColor = Apparel.DrawColor,
                    icon = ContentFinder<Texture2D>.Get(Props.toggleUiIconPath),
                    isActive = () => isHatOn,
                    toggleAction = delegate
                    {
                        isHatOn = !isHatOn;
                        Pawn.Drawer.renderer.SetAllGraphicsDirty();
                    },
                    turnOffSound = null,
                    turnOnSound = null
                };
                yield return command_Toggle;
            }
        }
    }
}