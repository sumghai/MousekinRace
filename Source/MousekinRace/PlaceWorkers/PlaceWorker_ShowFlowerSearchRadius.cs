using UnityEngine;
using Verse;

namespace MousekinRace
{
    public class PlaceWorker_ShowFlowerSearchRadius : PlaceWorker
    {
        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
        {
            Map currentMap = Find.CurrentMap;
            GenDraw.DrawFieldEdges(Building_Beehive.FlowerSearchCellsAround(center, currentMap));
        }
    }
}
