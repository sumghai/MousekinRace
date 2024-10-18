using HarmonyLib;
using RimWorld;
using Verse;

namespace MousekinRace
{
    // Conditionally patch leader and moral guide titles for Mousekin player faction
    // for message indicating rituals requiring specific roles
    [HarmonyPatch(typeof(RitualRoleTag), nameof(RitualRoleTag.AppliesToRole))]
    public static class Harmony_RitualRoleTag_AppliesToRole_ReplaceRoleTitlesForMousekinPlayer
    {
        static void Postfix(ref string reason, Precept_Ritual ritual, Pawn p, bool skipReason)
        {
            if (reason != null && p != null && !skipReason) 
            {
                foreach (RitualRole role in ritual.behavior.def.roles)
                {
                    reason = Utils.ReplaceIdeoRoleTitlesForMousekinPlayer(reason, role.FindInstance(ritual.ideo));
                }
            }
        }
    }
}
