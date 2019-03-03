using System.Collections.Generic;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace Sitecore.Extensions.ContentEditor.ContentTree.JumpList.Repository
{
    /// <summary>
    /// JumpList repository contract
    /// </summary>
    public interface IJumpListRepository
    {
        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        void Add(Item item);
        /// <summary>
        /// Cleans the items using the specified database name.
        /// </summary>
        /// <param name="databaseName">Name of the database.</param>
        void Clean(string databaseName);
        /// <summary>
        /// Gets the item with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="databaseName">Name of the database.</param>
        /// <returns></returns>
        Item Get(ID id, string databaseName);
        /// <summary>
        /// Gets all.
        /// </summary>
        /// <param name="databaseName">Name of the database.</param>
        /// <returns></returns>
        IList<Item> GetAll(string databaseName);
        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        void Remove(Item item);
        /// <summary>
        /// Checks if the specified item exits.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        bool Exist(Item item);
    }
}