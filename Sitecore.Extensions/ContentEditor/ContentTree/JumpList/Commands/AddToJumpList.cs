using System;
using System.Collections.Specialized;
using System.Linq;
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

            try
            {
                var alreadyAdded = _jumpList.Exist(item);
                return alreadyAdded ? CommandState.Hidden : CommandState.Enabled;
            }
            catch (Exception ex)
            {
                Log.Error($"Error checking if the {nameof(AddToJumpList)} command should be enabled", ex, this);
                return CommandState.Hidden;
            }
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

            try
            {
                _jumpList.Add(items.FirstOrDefault());
            }
            catch (Exception ex)
            {
                var message = "An error has ocurred adding the item to the JumpList.";
                Log.Error(message, ex, this);
                Context.ClientPage.ClientResponse.Alert(message);
            }
        }
    }
}