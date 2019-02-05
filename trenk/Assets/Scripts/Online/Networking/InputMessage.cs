public class InputMessage
{
    public short GameStep { get; private set; }
    public byte Direction { get; private set; }

    public InputMessage(short step, byte dir)
    {
        GameStep = step;
        Direction = dir;
    }
}
