using Verse;

namespace MousekinRace
{
    public class Utils
    {      
        //Determine whether a given pawn is a Mousekin
        public static bool IsMousekin(Pawn pawn)
        {
            return pawn.def.Equals(MousekinDefOf.Mousekin);
        }
    }
}
