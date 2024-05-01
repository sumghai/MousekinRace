using RimWorld;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    public class Dialog_AllegianceExtraInfo : Window
    {
        public TaggedString title;
        public TaggedString contents;

        public override Vector2 InitialSize => new Vector2(800f, 800f);

        public Vector2 scrollPosition = Vector2.zero;

        public Dialog_AllegianceExtraInfo(TaggedString title, TaggedString contents)
        {
            forcePause = true;
            doCloseX = true;
            doCloseButton = true;
            closeOnClickedOutside = true;
            absorbInputAroundWindow = true;
            this.title = title;
            this.contents = contents;
        }
        
        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Medium;
            Rect titleRect = new Rect(inRect.xMin, inRect.yMin, inRect.width, Text.CalcHeight(title, inRect.width));
            Widgets.Label(titleRect, title);

            Text.Font = GameFont.Small;
            Rect scrollableAreaRect = new Rect(inRect.xMin, titleRect.yMax + StandardMargin, inRect.width, inRect.height - titleRect.height - StandardMargin - FooterRowHeight);
            float scrollableContentsWidth = scrollableAreaRect.width - 16f;
            Rect scrollableContentsRect = new Rect(0f, 0f, scrollableContentsWidth, Text.CalcHeight(contents, scrollableContentsWidth));
            Widgets.BeginScrollView(scrollableAreaRect, ref scrollPosition, scrollableContentsRect);
            Widgets.Label(scrollableContentsRect, contents);
            Widgets.EndScrollView();
        }

        public static TaggedString GenerateBenefitsDesc(Faction allegianceFaction) 
        {
            TaggedString descBody = new();

            // todo - populate with content

            return descBody;
        }

        public static TaggedString GenerateCostsDesc(Faction allegianceFaction)
        {
            TaggedString descBody = new();

            if (ModsConfig.IdeologyActive)
            {
                descBody += "- " + "MousekinRace_AllegianceSys_ViewExtraInfoDialog_PartIdeoChange".Translate(allegianceFaction.ideos.PrimaryIdeo.ToString().Colorize(allegianceFaction.ideos.PrimaryIdeo.Color)) + "\n";
            }

            // todo - populate with content

            return descBody;
        }
    }
}
