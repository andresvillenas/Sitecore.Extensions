using System;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Events;

namespace Sitecore.Extensions.ContentEditor.ContentTree.JumpList.Events
{
    /// <summary>
    /// Item event handlers for the JumpList
    /// </summary>
    public class ItemEventHandler
    {
        private readonly JumpList _jumpList;

        public ItemEventHandler()
        {
            _jumpList = new JumpList();
        }

        protected void OnItemDeleted(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            if (!(Event.ExtractParameter(args, 0) is Item parameter1))
                return;

            _jumpList.Remove(parameter1);
        }

        protected void OnItemRenamed(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            if (!(Event.ExtractParameter(args, 0) is Item parameter1))
                return;

            if (_jumpList.Exist(parameter1))
                _jumpList.OnDataContextChanged(null, null);
        }
    }
}