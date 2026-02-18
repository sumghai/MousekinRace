using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    public class Dialog_AllegianceExtraInfo : Window
    {
        public TaggedString title;
        public TaggedString contents;
        public List<ThingDef> unlockedCraftableThingDefs = new();

        public override Vector2 InitialSize => new Vector2(800f, 800f);

        public Vector2 scrollPosition = Vector2.zero;

        public Dialog_AllegianceExtraInfo(TaggedString title, TaggedString contents, List<ThingDef> unlockedCraftableThingDefs = null)
        {
            forcePause = true;
            doCloseX = true;
            doCloseButton = true;
            closeOnClickedOutside = true;
            absorbInputAroundWindow = true;
            this.title = title;
            this.contents = contents;
            this.unlockedCraftableThingDefs = unlockedCraftableThingDefs;
        }
        
        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Medium;
            Rect titleRect = new Rect(inRect.xMin, inRect.yMin, inRect.width, Text.CalcHeight(title, inRect.width));
            Widgets.Label(titleRect, title);

            Text.Font = GameFont.Small;
            Rect scrollableAreaRect = new Rect(inRect.xMin, titleRect.yMax + StandardMargin, inRect.width, inRect.height - titleRect.height - StandardMargin - FooterRowHeight);

            float scrollableContentsWidth = scrollableAreaRect.width - 16f;
            float textBlockHeight = Text.CalcHeight(contents, scrollableContentsWidth);

            float craftableItemsHyperlinksY = inRect.yMin + textBlockHeight;
            float craftableItemsHyperlinkRowHeight = 24f;
            Rect craftableItemsHyperlinksRect = new Rect(0f, craftableItemsHyperlinksY, scrollableContentsWidth, 0f);
            if (unlockedCraftableThingDefs != null && unlockedCraftableThingDefs.Count > 0)
            {
                craftableItemsHyperlinksRect.height = craftableItemsHyperlinkRowHeight * unlockedCraftableThingDefs.Count;
            }
            Rect scrollableContentsRect = new Rect(0f, 0f, scrollableContentsWidth, textBlockHeight + craftableItemsHyperlinksRect.height);

            Widgets.BeginScrollView(scrollableAreaRect, ref scrollPosition, scrollableContentsRect);
            Widgets.Label(scrollableContentsRect, contents);
            if (unlockedCraftableThingDefs != null && unlockedCraftableThingDefs.Count > 0)
            {
                foreach (ThingDef def in unlockedCraftableThingDefs)
                {
                    Dialog_InfoCard.Hyperlink hyperlink = new Dialog_InfoCard.Hyperlink(def);
                    Widgets.HyperlinkWithIcon(new Rect(0f, craftableItemsHyperlinksY, scrollableContentsWidth, craftableItemsHyperlinkRowHeight), hyperlink);
                    craftableItemsHyperlinksY += craftableItemsHyperlinkRowHeight;
                }
            }
            Widgets.EndScrollView();
        }
    }
}
