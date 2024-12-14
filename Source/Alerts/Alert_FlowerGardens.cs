using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    public class Alert_FlowerGardens : Alert
    {
        public List<string> gardenQualityRating = new()
        {
            "QualityCategory_Awful".Translate().CapitalizeFirst(),
            "QualityCategory_Poor".Translate().CapitalizeFirst(),
            "Adjective_Mediocre".Translate().CapitalizeFirst(),
            "QualityCategory_Good".Translate().CapitalizeFirst(),
            "QualityCategory_Excellent".Translate().CapitalizeFirst(),
            "Adjective_Beautiful".Translate().CapitalizeFirst()
        };
        
        public Alert_FlowerGardens() 
        {
            requireIdeology = true;
        }

        public override string GetLabel()
        {
            MapComponent_FlowerTracker flowerTracker = Find.CurrentMap.GetComponent<MapComponent_FlowerTracker>();
            return (flowerTracker?.gardenSize > 0) ? "AlertFlowerGardenLabel".Translate(GetGardenQuality(flowerTracker)) : MousekinDefOf.Humanlike_Thought_GardensDesired.stages[0].LabelCap;
        }

        public override TaggedString GetExplanation()
        {
            MapComponent_FlowerTracker flowerTracker = Find.CurrentMap.GetComponent<MapComponent_FlowerTracker>();

            if (flowerTracker?.gardenSize > 0)
            {                
                return "AlertFlowerGardenDesc".Translate(
                    Faction.OfPlayer.ideos.PrimaryIdeo.Named("IDEO"),
                    flowerTracker.gardenSize, 
                    flowerTracker.playerFlowersPlanted.Count,
                    flowerTracker.PlayerFlowerVarietiesPlanted,
                    GetGardenQuality(flowerTracker)) +
                    (gardenQualityRating.GetRange(0, 3).Contains((string)GetGardenQuality(flowerTracker)) ? "\n\n" + "AlertFlowerGardenDesc_PlantMoreSuffix".Translate() : null);
            }
            return "AlertNoFlowerGardensDesc".Translate(Faction.OfPlayer.ideos.PrimaryIdeo.Named("IDEO"));
        }

        public override AlertReport GetReport()
        {
            return Find.CurrentMap.IsPlayerHome && Faction.OfPlayer.ideos.PrimaryIdeo.HasPrecept(MousekinDefOf.Mousekin_Precept_FlowersDesired);
        }

        public TaggedString GetGardenQuality(MapComponent_FlowerTracker flowerTracker)
        {
            int gardenArea = flowerTracker.gardenSize;
            int flowerCount = flowerTracker.playerFlowersPlanted.Count;
            int flowerVarieties = flowerTracker.PlayerFlowerVarietiesPlanted;

            int grade = 0;
            Color gradeColor = new();
            
            if (gardenArea > 0 && gardenArea < flowerTracker.GardenAreaThresLow) // No or undersized gardens
            {
                if (flowerCount < flowerTracker.flowersPlantedThresLow || flowerVarieties < flowerTracker.flowerVarietyThresMed)
                {
                    grade = 0;
                }
                else
                {
                    grade = 1;
                }
            }
            else // Adequate garden sizes
            {
                if (flowerCount < flowerTracker.flowersPlantedThresLow)
                {
                    if (flowerVarieties < flowerTracker.flowerVarietyThresMed)
                    {
                        grade = 0;
                    }
                    else if (flowerVarieties >= flowerTracker.flowerVarietyThresMed && flowerVarieties < flowerTracker.flowerVarietyThresHigh)
                    {
                        grade = 1;
                    }
                    else // high variety
                    {
                        grade = 2;
                    }
                }
                else if (flowerCount >= flowerTracker.flowersPlantedThresLow && flowerCount < flowerTracker.flowersPlantedThresMed)
                {
                    if (flowerVarieties < flowerTracker.flowerVarietyThresMed)
                    {
                        grade = 1;
                    }
                    else // greater than or equal to medium variety
                    {
                        grade = 2;
                    }
                }
                else if (flowerCount >= flowerTracker.flowersPlantedThresMed && flowerCount < flowerTracker.flowersPlantedThresHigh)
                {
                    if (flowerVarieties < flowerTracker.flowerVarietyThresMed)
                    {
                        grade = 2;
                    }
                    else if (flowerVarieties >= flowerTracker.flowerVarietyThresMed && flowerVarieties < flowerTracker.flowerVarietyThresHigh)
                    {
                        grade = 3;
                    }
                    else // high variety
                    {
                        grade = 4;
                    }
                }
                else // high flower count
                {
                    if (flowerVarieties < flowerTracker.flowerVarietyThresMed)
                    {
                        grade = 2;
                    }
                    else if (flowerVarieties >= flowerTracker.flowerVarietyThresMed && flowerVarieties < flowerTracker.flowerVarietyThresHigh)
                    {
                        grade = 4;
                    }
                    else // high variety
                    {
                        grade = 5;
                    }
                }
            }

            switch (grade)
            {
                case < 2:
                    gradeColor = Color.red;
                    break;
                case > 2:
                    gradeColor = Color.green;
                    break;
                default:
                    gradeColor = Color.yellow;
                    break;
            }

            return gardenQualityRating[grade].Colorize(gradeColor);
        }
    }
}
