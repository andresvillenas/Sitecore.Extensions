using System.Collections.Specialized;
using System.Linq;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;

namespace Sitecore.Extensions.ContentEditor.ContentTree.JumpList.Commands
{
    /// <summary>
    /// Command that allows to add an item to the JumpList
    /// </summary>
    /// <seealso cref="Sitecore.Shell.Framework.Commands.Command" />
    public class AddToJumpList : Command
    {
        private readonly JumpList _jumpList;

        public AddToJumpList()
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

            var alreadyAdded = _jumpList.Exist(item);

            return alreadyAdded ? CommandState.Hidden : CommandState.Enabled;
        }

        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull(context, nameof(context));
            if (context.Items.Length != 1)
                return;
            var parameters = new NameValueCollection
            {
                ["items"] = SerializeItems(context.Items)
            };
            Context.ClientPage.Start(this, "Run", parameters);
        }

        protected void Run(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            if (!SheerResponse.CheckModified())
                return;
            var items = DeserializeItems(args.Parameters["items"]);

            using (new StatisticDisabler(StatisticDisablerState.ForItemsWithoutVersionOnly))
                _jumpList.Add(items.FirstOrDefault());
        }
    }
}