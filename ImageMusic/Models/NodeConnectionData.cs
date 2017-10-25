using CoreAnimation;

namespace ImageMusic
{
    public class NodeConnectionData
    {
        public CALayer ConnectionLayer { get; set; }

        public SourceNodeView SourceNodeView { get; set; }

        public TargetNodeView TargetNodeView { get; set; }
    }
}
