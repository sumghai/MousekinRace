using System.Collections.Generic;
using Verse;

namespace MousekinRace
{
    public class MapComponent_Fertilizer(Map map) : MapComponent(map)
    {
        public Dictionary<IntVec3, float> cellFertilityBonus = [];

        public override void MapComponentTick()
        {
            base.MapComponentTick();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref cellFertilityBonus, "cellFertilityBonus", LookMode.Value, LookMode.Value);
        }
    }
}
