using RimWorld;
using System.Linq;
using Verse;
using Verse.AI.Group;

namespace MousekinRace
{
    public class GatheringWorker_ChurchService : GatheringWorker
    {
        public override Pawn FindOrganizer(Map map)
        {
            return ChurchService_Utils.GetRandomMousekinPriest(map);
        }

        public override bool CanExecute(Map map, Pawn organizer = null)
        {
            organizer ??= FindOrganizer(map);
            if (organizer == null)
            {
                return false;
            }
            return organizer != null && TryFindGatherSpot(organizer, out IntVec3 _);
        }

        public override LordJob CreateLordJob(IntVec3 spot, Pawn organizer)
        {
            return new LordJob_Joinable_ChurchService(spot, organizer, def);
        }

        public override bool TryFindGatherSpot(Pawn organizer, out IntVec3 spot)
        {            
            spot = default;

            Room church = organizer.Map.regionGrid.allRooms.Find(r => r.Role == MousekinDefOf.Mousekin_RoomChurch);
            if (church == null)
            {
                return false;
            }

            Thing churchLectern = church.ContainedAndAdjacentThings.Where(t => t.def == MousekinDefOf.Mousekin_ChurchLectern).FirstOrDefault();
            if (churchLectern == null || churchLectern.Position.IsForbidden(organizer))
            {
                return false;
            }

            spot = churchLectern.InteractionCell;

            return true;
        }
    }
}
