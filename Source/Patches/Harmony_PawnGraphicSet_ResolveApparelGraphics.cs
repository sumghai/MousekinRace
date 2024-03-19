using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MousekinRace
{
    // TODO - Fix
    
    // After resolving a pawn's apparel graphics, we conditionally render:
    // - Any headgear attached to apparel (e.g. hoods, masks, helmets) depending on the Gizmo state in CompApparelWithAttachedHeadgear
    /*[HarmonyPatch(typeof(PawnGraphicSet), nameof(PawnGraphicSet.ResolveApparelGraphics))]
    public static class Harmony_PawnGraphicSet_ResolveApparelGraphics
    {
        static void Postfix(Pawn ___pawn)
        {
            using (List<Apparel>.Enumerator enumerator = ___pawn.apparel.WornApparel.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.TryGetComp<CompApparelWithAttachedHeadgear>() is CompApparelWithAttachedHeadgear comp)
                    {
                        Apparel hoodApparel = (Apparel)ThingMaker.MakeThing(comp.Props.attachedHeadgearDef, enumerator.Current.Stuff);

                        hoodApparel.DrawColor = enumerator.Current.DrawColor;

                        ApparelGraphicRecordGetter.TryGetGraphicApparel(hoodApparel, ___pawn.story.bodyType, out ApparelGraphicRecord hoodApparelGraphicRecord);

                        if (comp.isHatOn)
                        {
                            ___pawn.Drawer.renderer.graphics.apparelGraphics.Add(hoodApparelGraphicRecord);
                        }
                        else
                        {
                            ___pawn.Drawer.renderer.graphics.apparelGraphics.RemoveAll(x => x.sourceApparel == hoodApparel);
                        }
                    }
                }
            }
        }
    }*/
}