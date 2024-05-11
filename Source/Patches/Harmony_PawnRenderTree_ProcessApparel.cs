using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MousekinRace
{
    [HarmonyPatch(typeof(PawnRenderTree), nameof(PawnRenderTree.ProcessApparel))]
    public static class Harmony_PawnRenderTree_ProcessApparel_AddHoodNode
    {
        static void Postfix(PawnRenderTree __instance, Apparel ap)
        {
            if (ap.comps.OfType<CompApparelWithAttachedHeadgear>().FirstOrDefault() is CompApparelWithAttachedHeadgear comp && comp.CompRenderNodes() is List<PawnRenderNode> renderNodes)
            {
                foreach(PawnRenderNode node in renderNodes) 
                {
                    if (__instance.ShouldAddNodeToTree(node?.Props))
                    {
                        __instance.AddChild(node, null);
                    }
                }
            }
        }
    }
}
