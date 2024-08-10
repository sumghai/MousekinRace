using RimWorld;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    public class CompStateDependentBuildingEmissive : ThingComp
    {
        protected CompPowerTrader powerComp;

        protected CompRefuelable refuelableComp;

        protected CompBreakdownable breakdownableComp;

        public CompProperties_StateDependentBuildingEmissive Props => (CompProperties_StateDependentBuildingEmissive)props;

        public override void PostDraw()
        {
            base.PostDraw();

            float emissiveAlpha = (!FlickUtility.WantsToBeOn(parent) || (powerComp != null && !powerComp.PowerOn) || (refuelableComp != null && !refuelableComp.HasFuel) || (breakdownableComp != null && breakdownableComp.BrokenDown)) ? 0 : 1;

            Mesh emissiveMesh = Props.emissiveGraphicData.Graphic.MeshAt(parent.Rotation);

            Vector3 emssiveDrawPos = parent.DrawPos;

            emssiveDrawPos.y += 0.03f;

            Graphics.DrawMesh(emissiveMesh, emssiveDrawPos + Props.emissiveGraphicData.drawOffset.RotatedBy(parent.Rotation), Quaternion.identity, FadedMaterialPool.FadedVersionOf(Props.emissiveGraphicData.Graphic.MatAt(parent.Rotation), emissiveAlpha), 0);
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            powerComp = parent.GetComp<CompPowerTrader>();
            refuelableComp = parent.GetComp<CompRefuelable>();
            breakdownableComp = parent.GetComp<CompBreakdownable>();
        }
    }
}