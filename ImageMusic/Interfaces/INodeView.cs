namespace ImageMusic
{
    public interface INodeView
    {
        void MakeConnection(INodeView nodeView);

        void ClearConnection();
    }
}
