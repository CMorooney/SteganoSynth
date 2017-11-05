using System;

namespace ImageMusic
{
    public class SourceNodeView : BaseNodeView
    {
        public readonly ColorComponent ColorComponent;

        public SourceNodeView(Enum componentType) : base(componentType)
        {
            ColorComponent = (ColorComponent)componentType;
            NodeNameLabel.TextColor = ((ColorComponent)componentType).GetColor();
        }

        public override bool CanConnectToNode(BaseNodeView other)
        {
            return
                other != this &&
                other is TargetNodeView;
        }

        protected override nfloat GetXForLabel() => 0;

        protected override nfloat GetXForNodePort() => Frame.Width - (NodePortSize * 1.25f);
    }
}
