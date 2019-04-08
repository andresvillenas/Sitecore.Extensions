using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Security.Accounts;
using Sitecore.SecurityModel;

namespace Sitecore.Extensions.ContentEditor.ContentTree.JumpList.Repository
{
    public class JumpListRepository : IJumpListRepository
    {
        private const string JumpListContainerGuid = "{7F0A1B0E-9694-41E7-8D0C-4386680269A2}";
        private const string JumpListTemplateGuid = "{A8683095-7CB3-4EC5-939B-20D1F9A3C9B1}";
        private const string JumpListUserFieldGuid = "{306F9098-F453-437B-885E-0DE37FC1899F}";
        private const string MasterDatabaseName = "master";
        private const string ItemsInMasterFieldGuid = "{80652879-87B8-43CF-B66F-0B53FDEE872C}";
        private const string WebDatabaseName = "web";
        private const string ItemsInWebFieldGuid = "{F46C6B6A-134D-48CF-B74D-FD1C702DE17A}";
        private const string CoreDatabaseName = "core";
        private const string ItemsInCoreFieldGuid = "{C4C8B147-F3C0-46CD-9456-3DA064A0B22F}";

        private static readonly Dictionary<string, string> DatabaseFieldMap = new Dictionary<string, string>
        {
            {MasterDatabaseName, ItemsInMasterFieldGuid},
            {WebDatabaseName, ItemsInWebFieldGuid},
            {CoreDatabaseName, ItemsInCoreFieldGuid}
        };

        public void Add(User user, Item item)
        {
            Add(user, item.ID, item.Database.Name);
        }

        public void Clean(User user, string databaseName)
        {
            var itemsPerDatabaseField = GetFieldByDatabaseName(user, databaseName, out Item jumpListItem);
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

        public Item Get(User user, ID id, string databaseName)
        {
            return GetAll(user, databaseName).FirstOrDefault(i => i.ID == id);
        }

        public IList<Item> GetAll(User user, string databaseName)
        {
            var pinnedItemsField = GetFieldByDatabaseName(user, databaseName, out Item _);

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

        public void Remove(User user, Item item)
        {
            Remove(user, item.ID, item.Database.Name);
        }

        public bool Exist(User user, Item item)
        {
            return Exist(user, item.ID, item.Database.Name);
        }

        public bool ExistJumpList(User user)
        {
            return GetJumpListItem(user) != null;
        }

        private void Add(User user, ID id, string databaseName)
        {
            var itemsPerDatabaseField = GetFieldByDatabaseName(user, databaseName, out Item jumpListItem);
            if (itemsPerDatabaseField == null)
                return;

            try
            {
                using (new StatisticDisabler(StatisticDisablerState.ForItemsWithoutVersionOnly))
                {
                    jumpListItem.Editing.BeginEdit();
                    itemsPerDatabaseField.Add(id.ToString());
                    jumpListItem.Editing.EndEdit();
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message, this);
                jumpListItem.Editing.CancelEdit();
            }
        }

        private void Remove(User user, ID id, string databaseName)
        {
            var itemsPerDatabaseField = GetFieldByDatabaseName(user, databaseName, out Item jumpListItem);
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

        private bool Exist(User user, ID id, string databaseName)
        {
            var itemsPerDatabaseField = GetFieldByDatabaseName(user, databaseName, out Item _);
            return itemsPerDatabaseField != null && itemsPerDatabaseField.Contains(id.ToString());
        }

        private MultilistField GetFieldByDatabaseName(User user, string databaseName, out Item jumpListItem)
        {
            jumpListItem = GetJumpListItem(user);

            if (jumpListItem == null)
                return null;

            var fieldToUse = DatabaseFieldMap[databaseName];

            MultilistField pinnedItemsField = jumpListItem.Fields[fieldToUse];

            return pinnedItemsField;
        }

        private Item GetJumpListItem(User user)
        {
            var query = $"/sitecore/content/Applications/Extensions/Content Editor/Content Tree/Jump List/*[@@templateid='{JumpListTemplateGuid}' " +
                $"and @User='{user.Name}']";

            var item = Database
                .GetDatabase(CoreDatabaseName)
                .SelectSingleItem(query);

            return item ?? CreateJumpListForUser(user);
        }

        private Item CreateJumpListForUser(User user)
        {
            using (new SecurityDisabler())
            {
                try
                {
                    var masterDb = Configuration.Factory.GetDatabase(CoreDatabaseName);
                    var parentItem = masterDb.Items[JumpListContainerGuid];
                    if (parentItem == null)
                        throw new Exception("JumpList container item not found.");

                    var template = masterDb.GetTemplate(JumpListTemplateGuid);
                    if (template == null)
                        throw new Exception("JumpList template not found.");

                    var itemName = ItemUtil.ProposeValidItemName($"JumpList-{user.Name}");

                    var jumpListItem = parentItem.Add(itemName, template);
                    jumpListItem.Editing.BeginEdit();
                    jumpListItem[JumpListUserFieldGuid] = user.Name;
                    jumpListItem.Editing.EndEdit();

                    return jumpListItem;
                }
                catch (Exception e)
                {
                    Log.Error($"Error creating the JumpList item for the current user. {e.Message}", this);
                    return null;
                }
            }
        }
    }
}