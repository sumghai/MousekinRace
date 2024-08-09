using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    public class CompProperties_FireOverlayMulti : CompProperties_FireOverlay
    {
        public List<FireOffsetEntry> fires;

        public Vector3 DrawOffsetForRot(FireOffsetEntry fire, Rot4 rot)
        {
            return rot.AsInt switch
            {
                0 => fire.offsetNorth ?? fire.offset,
                1 => fire.offsetEast ?? fire.offset,
                2 => fire.offsetSouth ?? fire.offset,
                3 => fire.offsetWest ?? fire.offset,
                _ => fire.offset,
            };
        }

        public override void DrawGhost(IntVec3 center, Rot4 rot, ThingDef thingDef, Color ghostCol, AltitudeLayer drawAltitude, Thing thing = null)
        {
            Graphic graphic = GhostUtility.GhostGraphicFor(CompFireOverlay.FireGraphic, thingDef, ghostCol);
            foreach (FireOffsetEntry fire in fires)
            {
                Vector3 loc = center.ToVector3ShiftedWithAltitude(drawAltitude) + thingDef.graphicData.DrawOffsetForRot(rot) + DrawOffsetForRot(fire, rot);
                graphic.DrawFromDef(loc, rot, thingDef);
            }
        }

        public CompProperties_FireOverlayMulti()
        {
            compClass = typeof(CompFireOverlayMulti);
        }
    }

    public class FireOffsetEntry 
    {
        public Vector3 offset;
        public Vector3? offsetNorth;
        public Vector3? offsetSouth;
        public Vector3? offsetWest;
        public Vector3? offsetEast;
    }
}
