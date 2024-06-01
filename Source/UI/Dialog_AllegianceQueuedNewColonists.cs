using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    public class Dialog_AllegianceQueuedNewColonists : Window
    {
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
            var listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.Label("New colonist groups in queue: " + recruitedPawnGroups.Count.ToString());
            listingStandard.End();
        }
    }
}
