using System;
using System.Collections.Specialized;
using System.Linq;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Extensions.ContentEditor.ContentTree.JumpList.Repository;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;

namespace Sitecore.Extensions.ContentEditor.ContentTree.JumpList.Commands
{
    [Serializable]
    public class AddToJumpList : Command
    {
        private readonly IJumpListRepository _jumpListRepository;

        public AddToJumpList()
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

            var alreadyAdded = _jumpListRepository.Get(item.ID, item.Database.Name) != null;

            return alreadyAdded ? CommandState.Hidden : CommandState.Enabled;
        }

        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull((object)context, nameof(context));
            if (context.Items.Length != 1)
                return;
            NameValueCollection parameters = new NameValueCollection();
            parameters["items"] = this.SerializeItems(context.Items);
            Context.ClientPage.Start((object)this, "Run", parameters);
        }

        protected void Run(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull((object)args, nameof(args));
            if (!SheerResponse.CheckModified())
                return;
            var items = this.DeserializeItems(args.Parameters["items"]);
            using (new StatisticDisabler(StatisticDisablerState.ForItemsWithoutVersionOnly))
                _jumpListRepository.Add(items.FirstOrDefault());

            var jumpList = new JumpList();
            Context.ClientPage.ClientResponse.SetInnerHtml("JumpListActualSize", jumpList.RenderList());
        }
    }
}