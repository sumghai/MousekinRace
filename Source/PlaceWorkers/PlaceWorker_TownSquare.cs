using RimWorld;
using System.Linq;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    public class PlaceWorker_TownSquare : PlaceWorker
    {
        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
        {
            if (def.GetCompProperties<CompProperties_TownSquare>() != null)
            {
                GenDraw.DrawFieldEdges(CompTownSquare.CalculateSquareCells(center, def.GetCompProperties<CompProperties_TownSquare>()).ToList(), AllowsPlacing(def, center, rot, Find.CurrentMap) ? Color.white : Color.red);
            }
        }

        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            if (CompTownSquare.HasObstructedCellsWithinSquare(loc, map, ((ThingDef)checkingDef).GetCompProperties<CompProperties_TownSquare>()))
            {
                return new AcceptanceReport("MousekinRace_PlaceWorker_MustBeClearOfObstructions".Translate(checkingDef.LabelCap));
            }

            if (CompTownSquare.HasUnsuitableTerrainCellsWithinSquare(loc, map, ((ThingDef)checkingDef).GetCompProperties<CompProperties_TownSquare>(), checkingDef.terrainAffordanceNeeded))
            {
                return new AcceptanceReport("TerrainCannotSupport_TerrainAffordance".Translate(checkingDef, checkingDef.terrainAffordanceNeeded));
            }

            return true;
        }
    }
}
