using System;
using SteganoSynth.Core;

namespace SteganoSynth
{
    public class SourceNodeView : BaseNodeView
    {
        public readonly ColorComponent ColorComponent;

        public SourceNodeView(Enum componentType) : base(componentType)
        {
            ColorComponent = (ColorComponent)componentType;

            var color = ((ColorComponent)componentType).GetColor();

            NodeNameLabel.TextColor = color.ToNSColor();
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
