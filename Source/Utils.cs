using AlienRace;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI.Group;

namespace MousekinRace
{
    public static class Utils
    {
        // Determine whether a given pawn or pawnKindDef is a Mousekin
        public static bool IsMousekin(this Pawn pawn)
        {
            return pawn.def.Equals(MousekinDefOf.Mousekin);
        }

        public static bool IsMousekin(this PawnKindDef pawnKindDef)
        {
            return (pawnKindDef != null) ? pawnKindDef.race.Equals(MousekinDefOf.Mousekin) : false;
        }

        // Determine if a faction's ideo/culture is Mousekin
        public static bool IsMousekin(this CultureDef culture)
        {
            return culture.defName.Contains("Mousekin");
        }

        // Determine if a faction's ideo/culture is (based on) the Mousekin Kingdom
        // (applies to both default Mousekin player and NPC Mousekin Kingdom faction
        public static bool IsMousekinKingdomLike(this CultureDef culture)
        {
            return culture.IsMousekin() && culture.defName.Contains("Kingdom");
        }

        // Determine if a faction's ideo/culture is (based on) the Independent Mousekin Towns
        public static bool IsMousekinIndyTownLike(this CultureDef culture)
        {
            return culture.IsMousekin() && culture.defName.Contains("IndyTown");
        }

        // Determine if a faction's ideo/culture is (based on) the Rodemani Nomads
        public static bool IsMousekinNomadLike(this CultureDef culture)
        {
            return culture.IsMousekin() && culture.defName.Contains("Nomad");
        }

        // Determine if a faction's ideo/culture is (based on) the Brigands
        public static bool IsMousekinBrigandLike(this CultureDef culture)
        {
            return culture.IsMousekin() && culture.defName.Contains("Brigand");
        }

        // Determine whether a given pawn is a non-Mousekin rodentkind
        public static bool IsNonMousekinRodentkind(this Pawn pawn)
        {
            List<ThingDef_AlienRace> otherRodentRaces = Enumerable.Concat(
                MousekinDefOf.Mousekin.GetModExtension<OtherRodentRacesExtension>().differentRodentRaces,
                MousekinDefOf.Mousekin.GetModExtension<OtherRodentRacesExtension>().hostileRodentRaces
            ).ToList();

            return otherRodentRaces.Contains(pawn.def);
        }

        // Determine whether a given pawn is a heretic by Mousekin standards:
        // - Mousekins with either the Apostate or Devotionist trait
        // - Non-Mousekin rodentkinds in general
        public static bool IsHeretic(this Pawn pawn)
        {
            return pawn.IsMousekin() &&
                pawn.story.traits.DegreeOfTrait(MousekinDefOf.Mousekin_TraitSpectrum_Faith) > 0 &&
                pawn.story.traits.DegreeOfTrait(MousekinDefOf.Mousekin_TraitSpectrum_Faith) < 3;
        }

        // Determine whether a pawn is being executed as part of the Purging Flames ritual
        // Note that we directly check by whether or not they have ritual role of heretic, and not simply IsHeretic()
        public static bool IsHereticBeingPurgedByFire(this Pawn pawn)
        {
            return pawn.GetLord()?.LordJob is LordJob_Ritual_HereticExecution hereticExecution && hereticExecution.heretics.Contains(pawn);
        }

        // Get the primary race of any given faction
        public static ThingDef_AlienRace GetRaceOfFaction(FactionDef faction) => (faction.basicMemberKind?.race ?? faction.pawnGroupMakers?.SelectMany(selector: groupMaker => groupMaker.options).GroupBy(keySelector: groupMaker => groupMaker.kind.race).OrderByDescending(keySelector: g => g.Count()).First().Key) as ThingDef_AlienRace;

        // Get percentage of player faction free colonists that are Mousekins
        public static float PercentColonistsAreMousekins()
        {
            List<Pawn> allColonists = PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonists.Where(p => !p.IsSlave && !p.IsPrisoner).ToList();
            int playerFactionTotalColonistCount = allColonists.Count();
            int playerFactionMousekinColonistCount = allColonists.Where(p => IsMousekin(p)).Count();
            return (playerFactionTotalColonistCount == 0) ? 0 : (float) playerFactionMousekinColonistCount / playerFactionTotalColonistCount;
        }

        // Return a human-readable year with "year" being conditionally pluralized
        public static string YearHumanReadable(float years)
        {
            return (years != 1.0f) ? "PeriodYears".Translate(years.ToString("0.#")) : "Period1Year".Translate();
        }

        // Set the Mousekin religious affinity level for a given pawn
        // Includes handling for pawns without any affinity in the first place
        public static void ChangePawnReligiousAffinity(this Pawn pawn, ReligiousTraitAffinity newAffinity)
        {
            TraitDef affinityTrait = MousekinDefOf.Mousekin_TraitSpectrum_Faith;

            if (pawn.story.traits.HasTrait(affinityTrait))
            {
                pawn.story.traits.GetTrait(affinityTrait).degree = (int)newAffinity;
            }
            else
            {
                pawn.story.traits.GainTrait(new Trait(affinityTrait, (int)newAffinity), true);
            }
        }

        // Replace the leader and moral guide ideo role titles for Mousekin Player faction
        // Includes optional boolean flag to remove any indefinite articles from the title
        public static string ReplaceIdeoRoleTitlesForMousekinPlayer(string originalString, Precept_Role role, bool removeIndef = false)
        {   
            if (role != null && role.ideo.culture.IsMousekin())
            {                
                string tempOutput = originalString;

                string tgtRoleLabelCap = removeIndef ? Find.ActiveLanguageWorker.WithIndefiniteArticle(role.LabelCap) : role.LabelCap;

                if (role.def.leaderRole)
                {
                    originalString = tempOutput.Replace(tgtRoleLabelCap, "MousekinRace_PreceptRole_PlayerLeaderTitle".Translate());
                }
                if (role.ideo.culture.IsMousekinKingdomLike() && role.def == PreceptDefOf.IdeoRole_Moralist)
                {
                    originalString = tempOutput.Replace(tgtRoleLabelCap, MousekinDefOf.MousekinPriest.label.Replace(MousekinDefOf.Mousekin.label, "").Trim().CapitalizeFirst());
                }
            }
            return originalString;
        }

        // For Mousekin Great Pine cost list strings, replace the original Minified Tree text
        // with an amended version that includes a "(Pine Tree)" suffix
        public static string PatchMinifiedTreeWithPineTreeSuffix(this string originalString)
        {
            // Handle both lowercase and title case variants
            originalString = originalString.Replace(ThingDefOf.MinifiedTree.label, ThingDefOf.MinifiedTree.label + $" ({ThingDefOf.Plant_TreePine.label})");
            originalString = originalString.Replace(ThingDefOf.MinifiedTree.LabelCap, ThingDefOf.MinifiedTree.LabelCap + $" ({ThingDefOf.Plant_TreePine.LabelCap})");

            return originalString;
        }

        // Regenerate precepts for Mousekin ideos
        // (useful if the player is migrating an existing save created prior to the v1.2 mod update)
        public static void RegenerateIdeoPrecepts()
        {
            if (Current.ProgramState == ProgramState.Playing)
            {
                List<Ideo> mousekinIdeos = Find.IdeoManager.ideos.Where(x => x.culture.IsMousekin()).ToList();

                if (mousekinIdeos.Count > 0)
                {
                    foreach (Ideo ideo in mousekinIdeos)
                    {
                        ideo.foundation.RandomizePrecepts(init: true, new IdeoGenerationParms(IdeoUIUtility.FactionForRandomization(ideo)));
                        ideo.RegenerateDescription();
                        ideo.anyPreceptEdited = false;
                        ideo.style.RecalculateAvailableStyleItems(); // Also regen Mousekin hair and beard styles
                    }
                    Messages.Message("MousekinRace_MessageIdeoPreceptRegen_Done".Translate(mousekinIdeos.Count()), MessageTypeDefOf.TaskCompletion, false);
                }
                else
                {
                    Messages.Message("MousekinRace_MessageIdeoPreceptRegen_NoMouseIdeosFound".Translate(), MessageTypeDefOf.RejectInput, false);
                }
            }
            else
            {
                Messages.Message("MousekinRace_MessageIdeoPreceptRegen_MustLoadSavegame".Translate(), MessageTypeDefOf.RejectInput, false);
            }
        }
    }
}
