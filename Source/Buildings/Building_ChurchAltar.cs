using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace MousekinRace
{
    public class Building_ChurchAltar : Building
    {
        public List<(PawnKindDef, string)> recruitableClergyPawns = new List<(PawnKindDef, string)>
        {
            (MousekinDefOf.MousekinPriest, "UI/Icons/AllegianceSystem/RecruitMousekinPriest"),
            (MousekinDefOf.MousekinNun, "UI/Icons/AllegianceSystem/RecruitMousekinNun")
        };

        public override void Notify_ThingSelected()
        {
            base.Notify_ThingSelected();
            GameComponent_Allegiance.Instance.RecacheAvailableSilver();
        }

        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(base.GetInspectString());

            if (!GameComponent_Allegiance.Instance.recruitedColonistsQueue.Empty()) 
            {
                RecruitedPawnGroup clergyPawnGroup = GameComponent_Allegiance.Instance.recruitedColonistsQueue.FirstOrDefault(x => x.pawns.FirstOrDefault().kindDef == MousekinDefOf.MousekinPriest || x.pawns.FirstOrDefault().kindDef == MousekinDefOf.MousekinNun);

                if (clergyPawnGroup != null)
                {
                    stringBuilder.Append("\n" + "MousekinRace_ChurchAltar_NewClergyArrivalTime".Translate(clergyPawnGroup.pawns.First().KindLabel.Replace(MousekinDefOf.Mousekin.label, "").Trim().CapitalizeFirst(), (clergyPawnGroup.spawnTick - Find.TickManager.TicksGame).ToStringTicksToPeriod()));
                }
            }

            return stringBuilder.ToString();
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo c in base.GetGizmos())
            {
                yield return c;
            }

            // Recruit buttons for Priests and Nuns
            foreach ((PawnKindDef pawnKindDef, string buttonIconPath) in recruitableClergyPawns)
            {
                string pawnKindLabel = pawnKindDef.label.Replace(MousekinDefOf.Mousekin.label, "").Trim().CapitalizeFirst();

                float silverCost;

                float silverCostNoAllegianceOverhead = 500f;

                if (GameComponent_Allegiance.Instance.HasDeclaredAllegiance)
                {
                    silverCost = GameComponent_Allegiance.Instance.alignedFaction.def.GetModExtension<AlliableFactionExtension>().recruitableColonistSettings.options.FirstOrDefault(o => o.pawnKind == pawnKindDef).basePrice;
                }
                else
                { 
                    silverCost = MousekinDefOf.Mousekin_FactionKingdom.GetModExtension<AlliableFactionExtension>().recruitableColonistSettings.options.FirstOrDefault(o => o.pawnKind == pawnKindDef).basePrice + silverCostNoAllegianceOverhead;
                }

                yield return new Command_Action
                {
                    defaultLabel = "MousekinRace_CommandInviteLabel".Translate(pawnKindLabel),
                    defaultDesc = "MousekinRace_CommandInviteDesc".Translate(pawnKindLabel, silverCost),
                    icon = ContentFinder<Texture2D>.Get(buttonIconPath, true),
                    iconDrawColorOverride = GameComponent_Allegiance.Instance.alignedFaction?.def.DefaultColor ?? Color.white,
                    iconOffset = new(0.1f, 0f),
                    iconDrawScale = 1.25f,
                    disabled = GameComponent_Allegiance.Instance.availableSilver < silverCost,
                    disabledReason = "NotEnoughSilver".Translate().CapitalizeFirst(),
                    action = delegate
                    {
                        Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("MousekinRace_AllegianceSys_Recruit_Confirmation".Translate("IndefiniteForm".Translate(pawnKindLabel), "", silverCost), delegate 
                        {
                            AllegianceSys_Utils.GenerateNewColonistsToQueue(silverCost, pawnKindDef);
                            SoundDefOf.ExecuteTrade.PlayOneShotOnCamera();
                        }));
                    }
                };
            }
        }
    }
}
