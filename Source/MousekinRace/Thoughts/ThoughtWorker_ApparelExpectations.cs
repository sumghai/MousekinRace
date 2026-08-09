using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MousekinRace
{
    public class ThoughtWorker_ApparelExpectations : ThoughtWorker
    {       
        public override string PostProcessDescription(Pawn p, string description)
        {
            if (p.kindDef.GetModExtension<ApparelExpectationExtension>() is not ApparelExpectationExtension apparelExpectationExtension)
            {
                return string.Empty;
            }
            else
            {
                List<ThingDef> requiredApparelDefs = p.kindDef.apparelRequired;
                List<ThingDef> optionalApparelDefs = apparelExpectationExtension.optionalApparel;

                // Convert directly to HashSets
                HashSet<ThingDef> requiredApparelDefsHashSet = [.. requiredApparelDefs];
                HashSet<ThingDef> optionalApparelDefsHashSet = [.. optionalApparelDefs];
                HashSet<ThingDef> wornApparelDefsHashSet = [.. p.apparel.WornApparel.Select(apparel => apparel.def)];

                StringBuilder stringBuilder = new();

                // Required apparel worn
                HashSet<ThingDef> wornRequiredApparelDefsHashSet = [.. wornApparelDefsHashSet.Intersect(requiredApparelDefsHashSet)];
                if (wornRequiredApparelDefsHashSet.Count > 0)
                {
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("MousekinRace_Thought_RequiredApparelWorn".Translate());
                    foreach (ThingDef def in wornRequiredApparelDefsHashSet)
                    {
                        stringBuilder.AppendLine("- " + def.LabelCap);
                    }
                }

                // Optional apparel worn
                HashSet<ThingDef> wornOptionalApparelDefsHashSet = [.. wornApparelDefsHashSet.Intersect(optionalApparelDefsHashSet)];
                if (wornOptionalApparelDefsHashSet.Count > 0)
                {
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("MousekinRace_Thought_OptionalApparelWorn".Translate());
                    foreach (ThingDef def in wornOptionalApparelDefsHashSet)
                    {
                        stringBuilder.AppendLine("- " + def.LabelCap);
                    }
                }

                // Inappropriate apparel worn
                HashSet<ThingDef> wornInappropriateApparelDefsHashSet = [.. wornApparelDefsHashSet];
                wornInappropriateApparelDefsHashSet.ExceptWith(requiredApparelDefsHashSet);
                wornInappropriateApparelDefsHashSet.ExceptWith(optionalApparelDefsHashSet);
                if (wornInappropriateApparelDefsHashSet.Count > 0)
                {
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("MousekinRace_Thought_InappropriateApparelWorn".Translate());
                    foreach (ThingDef def in wornInappropriateApparelDefsHashSet)
                    {
                        stringBuilder.AppendLine("- " + def.LabelCap);
                    }
                }

                // Required apparel missing
                HashSet<ThingDef> missingRequiredApparelDefsHashSet = [.. requiredApparelDefsHashSet];
                missingRequiredApparelDefsHashSet.ExceptWith(wornApparelDefsHashSet);
                if (missingRequiredApparelDefsHashSet.Count > 0)
                {
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("MousekinRace_Thought_RequiredApparelMissing".Translate());
                    foreach (ThingDef def in missingRequiredApparelDefsHashSet)
                    {
                        stringBuilder.AppendLine("- " + def.LabelCap);
                    }
                }

                return description.Formatted(p.kindDef.LabelCap) + "\n" + stringBuilder;
            }
        }

        public override ThoughtState CurrentStateInternal(Pawn pawn)
        {
            // Skip pawns who don't wear apparel at all, or whose kinddefs lack the apparel expectations mod extension
            if (pawn.apparel == null || pawn.kindDef.GetModExtension<ApparelExpectationExtension>() is not ApparelExpectationExtension apparelExpectationExtension)
            {
                return ThoughtState.Inactive;
            }
            else
            {
                List<ThingDef> requiredApparelDefs = pawn.kindDef.apparelRequired;
                List<ThingDef> optionalApparelDefs = apparelExpectationExtension.optionalApparel;
                List<ThingDef> wornApparelDefs = [.. pawn.apparel.WornApparel.Select(apparel => apparel.def)];

                // Convert to HashSets for better performance
                HashSet<ThingDef> requiredApparelDefsHashSet = [.. requiredApparelDefs];
                HashSet<ThingDef> optionalApparelDefsHashSet = [.. optionalApparelDefs];
                HashSet<ThingDef> wornApparelDefsHashSet = [.. wornApparelDefs];

                // If the pawn is wearing all apparel required by their pawnkind definition, and no apparel outside of approved optional items
                if (requiredApparelDefsHashSet.IsSubsetOf(wornApparelDefsHashSet) && wornApparelDefsHashSet.All(def => requiredApparelDefsHashSet.Contains(def) || optionalApparelDefsHashSet.Contains(def)))
                {
                    return ThoughtState.ActiveAtStage(1);
                }
                else
                {
                    return ThoughtState.ActiveAtStage(0);
                }
            }
        }

        public bool ApparelIsInList(Apparel apparel, List<ThingDef> list)
        {
            return list.Contains(apparel.def);
        }
    }
}
