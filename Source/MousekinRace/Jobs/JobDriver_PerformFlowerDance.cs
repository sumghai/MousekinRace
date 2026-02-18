using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MousekinRace
{
    public class JobDriver_PerformFlowerDance : JobDriver
    {
        public int ticksBetweenDanceStep = 60;

        public int hoppingProgressTick = 0;

        public float hoppingMaxHeight = 0.2f;

        public override bool TryMakePreToilReservations(bool errorOnFailed) => true;

        public override Vector3 ForcedBodyOffset
        {
            get
            {
                float timestep = hoppingProgressTick * (float)Math.PI / (ticksBetweenDanceStep / 2);
                float z = (float)(hoppingMaxHeight * Math.Abs(Math.Sin(timestep)));
                return new Vector3(0f, 0f, z);
            }
        }

        public override IEnumerable<Toil> MakeNewToils()
        {
            Toil dance = ToilMaker.MakeToil("MakeNewToils");
            dance.initAction = delegate
            {
                // All nuns should face the same way to start with, so that their dance moves are synchronized
                pawn.Rotation = Rot4.South;
            };
            dance.tickAction = delegate
            {
                hoppingProgressTick++;
                if (hoppingProgressTick > ticksBetweenDanceStep) 
                {
                    hoppingProgressTick = 0;
                }
                
                if (pawn.IsHashIntervalTick(ticksBetweenDanceStep))
                {
                    // Could be optimized
                    if (pawn.Rotation == Rot4.North)
                    {
                        pawn.Rotation = Rot4.East;
                    }
                    else if (pawn.Rotation == Rot4.East)
                    {
                        pawn.Rotation = Rot4.South;
                    }
                    else if (pawn.Rotation == Rot4.South)
                    {
                        pawn.Rotation = Rot4.West;
                    }
                    else
                    { 
                        pawn.Rotation = Rot4.North;
                    }
                }
            };
            dance.defaultCompleteMode = ToilCompleteMode.Never;
            dance.socialMode = RandomSocialMode.Off;
            yield return dance;
        }
    }
}
