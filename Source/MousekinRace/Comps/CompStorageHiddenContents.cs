using RimWorld;
using Verse;

namespace MousekinRace
{
    public class CompStorageHiddenContents : ThingComp
    {
        public static bool ThingIsInStorage(Thing thing)
        {            
            return thing.Spawned && thing.def.category == ThingCategory.Item && thing.Position.GetFirstThing<Building_Storage>(thing.Map) is Building_Storage storage && storage.def.HasComp(typeof(CompStorageHiddenContents)) && storage.cachedOccupiedCells.Contains(thing.Position);
        }

        public static void EjectThing(Thing thing)
        {
            IntVec3 originalLocInStorage = thing.Position;
            Map map = thing.Map;

            thing.DeSpawn();

            if (!GenPlace.TryPlaceThing(thing, originalLocInStorage, map, ThingPlaceMode.Near, null,
                delegate (IntVec3 newLoc)
                {
                    foreach (var t in map.thingGrid.ThingsListAtFast(newLoc))
                    {
                        if (t is Building_Storage)
                        {
                            return false;
                        }
                    }
                    return true;
                }))
            {
                GenSpawn.Spawn(thing, originalLocInStorage, map);
            }

            if (thing.TryGetComp(out CompForbiddable comp))
            {
                comp.Forbidden = true;
            }
        }
    }
}
