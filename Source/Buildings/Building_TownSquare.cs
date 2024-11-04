using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using Verse;
using Verse.AI.Group;

namespace MousekinRace
{
    public class Building_TownSquare : Building
    {
        public CompTownSquare compTownSquare;
        
        public IntVec3 centerCellPos;

        private int count;
        
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            GameComponent_Allegiance.Instance.RecacheTownSquares();
            GameComponent_Allegiance.Instance.ShowAllegianceSysIntroLetterFirstTime();
            compTownSquare = GetComp<CompTownSquare>();
            centerCellPos = base.Position;
            centerCellPos.x += compTownSquare.Props.squareCenterOffset.x;
            centerCellPos.z += compTownSquare.Props.squareCenterOffset.z;
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            base.DeSpawn(mode);
            GameComponent_Allegiance.Instance.RecacheTownSquares();
        }

        // Makes the Town Square the preferred gathering spot for trade caravans and visitors
        // Based on [KV] Trading Spot
        public override void Tick()
        {
            base.Tick();

            if (count++ <= 1)
            {
                return;
            }

            count = 0;

            List<Lord> lords = base.Map.lordManager.lords;

            for (int i = 0; i < lords.Count; i = checked(i + 1))
            {
                Lord lord = lords[i];
                
                if (!(lord.LordJob is LordJob_TradeWithColony) && !CheckVisitor(lord.LordJob))
                {
                    continue;
                }

                FieldInfo field = lord.LordJob.GetType().GetField("chillSpot", BindingFlags.Instance | BindingFlags.NonPublic);
                IntVec3 intVec = (IntVec3)field.GetValue(lord.LordJob);
                
                if (intVec.x != centerCellPos.x || intVec.y != centerCellPos.y || intVec.z != centerCellPos.z)
                {
                    field.SetValue(lord.LordJob, centerCellPos);
                }
                
                LordToil curLordToil = lord.CurLordToil;
                
                if (curLordToil is LordToil_Travel)
                {
                    LordToil_Travel lordToil_Travel = (LordToil_Travel)curLordToil;
                    if (lordToil_Travel.FlagLoc != centerCellPos)
                    {
                        lordToil_Travel.SetDestination(centerCellPos);
                        lordToil_Travel.UpdateAllDuties();
                    }
                }
                else if (curLordToil is LordToil_DefendPoint)
                {
                    LordToil_DefendPoint lordToil_DefendPoint = (LordToil_DefendPoint)curLordToil;
                    if (lordToil_DefendPoint.FlagLoc != centerCellPos)
                    {
                        lordToil_DefendPoint.SetDefendPoint(centerCellPos);
                        lordToil_DefendPoint.UpdateAllDuties();
                    }
                }
                
                // Maintain visiting faction animals' food needs
                foreach (Pawn ownedPawn in lord.ownedPawns)
                {
                    if (ownedPawn.RaceProps.Animal && ownedPawn.needs != null && ownedPawn.needs.food != null && ownedPawn.needs.food.CurLevel <= ownedPawn.needs.food.MaxLevel)
                    {
                        ownedPawn.needs.food.CurLevel = ownedPawn.needs.food.MaxLevel;
                    }
                }
            }
        }

        private bool CheckVisitor(LordJob lordJob)
        {
            if (ModCompatibility.OrionHospitalityIsActive) 
            { 
                return false;
            }
            if (lordJob is LordJob_VisitColony) 
            { 
                return true;
            }
            return false;
        }
    }
}
