using RimWorld;
using Verse;

namespace MousekinRace
{
    public class LordToilData_ChurchServiceGiveSermon : LordToilData_Gathering
    {
        // todo - decide if we need to restrict spectating from one side only
        public SpectateRectSide spectateRectAllowedSides = SpectateRectSide.All;
        public CellRect spectateRect;
        public SpectateRectSide spectateRectPreferredSide;

        public override void ExposeData()
        {
            Scribe_Values.Look<CellRect>(ref this.spectateRect, "spectateRect", new CellRect(), false);
            Scribe_Values.Look<SpectateRectSide>(ref this.spectateRectAllowedSides, "spectateRectAllowedSides", SpectateRectSide.None, false);
            Scribe_Values.Look<SpectateRectSide>(ref this.spectateRectPreferredSide, "spectateRectPreferredSide", SpectateRectSide.None, false);
        }
    }
}
