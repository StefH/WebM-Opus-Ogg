namespace Matroska.Models
{
    public enum TrackType : ulong
    {
        Video = 1,
        Audio = 2,
        Complex = 3,
        Logo = 0x1,
        SubTitle = 0x11,
        Buttons = 0x11,
        Control = 0x20
    }
}