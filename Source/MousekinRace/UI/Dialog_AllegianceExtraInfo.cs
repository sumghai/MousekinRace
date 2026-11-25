using RimWorld;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    public class Dialog_AllegianceExtraInfo : Window
    {
        public TaggedString title;
        public Faction factionOption;
        public AllegianceExtraInfoMode mode;

        public override Vector2 InitialSize => new Vector2(800f, 800f);

        public float scrollableContentsHeight = 1f; // Placeholder value, will be auto-updated on next frame

        public Vector2 scrollPosition = Vector2.zero;

        public Dialog_AllegianceExtraInfo(TaggedString title, Faction factionOption, AllegianceExtraInfoMode mode)
        {
            forcePause = true;
            doCloseX = true;
            doCloseButton = true;
            closeOnClickedOutside = true;
            absorbInputAroundWindow = true;
            this.title = title;
            this.factionOption = factionOption;
            this.mode = mode;
        }

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Medium;
            Rect titleRect = new(inRect.xMin, inRect.yMin, inRect.width, Text.CalcHeight(title, inRect.width));
            Widgets.Label(titleRect, title);

            Text.Font = GameFont.Small;
            Rect scrollableAreaRect = new(inRect.xMin, titleRect.yMax + StandardMargin, inRect.width, inRect.height - titleRect.height - StandardMargin - FooterRowHeight);

            float scrollableContentsWidth = scrollableAreaRect.width - 16f;

            Rect scrollableContentsRect = new(0f, 0f, scrollableContentsWidth, scrollableContentsHeight);

            Widgets.BeginScrollView(scrollableAreaRect, ref scrollPosition, scrollableContentsRect);

            Rect contents = AllegianceSys_Utils.DrawBenefitCostInfoBlock(scrollableContentsWidth, factionOption, mode);

            scrollableContentsHeight = contents.height;

            Widgets.EndScrollView();
        }
    }
}
