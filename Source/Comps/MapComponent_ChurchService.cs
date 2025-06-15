using RimWorld;
using Verse;

namespace MousekinRace
{
    public class MapComponent_ChurchService : MapComponent
    {
        public const int churchServiceBeginHour = 9; // 9 am
        
        public MapComponent_ChurchService(Map map) : base(map)
        {
        }

        public override void MapComponentTick()
        {
            base.MapComponentTick();

            // Every in-game hour, check if it is:
            // - Day 5, 10 or 15 of the current quadrum (essentially every 5 days)
            // - The hour at which church services are supposed to start
            if (GenTicks.TicksAbs % 2500 == 0 && (GenLocalDate.DayOfQuadrum(map) + 1) % 5 == 0 && GenLocalDate.HourOfDay(map) == churchServiceBeginHour)
            {
                // If there are enough potential worshippers
                if (ChurchService_Utils.EnoughPlayerMousekinWorshippers(map))
                {
                    GatheringWorker_ChurchService churchServiceWorker = MousekinDefOf.Mousekin_GatheringChurchService.Worker as GatheringWorker_ChurchService;

                    // If there is a properly-furnished and accessible church on the map, as well as a priest available,
                    // start a church service gathering
                    if (ChurchService_Utils.ValidChurchFound(map, out _) && ChurchService_Utils.GetRandomMousekinPriest(map) is Pawn priest && churchServiceWorker.CanExecute(map, priest))
                    {
                        churchServiceWorker.TryExecute(map, priest);
                    }
                    // Otherwise, notify the player and give all potential worshippers a mood debuff
                    else 
                    {
                        Messages.Message("MousekinRace_MessageChurchMissedService".Translate(), MessageTypeDefOf.NegativeEvent);

                        foreach (Pawn potentialWorshipper in ChurchService_Utils.GetMousekinPotentialWorshippers(map))
                        {
                            potentialWorshipper.needs.mood.thoughts.memories.TryGainMemory(MousekinDefOf.Mousekin_Thought_ChurchMissedService);
                        }
                    }
                }
            }
        }
    }
}
