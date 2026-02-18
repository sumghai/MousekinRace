using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    public class Building_CellarOutdoor : Building_Storage
    {
        public IntVec3 StorageCellPos => Position + new IntVec3(0, 0, -2);

        public override IEnumerable<IntVec3> AllSlotCells()
        {
            if (!Spawned)
            {
                yield break;
            }
            yield return StorageCellPos;
        }

        public static bool ThingIsInCellar(Thing thing)
        { 
            return thing.Spawned && thing.def.category == ThingCategory.Item && thing.Position.GetFirstThing<Building_CellarOutdoor>(thing.Map) is Building_CellarOutdoor cellarOutdoor && thing.Position == cellarOutdoor.StorageCellPos;
        }

        public void EjectThing(Thing thing)
        {
            IntVec3 originalLocInCellar = thing.Position;
            Map map = thing.Map;

            thing.DeSpawn();

            if (!GenPlace.TryPlaceThing(thing, InteractionCell, map, ThingPlaceMode.Near, null, 
                delegate (IntVec3 newLoc)
                {
                    foreach (var t in map.thingGrid.ThingsListAtFast(newLoc))
                    {
                        if (t is Building_CellarOutdoor)
                        {
                            return false;
                        }
                    }
                    return true;
                }))
            { 
                GenSpawn.Spawn(thing, originalLocInCellar, map);
            }

            if (thing.TryGetComp(out CompForbiddable comp))
            {
                comp.Forbidden = true;
            }
        }
        
        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo gizmo in base.GetGizmos())
            {
                if (gizmo is Command_SelectStorage)
                {
                    continue;
                }
                yield return gizmo;
            }
        }

        public override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            base.DrawAt(drawLoc, flip);
            if (def.GetModExtension<CellarOutdoorGraphicsExtension>() is CellarOutdoorGraphicsExtension graphicExt && graphicExt.exposedWallGraphicData != null)
            {
                Mesh exposedWallMesh = graphicExt.exposedWallGraphicData.Graphic.MeshAt(Rotation);
                Vector3 exposedWallDrawPos = DrawPos + graphicExt.exposedWallGraphicData.drawOffset;
                exposedWallDrawPos.y = AltitudeLayer.BuildingBelowTop.AltitudeFor() + graphicExt.exposedWallGraphicData.drawOffset.y;
                Graphics.DrawMesh(exposedWallMesh, exposedWallDrawPos, Quaternion.identity, graphicExt.exposedWallGraphicData.Graphic.GetColoredVersion(graphicExt.exposedWallGraphicData.Graphic.Shader, DrawColor, DrawColorTwo).MatAt(Rotation), 0);
            }
        }
    }
}
