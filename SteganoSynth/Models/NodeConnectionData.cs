using CoreAnimation;

namespace SteganoSynth
{
    public class NodeConnectionData
    {
        public CALayer ConnectionLayer { get; set; }

        public SourceNodeView SourceNodeView { get; set; }

        public TargetNodeView TargetNodeView { get; set; }
    }
}
