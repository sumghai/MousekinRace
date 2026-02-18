using UnityEngine;
using Verse;

namespace MousekinRace
{
    public class BookcaseRenderExtension : DefModExtension
    {
        public GraphicData bookshelfGraphicSouth;
        
        public BookshelfRotOffsets bookshelfRotOffsets = new();

        public Vector3 bookSouthRotSecondRowOffset = new();
        
        public Vector3 bookWestRotDecrementOffset = new();

        public Vector3 bookWestRotFalloffOffset = new();

        public Vector3 bookshelfDrawOffsetSouth = new();

        public Vector3 bookendDrawOffsetNorth = new();

        public Vector3 bookendDrawOffsetEast = new();
    }

    public class BookshelfRotOffsets
    {
        public Vector3 offsetNorth = new();
        public Vector3 offsetEast = new();
        public Vector3 offsetSouth = new();
        public Vector3 offsetWest = new();
    }
}
