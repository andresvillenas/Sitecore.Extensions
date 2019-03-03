using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace Sitecore.Extensions.ContentEditor.ContentTree.JumpList.Repository
{
    public class JumpListRepository : IJumpListRepository
    {
        private const string PinListItemGuid = "{7EB6D120-44C1-458C-8F20-8B9A26C4E250}";
        private const string MasterDatabaseName = "master";
        private const string ItemsInMasterFieldGuid = "{80652879-87B8-43CF-B66F-0B53FDEE872C}";
        private const string WebDatabaseName = "web";
        private const string ItemsInWebFieldGuid = "{F46C6B6A-134D-48CF-B74D-FD1C702DE17A}";
        private const string CoreDatabaseName = "core";
        private const string ItemsInCoreFieldGuid = "{C4C8B147-F3C0-46CD-9456-3DA064A0B22F}";

        private static readonly Dictionary<string, string> DatabaseFieldMap = new Dictionary<string, string>
        {
            { MasterDatabaseName, ItemsInMasterFieldGuid },
            { WebDatabaseName, ItemsInWebFieldGuid },
            { CoreDatabaseName, ItemsInCoreFieldGuid }
        };

        public void Add(Item item)
        {
            Add(item.ID, item.Database.Name);
        }

        public void Clean(string databaseName)
        {
            var itemsPerDatabaseField = GetFieldByDatabaseName(databaseName, out Item jumpListItem);
            if (itemsPerDatabaseField == null)
                return;

            try
            {
                jumpListItem.Editing.BeginEdit();

                itemsPerDatabaseField.Value = string.Empty;

                jumpListItem.Editing.EndEdit();
            }
            catch (Exception e)
            {
                Log.Error(e.Message, this);
                jumpListItem.Editing.CancelEdit();
            }
        }

        public Item Get(ID id, string databaseName)
        {
            return GetAll(databaseName).FirstOrDefault(i => i.ID == id);
        }

        public IList<Item> GetAll(string databaseName)
        {
            var pinnedItemsField = GetFieldByDatabaseName(databaseName, out Item _);

            if (pinnedItemsField == null)
                return new List<Item>();

            var targetIDs = pinnedItemsField.TargetIDs;

            var database = Database.GetDatabase(databaseName);
            var items = new List<Item>();
            foreach (var targetId in targetIDs)
            {
                var item = database.GetItem(targetId);
                if (item == null)
                    Log.Warn($"Item with id '{targetId}' not found.", this);
                else
                    items.Add(item);
            }
            return items;
        }

        public void Remove(Item item)
        {
            Remove(item.ID, item.Database.Name);
        }

        public bool Exist(Item item)
        {
            return Exist(item.ID, item.Database.Name);
        }

        private void Add(ID id, string databaseName)
        {
            var itemsPerDatabaseField = GetFieldByDatabaseName(databaseName, out Item jumpListItem);
            if (itemsPerDatabaseField == null)
                return;

            try
            {
                jumpListItem.Editing.BeginEdit();

                itemsPerDatabaseField.Add(id.ToString());

                jumpListItem.Editing.EndEdit();
            }
            catch (Exception e)
            {
                Log.Error(e.Message, this);
                jumpListItem.Editing.CancelEdit();
            }
        }

        private void Remove(ID id, string databaseName)
        {
            var itemsPerDatabaseField = GetFieldByDatabaseName(databaseName, out Item jumpListItem);
            if (itemsPerDatabaseField == null)
                return;

            try
            {
                jumpListItem.Editing.BeginEdit();

                itemsPerDatabaseField.Remove(id.ToString());

                jumpListItem.Editing.EndEdit();
            }
            catch (Exception e)
            {
                Log.Error(e.Message, this);
                jumpListItem.Editing.CancelEdit();
            }
        }

        private bool Exist(ID id, string databaseName)
        {
            var itemsPerDatabaseField = GetFieldByDatabaseName(databaseName, out Item _);
            return itemsPerDatabaseField != null && itemsPerDatabaseField.Contains(id.ToString());
        }

        private MultilistField GetFieldByDatabaseName(string databaseName, out Item jumpListItem)
        {
            jumpListItem = GetJumpListItem();

            var fieldToUse = DatabaseFieldMap[databaseName];

            MultilistField pinnedItemsField = jumpListItem.Fields[fieldToUse];

            return pinnedItemsField;
        }

        private Item GetJumpListItem()
        {
            return Database.GetDatabase(CoreDatabaseName).GetItem(PinListItemGuid);
        }
    }
}