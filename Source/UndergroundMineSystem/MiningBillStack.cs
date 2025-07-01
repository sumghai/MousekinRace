using System.Collections.Generic;
using Verse;

namespace MousekinRace
{
    public class MiningBillStack : IExposable
    {
        public const int MaxCount = 15;

        public List<MiningBill> miningBills = [];

        public MiningBill FirstShouldDoNow
        {
            get
            {
                for (int i = 0; i < miningBills.Count; i++)
                {
                    if (miningBills[i].ShouldDoNow())
                    {
                        return miningBills[i];
                    }
                }
                return null;
            }
        }

        public bool AnyShouldDoNow
        {
            get
            {
                for (int i = 0; i < miningBills.Count; i++)
                {
                    if (miningBills[i].ShouldDoNow())
                    {
                        return true;
                    }
                }
                return false;
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
            Scribe_Collections.Look(ref miningBills, "miningBills", LookMode.Deep);
            if (Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
            {
                if (miningBills.RemoveAll((MiningBill x) => x == null) != 0)
                {
                    Log.Error("Some mining bills were null after loading.");
                }
                for (int i = 0; i < miningBills.Count; i++)
                {
                    miningBills[i].miningBillStack = this;
                }
            }
        }

        public IEnumerator<MiningBill> GetEnumerator() => miningBills.GetEnumerator();

        public void AddMiningBill(ThingDef mineableThing, ThingWithComps parent, int targetCount = 1)
        {
            MiningBill miningBill = new(mineableThing, parent);
            miningBill.Setup();
            miningBill.targetCount = targetCount;
            miningBills.Add(miningBill);
        }

        public void AddMiningBill(MiningBill miningBill, ThingWithComps parent)
        {
            miningBill.miningBillStack = this;
            miningBill.parent = parent;
            miningBills.Add(miningBill);
        }

        public void Delete(MiningBill miningBill)
        {
            if (miningBills.Contains(miningBill))
            {
                miningBills.Remove(miningBill);
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
        }

        public int IndexOf(MiningBill miningBill) => miningBills.IndexOf(miningBill);
    }
}
