using RimWorld;
using System.Collections.Generic;
using Verse.AI.Group;
using Verse;

namespace MousekinRace
{
    public class StageEndTrigger_AllArrived : StageEndTrigger
    {
        public string roleId;
        
        public override Trigger MakeTrigger(LordJob_Ritual ritual, TargetInfo spot, IEnumerable<TargetInfo> foci, RitualStage stage)
        {
            LordJob_Ritual_FlowerDance flowerDance = ritual as LordJob_Ritual_FlowerDance;
            Trigger_Custom trigger = new(delegate (TriggerSignal signal)
            {
                foreach (var pawn in flowerDance.assignments.AssignedPawns(roleId))
                {
                    if (!flowerDance.PawnTagSet(pawn, "Arrived")) 
                    { 
                        return false;
                    }
                }
                return true;
            });
            return trigger;
        }
    }
}
