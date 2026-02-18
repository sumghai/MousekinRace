using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MousekinRace
{
    // Royalty DLC Royal Ascent ending
    // If there are any Mousekins among the departing colonists, append a blurb revealing
    // what the Empire *really* thinks of the "talking mice"
    [HarmonyPatch(typeof(QuestPart_EndGame), nameof(QuestPart_EndGame.Notify_QuestSignalReceived))]
    public static class Harmony_QuestPart_EndGame_Notify_QuestSignalReceived_AddMousekinSuffixToEndingText
    {
        static void Prefix(Signal signal, QuestPart_EndGame __instance)
        {
            if (__instance.quest.root == MousekinDefOf.EndGame_RoyalAscent && signal.tag == __instance.inSignal)
            {
                signal.args.TryGetArg<List<Pawn>>("SENTCOLONISTS", out List<Pawn> sentColonists);

                if (sentColonists.Count > 0 && sentColonists.Any(p => p.IsMousekin()))
                {
                    __instance.endingText += "\n\n\n" + "MousekinRace_GameOver_RoyalAscent_DeparteesIncludeMice_EndingTextSuffix".Translate();
                }
            }
        }
    }
}
