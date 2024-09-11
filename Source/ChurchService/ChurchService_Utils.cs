using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MousekinRace
{
    public static class ChurchService_Utils
    {
        public const int MinNumWorshippers = 5;

        // Get a list of non-Apostate Mousekin player colonists on a given map as (potential) worshippers
        // - Apostates have trait degree = 1
        // - Mousekins with no religious affinity are also considered worshippers
        public static List<Pawn> GetMousekinPotentialWorshippers(Map map)
        { 
            return map.mapPawns.PawnsInFaction(Faction.OfPlayer).Where(p => p.IsMousekin() && p.story.traits.DegreeOfTrait(MousekinDefOf.Mousekin_TraitSpectrum_Faith) != 1).ToList();
        }

        // Determine if there are enough (i.e. 5 or more) Mousekin worshippers on a given map
        public static bool EnoughPlayerMousekinWorshippers(Map map)
        {
            return GetMousekinPotentialWorshippers(map).Count() > MinNumWorshippers;
        }

        // Determine if there is a properly-configured church room/building on a given map
        // - Start by searching for a Mousekin altar
        // - Check that it is enclosed in a room
        // - Check that the room also has a lectern and at least 3 pews
        // - Return additional data regarding what furnishings are present (for showing met/unmet requirements)
        public static bool ValidChurchFound(Map map, out Building altarOnMap, out bool lecternOnMap, out bool enoughPewsOnMap)
        {
            // Get the first (usually only) church altar on the map
            Building altar = map.listerBuildings.AllBuildingsColonistOfDef(MousekinDefOf.Mousekin_ChurchAltar).FirstOrDefault();

            bool altarInValidRoom = false;
            bool lecternFound = false;
            int pewsFound = 0;
            int MinNumPews = 3;

            // If an altar exists, and is enclosed in a room (and is thus a Church)
            if (altar != null && RoomRoleWorker_Church.Validate(altar.GetRoom()))
            {
                altarInValidRoom = true;

                // Check if church furniture requirements have been met
                List<Thing> containedAndAdjacentThings = altar.GetRoom().ContainedAndAdjacentThings;
                for (int j = 0; j < containedAndAdjacentThings.Count; j++)
                {
                    // Look for any lecterns
                    if (containedAndAdjacentThings[j].def == MousekinDefOf.Mousekin_ChurchLectern)
                    {
                        lecternFound = true;
                    }
                    // Count number of pews
                    if (containedAndAdjacentThings[j].def == MousekinDefOf.Mousekin_ChurchPew)
                    {
                        pewsFound++;
                    }
                    // Stop checking if requirements are already met
                    if (lecternFound && (pewsFound >= MinNumPews))
                    {
                        break;
                    }
                }
            }

            altarOnMap = altar ?? null;
            lecternOnMap = lecternFound;
            enoughPewsOnMap = (pewsFound >= MinNumPews);

            return altarInValidRoom && lecternFound && enoughPewsOnMap;
        }

        // Override of ValidChurchFound() that only also returns the church altar on the map
        public static bool ValidChurchFound(Map map, out Building altarOnMap)
        {
            return ValidChurchFound(map, out altarOnMap, out _, out _);
        }

        // Get a list of cells behind the last row of pews/seats in a church,
        // where pawns without seats can stand
        public static List<IntVec3> GetStandableCellsBehindPews(Map map, Building lectern)
        {
            // Find the church room, given a lectern
            Room churchRoom = lectern.Position.GetRoom(map);

            // Get a list of pews/seats in the church room
            List<Thing> sittables = churchRoom.ContainedAndAdjacentThings.Where(x => x.def.category == ThingCategory.Building && x.def.building.isSittable).ToList();

            List<IntVec3> foundSpots = [];

            // Take the direction the lectern is facing, find the furthest row/column of seats from it,
            // then mark all the free cells beyond that as standable
            // (we +/- 1 so that pawns don't simply stand in the exact same row/column as the furthest seats
            switch (lectern.Rotation.AsInt)
            {
                case Rot4.NorthInt:
                    foundSpots = churchRoom.Cells.Where(c => c.z - 1 > sittables.MaxBy(t => t.Position.z).Position.z).ToList();
                    break;
                case Rot4.EastInt:
                    foundSpots = churchRoom.Cells.Where(c => c.x - 1 > sittables.MaxBy(t => t.Position.x).Position.x).ToList();
                    break;
                case Rot4.SouthInt:
                    foundSpots = churchRoom.Cells.Where(c => c.z + 1 < sittables.MaxBy(t => t.Position.z).Position.z).ToList();
                    break;
                case Rot4.WestInt:
                    foundSpots = churchRoom.Cells.Where(c => c.x + 1 < sittables.MaxBy(t => t.Position.x).Position.x).ToList();
                    break;
            }

            return foundSpots;
        }

        // Find a random Mousekin Priest from the player colonists on a given map
        public static Pawn GetRandomMousekinPriest(Map map)
        { 
            return map.mapPawns.PawnsInFaction(Faction.OfPlayer).Where(p => p.kindDef == MousekinDefOf.MousekinPriest).RandomElementWithFallback();
        }
    }
}
