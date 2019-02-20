using System;
using System.Collections.Generic;

public class InputMessage
{
    public short GameStep { get; private set; }
    public List<byte> Moves {get; private set;}

    public InputMessage(short step, List<byte> moves)
    {
        GameStep = step;
        Moves = moves;
    }

    public InputMessage(byte[] data)
    {
        GameStep = BitConverter.ToInt16(data, 0);
        Moves = new List<byte>();

        for (int i = 2; i < data.Length; i++)
            Moves.Add(data[i]);
    }
}
