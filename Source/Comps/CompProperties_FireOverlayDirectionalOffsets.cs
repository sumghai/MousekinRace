using RimWorld;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    public class CompProperties_FireOverlayDirectionalOffsets : CompProperties_FireOverlay
    {
        public DirectionalOffsets directionalOffsets = null;

        public CompProperties_FireOverlayDirectionalOffsets()
        {
            compClass = typeof(CompFireOverlayDirectionalOffsets);
        }

        public override void DrawGhost(IntVec3 center, Rot4 rot, ThingDef thingDef, Color ghostCol, AltitudeLayer drawAltitude, Thing thing = null)
        {
            GhostUtility.GhostGraphicFor(CompFireOverlay.FireGraphic, thingDef, ghostCol).DrawFromDef(center.ToVector3ShiftedWithAltitude(drawAltitude) + directionalOffsets.GetOffset(rot), rot, thingDef);
        }
    }

    public class DirectionalOffsets
    {
        public Vector3 GetOffset(Rot4 rot) =>
            rot == Rot4.South ? this.south :
            rot == Rot4.North ? this.north :
            rot == Rot4.East ? this.east :
            this.west != Vector3.zero ? this.west : new Vector3(-this.east.x, this.east.y, this.east.z);

        public Vector3 north = Vector3.zero;
        public Vector3 east = Vector3.zero;
        public Vector3 south = Vector3.zero;
        public Vector3 west = Vector3.zero;
    }
}
