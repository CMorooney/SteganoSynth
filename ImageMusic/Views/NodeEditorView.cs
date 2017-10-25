using System;
using Foundation;
using AppKit;
using CoreGraphics;
using CoreAnimation;
using System.Collections.Generic;
using System.Linq;

namespace ImageMusic
{
    public partial class NodeEditorView : NSView
    {
        List<BaseNodeView> Nodes;

        BaseNodeView CurrentStartNode, CurrentEndNode;
        CAShapeLayer DrawingLayer;
        CGPoint CurrentStartPosition, CurrentEndPosition;

        public override bool IsFlipped => true;

        #region Constructors

        // Called when created from unmanaged code
        public NodeEditorView(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public NodeEditorView(NSCoder coder) : base(coder)
        {
            Initialize();
        }

        public NodeEditorView(CGRect frame) : base(frame)
        {
            Initialize();
        }

        // Shared initialization code
        void Initialize()
        {
            WantsLayer = true;

            DrawingLayer = CreateLayer();
            Layer.AddSublayer(DrawingLayer);
        }

        #endregion

        #region Lifecycle

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            CreateAndDrawNodes();
        }

        #endregion

        #region Node creation

        void CreateAndDrawNodes()
        {
            Nodes = new List<BaseNodeView>();

            nfloat verticalPadding = 10;
            nfloat windowPadding = 10;
            nfloat y = windowPadding;

            //draw color component source nodes
            foreach (var colorComponent in Enum.GetValues(typeof(ColorComponent)).Cast<ColorComponent>())
            {
                var sourceNode = new SourceNodeView(colorComponent);
                var frame = sourceNode.Frame;
                frame.Y = y;
                frame.X += windowPadding;
                sourceNode.Frame = frame;

                Nodes.Add(sourceNode);
                AddSubview(sourceNode);

                y += sourceNode.Frame.Height + verticalPadding;
            }

            //reset Y and draw target modifier nodes
            y = windowPadding;

            foreach (var targetModifier in Enum.GetValues(typeof(TargetModifier)).Cast<TargetModifier>())
            {
                var targetNode = new TargetNodeView(targetModifier);
                var frame = targetNode.Frame;
                frame.Y = y;
                frame.X = Frame.Width - frame.Width - windowPadding;
                targetNode.Frame = frame;

                Nodes.Add(targetNode);
                AddSubview(targetNode);

                y += targetNode.Frame.Height + verticalPadding;
            }
        }

        #endregion

        #region Mouse events

        public override void MouseDown(NSEvent theEvent)
        {
            base.MouseDown(theEvent);

            CurrentStartPosition = ConvertPointFromView(theEvent.LocationInWindow, null);

            CurrentStartNode = GetInteractingNodePort();

            if (CurrentStartNode != null)
            {
                CurrentStartNode.SetHasConnection(true);
            }
        }

        public override void MouseDragged(NSEvent theEvent)
        {
            base.MouseDragged(theEvent);

            if (CurrentStartNode == null)
            {
                return;
            }

            var path = new CGPath();
            path.MoveToPoint(CurrentStartPosition);
            path.AddLineToPoint(ConvertPointFromView(theEvent.LocationInWindow, null));
            DrawingLayer.Path = path;
        }


        public override void MouseUp(NSEvent theEvent)
        {
            base.MouseUp(theEvent);

            CurrentEndPosition = ConvertPointFromView(theEvent.LocationInWindow, null);
            CurrentEndNode = GetInteractingNodePort();

            if (CurrentEndNode != null)
            {
                ValidConnectionMade();
            }
            else
            {
                NoConnectionMade();
            }

            ResetDragStates();
        }

        #endregion

        #region Connection logic

        void ValidConnectionMade ()
        {
            CurrentStartNode.SetHasConnection(true);
            CurrentEndNode.SetHasConnection(true);

            var path = new CGPath();
            path.MoveToPoint(CurrentStartPosition);
            path.AddLineToPoint(CurrentEndPosition);

            var layer = CreateLayer();
            layer.Path = path;
            Layer.AddSublayer(layer);
        }

        void NoConnectionMade()
        {
            CurrentStartNode.SetHasConnection(false);
        }

        #endregion

        void ResetDragStates()
        {
            DrawingLayer.Path = null;
            CurrentStartPosition = CurrentEndPosition = new CGPoint(0, 0);
            CurrentStartNode = CurrentEndNode = null;
        }

        CAShapeLayer CreateLayer () => new CAShapeLayer
        {
            StrokeColor = NSColor.Black.CGColor,
            FillColor = NSColor.Black.CGColor,
            LineWidth = 5
        };

        BaseNodeView GetInteractingNodePort() => Nodes.FirstOrDefault(n => n.IsUserInteractingWithPort());
    }
}