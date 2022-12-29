using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MousekinRace
{
    public class CompAutoCutWindmill : CompAutoCutWindTurbine
    {
        public override IEnumerable<IntVec3> GetAutoCutCells()
        {
            return CompWindmill.CalculateWindCells(parent.Position, parent.TryGetComp<CompWindmill>().capDirection, parent.TryGetComp<CompWindmill>().Props);
        }
    }
}
