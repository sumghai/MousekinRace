using RimWorld;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    [StaticConstructorOnStartup]
    public class CompFireOverlayMulti : CompFireOverlay
    {
        public new CompProperties_FireOverlayMulti Props => (CompProperties_FireOverlayMulti)props;

        public override void PostDraw()
        {
            if (refuelableComp == null || refuelableComp.HasFuel)
            {
                foreach (FireOffsetEntry fire in Props.fires) 
                {
                    Vector3 drawPos = parent.DrawPos + Props.DrawOffsetForRot(fire, parent.Rotation);
                    drawPos.y += 1f / 26f;
                    FireGraphic.Draw(drawPos, parent.Rotation, parent);
                }
            }
        }
    }
}