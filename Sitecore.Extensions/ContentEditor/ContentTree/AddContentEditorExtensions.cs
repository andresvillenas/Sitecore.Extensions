using Sitecore.Pipelines;

namespace Sitecore.Extensions.ContentEditor.ContentTree
{
    public class AddContentEditorExtensions
    {
        public void Process(PipelineArgs args)
        {
            new JumpList.JumpList().Initialize(); // TODO: Change the way the controls are injected
        }
    }
}