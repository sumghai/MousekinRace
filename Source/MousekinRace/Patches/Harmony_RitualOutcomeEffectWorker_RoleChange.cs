using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MousekinRace
{
    // Restrict which Mousekin pawnkinds can accept specific role changes in Mousekin ideologies
    [HarmonyPatch(typeof(RitualOutcomeEffectWorker_RoleChange), nameof(RitualOutcomeEffectWorker_RoleChange.BlockingIssues))]
    public static class Harmony_RitualOutcomeEffectWorker_RoleChange_BlockingIssues_ValidateMousekinPawnkinds
    {
        static IEnumerable<string> Postfix(IEnumerable<string> values, Precept_Ritual ritual, RitualRoleAssignments assignments)
        {
            if (ritual?.def == PreceptDefOf.RoleChange && ritual.ideo?.culture?.IsMousekin() == true && assignments.FirstAssignedPawn("role_changer") is Pawn roleChangingPawn)
            {
                Precept_Role selectedRole = assignments.RoleChangeSelection;
                PreceptDef selectedRoleDef = selectedRole.def;

                // Mousekin ideo moralist roles specifically require the Mousekin Priest pawnkind
                if (selectedRoleDef == PreceptDefOf.IdeoRole_Moralist && roleChangingPawn.kindDef != MousekinDefOf.MousekinPriest)
                {
                    yield return "MousekinRace_MessageRitualRoleMustBeMousekinPriest".Translate(Utils.ReplaceIdeoRoleTitlesForMousekinPlayer(selectedRole.LabelCap, selectedRole), MousekinDefOf.MousekinPriest.LabelCap, MousekinDefOf.Mousekin_ChurchAltar.LabelCap, "MousekinRace_Settings_SectionAllegianceSys_Heading".Translate());
                }
                // Mousekin Priests pawnkinds cannot become ideology/faction leaders
                else if (selectedRoleDef == PreceptDefOf.IdeoRole_Leader && roleChangingPawn.kindDef == MousekinDefOf.MousekinPriest)
                {
                    yield return "MousekinRace_MessageRitualRoleMustBeNonMousekinPriest".Translate(MousekinDefOf.MousekinPriest.LabelCap, Utils.ReplaceIdeoRoleTitlesForMousekinPlayer(selectedRole.LabelCap, selectedRole));
                }
            }
        }
    }
}
