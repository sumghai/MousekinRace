using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI.Group;

namespace MousekinRace
{
    public class LordJob_Ritual_HereticExecution : LordJob_Ritual
    {
        public Pawn moralist;
        public List<Pawn> heretics = [];
        public List<Pawn> escorts = [];
        public ThingDef stakeMoteDef;
        public List<IntVec3> tmpStakePositions = [];
        public Dictionary<IntVec3, Mote> cachedStakePositions = [];

        public LordJob_Ritual_HereticExecution()
        { 
        }

        public LordJob_Ritual_HereticExecution(TargetInfo selectedTarget, Precept_Ritual ritual, RitualObligation obligation, List<RitualStage> allStages, RitualRoleAssignments assignments, Pawn organizer = null, IntVec3? spotOverride = null)
        : base(selectedTarget, ritual, obligation, allStages, assignments, organizer, spotOverride)
        {
            foreach (RitualRole role in assignments.AllRolesForReading)
            {
                if (role != null)
                {
                    if (role.id.Contains("mousekinHeretic"))
                    {
                        foreach (Pawn pawn in assignments.AssignedPawns(role))
                        {
                            pawn.guilt?.Notify_Guilty();
                            heretics.Add(pawn);
                            pawnsDeathIgnored.Add(pawn);
                        }
                    }

                    if (role.id.Contains("mousekinEscort"))
                    {
                        foreach (Pawn pawn in assignments.AssignedPawns(role))
                        {
                            escorts.Add(pawn);
                        }
                    }
                }
            }

            stakeMoteDef = (ritual.behavior.def.stages.SelectMany(stage => stage.roleBehaviors).SelectMany(behavior => behavior.customPositions).First(pos => pos.GetType() == typeof(RitualPosition_HereticExecutionStakes)) as RitualPosition_HereticExecutionStakes).stakeMoteDef;
        }

        public override void LordJobTick()
        {
            base.LordJobTick();

            if (!cachedStakePositions.Any() && tmpStakePositions.Any())
            {
                foreach (IntVec3 pos in tmpStakePositions)
                {
                    Mote mote = MoteMaker.MakeStaticMote(pos, Map, stakeMoteDef);
                    cachedStakePositions.Add(pos, mote);
                }
            }
            else 
            {
                foreach (var pos in cachedStakePositions)
                {
                    if (pos.Value.Spawned) 
                    {
                        pos.Value.Maintain();
                    }                   
                }
            }
        }

        public override void Notify_PawnLost(Pawn p, PawnLostCondition condition)
        {
            if (p.Dead && heretics.Contains(p))
            {
                p.health.killedByRitual = true;
                if (cachedStakePositions.First(pos => pos.Key == p.Position).Value is Mote stakeMote)
                {
                    stakeMote.DeSpawn();
                }
            }
        }

        public override void Cleanup()
        {
            base.Cleanup();
            foreach (var pos in cachedStakePositions.Where(x => x.Value.Spawned))
            { 
                pos.Value.DeSpawn();
            }
            tmpStakePositions.Clear();
            cachedStakePositions.Clear();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref heretics, "heretics", LookMode.Reference);
            Scribe_Collections.Look(ref escorts, "escorts", LookMode.Reference);
        }
    }
}
