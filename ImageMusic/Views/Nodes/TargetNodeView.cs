using System;

namespace ImageMusic
{
    public class TargetNodeView : BaseNodeView
    {
        public TargetNodeView(Enum componentType) : base(componentType) {}

        public override void ClearConnection()
        {
            throw new NotImplementedException();
        }

        public override void MakeConnection(INodeView nodeView)
        {
            throw new NotImplementedException();
        }

        protected override nfloat GetXForLabel() => Frame.Width - LabelWidth;

        protected override nfloat GetXForNodePort() => NodePortSize * .25f;
    }
}