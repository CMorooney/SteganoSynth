using System;

namespace ImageMusic
{
    public class SourceNodeView : BaseNodeView
    {
        public SourceNodeView(Enum componentType) : base(componentType)
        {
            NodeNameLabel.TextColor = ((ColorComponent)componentType).GetColor();
        }

        public override void ClearConnection()
        {
            throw new NotImplementedException();
        }

        public override void MakeConnection(INodeView nodeView)
        {
            throw new NotImplementedException();
        }

        protected override nfloat GetXForLabel() => 0;

        protected override nfloat GetXForNodePort() => Frame.Width - (NodePortSize * 1.25f);
    }
}
