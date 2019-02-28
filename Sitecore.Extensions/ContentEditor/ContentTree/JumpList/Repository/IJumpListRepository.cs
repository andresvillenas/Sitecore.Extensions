using System.Collections.Generic;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace Sitecore.Extensions.ContentEditor.ContentTree.JumpList.Repository
{
    public interface IJumpListRepository
    {
        void Add(Item item);
        void Clean(string databaseName);
        Item Get(ID id, string databaseName);
        IList<Item> GetAll(string databaseName);
        void Remove(Item item);
    }
}