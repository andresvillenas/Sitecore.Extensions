using System.Linq;
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

            var itemNotRemoved = _jumpList.Exist(item);

            return itemNotRemoved ? CommandState.Enabled : CommandState.Hidden;
        }

        public override void Execute(CommandContext context)
        {
            var item = context.Items.FirstOrDefault();

            if (item == null)
                return;

            _jumpList.Remove(item);
        }
    }
}