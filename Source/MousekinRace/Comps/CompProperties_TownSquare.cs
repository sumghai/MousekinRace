using System.Collections.Generic;
using Verse;

namespace MousekinRace
{
    public class CompProperties_TownSquare : CompProperties
    {
        public IntVec2 squareSize = IntVec2.One;

        public IntVec2 squareCenterOffset = IntVec2.Zero;

        public IntVec2 speechSpeakerOffset = IntVec2.Zero;

        public GraphicData flagPoleGraphicData = null;

        public List<TerrainDef> acceptablePavedTerrainDefs;
        
        public CompProperties_TownSquare()
        {
            compClass = typeof(CompTownSquare);
        }
    }
}
