using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    public class Page_IdeoFixedByScenario : Page
    {
        public override string PageTitle => "MousekinRace_Page_IdeoFixedByScenario_Title".Translate();

        public Ideo tgtIdeo;

        public override void PostOpen()
        {
            base.PostOpen();
            Find.IdeoManager.RemoveUnusedStartingIdeos();
            tgtIdeo = GetFixedIdeo();
            Faction.OfPlayer.ideos.SetPrimary(tgtIdeo);
            foreach (Ideo item in Find.IdeoManager.IdeosListForReading)
            {
                item.initialPlayerIdeo = false;
            }
        }

        public override void DoWindowContents(Rect inRect)
        {
            DrawPageTitle(inRect);
            float num = 45f; // Height of DrawPageTitle() block
            Rect mainRect = GetMainRect(inRect);

            TaggedString taggedDescString = "MousekinRace_Page_IdeoFixedByScenario_Desc".Translate(Current.Game.Scenario.ToString());
            float num2 = Text.CalcHeight(taggedDescString, mainRect.width);
            Rect descRect = mainRect;
            Widgets.Label(descRect, taggedDescString);

            num += num2 + 10f;
            IdeoUIUtility.DoNameAndSymbol(ref num, mainRect.width, tgtIdeo, IdeoEditMode.None);

            Texture2D ideoBannerImg = ContentFinder<Texture2D>.Get("UI/Ideoligions/MousekinKingdomIdeoBanner");
            Rect ideoBannerImgRect = new Rect(mainRect.xMin, num, mainRect.width, ideoBannerImg.height * (mainRect.width / ideoBannerImg.width));
            GUI.DrawTexture(ideoBannerImgRect, ideoBannerImg);

            num += ideoBannerImgRect.height + 10f;
            TaggedString taggedButtonString = "ClickForMoreInfo".Translate().Replace(".", "").Replace("。", "");
            float ideoInfoButtonWidth = Math.Max(Page.BottomButSize.x, Text.CalcSize(taggedButtonString).x + (Page.BottomButSize.y - Text.CalcSize(taggedButtonString).y));
            Rect ideoInfoButtonRect = new Rect((mainRect.width - ideoInfoButtonWidth) / 2, num, ideoInfoButtonWidth, Page.BottomButSize.y);
            if (Widgets.ButtonText(ideoInfoButtonRect, taggedButtonString))
            {
                Find.WindowStack.Add(new Dialog_IdeosDuringLanding());
            }

            DoBottomButtons(inRect);
        }

        public override bool CanDoNext()
        {
            if (!base.CanDoNext())
            {
                return false;
            }
            tgtIdeo.initialPlayerIdeo = true;
            Find.Scenario.PostIdeoChosen();
            return true;
        }

        // Get the fixed ideo for the scenario
        public Ideo GetFixedIdeo()
        {
            // Find the (first) faction required by the scenario with the ideo to be used by the player
            IEnumerable<ScenPart_RequiredFaction> scenParts = Current.Game.Scenario.AllParts.OfType<ScenPart_RequiredFaction>();
            FactionDef tgtFactionDef = scenParts.FirstOrDefault(scenPart => scenPart.useFactionIdeoForPlayer == true).factionDef;
            Faction tgtFaction = Find.FactionManager.FirstFactionOfDef(tgtFactionDef);

            // Get the faction's ideo
            return tgtFaction.ideos.PrimaryIdeo;
        }
    }
}
