using System;
using Foundation;
using AppKit;
using CoreGraphics;
using CoreAnimation;
using System.Collections.Generic;
using System.Linq;
using SteganoSynth.Core;

namespace ImageMusic
{
    public partial class NodeEditorView : NSView
    {
        List<BaseNodeView> Nodes;
        List<NodeConnectionData> NodeConnectionData;
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
            NodeConnectionData = new List<NodeConnectionData>();

            WantsLayer = true;

            DrawingLayer = CreateLayer(NSColor.Black);
            Layer.AddSublayer(DrawingLayer);
        }

        #endregion

        #region Lifecycle

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            CreateAndDrawNodes();
            SetDefaultConnections();
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

                sourceNode.MouseExitedPort += MouseExitedPort;

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

                targetNode.MouseExitedPort += MouseExitedPort;

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

            CurrentStartNode = GetInteractingNodePort();

            if (CurrentStartNode != null)
            {
                SetNodeColors();
                SetDrawingLayerColor();

                CurrentStartPosition = CurrentStartNode.GetNodePortMidPoint();
                CurrentStartNode.SetHasConnection(true);
            }
        }

        public override void MouseDragged(NSEvent theEvent)
        {
            base.MouseDragged(theEvent);

            if (!IsDrawing)
            {
                return;
            }

            CurrentEndNode = GetInteractingNodePort();

            CurrentEndPosition = ConvertPointFromView(theEvent.LocationInWindow, null);

            SetNodeColors();
            SetDrawingLayerColor();

            DrawPathOnLayer(DrawingLayer);
        }


        public override void MouseUp(NSEvent theEvent)
        {
            base.MouseUp(theEvent);

            CurrentEndNode = GetInteractingNodePort();

            if (IsDrawing && IsPendingConnectionValid())
            {
                CurrentEndPosition = CurrentEndNode.GetNodePortMidPoint();
                ValidConnectionMade();
            }
            else
            {
                NoConnectionMade();
            }

            SetNodeColors();
            SetDrawingLayerColor();
            ResetDragStates();
        }

        #endregion

        #region Drawing

        CAShapeLayer CreateLayer(NSColor color) => new CAShapeLayer
        {
            StrokeColor = color.CGColor,
            FillColor = NSColor.Clear.CGColor,
            LineWidth = 2
        };

        void SetDrawingLayerColor()
        {
            var sourceNode = GetSourceNode();

            if (sourceNode != null)
            {
                var color = sourceNode.ColorComponent.GetColor();
                DrawingLayer.StrokeColor = color.ToNSColor().CGColor;
            }
            else
            {
                DrawingLayer.StrokeColor = NSColor.Black.CGColor;
            }
        }

        void SetNodeColors()
        {
            var sourceNode = GetSourceNode();

            var color = NSColor.Black;

            if (sourceNode != null && sourceNode.GetHasConnection())
            {
                color = sourceNode.ColorComponent.GetColor().ToNSColor();
            }

            CurrentStartNode?.SetNodePortColor(color);
            CurrentEndNode?.SetNodePortColor(color);
        }

        void DrawPathOnLayer(CAShapeLayer layer)
        {
            var offset = Math.Abs(CurrentStartPosition.X - CurrentEndPosition.X) / 5;

            var ctrlPointA = new CGPoint(CurrentEndPosition.X - offset, CurrentStartPosition.Y);
            var ctrlPointB = new CGPoint(CurrentStartPosition.X + offset, CurrentEndPosition.Y);

            var path = new CGPath();
            path.MoveToPoint(CurrentStartPosition);
            path.AddCurveToPoint(
                ctrlPointA.X, ctrlPointA.Y,
                ctrlPointB.X, ctrlPointB.Y,
                CurrentEndPosition.X, CurrentEndPosition.Y);
            
            layer.Path = path;
        }

        #endregion

        #region Connection logic

        void ValidConnectionMade ()
        {
            CurrentStartNode.SetHasConnection(true);
            CurrentEndNode.SetHasConnection(true);

            var connectingSourceNode = GetSourceNode();
            var connectingTargetNode = GetTargetNode();

            var layer = CreateLayer(connectingSourceNode.ColorComponent.GetColor().ToNSColor());
            DrawPathOnLayer(layer);

            var existingConnections = NodeConnectionData.Where(n =>
                                        n.SourceNodeView.ColorComponent == connectingSourceNode.ColorComponent ||
                                        n.TargetNodeView.TargetModifier == connectingTargetNode.TargetModifier)
                                        .ToList();

            if (existingConnections.Count >= 0)
            {
                foreach (var existingConnection in existingConnections)
                {
                    var sourceNode = existingConnection.SourceNodeView;
                    var targetNode = existingConnection.TargetNodeView;

                    if (sourceNode.ColorComponent != connectingSourceNode.ColorComponent)
                    {
                        sourceNode.SetHasConnection(false);
                    }
                    if (targetNode.TargetModifier != connectingTargetNode.TargetModifier)
                    {
                        targetNode.SetNodePortColor(NSColor.Black);
                        targetNode.SetHasConnection(false);
                    }

                    existingConnection.ConnectionLayer.RemoveFromSuperLayer();
                    NodeConnectionData.Remove(existingConnection);
                }
            }

            NodeConnectionData.Add (new NodeConnectionData
            {
                SourceNodeView = connectingSourceNode,
                TargetNodeView = connectingTargetNode,
                ConnectionLayer = layer
            });

            Layer.AddSublayer(layer);

            SynthSettings.Instance.SetSourceForTarget
                         (connectingSourceNode.ColorComponent, connectingTargetNode.TargetModifier);

            SetNodeColors();
        }

        void NoConnectionMade()
        {
            CurrentStartNode?.SetHasConnection(false);
            CurrentEndNode?.SetHasConnection(false);
        }

        #endregion

        void SetDefaultConnections()
        {
            var settings = SynthSettings.Instance;

            var sourceNodes = Nodes.Where(n => n is SourceNodeView);
            var targetNodes = Nodes.Where(n => n is TargetNodeView);

            foreach (var targetModifier in Enum.GetValues(typeof(TargetModifier)).Cast<TargetModifier>())
            {
                var source = settings.GetSourceForTarget(targetModifier);

                CurrentStartNode = sourceNodes.First(n => (n as SourceNodeView).ColorComponent == source);
                CurrentEndNode = targetNodes.First(n => (n as TargetNodeView).TargetModifier == targetModifier);

                CurrentStartPosition = CurrentStartNode.GetNodePortMidPoint();
                CurrentEndPosition = CurrentEndNode.GetNodePortMidPoint();

                ValidConnectionMade();
            }
        }

        void MouseExitedPort(object sender, EventArgs e)
        {
            if(!IsDrawing)
            {
                return;
            }
            
            var nodeView = sender as BaseNodeView;

            if (!nodeView.GetHasConnection())
            {
                nodeView.SetNodePortColor(NSColor.Black);
            }
        }

        bool IsPendingConnectionValid()
        {
            return
                CurrentStartNode.CanConnectToNode(CurrentEndNode) &&
                CurrentEndNode.CanConnectToNode(CurrentStartNode);
        }

        void ResetDragStates()
        {
            DrawingLayer.Path = null;
            CurrentStartPosition = CurrentEndPosition = new CGPoint(0, 0);
            CurrentStartNode = CurrentEndNode = null;
        }

        bool IsDrawing => CurrentStartNode != null;

        BaseNodeView GetInteractingNodePort() => Nodes.FirstOrDefault(n => n.IsUserInteractingWithPort());

        SourceNodeView GetSourceNode()
        {
            return CurrentStartNode is SourceNodeView ?
            CurrentStartNode as SourceNodeView : CurrentEndNode as SourceNodeView;
        }

        TargetNodeView GetTargetNode()
        {
            return CurrentStartNode is TargetNodeView ?
                CurrentStartNode as TargetNodeView : CurrentEndNode as TargetNodeView;
        }
    }
}