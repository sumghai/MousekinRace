﻿using RimWorld;
using Verse;

namespace MousekinRace
{
    public class ThoughtWorker_FlowersPlanted : ThoughtWorker_Precept
    {
        public override ThoughtState ShouldHaveThought(Pawn p)
        {
            return ThoughtState.ActiveAtStage(ThoughtStageIndex(p));
        }

        public int ThoughtStageIndex(Pawn p)
        {
            MapComponent_FlowerTracker flowerTracker = p.Map.GetComponent<MapComponent_FlowerTracker>();
            int flowersPlanted = flowerTracker.playerFlowersPlanted.Count;
            int varietyCount = flowerTracker.playerFlowerVarietiesPlanted;

            // dev - remove before release
            Log.Warning($"{p} : {flowersPlanted} flowers of {varietyCount} varieties planted (qty thresholds: {flowerTracker.flowersPlantedThresLow} low, {flowerTracker.flowersPlantedThresMed} med , {flowerTracker.flowersPlantedThresHigh} high) (variety thresholds: {flowerTracker.flowerVarietyThresMed} med, {flowerTracker.flowerVarietyThresHigh} high)");

            if (flowersPlanted == 0)
            {
                return 0;
            }
            if (flowersPlanted < flowerTracker.flowersPlantedThresLow)
            {
                return 1;
            }
            if (flowersPlanted >= flowerTracker.flowersPlantedThresMed)
            {
                if (varietyCount >= flowerTracker.flowerVarietyThresMed && varietyCount < flowerTracker.flowerVarietyThresHigh)
                {
                    return 3;
                }
                if (flowersPlanted >= flowerTracker.flowersPlantedThresHigh && varietyCount >= flowerTracker.flowerVarietyThresHigh)
                {
                    return 4;
                }
            }
            return 2;
        }
    }
}
