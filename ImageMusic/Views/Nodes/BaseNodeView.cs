using System;
using AppKit;
using CoreGraphics;

namespace ImageMusic
{
    public abstract class BaseNodeView : NSView, INodeView
    {
        protected NSTextField NodeNameLabel;

        protected const int NodePortSize = 12;
        protected const int LabelWidth = 135;

        NodePort NodePort;

        const int LabelHeight = 18;

        protected BaseNodeView(Enum componentType): base (new CGRect(0, 0, 165, 25))
        {
            SetUpUIElements(componentType);
        }

        public override bool IsFlipped => true;

        void SetUpUIElements(Enum componentType)
        {
            NodeNameLabel = new NSTextField
            {
                Frame = new CGRect (GetXForLabel(),
                                    Frame.Height/2 - LabelHeight/2,
                                    LabelWidth, LabelHeight),
                Bezeled = false,
                DrawsBackground = false,
                Editable = false,
                Selectable = false,
                StringValue = componentType.GetFriendlyName()
            };

            AddSubview(NodeNameLabel);

            NodePort = new NodePort(
                new CGRect(GetXForNodePort(),
                Frame.Height / 2 - NodePortSize / 2,
                NodePortSize, NodePortSize)
            );

            AddSubview(NodePort);
        }

        public void SetHasConnection(bool hasConnection)
        {
            NodePort.HasConnection = hasConnection;
            NodePort.SetNeedsDisplayInRect(NodePort.Bounds);
        }

        public bool IsUserInteractingWithPort() => NodePort.IsHovering;

        protected abstract nfloat GetXForLabel();

        protected abstract nfloat GetXForNodePort();

        #region INodeView implementation

        public abstract void ClearConnection();

        public abstract void MakeConnection(INodeView nodeView);

        #endregion
    }
}
