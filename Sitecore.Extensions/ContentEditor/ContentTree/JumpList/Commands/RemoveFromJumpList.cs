using System;
using System.Linq;
using Sitecore.Diagnostics;
using Sitecore.Shell.Framework.Commands;

namespace Sitecore.Extensions.ContentEditor.ContentTree.JumpList.Commands
{
    /// <summary>
    /// Command that allows to remove an item from the JumpList
    /// </summary>
    /// <seealso cref="Sitecore.Shell.Framework.Commands.Command" />
    public class RemoveFromJumpList : Command
    {
        private readonly JumpList _jumpList;

        public RemoveFromJumpList()
        {
            _jumpList = new JumpList();
        }

        public override CommandState QueryState(CommandContext context)
        {
            if (!context.IsContextMenu)
                return CommandState.Hidden;

            if (context.Items.Length != 1)
                return CommandState.Hidden;

            var item = context.Items.FirstOrDefault();

            if (item == null)
                return CommandState.Hidden;

            try
            {
                var itemNotRemoved = _jumpList.Exist(item);

                return itemNotRemoved ? CommandState.Enabled : CommandState.Hidden;
            }
            catch (Exception ex)
            {
                Log.Error($"Error checking if the {nameof(RemoveFromJumpList)} command should be enabled.", ex, this);
                return CommandState.Hidden;
            }
        }

        public override void Execute(CommandContext context)
        {
            var item = context.Items.FirstOrDefault();

            if (item == null)
                return;

            try
            {
                _jumpList.Remove(item);
            }
            catch (Exception ex)
            {
                var message = "An error has ocurred removing the item from the JumpList.";
                Log.Error(message, ex, this);
                Context.ClientPage.ClientResponse.Alert(message);
            }
        }
    }
}