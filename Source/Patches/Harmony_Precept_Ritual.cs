using HarmonyLib;
using RimWorld;

namespace MousekinRace
{
    // Conditionally patch leader and moral guide titles for Mousekin player faction
    // for ritual gizmo roles description list
    [HarmonyPatch(typeof(Precept_Ritual), nameof(Precept_Ritual.RolesDescription))]
    public static class Harmony_Precept_Ritual_RolesDescription_ReplaceRoleTitlesForMousekinPlayer
    {
        static void Postfix(ref string __result, ref Precept_Ritual __instance)
        {
            if (__result != null) 
            {
                foreach (RitualRole role in __instance.behavior.def.roles) 
                {
                    __result = Utils.ReplaceIdeoRoleTitlesForMousekinPlayer(__result, role.FindInstance(__instance.ideo));
                }
            }
        }
    }
}
