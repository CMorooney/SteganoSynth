using System;

namespace ImageMusic
{
    public class TargetNodeView : BaseNodeView
    {
        public TargetNodeView(Enum componentType) : base(componentType) {}

        public override bool CanConnectToNode(BaseNodeView other)
        {
            return
                other != this &&
                other is SourceNodeView;
        }

        protected override nfloat GetXForLabel() => Frame.Width - LabelWidth;

        protected override nfloat GetXForNodePort() => NodePortSize * .25f;
    }
}