using RimWorld;
using Verse;

namespace MousekinRace
{
    public class LordToilData_ChurchServiceGiveSermon : LordToilData_Gathering
    {
        public CellRect spectateRect;
        public SpectateRectSide spectateRectAllowedSides = SpectateRectSide.All;
        public SpectateRectSide spectateRectPreferredSide;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref this.spectateRect, "spectateRect", new CellRect(), false);
            Scribe_Values.Look(ref this.spectateRectAllowedSides, "spectateRectAllowedSides", SpectateRectSide.None, false);
            Scribe_Values.Look(ref this.spectateRectPreferredSide, "spectateRectPreferredSide", SpectateRectSide.None, false);
        }
    }
}
