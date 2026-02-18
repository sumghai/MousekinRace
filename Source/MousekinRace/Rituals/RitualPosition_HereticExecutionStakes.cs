using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MousekinRace
{
    public class RitualPosition_HereticExecutionStakes : RitualPosition
    {
        public ThingDef stakeMoteDef;
        
        public static int hereticPos_zOffset = 2;
        
        public static IntVec3 hereticPos_farLeft = new(-2, 0, hereticPos_zOffset);
        public static IntVec3 hereticPos_nearLeft = new(-1, 0, hereticPos_zOffset);
        public static IntVec3 hereticPos_center = new(0, 0, hereticPos_zOffset);
        public static IntVec3 hereticPos_nearRight = new(1, 0, hereticPos_zOffset);
        public static IntVec3 hereticPos_farRight = new(2, 0, hereticPos_zOffset);

        public List<IntVec3> hereticPos_num2 = [hereticPos_nearLeft, hereticPos_nearRight];
        public List<IntVec3> hereticPos_num3 = [hereticPos_farLeft, hereticPos_center, hereticPos_farRight];

        public override PawnStagePosition GetCell(IntVec3 spot, Pawn p, LordJob_Ritual ritual)
        {
            LordJob_Ritual_HereticExecution hereticExecution = ritual as LordJob_Ritual_HereticExecution;
            Building_TownSquare townSquare = hereticExecution.selectedTarget.Thing as Building_TownSquare;
            int totalHereticsCount = hereticExecution.heretics.Count;
            int hereticEscortPairingIndex = hereticExecution.escorts.FindIndex(pawn => pawn == p);
            Pawn hereticForThisEscort = hereticExecution.heretics[hereticEscortPairingIndex];

            IntVec3 haulToPos = townSquare.centerCellPos;

            switch (totalHereticsCount) 
            {
                case 1:
                    haulToPos += hereticPos_center;
                    break;
                case 2:
                    haulToPos += hereticPos_num2[hereticEscortPairingIndex];
                    break;
                case 3:
                    haulToPos += hereticPos_num3[hereticEscortPairingIndex];
                    break;
            }

            hereticExecution.tmpStakePositions.Add(haulToPos);
            return new PawnStagePosition(haulToPos, townSquare, Rot4.South, false);
        }
    }
}
