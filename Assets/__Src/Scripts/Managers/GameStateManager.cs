using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameStateManager
{
    public enum State { Menu, Online, Offline, Loading }

    public static State GameState = State.Menu;
}
