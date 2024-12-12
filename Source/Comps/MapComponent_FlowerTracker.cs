using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MousekinRace
{
    public class MapComponent_FlowerTracker : MapComponent
    {
        public const int FlowerThresholdMedBaseline = 20;

        public const int FlowerThresholdHighBaseline = 30;

        public int flowerThresholdLow = 10;

        public int flowerThresholdMed = FlowerThresholdMedBaseline; // Default

        public int flowerThresholdHigh = FlowerThresholdHighBaseline; // Default

        public int GardenAreaThresholdLow = 32; // Equivalent to two 4x4 (16 cell) flower patches

        public const int MinQtyForVarietyToBeCounted = 5;

        public int gardenSize = 0;

        public HashSet<Plant> playerFlowersPlanted = [];

        public List<int> playerFlowerDestructionTicks = [];

        // Count the varieties of flowers planted by players
        //
        // To prevent cheesing of the max variety bonuses (by planting lots of one type, and only one of each of all other types),
        // we only include a given variety to the count if a minimum number of them are planted.
        public int playerFlowerVarietiesPlanted => playerFlowersPlanted.GroupBy(p => p.def).Select(group => new { Count = group.Count() }).Where(x => x.Count >= MinQtyForVarietyToBeCounted).Count();

        public int ticksToForget = GenDate.TicksPerYear;

        public MapComponent_FlowerTracker(Map map) : base(map)
        {
        }

        public override void MapComponentTick()
        {
            // Every in-game hour, recalculate threshold amounts for flowers planted
            if (GenTicks.TicksAbs % GenDate.TicksPerHour == 0)
            {
                int preceptBelievers = Faction.OfPlayer.ideos.PrimaryIdeo.ColonistBelieverCountCached;

                flowerThresholdMed = BelieverScaledFlowerThreshold(preceptBelievers, FlowerThresholdMedBaseline);
                flowerThresholdHigh = BelieverScaledFlowerThreshold(preceptBelievers, FlowerThresholdHighBaseline);
            }

            // Every in-game day:
            //
            // 1) Calculate the total area of growing zones set to grow flowers
            //
            // 2) "Forget" about flowers that were destroyed over a year ago
            //
            // We don't forget about flowers planted, unless they were despawned by other means;
            // this supports edge cases like perpetual spring/summer biomes, or long-lived flowers
            if (GenTicks.TicksAbs % GenDate.TicksPerDay == 0)
            {
                gardenSize = GetFlowerGardensTotalArea();
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

        // Count area (number of cells) that are growing areas designated to grow flowers
        public int GetFlowerGardensTotalArea()
        {
            List<Zone> zonesList = map.zoneManager.AllZones;

            int totalArea = 0;
            
            foreach (Zone zone in zonesList)
            {
                if (zone is IPlantToGrowSettable plantableZone && MousekinDefOf.Mousekin_ValidFlowers.flowerPlants.Contains(plantableZone.GetPlantDefToGrow()))
                {
                    totalArea += zone.CellCount;
                }
            }

            return totalArea;
        }

        // Scale the flower thresholds based on ideo/precept believer count using an asymptotic exponential formula
        public int BelieverScaledFlowerThreshold(int believers, int baselineThreshold)
        {
            float growthRate = 0.038f;
            float multiplier = 16.7f;
            float k = 0.25f;
            int stepSize = 10;
            
            return Math.Max(baselineThreshold, (int)Math.Round(multiplier * baselineThreshold * (1 - Math.Exp(k - (growthRate * believers))) / stepSize) * stepSize);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref gardenSize, "gardenSize", GetFlowerGardensTotalArea());
            Scribe_Collections.Look(ref playerFlowersPlanted, "playerFlowersPlanted", LookMode.Reference);
            Scribe_Collections.Look(ref playerFlowerDestructionTicks, "playerFlowerDestructionTicks", LookMode.Value);
        }
    }
}
