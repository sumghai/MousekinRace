using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace MousekinRace
{
    public class LordJob_Joinable_ChurchService : LordJob_Joinable_Gathering
    {
        public const float DurationHours = 2f;

        public virtual ThoughtDef AttendeeThought => MousekinDefOf.Mousekin_Thought_ChurchAttendedService;

        public virtual ThoughtDef MissedThought => MousekinDefOf.Mousekin_Thought_ChurchMissedService;

        public virtual ThoughtDef OrganizerThought => MousekinDefOf.Mousekin_Thought_ChurchHeldService;

        public override bool AllowStartNewGatherings => false;

        public override bool OrganizerIsStartingPawn => true;

        public LordJob_Joinable_ChurchService()
        { 
        }

        public LordJob_Joinable_ChurchService(IntVec3 spot, Pawn organizer, GatheringDef gatheringDef)
          : base(spot, organizer, gatheringDef)
        {
            durationTicks = (int)(DurationHours * GenDate.TicksPerHour);
        }

        public bool IsGuest(Pawn p)
        {
            if (!p.RaceProps.Humanlike)
            {
                return false;
            }
            if (p == organizer)
            {
                return false;
            }
            if (p.Faction == organizer.Faction)
            {
                return false;
            }
            if (!GatheringsUtility.ShouldGuestKeepAttendingGathering(p))
            {
                return false;
            }

            return true;
        }

        public override float VoluntaryJoinPriorityFor(Pawn p)
        {
            // Priests and all worshippers must attend
            if (p == organizer || ChurchService_Utils.GetMousekinPotentialWorshippers(p.Map).Contains(p)) 
            {
                return 100f;
            }

            // Non-Apostate Mousekin guests can attend socially
            if (IsGuest(p))
            {
                if (p.IsMousekin() && p.story.traits.DegreeOfTrait(MousekinDefOf.Mousekin_TraitSpectrum_Faith) != 1)
                {
                    return VoluntarilyJoinableLordJobJoinPriorities.SocialGathering;
                }
            }

            // Humans, Apostate Mousekins and non-Mousekins in general should not attend at all
            return 0f;
        }

        public override LordToil CreateGatheringToil(IntVec3 spot, Pawn organizer, GatheringDef gatheringDef)
        {
            return new LordToil_ChurchService(spot, gatheringDef, organizer);
        }

        public override Trigger_TicksPassed GetTimeoutTrigger()
        {
            return new Trigger_TicksPassedAfterConditionMet(durationTicks, () => GatheringsUtility.InGatheringArea(organizer.Position, spot, organizer.Map), 60);
        }

        public override StateGraph CreateGraph()
        {
            StateGraph stateGraph = new();
            LordToil churchServiceToil = CreateGatheringToil(spot, organizer, gatheringDef);
            stateGraph.AddToil(churchServiceToil);
            LordToil_End lordToil_End = new();
            stateGraph.AddToil(lordToil_End);

            Transition transition = new (churchServiceToil, lordToil_End);
            transition.AddTrigger(new Trigger_TickCondition(ShouldBeCalledOff));
            transition.AddTrigger(new Trigger_PawnKilled());
            transition.AddTrigger(new Trigger_PawnLost(PawnLostCondition.LeftVoluntarily, organizer));
            transition.AddPreAction(new TransitionAction_Custom((Action)delegate
            {
                ApplyOutcome((LordToil_ChurchService)churchServiceToil);
            }));
            transition.AddPreAction(new TransitionAction_Message(gatheringDef.calledOffMessage, MessageTypeDefOf.NegativeEvent, new TargetInfo(spot, Map)));
            stateGraph.AddTransition(transition);

            timeoutTrigger = GetTimeoutTrigger();
            Transition transition2 = new(churchServiceToil, lordToil_End);
            transition2.AddTrigger(timeoutTrigger);
            transition2.AddPreAction(new TransitionAction_Custom((Action)delegate
            {
                ApplyOutcome((LordToil_ChurchService)churchServiceToil);
            }));
            transition2.AddPreAction(new TransitionAction_Message(gatheringDef.finishedMessage, MessageTypeDefOf.PositiveEvent, new TargetInfo(spot, Map)));
            stateGraph.AddTransition(transition2);

            return stateGraph;
        }

        public virtual void ApplyOutcome(LordToil_ChurchService churchServiceToil)
        {
            LordToilData_ChurchService lordToilData_Gathering = churchServiceToil.data as LordToilData_ChurchService;
            Building churchAltar = lordToilData_Gathering.churchAltar;

            List<Pawn> attendeePawns = new();
            List<Pawn> absentPawns = new();

            // Apply the appropriate memory to priests and worshippers who attended the church service,
            // as well as those who did not join the church service in time before it ended
            //
            // Worshippers will also get a custom work frenzy inspiration
            foreach (Pawn pawn in lord.ownedPawns)
            {
                bool isOrganiser = pawn == organizer;
                ThoughtDef thoughtDef;
                if (lordToilData_Gathering.presentForTicks.TryGetValue(pawn, out var value) && value > 0)
                {
                    attendeePawns.Add(pawn);
                    thoughtDef = isOrganiser ? OrganizerThought : AttendeeThought;
                }
                else
                {
                    absentPawns.Add(pawn);
                    thoughtDef = MissedThought;
                }

                pawn.needs.mood?.thoughts.memories.TryGainMemory(thoughtDef);

                if (!isOrganiser)
                {
                    pawn.mindState.inspirationHandler.TryStartInspiration(MousekinDefOf.Mousekin_Inspiration_FrenzyWork, sendLetter: false);
                }
            }

            // Send letter summarizing outcome of sermon and calculate tithe amount
            Find.LetterStack.ReceiveLetter("MousekinRace_Letter_ChurchServiceConcluded".Translate(), ChurchService_Utils.GetSummaryAndTitheAmount(organizer, attendeePawns, absentPawns, out int titheAmount), LetterDefOf.NeutralEvent);

            // Spawn silver at altar
            Thing t = ThingMaker.MakeThing(ThingDefOf.Silver);
            t.stackCount = titheAmount;
            GenPlace.TryPlaceThing(t, churchAltar.positionInt, Map, ThingPlaceMode.Direct);
        }
    }
}