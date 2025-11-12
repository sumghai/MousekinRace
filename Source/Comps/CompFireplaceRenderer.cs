using RimWorld;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    [StaticConstructorOnStartup]
    public class CompFireplaceRenderer : CompFireOverlayBase
    {
        protected CompPowerTrader powerComp;

        protected CompRefuelable refuelableComp;

        protected CompBreakdownable breakdownableComp;

        protected static readonly Graphic FireGraphic = GraphicDatabase.Get<Graphic_Flicker>("Things/Special/Fire", ShaderDatabase.TransparentPostLight, Vector2.one, Color.white);

        public new CompProperties_FireplaceRenderer Props => (CompProperties_FireplaceRenderer)props;

        public override void PostDraw()
        {
            base.PostDraw();

            Mesh fireboxMesh = Props.fireboxGraphicData.Graphic.MeshAt(parent.Rotation);

            Vector3 fireboxDrawPos = parent.DrawPos;

            fireboxDrawPos.y -= 0.06f;

            Graphics.DrawMesh(fireboxMesh, fireboxDrawPos + Props.fireboxGraphicData.drawOffset.RotatedBy(parent.Rotation), Quaternion.identity, Props.fireboxGraphicData.Graphic.GetColoredVersion(Props.fireboxGraphicData.Graphic.Shader, parent.DrawColor, parent.DrawColorTwo).MatAt(parent.Rotation, null), 0);

            if (refuelableComp == null || refuelableComp.HasFuel)
            {
                Mesh glowMesh = Props.glowGraphicData.Graphic.MeshAt(parent.Rotation);

                Vector3 glowDrawPos = parent.DrawPos;

                glowDrawPos.y -= 0.04f;

                Graphics.DrawMesh(glowMesh, glowDrawPos + Props.glowGraphicData.drawOffset.RotatedBy(parent.Rotation), Quaternion.identity, FadedMaterialPool.FadedVersionOf(Props.glowGraphicData.Graphic.MatAt(parent.Rotation, null), 1), 0);

                if (parent.Rotation == Rot4.North)
                {
                    if (Props.fires.Any())
                    {
                        foreach (FiresWithOffsets curFire in Props.fires)
                        {
                            Vector3 flameDrawPos = parent.DrawPos + curFire.offset;
                            flameDrawPos.y -= 0.02f;
                            FireGraphic.Draw(flameDrawPos, Rot4.North, parent);
                        }
                    }
                    else
                    {
                        Vector3 flameDrawPos = parent.DrawPos;
                        flameDrawPos.y -= 0.02f;
                        FireGraphic.Draw(flameDrawPos, Rot4.North, parent);
                    }
                }
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            powerComp = parent.GetComp<CompPowerTrader>();
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