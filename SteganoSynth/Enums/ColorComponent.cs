namespace ImageMusic
{
    public enum ColorComponent
    {
        [FriendlyName("Red Component")]
        [Color (255, 0, 0)]
        Red,

        [FriendlyName("Green Component")]
        [Color (63, 122, 77)]
        Green,

        [FriendlyName("Blue Component")]
        [Color (0, 0, 255)]
        Blue,

        [FriendlyName("Brightness Component")]
        [Color (0, 0, 0)]
        Brightness,

        [FriendlyName("Hue Component")]
        [Color (0, 0, 0)]
        Hue
    }
}