using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MousekinRace
{
    public class Building_StorageCellar : Building, ILoadReferenceable, IThingHolder, IHaulDestination, IHaulEnroute, IHaulSource, IStorageGroupMember, IStoreSettingsParent, ISearchableContents
    {
        // todo
        // - fix SpaceRemainingFor()
        // - fix item search visibility
        // - fix counting of items in resources list
        // - allow pawns to fetch items from cellar to fulfill bills (can already reorganize between storages)
        // - add refrigeration using map cell caching
        
        public CompCellarOutdoor compCellarOutdoor;
        
        public ThingOwner innerContainer;

        public StorageSettings settings;

        public StorageGroup storageGroup;

        public static StringBuilder sb = new StringBuilder();

        public int MaxStoredItems => def.building.maxItemsInCell * (compCellarOutdoor.Props.storageCells > 0 ? compCellarOutdoor.Props.storageCells : def.size.Area);

        public IReadOnlyList<Thing> StoredItems => innerContainer.ToList();

        public ThingOwner SearchableContents => innerContainer;

        public bool StorageTabVisible => true;

        StorageGroup IStorageGroupMember.Group
        {
            get
            {
                return storageGroup;
            }
            set
            {
                storageGroup = value;
            }
        }

        bool IStorageGroupMember.DrawConnectionOverlay => base.Spawned;

        string IStorageGroupMember.StorageGroupTag => def.building.storageGroupTag;

        StorageSettings IStorageGroupMember.StoreSettings => GetStoreSettings();

        StorageSettings IStorageGroupMember.ParentStoreSettings => GetParentStoreSettings();

        StorageSettings IStorageGroupMember.ThingStoreSettings => settings;

        bool IStorageGroupMember.ShowRenameButton => base.Faction == Faction.OfPlayer;

        bool IStorageGroupMember.DrawStorageTab => true;

        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
        }

        public ThingOwner GetDirectlyHeldThings()
        {
            return innerContainer;
        }

        public StorageSettings GetParentStoreSettings()
        {
            return def.building.fixedStorageSettings;
        }

        public StorageSettings GetStoreSettings()
        {
            if (storageGroup != null)
            {
                return storageGroup.GetStoreSettings();
            }
            return settings;
        }

        public bool Accepts(Thing t)
        {
            return GetStoreSettings().AllowedToAccept(t);
        }

        public int SpaceRemainingFor(ThingDef _)
        {
            // todo - fix based on stack sizes
            return MaxStoredItems - StoredItems.Count();
        }

        public void Notify_SettingsChanged()
        {
            if (base.Spawned)
            {
                base.MapHeld.listerHaulables.Notify_HaulSourceChanged(this);
            }
        }

        public Building_StorageCellar()
        {
            innerContainer = new ThingOwner<Thing>(this, oneStackOnly: false);
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            compCellarOutdoor = GetComp<CompCellarOutdoor>();

            if (storageGroup != null && map != storageGroup.Map)
            {
                StorageSettings storeSettings = storageGroup.GetStoreSettings();
                storageGroup.RemoveMember(this);
                storageGroup = null;
                settings.CopyFrom(storeSettings);
            }
        }

        public override void PostMake()
        {
            base.PostMake();
            settings = new StorageSettings(this);
            if (def.building.defaultStorageSettings != null)
            {
                settings.CopyFrom(def.building.defaultStorageSettings);
            }
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            if (storageGroup != null)
            {
                storageGroup?.RemoveMember(this);
                storageGroup = null;
            }
            innerContainer.TryDropAll(base.Position, base.Map, ThingPlaceMode.Near);
            base.DeSpawn(mode);
        }

        public override void DrawExtraSelectionOverlays()
        {
            base.DrawExtraSelectionOverlays();
            StorageGroupUtility.DrawSelectionOverlaysFor(this);
        }

        public override string GetInspectString()
        {
            sb.Clear();
            sb.Append(base.GetInspectString());
            if (Spawned)
            {
                if (storageGroup != null)
                {
                    sb.AppendLineIfNotEmpty();
                    sb.Append(string.Format("{0}: {1} ", "StorageGroupLabel".Translate(), storageGroup.RenamableLabel.CapitalizeFirst()));
                    if (storageGroup.MemberCount > 1)
                    {
                        sb.Append(string.Format("({0})", "NumBuildings".Translate(storageGroup.MemberCount)));
                    }
                    else
                    {
                        sb.Append(string.Format("({0})", "OneBuilding".Translate()));
                    }
                }
            }
            return sb.ToString();
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }
            foreach (Gizmo item2 in StorageSettingsClipboard.CopyPasteGizmosFor(GetStoreSettings()))
            {
                yield return item2;
            }
            if (StorageTabVisible && base.MapHeld != null)
            {
                foreach (Gizmo item3 in StorageGroupUtility.StorageGroupMemberGizmos(this))
                {
                    yield return item3;
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref innerContainer, "innerContainer", this);
            Scribe_Deep.Look(ref settings, "settings", this);
            Scribe_References.Look(ref storageGroup, "storageGroup");
        }
    }
}
