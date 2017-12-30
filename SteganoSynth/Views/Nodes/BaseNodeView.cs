using System;
using AppKit;
using CoreGraphics;
using SteganoSynth.Core;

namespace ImageMusic
{
    public abstract class BaseNodeView : NSView
    {
        public event EventHandler MouseEnteredPort;
        public event EventHandler MouseExitedPort;

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

            NodePort.MouseDidEnter += MouseEnteredNodePort;
            NodePort.MouseDidExit += MouseExitedNodePort;

            AddSubview(NodePort);
        }

        void MouseExitedNodePort(object sender, EventArgs e)
        {
            MouseExitedPort?.Invoke(this, e);
        }

        void MouseEnteredNodePort(object sender, EventArgs e)
        {
            MouseEnteredPort?.Invoke(this, e);
        }

        public void SetNodePortColor(NSColor color)
        {
            NodePort.SetColor(color);
        }

        public void SetHasConnection(bool hasConnection)
        {
            NodePort.HasConnection = hasConnection;
            NodePort.SetNeedsDisplayInRect(NodePort.Bounds);
        }

        public bool GetHasConnection()
        {
            return NodePort.HasConnection;
        }

        public bool IsUserInteractingWithPort() => NodePort.IsHovering;

        public CGPoint GetNodePortMidPoint() =>
            new CGPoint(NodePort.Frame.GetMidX() + Frame.X, NodePort.Frame.GetMidY() + Frame.Y);

        protected abstract nfloat GetXForLabel();

        protected abstract nfloat GetXForNodePort();

        public abstract bool CanConnectToNode(BaseNodeView other);
    }
}
