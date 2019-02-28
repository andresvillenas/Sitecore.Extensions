using System.Linq;
using Sitecore.Extensions.ContentEditor.ContentTree.JumpList.Repository;
using Sitecore.Shell.Framework.Commands;

namespace Sitecore.Extensions.ContentEditor.ContentTree.JumpList.Commands
{
    public class RemoveFromJumpList : Command
    {
        private readonly IJumpListRepository _jumpListRepository;

        public RemoveFromJumpList()
        {
            _jumpListRepository = new JumpListRepository();
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

            var notRemoved = _jumpListRepository.Get(item.ID, item.Database.Name) != null;

            return notRemoved ? CommandState.Enabled : CommandState.Hidden;
        }

        public override void Execute(CommandContext context)
        {
            var item = context.Items.FirstOrDefault();

            if (item == null)
                return;

            _jumpListRepository.Remove(item);

            var jumpList = new JumpList();
            Context.ClientPage.ClientResponse.SetInnerHtml("JumpListActualSize", jumpList.RenderList());
        }
    }
}