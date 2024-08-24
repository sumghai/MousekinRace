using System.Collections.Generic;
using System;
using UnityEngine;
using Verse;
using Verse.AI;
using RimWorld;

namespace MousekinRace
{
    [StaticConstructorOnStartup]
    public class JobDriver_ChurchServiceGiveSermon : JobDriver
    {
        public static readonly IntRange MoteInterval = new IntRange(300, 500);
        public static readonly Texture2D moteImage = ContentFinder<Texture2D>.Get("UI/Ideoligions/FlowerCross", true);
        public const TargetIndex ChurchLecternIndex = TargetIndex.A;
        public const TargetIndex FacingIndex = TargetIndex.B;

        public Building ChurchLectern => TargetThingA as Building;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve((LocalTargetInfo)ChurchLectern, job, 1, -1, null, errorOnFailed);
        }

        public override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            yield return Toils_General.Do(new Action(delegate () 
            { 
                job.SetTarget(TargetIndex.B, (LocalTargetInfo)(ChurchLectern.InteractionCell + ChurchLectern.Rotation.FacingCell)); 
            }));

            Toil toil = new();
            toil.FailOnCannotTouch(TargetIndex.A, PathEndMode.InteractionCell);
            toil.FailOn(new Func<bool>(() => RoomRoleWorker_Church.Validate(ChurchLectern.GetRoom(RegionType.Set_Passable))));
            
            toil.tickAction = new Action(delegate ()
            {
                pawn.skills.Learn(SkillDefOf.Social, 0.3f, false);
                if (pawn.IsHashIntervalTick(MoteInterval.RandomInRange))
                {
                    MoteMaker.MakeSpeechBubble(pawn, moteImage);
                }
                rotateToFace = TargetIndex.B;
            });
            toil.defaultCompleteMode = ToilCompleteMode.Never;

            yield return toil;
        }
    }
}
