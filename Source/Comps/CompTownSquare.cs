using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    [StaticConstructorOnStartup]
    public class CompTownSquare : ThingComp
    {
        public CompProperties_TownSquare Props => (CompProperties_TownSquare)props;

        public static bool HasObstructedCellsWithinSquare(IntVec3 pos, Map map, CompProperties_TownSquare props)
        {
            foreach (IntVec3 item in CalculateSquareCells(pos, props))
            {
                if (item.InBounds(map))
                {
                    List<Thing> list = map.thingGrid.ThingsListAt(item);
                    for (int i = 0; i < list.Count; i++)
                    {
                        // Ignore the Town Square flagpole itself or terrain defs
                        // and look for any artificial buildings or mineable chunks
                        // (Plants are skipped, as they will be auto-cut)
                        if (list[i].def != MousekinDefOf.Mousekin_TownSquare && list[i].def.entityDefToBuild is not TerrainDef && (list[i].def.IsBuildingArtificial || list[i].def.mineable))
                        {
                            // (Some special buildings can be built on Town Squares)
                            if (list[i].def.tradeTags?.Contains("Mousekin_TownSquare_NotObstruction") ?? false)
                            {
                                return false;
                            }
                            
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool HasUnsuitableTerrainCellsWithinSquare(IntVec3 pos, Map map, CompProperties_TownSquare props, TerrainAffordanceDef terrainAffordanceDef)
        {
            foreach (IntVec3 item in CalculateSquareCells(pos, props))
            {
                if (item.InBounds(map))
                {
                    if (!item.GetTerrain(map).affordances.Contains(terrainAffordanceDef))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool HasUnpavedCellsWithinSquare(IntVec3 pos, Map map, CompProperties_TownSquare props)
        {
            foreach (IntVec3 item in CalculateSquareCells(pos, props))
            {
                if (item.InBounds(map))
                {
                    if (!props.acceptablePavedTerrainDefs.Contains(item.GetTerrain(map)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static IEnumerable<IntVec3> CalculateSquareCells(IntVec3 center, CompProperties_TownSquare props)
        {
            
            CellRect rect = default(CellRect);
            int width = props.squareSize.x;
            int length = props.squareSize.z;

            int offsetX = props.squareCenterOffset.x;
            int offsetZ = props.squareCenterOffset.z;

            rect.minX = center.x - (int)Math.Floor((decimal)(width / 2)) + offsetX;
            rect.minZ = center.z - (int)Math.Floor((decimal)(length / 2)) + offsetZ;
            rect.maxX = center.x + (int)Math.Floor((decimal)(width / 2)) + offsetX;
            rect.maxZ = center.z + (int)Math.Floor((decimal)(length / 2)) + offsetZ;

            for (int z = rect.minZ; z <= rect.maxZ; z++)
            {
                for (int x = rect.minX; x <= rect.maxX; x++)
                {
                    yield return new IntVec3(x, 0, z);
                }
            }
        }

        public override void PostDraw()
        {
            base.PostDraw();

            GraphicData flagPoleGraphicData = Props.flagPoleGraphicData;

            if (flagPoleGraphicData != null) 
            {
                Mesh flagPoleMesh = Props.flagPoleGraphicData.Graphic.MeshAt(parent.Rotation);
                Graphics.DrawMesh(flagPoleMesh, parent.DrawPos + flagPoleGraphicData.drawOffset, Quaternion.identity, flagPoleGraphicData.Graphic.GetColoredVersion(flagPoleGraphicData.Graphic.Shader, parent.DrawColor, parent.DrawColorTwo).MatAt(parent.Rotation), 0);
            }

            if (GameComponent_Allegiance.Instance.alignedFaction is Faction allegianceFaction && allegianceFaction.def.GetModExtension<AlliableFactionExtension>() is AlliableFactionExtension facExt && facExt.flagGraphicData != null)
            {
                Mesh flagMesh = facExt.flagGraphicData.Graphic.MeshAt(parent.Rotation);
                Graphics.DrawMesh(flagMesh, parent.DrawPos + facExt.flagGraphicData.drawOffset, Quaternion.identity, facExt.flagGraphicData.Graphic.GetColoredVersion(facExt.flagGraphicData.Graphic.Shader, parent.DrawColor, parent.DrawColorTwo).MatAt(parent.Rotation), 0);
            }
        }
    }
}