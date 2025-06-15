using RimWorld;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    public class Dialog_AllegianceRenamePlayerFaction : Dialog_GiveName
    {
        public Faction allegianceFaction;
        
        public override int FirstCharLimit => 64;

        public Dialog_AllegianceRenamePlayerFaction()
        {
            allegianceFaction = GameComponent_Allegiance.Instance.alignedFaction;
            suggestingPawn = allegianceFaction.leader;
            RulePackDef newFactionNameMaker = allegianceFaction.def.GetModExtension<AlliableFactionExtension>().playerFactionNameMaker ?? Faction.OfPlayer.def.factionNameMaker;
            nameGenerator = () => NameGenerator.GenerateName(newFactionNameMaker, IsValidName);
            curName = nameGenerator();
            nameMessageKey = "MousekinRace_AllegianceSys_RenameFactionDialog_Message";
            gainedNameMessageKey = "MousekinRace_MessageFactionRenamedViaAllegiance";
            invalidNameMessageKey = "PlayerFactionNameIsInvalid";
        }

        public override void DoWindowContents(Rect rect)
        {
            Text.Font = GameFont.Small;
            bool flag = false;
            if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter))
            {
                flag = true;
                Event.current.Use();
            }
            Rect rect2;

            TaggedString factionLeaderNameAndTitle = "MousekinRace_AllegianceSys_LeaderTitleFullName".Translate(allegianceFaction.LeaderTitle, suggestingPawn.NameFullColored);
            TaggedString membershipToFactionLabel = AllegianceSys_Utils.MembershipToFactionLabel(allegianceFaction, true);
            Widgets.Label(new Rect(0f, 0f, rect.width, rect.height), nameMessageKey.Translate(factionLeaderNameAndTitle, membershipToFactionLabel));
            if (nameGenerator != null && Widgets.ButtonText(new Rect(rect.width / 2f + 90f, 80f, rect.width / 2f - 90f, 35f), "Randomize".Translate()))
            {
                curName = nameGenerator();
            }
            curName = Widgets.TextField(new Rect(0f, 80f, rect.width / 2f + 70f, 35f), curName, FirstCharLimit);
            rect2 = new Rect(rect.width / 2f + 90f, rect.height - 35f, rect.width / 2f - 90f, 35f);

            if (!(Widgets.ButtonText(rect2, "OK".Translate()) || flag))
            {
                return;
            }

            string newPlayerFactionName = curName?.Trim();
            if (IsValidName(newPlayerFactionName))
            {
                Named(newPlayerFactionName);
                Messages.Message(gainedNameMessageKey.Translate(newPlayerFactionName, membershipToFactionLabel), MessageTypeDefOf.TaskCompletion, historical: false);
                Find.WindowStack.TryRemove(this);
            }
            else
            {
                Messages.Message(invalidNameMessageKey.Translate(), MessageTypeDefOf.RejectInput, historical: false);
            }
            Event.current.Use();
        }

        public override bool IsValidName(string s)
        {
            return NamePlayerFactionDialogUtility.IsValidName(s);
        }

        public override void Named(string s)
        {
            NamePlayerFactionDialogUtility.Named(s);
        }
    }
}
