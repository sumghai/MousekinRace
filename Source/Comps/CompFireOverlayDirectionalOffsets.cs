using RimWorld;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    [StaticConstructorOnStartup]
    public class CompFireOverlayDirectionalOffsets : CompFireOverlayBase
    {
        protected CompRefuelable refuelableComp;

        protected CompBreakdownable breakdownableComp;

        protected static readonly Graphic FireGraphic = GraphicDatabase.Get<Graphic_Flicker>("Things/Special/Fire", ShaderDatabase.TransparentPostLight, Vector2.one, Color.white);

        private new CompProperties_FireOverlayDirectionalOffsets Props
        {
            get
            {
                return (CompProperties_FireOverlayDirectionalOffsets)props;
            }
        }

        public override void PostDraw()
        {
            base.PostDraw();

            if (refuelableComp == null || refuelableComp.HasFuel)
            {
                Vector3 flameDrawPos = parent.DrawPos + Props.directionalOffsets.GetOffset(parent.Rotation);
                flameDrawPos.y += 0.04f;
                FireGraphic.Draw(flameDrawPos, Rot4.North, parent);
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            refuelableComp = parent.GetComp<CompRefuelable>();
            breakdownableComp = parent.GetComp<CompBreakdownable>();
        }

        public override void CompTick()
        {
            if ((refuelableComp == null || refuelableComp.HasFuel) && startedGrowingAtTick < 0)
            {
                startedGrowingAtTick = GenTicks.TicksAbs;
            }
        }
    }
}