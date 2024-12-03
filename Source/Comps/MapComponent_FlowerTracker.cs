using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MousekinRace
{
    public class MapComponent_FlowerTracker : MapComponent
    {
        public HashSet<Plant> playerFlowersPlanted = [];

        public List<int> playerFlowerDestructionTicks = [];

        public int ticksToForget = GenDate.TicksPerYear;

        public MapComponent_FlowerTracker(Map map) : base(map)
        {
        }

        public override void MapComponentTick()
        {
            // Every in-game day, "forget" about flowers that were destroyed over a year ago
            //
            // We don't forget about flowers planted, unless they were despawned by other means;
            // this supports edge cases like perpetual spring/summer biomes, or long-lived flowers
            if (GenTicks.TicksAbs % GenDate.TicksPerDay == 0)
            {
                playerFlowerDestructionTicks.RemoveWhere(t => Find.TickManager.TicksGame - t > ticksToForget);
            }
        }

        // Register player-planted flowers if the player faction has the Flowers desired precept
        public void Notify_FlowerPlanted(Plant flower)
        {
            if (PlayerHasFlowersDesiredPrecept() && IsValidFlower(flower))
            {
                playerFlowersPlanted.Add(flower);
            }
        }

        // Register destruction time of player-planted flowers deliberately destroyed by pawns/damage from non-Player factions,
        // if the player faction has the Flowers desired precept (ignores flowers that die from the cold)
        public void Notify_FlowerDestroyed(Plant flower, DamageInfo? dInfo)
        {
            if (PlayerHasFlowersDesiredPrecept() && IsValidFlower(flower))
            {
                playerFlowersPlanted.Remove(flower);

                if (dInfo?.Instigator != null && dInfo?.Instigator.Faction != Faction.OfPlayer)
                {
                    playerFlowerDestructionTicks.Add(Find.TickManager.TicksGame);
                }
            } 
        }

        // Check whether player faction ideo desires flowers
        public bool PlayerHasFlowersDesiredPrecept()
        {
            return Faction.OfPlayer.ideos.PrimaryIdeo.HasPrecept(MousekinDefOf.Mousekin_Precept_FlowersDesired);
        }

        // Valid flowers must meet all of the following criteria:
        // - In the (XML-defined) list of valid flowers
        // - Is planted in a growing zone or flower pot
        // - Is outside
        public bool IsValidFlower(Plant flower)
        {
            return MousekinDefOf.Mousekin_ValidFlowers.flowerPlants.Contains(flower.def) && flower.CurrentlyCultivated() && flower.IsOutside();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref playerFlowersPlanted, "playerFlowersPlanted", LookMode.Reference);
            Scribe_Collections.Look(ref playerFlowerDestructionTicks, "playerFlowerDestructionTicks", LookMode.Value);
        }
    }
}
