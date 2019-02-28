﻿using System.IO;
using System.Web.UI;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Extensions.ContentEditor.ContentTree.JumpList.Repository;
using Sitecore.Resources;
using Sitecore.Shell.Applications.ContentManager;
using Sitecore.Shell.Applications.ContentManager.Sidebars;
using Sitecore.Text;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;

namespace Sitecore.Extensions.ContentEditor.ContentTree.JumpList
{
    public class JumpList : Sidebar, IMessageHandler
    {
        private readonly IJumpListRepository _jumpListRepository;

        public JumpList()
        {
            _jumpListRepository = new JumpListRepository();
        }

        public override void ChangeRoot(Item root, Item folder)
        {
            base.ChangeRoot(root, folder);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public void HandleMessage(Message message)
        {
            Assert.ArgumentNotNull((object)message, nameof(message));
            Log.Info("Message received: " + message, this);
        }

        public override void Initialize(ContentEditorForm form, Item folder, Item root)
        {
            AddCSS();
            AddMainControl();
        }

        public override bool OnDataContextChanged(DataContext context, Message message)
        {
            return base.OnDataContextChanged(context, message);
        }

        public override void SetActiveItem(ID selectedID)
        {
            base.SetActiveItem(selectedID);
        }

        public string GetMainControl()
        {
            var listString =
                "<div id =\"JumpListPanel\" class=\"scJumpList\" " +
                "onclick=\"javascript:if (window.scGeckoActivate) window.scGeckoActivate(); return scContent.onTreeClick(this, event)\" " +
                "oncontextmenu=\"javascript:return scContent.onTreeContextMenu(this, event)\"     " +
                "onkeydown=\"javascript:return scContent.onTreeKeyDown(this, event)\">" +
                "<div id='JumpListActualSize'>" +
                $"{RenderList()}" +
                "</div>" +
                "</div>";

            return listString;
        }

        public override string ToString()
        {
            return string.Empty;
        }

        public override void Update(ID selectedID, bool force)
        {
            base.Update(selectedID, force);
        }


        /// <summary>Refreshes the sidebar.</summary>
        /// <param name="selected">The selected.</param>
        public virtual void Refresh()
        {
            string text = this.RenderList();
            Context.ClientPage.ClientResponse.Eval("scContent.refreshPinList()");
        }

        public virtual string RenderList()
        {
            var output = new HtmlTextWriter(new StringWriter());

            var pinnedItems = _jumpListRepository.GetAll(Client.ContentDatabase.Name);
            foreach (var pinnedItem in pinnedItems)
            {
                RenderPinItem(output, pinnedItem, string.Empty);
            }

            return output.InnerWriter.ToString();
        }

        private void RenderPinItem(HtmlTextWriter output, Item item, string inner)
        {
            Assert.ArgumentNotNull(output, nameof(output));
            Assert.ArgumentNotNull(item, nameof(item));
            Assert.ArgumentNotNull(inner, nameof(inner));

            var str = item.ID.ToShortID().ToString();
            output.Write("<div class=\"scContentTreeNode\">");
            var nodeId = GetNodeId(str);
            var className = GetClassName(item, false);
            output.Write("<a hidefocus=\"true\" id=\"");
            output.Write(nodeId);
            output.Write("\" href=\"#\" class=\"" + className + "\"");
            if (!string.IsNullOrEmpty(item.Help.Text))
            {
                output.Write("title=\"");
                output.Write(StringUtil.EscapeQuote(item.Help.Text));
                output.Write("\"");
            }
            output.Write(">");
            var style = GetStyle(item);
            output.Write("<span");
            output.Write(style);
            output.Write('>');
            RenderTreeNodeIcon(output, item);
            output.Write(item.Appearance.DisplayName);
            output.Write("</span>");
            output.Write("</a>");
            if (inner.Length > 0)
            {
                output.Write("<div>");
                output.Write(inner);
                output.Write("</div>");
            }
            output.Write("</div>");
        }

        private string GetNodeId(string shortId)
        {
            Assert.ArgumentNotNullOrEmpty(shortId, nameof(shortId));
            //return Id + "_Node_" + shortId;
            return string.Empty;
        }

        private static string GetStyle(Item item)
        {
            Assert.ArgumentNotNull(item, nameof(item));
            if (item.TemplateID == TemplateIDs.TemplateField)
                return string.Empty;
            var str = item.Appearance.Style;
            if (string.IsNullOrEmpty(str) && (item.Appearance.Hidden || item.RuntimeSettings.IsVirtual || item.IsItemClone))
                str = "color:#666666";
            if (!string.IsNullOrEmpty(str))
                str = " style=\"" + str + "\"";
            return str;
        }

        private static string GetClassName(Item item, bool active)
        {
            return !active ? (!IsItemUiStatic(item) ? "scContentTreeNodeNormal" : "scContentTreeNodeStatic") : "scContentTreeNodeActive";
        }

        private static bool IsItemUiStatic(Item item)
        {
            return item[FieldIDs.UIStaticItem] == "1";
        }

        private static void RenderTreeNodeIcon(HtmlTextWriter output, Item item)
        {
            Assert.ArgumentNotNull(output, nameof(output));
            Assert.ArgumentNotNull(item, nameof(item));
            output.Write(RenderIcon(item));
        }

        private static string RenderIcon(Item item)
        {
            Assert.ArgumentNotNull(item, nameof(item));

            var urlBuilder = new UrlBuilder(item.Appearance.Icon);
            if (item.Paths.IsMediaItem)
            {
                urlBuilder["rev"] = item.Statistics.Revision;
                urlBuilder["la"] = item.Language.ToString();
            }

            var imageBuilder = new ImageBuilder
            {
                Src = urlBuilder.ToString(),
                Width = 16,
                Height = 16,
                Class = "scContentTreeNodeIcon"
            };

            if (!string.IsNullOrEmpty(item.Help.Text))
                imageBuilder.Alt = item.Help.Text;

            return imageBuilder.ToString();
        }

        private void AddJavascripts()
        {
            // Nothing
        }

        private void AddCSS()
        {
            var cssFile = "/sitecore/shell/Themes/Standard/Default/Extensions.css";
            Context.ClientPage.Header.Controls.Add(new LiteralControl($"<link rel=\"stylesheet\" type=\"text/css\" href=\"{cssFile}\"/>"));
        }

        public void AddMainControl()
        {
            var mainControl = GetMainControl();
            GetPlaceholder().Controls.AddAt(0, new LiteralControl(mainControl));
        }
    }
}