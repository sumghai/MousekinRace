using RimWorld;
using Verse;

namespace MousekinRace
{
    public class RitualPosition_FlowerDanceSpots : RitualPosition
    {
        public float circleRadius = 2f; // Adjustable in XML

        public override PawnStagePosition GetCell(IntVec3 spot, Pawn p, LordJob_Ritual ritual)
        {
            LordJob_Ritual_FlowerDance flowerDance = ritual as LordJob_Ritual_FlowerDance;
            int totalNunsCount = flowerDance.nuns.Count;
            int curNunIndex = flowerDance.nuns.IndexOf(p);
            IntVec3 cell = spot + IntVec3.FromPolar(360f * (float)curNunIndex / (float)totalNunsCount, circleRadius);
            float angle = (ritual.selectedTarget.Cell - cell).ToVector3().AngleFlat();
            Rot4 facing = Pawn_RotationTracker.RotFromAngleBiased(angle);
            return new PawnStagePosition(cell, ritual.selectedTarget.Thing, facing, false);
        }
    }
}
