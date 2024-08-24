using RimWorld;
using Verse;
using Verse.AI;

namespace MousekinRace
{
    public class LordToil_ChurchService : LordToil_Gathering
    {
        public Pawn organizer;

        public new LordToilData_ChurchServiceGiveSermon Data => data as LordToilData_ChurchServiceGiveSermon;

        public LordToil_ChurchService(IntVec3 spot, GatheringDef gatheringDef, Pawn organizer)
          : base(spot, gatheringDef)
        {
            this.spot = spot;
            this.organizer = organizer;
            Log.Warning("LordToil_ChurchService() :: organizer = " + organizer);
            data = new LordToilData_ChurchServiceGiveSermon();
        }

        public static Building GetLecternFromInteractionCell(IntVec3 cell, Map map)
        {
            // Check all four cardinal directions from the given cell
            for (int i = 0; i < 4; i++)
            { 
                IntVec3 c = cell + GenAdj.CardinalDirections[i];
                if (c.InBounds(map))
                { 
                    Building edifice = c.GetEdifice(map);
                    if (edifice != null && edifice.def.hasInteractionCell && edifice.InteractionCell == cell)
                    { 
                        return edifice;
                    }
                }
            }
            return null;
        }

        public override void Init()
        {
            base.Init();
            Data.spectateRect = CellRect.CenteredOn(spot, 0);
            Rot4 rotation = GetLecternFromInteractionCell(spot, organizer.MapHeld).Rotation;
            Data.spectateRectAllowedSides = SpectateRectSide.All & ~rotation.Opposite.AsSpectateSide;
            Data.spectateRectPreferredSide = rotation.AsSpectateSide;
        }

        /*public override ThinkTreeDutyHook VoluntaryJoinDutyHookFor(Pawn p)
        {
            //if (p == this.organizer)
            //    return MousekinDefOf.Mousekin_DutyChurchServiceGiveSermon.hook;
            return MousekinDefOf.Mousekin_DutyChurchServiceGiveSermon.hook;
        }*/

        public override void UpdateAllDuties()
        {
            for (int index = 0; index < lord.ownedPawns.Count; ++index)
            {
                Pawn ownedPawn = lord.ownedPawns[index];
                if (ownedPawn == organizer)
                {
                    Building churchLectern = GetLecternFromInteractionCell(spot, Map);
                    ownedPawn.mindState.duty = new PawnDuty(MousekinDefOf.Mousekin_DutyChurchServiceGiveSermon, (LocalTargetInfo)spot, (LocalTargetInfo)churchLectern, -1f);
                    ownedPawn.jobs.EndCurrentJob(JobCondition.InterruptForced, true, true);
                }
                else
                {
                    ownedPawn.mindState.duty = new PawnDuty(MousekinDefOf.Mousekin_DutyChurchServiceSpectate)
                    {
                        spectateRect = Data.spectateRect,
                        spectateRectAllowedSides = Data.spectateRectAllowedSides,
                        spectateRectPreferredSide = Data.spectateRectPreferredSide
                    };
                }
            }
        }
    }
}
