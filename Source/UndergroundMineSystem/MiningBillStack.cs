using System.Collections.Generic;
using Verse;

namespace MousekinRace
{
    public class MiningBillStack : IExposable
    {
        public const int MaxCount = 15;

        public List<MiningBill> miningBills = [];

        public MiningBill currentMiningBill;

        public MiningBill FirstCanDo
        {
            get
            {
                if (currentMiningBill == null)
                {
                    for (int i = 0; i < miningBills.Count; i++)
                    {
                        var process = miningBills[i];
                        if (process.ShouldDoNow())
                        {
                            currentMiningBill = process;
                            break;
                        }
                    }
                }
                return currentMiningBill;
            }
        }

        public List<MiningBill> MiningBills
        {
            get
            {

                return miningBills;
            }
            set
            {
                miningBills = value;
            }
        }

        public MiningBillStack()
        { 
        }

        public void ExposeData()
        {
            Scribe_References.Look(ref currentMiningBill, "currentMiningBill");
            Scribe_Collections.Look(ref miningBills, "miningBills", LookMode.Deep);
        }

        public IEnumerator<MiningBill> GetEnumerator() => miningBills.GetEnumerator();

        public void Notify_MiningBillEnded()
        {
            currentMiningBill = null;
        }

        public void Notify_MiningBillChange()
        {
            // Current process isn't started, so we set it to null
            if (currentMiningBill != null && currentMiningBill.ticksRequired == currentMiningBill.TickLeft)
            {
                currentMiningBill = null;
            }
        }

        public void AddMiningBill(ThingDef mineableThing, ThingWithComps parent, int targetCount = 1)
        {
            MiningBill miningBill = new(mineableThing, parent);
            miningBill.Setup();
            miningBill.targetCount = targetCount;
            miningBills.Add(miningBill);
            Notify_MiningBillChange();
        }

        public void Delete(MiningBill miningBill)
        {
            if (miningBills.Contains(miningBill))
            {
                miningBills.Remove(miningBill);
            }
                
            // If current process is the one we delete and it didn't start, kill it
            if (miningBill == currentMiningBill && miningBill.Progress == 0f)
            {
                currentMiningBill = null;
            }
        }

        public void Reorder(MiningBill miningBill, int offset)
        {
            int num = miningBills.IndexOf(miningBill);
            num += offset;
            if (num >= 0)
            {
                miningBills.Remove(miningBill);
                miningBills.Insert(num, miningBill);
            }
            Notify_MiningBillChange();
        }

        public int IndexOf(MiningBill miningBill) => miningBills.IndexOf(miningBill);
    }
}
