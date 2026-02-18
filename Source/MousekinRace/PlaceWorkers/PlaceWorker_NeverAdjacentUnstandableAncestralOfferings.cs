using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    public class PlaceWorker_NeverAdjacentUnstandableAncestralOfferings : PlaceWorker
    {
        public float radius = 5;

        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
        {
            GenDraw.DrawFieldEdges(GenRadial.RadialCellsAround(center, radius + 0.9f, useCenter: true).ToList(), Color.white);
        }

        public override AcceptanceReport AllowsPlacing(BuildableDef def, IntVec3 center, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            foreach (IntVec3 item in GenRadial.RadialCellsAround(center, radius + 0.9f, useCenter: true))
            {
                if (!item.InBounds(map))
                {
                    return false;
                }
                List<Thing> list = map.thingGrid.ThingsListAt(item);
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] != thingToIgnore && (!item.Walkable(map) || list[i].def.passability != 0))
                    {
                        return "MustPlaceAdjacentStandable".Translate();
                    }
                }
            }
            return true;
        }
    }
}
