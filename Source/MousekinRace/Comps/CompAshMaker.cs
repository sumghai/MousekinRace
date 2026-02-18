using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    public class CompAshMaker : ThingComp
    {
        public CompProperties_AshMaker Props => (CompProperties_AshMaker)props;

        public float ashStored = 0f;

        public bool IsFull => ashStored >= Props.ashCapacity;

        public bool IsEmpty => ashStored <= 1;

        public void MakeAsh(float amount)
        {
            if (!IsFull) {
                ashStored += amount * Props.generationMultiplier;
            }
        }

        public Thing DumpAsh()
        {
            Thing ashThing = ThingMaker.MakeThing(Props.ashDef);
            ashThing.stackCount = Mathf.Min((int)ashStored, ashThing.def.stackLimit);
            ashStored = 0f;
            return ashThing;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref ashStored, "ashStored", 0);
        }

        public override string CompInspectStringExtra()
        {
            return "MousekinRace_AshesLevel".Translate((int)ashStored, (int)Props.ashCapacity);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (DebugSettings.ShowDevGizmos)
            {
                Command_Action command_Action0 = new()
                {
                    defaultLabel = "DEV: Set ash to 0",
                    action = delegate
                        {
                            ashStored = 0;
                        }
                };
                yield return command_Action0;

                Command_Action command_Action1 = new()
                {
                    defaultLabel = "DEV: Set ash to 50%",
                    action = delegate
                    {
                        ashStored = Props.ashCapacity / 2;
                    }
                };
                yield return command_Action1;

                Command_Action command_Action2 = new()
                {
                    defaultLabel = "DEV: Set ash to max",
                    action = delegate
                    {
                        ashStored = Props.ashCapacity;
                    }
                };
                yield return command_Action2;
            }
        }
    }
}
