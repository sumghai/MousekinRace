using RimWorld;
using System;
using Verse;
using Verse.AI.Group;

namespace MousekinRace
{
    public class LordJob_Joinable_ChurchService : LordJob_Joinable_Gathering
    {
        public const float DurationHours = 2f;

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
            /*StateGraph stateGraph = new();
            LordToil gatheringToil = CreateGatheringToil(spot, organizer, gatheringDef);
            stateGraph.AddToil(gatheringToil);
            LordToil_End lordToilEnd = new();
            stateGraph.AddToil(lordToilEnd);
            float serviceDuration = 8000f; // todo - what does this do?
            Transition transition1 = new(gatheringToil, lordToilEnd, false, true);
            transition1.AddTrigger(new Trigger_TickCondition(new Func<bool>(ShouldBeCalledOff), 1));
            transition1.AddTrigger(new Trigger_PawnKilled());
            transition1.AddTrigger(new Trigger_PawnLost(PawnLostCondition.LeftVoluntarily, organizer));
            stateGraph.AddTransition(transition1, false);
            timeoutTrigger = new Trigger_TicksPassedAfterConditionMet((int)serviceDuration, (() => GatheringsUtility.InGatheringArea(organizer.Position, spot, organizer.Map)), 60);
            Transition transition2 = new Transition(gatheringToil, lordToilEnd, false, true);
            transition2.AddTrigger(timeoutTrigger);
            transition2.AddPreAction(new TransitionAction_Custom(() => ApplyOutcome(1f)));
            stateGraph.AddTransition(transition2, false);
            return stateGraph;*/

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
            transition2.AddPreAction(new TransitionAction_Message(gatheringDef.finishedMessage, MessageTypeDefOf.SituationResolved, new TargetInfo(spot, Map)));
            stateGraph.AddTransition(transition2);

            return stateGraph;
        }

        public virtual void ApplyOutcome(LordToil_ChurchService churchServiceToil)
        {
            // todo - apply various effects on participants
            /* 
                foreach (Pawn ownedPawn in this.lord.ownedPawns)        iterates through all participants

                ownedPawn != this.organizer                             not the priest/organizer

                ownedPawn.needs.mood.thoughts.memories.TryGainMemory()  applying memories to a given participant
             */
        }
    }
}