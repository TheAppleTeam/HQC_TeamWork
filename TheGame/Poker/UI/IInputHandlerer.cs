﻿namespace Poker.UI
{
    public interface IInputHandlerer
    {
        int ReadRaise();

        int ReadChipsToAdd();

        int ReadSmallBlind();

        int ReadBigBlind();
    }
}
