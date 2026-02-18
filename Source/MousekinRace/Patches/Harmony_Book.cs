using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using Verse;

namespace MousekinRace
{
    // If a book def contains a special mod extension that supports multiple Name and Description Makers by genre,
    // pick a random Name/Description pair to generate the book's contents with
    [HarmonyPatch(typeof(Book), nameof(Book.GenerateBook))]
    public static class Harmony_Book_GenerateBook_ChooseFromMultipleNameDescPairs
    {
        static RulePackDef storedRandDescMaker;
        
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codeMatcher = new CodeMatcher(instructions);

            CodeMatch[] toMatch =
            [
                new CodeMatch(OpCodes.Call),
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Call),
                new CodeMatch(OpCodes.Callvirt),
                new CodeMatch(OpCodes.Ldfld)
            ];

            CodeInstruction[] toInsert_condReplaceNameMaker =
            [
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Harmony_Book_GenerateBook_ChooseFromMultipleNameDescPairs), nameof(ConditionallyReplaceNameMaker)))
            ];

            CodeInstruction[] toInsert_condReplaceDescMaker =
            [
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Harmony_Book_GenerateBook_ChooseFromMultipleNameDescPairs), nameof(ConditionallyReplaceDescMaker)))
            ];

            // Conditionally replace name maker
            codeMatcher.MatchEndForward(toMatch);
            codeMatcher.InsertAfter(toInsert_condReplaceNameMaker);

            // Conditionally replace description maker
            codeMatcher.MatchEndForward(toMatch);
            codeMatcher.InsertAfter(toInsert_condReplaceDescMaker);

            return codeMatcher.InstructionEnumeration();
        }

        private static RulePackDef ConditionallyReplaceNameMaker(RulePackDef originalNameMaker, Book book)
        {
            if (book.def.HasModExtension<BookNameDescByGenreExtension>() && !book.def.GetModExtension<BookNameDescByGenreExtension>().nameDescPairs.NullOrEmpty())
            {
                BookNameDescPair randBookNameDescPair = book.def.GetModExtension<BookNameDescByGenreExtension>().nameDescPairs.RandomElementByWeight(x => x.weight);
                storedRandDescMaker = randBookNameDescPair.descriptionMaker;
                return randBookNameDescPair.nameMaker;
            }

            return originalNameMaker;
        }

        private static RulePackDef ConditionallyReplaceDescMaker(RulePackDef originalDescMaker, Book book)
        {
            if (book.def.HasModExtension<BookNameDescByGenreExtension>() && !book.def.GetModExtension<BookNameDescByGenreExtension>().nameDescPairs.NullOrEmpty() && storedRandDescMaker != null)
            {
                return storedRandDescMaker;
            }

            return originalDescMaker;
        }
    }
}
