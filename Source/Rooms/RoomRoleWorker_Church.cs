using System.Collections.Generic;
using Verse;

namespace MousekinRace
{
    public class RoomRoleWorker_Church : RoomRoleWorker
    {
        // Only enclosed rooms are valid churches
        public static bool Validate(Room room)
        {
            if (room == null || room.OutdoorsForWork)
            {
                return false;
            }
            return true;
        }
        
        // Rooms with a church altar are considered churches
        public override float GetScore(Room room)
        {
            List<Thing> containedAndAdjacentThings = room.ContainedAndAdjacentThings;
            bool altarFound = false;
            for (int i = 0; i < containedAndAdjacentThings.Count; i++)
            {
                if (containedAndAdjacentThings[i].def == MousekinDefOf.Mousekin_ChurchAltar)
                {
                    altarFound = true;
                    break;
                }
            }
            return (altarFound && Validate(room)) ? 10000 : 0;
        }
    }
}
