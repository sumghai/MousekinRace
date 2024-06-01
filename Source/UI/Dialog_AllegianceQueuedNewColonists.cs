using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    public class Dialog_AllegianceQueuedNewColonists : Window
    {
        public TaggedString title = "MousekinRace_AllegianceSys_Recruit_ViewQueueDialog_Title".Translate();

        public List<RecruitedPawnGroup> recruitedPawnGroups = new();

        public override Vector2 InitialSize => new Vector2(800f, 800f);

        public Vector2 scrollPosition = Vector2.zero;

        public Dialog_AllegianceQueuedNewColonists(List<RecruitedPawnGroup> recruitedPawnGroups)
        {
            forcePause = true;
            doCloseX = true;
            doCloseButton = true;
            closeOnClickedOutside = true;
            absorbInputAroundWindow = true;
            this.recruitedPawnGroups = recruitedPawnGroups;
        }
        
        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Medium;
            Rect titleRect = new Rect(inRect.xMin, inRect.yMin, inRect.width, Text.CalcHeight(title, inRect.width));
            Widgets.Label(titleRect, title);

            Text.Font = GameFont.Small;
            Rect scrollableAreaRect = new Rect(inRect.xMin, titleRect.yMax + StandardMargin, inRect.width, inRect.height - titleRect.height - StandardMargin - FooterRowHeight);

            float scrollableContentsWidth = scrollableAreaRect.width - 16f;
            float pawnGroupRowHeight = 170f;
            float pawnPortraitSetColumnWidthPct = 0.8f;
            float pawnPortraitDrawSize = 100f;
            float pawnPortraitMargin = 10f;
            int pawnGroupRows = recruitedPawnGroups.Count;
            Rect scrollableContentsRect = new Rect(0f, 0f, scrollableContentsWidth, pawnGroupRowHeight * pawnGroupRows);

            Widgets.BeginScrollView(scrollableAreaRect, ref scrollPosition, scrollableContentsRect);
            for (int i = 0; i < pawnGroupRows; i++)
            {
                RecruitedPawnGroup curPawnGroup = recruitedPawnGroups[i];
                
                Rect curRowRect = new Rect(scrollableContentsRect.xMin, scrollableContentsRect.yMin + i * pawnGroupRowHeight, scrollableContentsWidth, pawnGroupRowHeight);
                if (i % 2 == 0)
                {
                    Widgets.DrawAltRect(curRowRect);
                }
                // Pawn group preview column
                // todo - colonist portraits
                for (int j = 0; j < curPawnGroup.pawns.Count; j++)
                {
                    Pawn curPawn = curPawnGroup.pawns[j];
                    Rect curPawnPortraitRect = new Rect(curRowRect.xMin + pawnPortraitMargin + (pawnPortraitDrawSize + pawnPortraitMargin) * (float) j, curRowRect.yMin + pawnPortraitMargin, pawnPortraitDrawSize, pawnPortraitDrawSize);
                    Widgets.DrawRectFast(curPawnPortraitRect, (j % 2 == 0) ? Color.red : Color.blue);

                    Text.Font = GameFont.Tiny;
                    Text.Anchor = TextAnchor.MiddleCenter;
                    Rect curPawnLegendRect = new Rect(curPawnPortraitRect.xMin, curPawnPortraitRect.yMax, curPawnPortraitRect.width, curRowRect.height - curPawnPortraitRect.height - pawnPortraitMargin * 2);
                    TaggedString curPawnLegendText = curPawn.NameFullColored + "\n" + curPawn.story.TitleShort + " (" + curPawn.ageTracker.AgeBiologicalYears.ToString() + ")";
                    Widgets.Label(curPawnLegendRect, curPawnLegendText);
                    Text.Font = GameFont.Small;
                }

                // Arrival time column
                Rect curRowArrivalTimeRect = new Rect(curRowRect.width * pawnPortraitSetColumnWidthPct, curRowRect.yMin, curRowRect.width * (1f - pawnPortraitSetColumnWidthPct), curRowRect.height);
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(curRowArrivalTimeRect, "MousekinRace_AllegianceSys_Recruit_ViewQueueDialog_ArrivalTime".Translate((curPawnGroup.spawnTick - Find.TickManager.TicksAbs).ToStringTicksToPeriod()));
                Text.Anchor = TextAnchor.UpperLeft;
            }
            Widgets.EndScrollView();
        }
    }
}
