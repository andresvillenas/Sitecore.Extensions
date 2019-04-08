using System.IO;
using System.Web.UI;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Extensions.ContentEditor.ContentTree.JumpList.Repository;
using Sitecore.Resources;
using Sitecore.Shell.Applications.ContentManager.Sidebars;
using Sitecore.Text;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;

namespace Sitecore.Extensions.ContentEditor.ContentTree.JumpList
{
    /// <summary>
    /// JumpList control.
    /// </summary>
    /// <remarks>Allows to pin an item at the top of the Content Tree of the Content Editor.</remarks>
    /// <seealso cref="Sitecore.Shell.Applications.ContentManager.Sidebars.Sidebar" />
    public class JumpList : Sidebar
    {
        public const string JumpListActualSizeDivId = "JumpListActualSize";
        private readonly IJumpListRepository _jumpListRepository;

        public JumpList()
        {
            _jumpListRepository = new JumpListRepository();
        }

        public virtual void Initialize()
        {
            AddCSS();
            AddJS();
            AddMainControl();
        }

        public void Add(Item item)
        {
            if (Exist(item))
                return;

            _jumpListRepository.Add(Context.User, item);
            OnDataContextChanged(null, null);
        }

        public bool Exist(Item item)
        {
            return _jumpListRepository.Exist(Context.User, item);
        }

        public void Remove(Item item)
        {
            if (!Exist(item))
                return;

            _jumpListRepository.Remove(Context.User, item);
            OnDataContextChanged(null, null);
        }

        public override bool OnDataContextChanged(DataContext context, Message message)
        {
            Context.ClientPage.ClientResponse.SetInnerHtml(JumpListActualSizeDivId, RenderInnerContent());
            return true;
        }

        protected virtual string Render()
        {
            var listString =
                "<div id =\"JumpListPanel\" class=\"scJumpList\" " +
                "onclick=\"javascript:if (window.scGeckoActivate) window.scGeckoActivate(); return scContent.onJumpListClick(this, event)\" " +
                "oncontextmenu=\"javascript:return scContent.onJumpListContextMenu(this, event)\"     " +
                "onkeydown=\"javascript:return scContent.onTreeKeyDown(this, event)\">" +
                "<div id='" + JumpListActualSizeDivId + "'>" +
                $"{RenderInnerContent()}" +
                "</div>" +
                "</div>";

            return listString;
        }

        protected virtual string RenderInnerContent()
        {
            var output = new HtmlTextWriter(new StringWriter());

            var pinnedItems = _jumpListRepository.GetAll(Context.User, Client.ContentDatabase.Name);
            foreach (var pinnedItem in pinnedItems)
            {
                RenderPinItem(output, pinnedItem, string.Empty);
            }

            return output.InnerWriter.ToString();
        }

        private void AddJS()
        {
            var jsFile = "/sitecore/shell/Applications/Extensions/JumpList.js";
            Context.ClientPage.Header.Controls.Add(new LiteralControl($"<script src=\"{jsFile}\" type=\"text/javascript\"></script>"));
        }

        private void AddCSS()
        {
            var cssFile = "/sitecore/shell/Applications/Extensions/JumpList.css";
            Context.ClientPage.Header.Controls.Add(new LiteralControl($"<link rel=\"stylesheet\" type=\"text/css\" href=\"{cssFile}\"/>"));
        }

        private void AddMainControl()
        {
            var mainControl = Render();
            GetPlaceholder().Controls.AddAt(0, new LiteralControl(mainControl));
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
            return "JumpList_Item_" + shortId;
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
    }
}