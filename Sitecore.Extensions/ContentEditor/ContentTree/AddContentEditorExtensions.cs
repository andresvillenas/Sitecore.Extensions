using Sitecore.Pipelines;
using Sitecore.Shell.Applications.ContentEditor.Pipelines.RenderContentEditor;

namespace Sitecore.Extensions.ContentEditor.ContentTree
{
    public class AddContentEditorExtensions
    {
        public void Process(PipelineArgs args)
        {
            if ((args as RenderContentEditorArgs)?.Item != null) // Only when the content editor is rendered.
            {
                new JumpList.JumpList().Initialize(); // TODO: Change the way the controls are injected
            }
        }
    }
}