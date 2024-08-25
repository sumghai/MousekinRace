﻿using RimWorld;
using System;
using Verse;
using Verse.AI.Group;

namespace MousekinRace
{
    public class LordJob_Joinable_ChurchService : LordJob_Joinable_Gathering
    {
        public const float DurationHours = 1.5f;

        public override bool AllowStartNewGatherings => false;

        public override bool OrganizerIsStartingPawn => true;

        public LordJob_Joinable_ChurchService()
        { 
        }

        public LordJob_Joinable_ChurchService(IntVec3 spot, Pawn organizer, GatheringDef gatheringDef)
          : base(spot, organizer, gatheringDef)
        {
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
            if (p == organizer) 
            {
                return 100f;
            }

            if (ChurchService_Utils.GetMousekinPotentialWorshippers(p.Map).Contains(p))
            { 
                return VoluntarilyJoinableLordJobJoinPriorities.SocialGathering;
            }

            if (IsGuest(p))
            {
                if (p.IsMousekin() && p.story.traits.DegreeOfTrait(MousekinDefOf.Mousekin_TraitSpectrum_Faith) != 1)
                {
                    return VoluntarilyJoinableLordJobJoinPriorities.SocialGathering;
                }
            }

            return base.VoluntaryJoinPriorityFor(p);
        }

        public override LordToil CreateGatheringToil(IntVec3 spot, Pawn organizer, GatheringDef gatheringDef)
        {
            return new LordToil_ChurchService(spot, gatheringDef, organizer);
        }

        public override StateGraph CreateGraph()
        {
            StateGraph stateGraph = new();
            LordToil gatheringToil = CreateGatheringToil(spot, organizer, gatheringDef);
            stateGraph.AddToil(gatheringToil);
            LordToil_End lordToilEnd = new();
            stateGraph.AddToil(lordToilEnd);
            float serviceDuration = 8000f; // todo - what does this do?
            Transition transition1 = new(gatheringToil, lordToilEnd, false, true);
            transition1.AddTrigger(new Trigger_TickCondition(new Func<bool>(ShouldBeCalledOff), 1));
            transition1.AddTrigger(new Trigger_PawnKilled());
            transition1.AddTrigger(new Trigger_PawnLost(PawnLostCondition.LeftVoluntarily, organizer));
            transition1.AddPreAction(new TransitionAction_Custom((Action)(() => ApplyOutcome((float)lord.ticksInToil / serviceDuration))));
            stateGraph.AddTransition(transition1, false);
            timeoutTrigger = new Trigger_TicksPassedAfterConditionMet((int)serviceDuration, (Func<bool>)(() => GatheringsUtility.InGatheringArea(organizer.Position, spot, organizer.Map)), 60);
            Transition transition2 = new Transition(gatheringToil, lordToilEnd, false, true);
            transition2.AddTrigger(timeoutTrigger);
            transition2.AddPreAction(new TransitionAction_Custom((Action)(() => ApplyOutcome(1f))));
            stateGraph.AddTransition(transition2, false);
            return stateGraph;
        }

        public override string GetReport(Pawn pawn)
        {
            // todo - translate to keyed strings
            if (pawn != organizer)
            {
                return "Attending church service";
            }
            return "Giving sermon";
        }

        public virtual void ApplyOutcome(float progress)
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