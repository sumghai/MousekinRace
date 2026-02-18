using RimWorld;
using Verse;

namespace MousekinRace
{
    public class RitualPosition_HereticExecutionMoralist : RitualPosition
    {
        public override PawnStagePosition GetCell(IntVec3 spot, Pawn p, LordJob_Ritual ritual)
        {
            LordJob_Ritual_HereticExecution hereticExecution = ritual as LordJob_Ritual_HereticExecution;
            Building_TownSquare townSquare = hereticExecution.selectedTarget.Thing as Building_TownSquare;
            return new PawnStagePosition(townSquare.centerCellPos, townSquare, Rot4.South, false);
        }
    }
}
