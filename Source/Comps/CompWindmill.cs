using ItemProcessor;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    [StaticConstructorOnStartup]
    public class CompWindmill : ThingComp
    {
        public CompProperties_Windmill Props => (CompProperties_Windmill)props;

        public static List<IntVec3> windmillCells = new();

        public int terraformProgressTicks;

        public Rot4 capDirection;

        public float spinPosition = Rand.Range(0f, 360f);

        public const float northSouthYscale = 0.75f;

        public int updateWeatherEveryXTicks = 250;

        public int ticksSinceWeatherUpdate;

        public List<IntVec3> windPathCells = new();

        public List<Thing> windPathBlockedByThings = new();

        public List<IntVec3> windPathBlockedCells = new();

        public float TerraformProgressDays => (float) terraformProgressTicks / 60000f;

        public float CurrentTerraformRadius => Mathf.Min(Props.terraformRadius, TerraformProgressDays / Props.daysToTerraformRadius * Props.terraformRadius);

        public int CachedNumOfTerraformedCells = 0;

        public float effectiveWindSpeed;

        public bool Working => effectiveWindSpeed >= 0.1f;

        public int TicksUntilRadiusInteger
        {
            get
            {
                float num = Mathf.Ceil(CurrentTerraformRadius) - CurrentTerraformRadius;
                if (num < 1E-05f)
                {
                    num = 1f;
                }
                float num2 = Props.terraformRadius / Props.daysToTerraformRadius;
                return (int)(num / num2 * 60000f);
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            RecalculateBlockages();
        }

        public override void PostDeSpawn(Map map)
        {
            terraformProgressTicks = 0;
        }

        public override void CompTick()
        {
            base.CompTick();

            if (spinPosition >= 360f)
            {
                spinPosition = 0f;
            }
            else
            {
                spinPosition += effectiveWindSpeed;
            }

            ticksSinceWeatherUpdate++;
            if (ticksSinceWeatherUpdate >= updateWeatherEveryXTicks)
            {
                effectiveWindSpeed = Mathf.Min(parent.Map.windManager.WindSpeed, 1.5f);
                ticksSinceWeatherUpdate = 0;
                RecalculateBlockages();
                if (windPathBlockedCells.Count > 0)
                {
                    float num2 = 0f;
                    for (int i = 0; i < windPathBlockedCells.Count; i++)
                    {
                        num2 += effectiveWindSpeed * 0.1f; // Larger sails, less affected by blocked cells
                    }
                    effectiveWindSpeed -= num2;
                    if (effectiveWindSpeed < 0)
                    {
                        effectiveWindSpeed = 0;
                    }
                }
            }

            // Pause VFE item processor if sails are not spinning
            if (parent is Building_ItemProcessor itemProcessor)
            {
                itemProcessor.isPaused = !Working;
            }
        }

        public override void CompTickRare()
        {
            if (Working)
            {
                terraformProgressTicks += 250;

                int num = GenRadial.NumCellsInRadius(CurrentTerraformRadius);

                if (num > CachedNumOfTerraformedCells)
                {
                    for (int i = 0; i < num; i++)
                    {
                        AffectCell(parent.Position + GenRadial.RadialPattern[i]);
                    }
                    CachedNumOfTerraformedCells = num;
                }
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref effectiveWindSpeed, "effectiveWindSpeed", 0);
            Scribe_Values.Look(ref capDirection, "capDirection", defaultValue: Rot4.South, forceSave: true);
            Scribe_Values.Look(ref ticksSinceWeatherUpdate, "updateCounter", 0);
            Scribe_Values.Look(ref terraformProgressTicks, "terraformProgressTicks", 0);
        }

        public override void PostDrawExtraSelectionOverlays()
        {
            if (CurrentTerraformRadius < Props.terraformRadius - 0.0001f)
            {
                GenDraw.DrawRadiusRing(parent.Position, CurrentTerraformRadius, Color.green);
                GenDraw.DrawRadiusRing(parent.Position, Props.terraformRadius, new Color(0f, 1f, 0f, 0.5f));
            }
            GenDraw.DrawFieldEdges(windPathCells);
        }

        public override void PostDraw()
        {
            base.PostDraw();

            GraphicData capGraphicData = Props.capGraphicData;
            GraphicData sailGraphicData = Props.sailGraphicData;

            // Draw the cap of the windmill
            if (capGraphicData != null)
            {
                Mesh capMesh = capGraphicData.Graphic.MeshAt(capDirection);
                Graphics.DrawMesh(capMesh, parent.DrawPos + capGraphicData.drawOffset, Quaternion.identity, capGraphicData.Graphic.GetColoredVersion(capGraphicData.Graphic.Shader, parent.DrawColor, parent.DrawColorTwo).MatAt(capDirection), 0);
            }

            // Draw the animated sails based on cap orientation
            if (sailGraphicData != null)
            {
                Mesh sailMesh;

                // Select the appropriate draw offset
                Vector3 sailDrawOffset = capDirection.AsInt switch
                {
                    Rot4.NorthInt => sailGraphicData.drawOffsetNorth ?? sailGraphicData.drawOffset,
                    Rot4.EastInt => sailGraphicData.drawOffsetEast ?? sailGraphicData.drawOffset,
                    Rot4.SouthInt => sailGraphicData.drawOffsetSouth ?? sailGraphicData.drawOffset,
                    Rot4.WestInt => sailGraphicData.drawOffsetWest ?? sailGraphicData.drawOffset,
                    _ => sailGraphicData.drawOffset,
                };

                // Set the rotation direction
                int sailDirection = (capDirection == Rot4.South || capDirection == Rot4.East) ? -1 : 1;

                // North/South cap direction
                if (capDirection == Rot4.North || capDirection == Rot4.South)
                {
                    // Iterate through each of the sails
                    for (int sail = 0; sail < Props.sailCount; sail++)
                    {
                        sailMesh = (capDirection == Rot4.South) ? MeshPool.GridPlane(sailGraphicData.Graphic.drawSize) : MeshPool.GridPlaneFlip(sailGraphicData.Graphic.drawSize);

                        Graphics.DrawMesh(sailMesh, parent.DrawPos + sailDrawOffset, Quaternion.identity * GenMath.ToQuat(sailDirection * spinPosition + (360f / Props.sailCount) * sail), sailGraphicData.Graphic.GetColoredVersion(capGraphicData.Graphic.Shader, parent.DrawColor, parent.DrawColorTwo).MatAt(capDirection), 0);
                    }
                }
                // East/West cap direction
                else
                {
                    // Iterate through each of the sails
                    for (int sail = 0; sail < Props.sailCount; sail++)
                    {
                        Vector3 sailAxleDrawPos = parent.DrawPos + sailDrawOffset;

                        float sailSpinPositionRaw = spinPosition - (360f / Props.sailCount) * sail;
                        float sailSpinPositionClean = (sailSpinPositionRaw < 0) ? sailSpinPositionRaw + 360f : sailSpinPositionRaw;

                        float sailRenderLength = Props.sailGraphicData.drawSize.x * Mathf.Sin(sailSpinPositionClean * Mathf.PI / 180);

                        if (sailRenderLength < 0f)
                        {
                            sailRenderLength *= -1f;
                        }

                        float sailLayerDrawOffset = (sailSpinPositionClean <= 90f || sailSpinPositionClean >= 270f) ? 0.1f : -0.1f;

                        Vector3 sailRenderVector = new(sailRenderLength, sailLayerDrawOffset, Props.sailGraphicData.drawSize.y * 0.2f);

                        sailAxleDrawPos.y += sailLayerDrawOffset;

                        Matrix4x4 sailRenderMatrix = default;
                        sailRenderMatrix.SetTRS(sailAxleDrawPos, Quaternion.identity * GenMath.ToQuat((sailDirection == 1) ? 90f : 270f), sailRenderVector);

                        Graphics.DrawMesh(sailSpinPositionClean < 180f ? MeshPool.plane10 : MeshPool.plane10Flip, sailRenderMatrix, sailGraphicData.Graphic.GetColoredVersion(capGraphicData.Graphic.Shader, parent.DrawColor, parent.DrawColorTwo).MatAt(capDirection), 0);
                    }
                }
            }
        }

        public override string CompInspectStringExtra()
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (DebugSettings.ShowDevGizmos)
            {
                stringBuilder.AppendLine("DEV Wind speed: " + effectiveWindSpeed + " / " + parent.Map.windManager.WindSpeed);
            }

            if (windPathBlockedCells.Count > 0)
            {
                Thing thing = null;
                if (windPathBlockedByThings != null)
                {
                    thing = windPathBlockedByThings[0];
                }
                if (thing != null)
                {
                    stringBuilder.AppendLine("WindTurbine_WindPathIsBlockedBy".Translate() + " " + thing.Label);
                }
                else
                {
                    stringBuilder.AppendLine("WindTurbine_WindPathIsBlockedByRoof".Translate());
                }
            }

            stringBuilder.AppendLine("TimePassed".Translate().CapitalizeFirst() + ": " + terraformProgressTicks.ToStringTicksToPeriod() + "\n" + "CurrentRadius".Translate().CapitalizeFirst() + ": " + CurrentTerraformRadius.ToString("F1"));

            if (TerraformProgressDays < Props.daysToTerraformRadius)
            {
                if (Working)
                {
                    stringBuilder.AppendLine("RadiusExpandsIn".Translate().CapitalizeFirst() + ": " + TicksUntilRadiusInteger.ToStringTicksToPeriod());
                }
                else
                {
                    stringBuilder.AppendLine("MousekinRace_Windmill_LowWindspeed".Translate());
                }
            }
            return stringBuilder.ToString().TrimEndNewlines();
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo item in base.CompGetWornGizmosExtra())
            {
                yield return item;
            }

            Command_Action command_Action = new()
            {
                defaultLabel = "MousekinRace_CommandRotateWindmillCapLabel".Translate(),
                defaultDesc = "MousekinRace_CommandRotateWindmillCapDesc".Translate(),
                action = delegate
                {
                    capDirection.Rotate(RotationDirection.Clockwise);
                    windPathCells.Clear();
                    windPathBlockedCells.Clear();
                    windPathBlockedByThings.Clear();
                    RecalculateBlockages();
                }
            };
            yield return command_Action;

            if (DebugSettings.ShowDevGizmos)
            {
                Command_Action command_Action2 = new Command_Action();
                command_Action2.defaultLabel = "DEV: Progress 1 day";
                command_Action2.action = delegate
                {
                    terraformProgressTicks += 60000;
                };
                yield return command_Action2;
            }
        }

        public static bool HasObstructedCellsWithinRadius(IntVec3 pos, Map map, float radius)
        {
            return GenRadial.RadialCellsAround(pos, radius, true).Where(c => c.InBounds(map) && c.Impassable(map)).Count() > 0;
        }

        public static bool HasOtherWindmillOrBlueprintWithinRadius(IntVec3 pos, Map map, float radius)
        {
            return GenRadial.RadialCellsAround(pos, radius, true).Where(c => c.InBounds(map) && (c.GetFirstThing(map, MousekinDefOf.Mousekin_Windmill) != null || c.GetFirstThing(map, MousekinDefOf.Blueprint_Mousekin_Windmill) != null)).Count() > 0;
        }

        public static IEnumerable<IntVec3> CalculateWindCells(IntVec3 center, Rot4 capDirection, CompProperties_Windmill props)
        {
            CellRect rectA = default(CellRect);
            CellRect rectB = default(CellRect);
            int length = (int)(Math.Floor(props.obstructionFreeRadius) - 2);
            int width = (int)(Math.Floor(props.sailGraphicData.drawSize.x) + 2);

            if (capDirection.IsHorizontal)
            {
                rectA.minX = center.x + 2;
                rectA.maxX = center.x + 2 + length;
                rectB.minX = center.x - 2 - length;
                rectB.maxX = center.x - 2;
                rectB.minZ = rectA.minZ = center.z - (width / 2);
                rectB.maxZ = rectA.maxZ = center.z + (width / 2);
            }
            else
            {
                rectA.minZ = center.z + 2;
                rectA.maxZ = center.z + 2 + length;
                rectB.minZ = center.z - 2 - length;
                rectB.maxZ = center.z - 2;
                rectB.minX = rectA.minX = center.x - (width / 2);
                rectB.maxX = rectA.maxX = center.x + (width / 2);
            }
            for (int z2 = rectA.minZ; z2 <= rectA.maxZ; z2++)
            {
                for (int x = rectA.minX; x <= rectA.maxX; x++)
                {
                    yield return new IntVec3(x, 0, z2);
                }
            }
            for (int z2 = rectB.minZ; z2 <= rectB.maxZ; z2++)
            {
                for (int x = rectB.minX; x <= rectB.maxX; x++)
                {
                    yield return new IntVec3(x, 0, z2);
                }
            }
        }

        public void RecalculateBlockages()
        {
            if (windPathCells.Count == 0)
            {
                IEnumerable<IntVec3> collection = CalculateWindCells(parent.Position, capDirection, Props);
                windPathCells.AddRange(collection);
            }
            windPathBlockedCells.Clear();
            windPathBlockedByThings.Clear();
            for (int i = 0; i < windPathCells.Count; i++)
            {
                IntVec3 intVec = windPathCells[i];
                if (parent.Map.roofGrid.Roofed(intVec))
                {
                    windPathBlockedByThings.Add(null);
                    windPathBlockedCells.Add(intVec);
                    continue;
                }
                List<Thing> list = parent.Map.thingGrid.ThingsListAt(intVec);
                for (int j = 0; j < list.Count; j++)
                {
                    Thing thing = list[j];
                    if (thing.def.blockWind && thing.def != parent.def)
                    {
                        windPathBlockedByThings.Add(thing);
                        windPathBlockedCells.Add(intVec);
                        break;
                    }
                }
            }
        }

        public void AffectCell(IntVec3 c)
        {
            Map map = parent.Map;
            TerrainDef terrain = c.GetTerrain(map);
            TerrainDef terrainToDryTo = GetTerrainToDryTo(map, terrain);
            if (terrainToDryTo != null)
            {
                map.terrainGrid.SetTerrain(c, terrainToDryTo);
            }
            TerrainDef terrainDef = map.terrainGrid.UnderTerrainAt(c);
            if (terrainDef != null)
            {
                TerrainDef terrainToDryTo2 = GetTerrainToDryTo(map, terrainDef);
                if (terrainToDryTo2 != null)
                {
                    map.terrainGrid.SetUnderTerrain(c, terrainToDryTo2);
                }
            }
        }

        public static TerrainDef GetTerrainToDryTo(Map map, TerrainDef terrainDef)
        {
            // Upgrade regular soil to rich soil
            if (terrainDef == TerrainDefOf.Soil)
            {
                return TerrainDefOf.SoilRich;
            }
            if (terrainDef.driesTo == null)
            {
                return null;
            }
            if (map.Biome == BiomeDefOf.SeaIce)
            {
                return TerrainDefOf.Ice;
            }
            return terrainDef.driesTo;
        }
    }
}