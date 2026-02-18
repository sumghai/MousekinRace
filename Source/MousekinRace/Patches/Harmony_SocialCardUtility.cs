using HarmonyLib;
using RimWorld;
using Verse;

namespace MousekinRace
{
    // Conditionally patch leader and moral guide role titles for Mousekin player faction
    // in the social tab and ritual dialog role selection display
    [HarmonyPatch(typeof(SocialCardUtility), nameof(SocialCardUtility.DrawPawnRole))]
    public static class Harmony_SocialCardUtility_DrawPawnRole_ReplaceRoleTitlesForMousekinPlayer
    {
        static void Prefix(Pawn pawn, Precept_Role role, ref string label)
        {
            // Case: changing roles
            if (role != null)
            {
                label = Utils.ReplaceIdeoRoleTitlesForMousekinPlayer(label, role);
            }
            // Case: removing a role from a pawn without assigning a new role
            else
            {
                Precept_Role pawnCurrentRole = pawn.Ideo.GetRole(pawn);
                if (pawnCurrentRole != null) 
                {
                    label = Utils.ReplaceIdeoRoleTitlesForMousekinPlayer(label, pawnCurrentRole);
                }
            }
        }
    }
}
