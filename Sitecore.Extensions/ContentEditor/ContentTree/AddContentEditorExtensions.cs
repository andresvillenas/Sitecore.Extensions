using Sitecore.Pipelines;

namespace Sitecore.Extensions.ContentEditor.ContentTree
{
    public class AddContentEditorExtensions
    {
        public void Process(PipelineArgs args)
        {
            var jumpList = new JumpList.JumpList();
            jumpList.Initialize(null, null, null);
        }
    }
}