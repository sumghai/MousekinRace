﻿using UnityEngine;
using Verse;

namespace MousekinRace
{
    public static class UI_Extensions
    {
        public static void Header(this Listing_Standard list, TaggedString headerTitle) 
        {
            Rect rect = list.GetRect(Text.CalcHeight(headerTitle, list.ColumnWidth));
            Widgets.DrawBoxSolid(rect, Widgets.OptionUnselectedBGFillColor);
            TextAnchor anchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(rect, headerTitle);
            Text.Anchor = anchor;
            list.Gap(4);
        }
    }
}
