using UnityEngine;
using System.Collections;

namespace Match3
{
    public static class Data
    {
        //--------------Neighbour---------------
        public const int UP = 0;
        public const int LEFT = 1;
        public const int RIGHT = 2;
        public const int DOWN = 3;

        //--------------Gem Type---------------
        public const int RED = 0;
        public const int YELLOW = 1;
        public const int BLUE = 2;
        public const int GREEN = 3;
        public const int PURPLE = 4;
        public const int WHITE = 5;
        public const int ORANGE = 6;

        public static string[] GEMTYPE = new string[7] { "Red", "Yellow", "Blue", "Green", "Purple", "White", "Orange" };

        //--------------Gem Status---------------
        public enum GemStatus
        {
            Ready,
            Moving,
            Removing
        }
    }
}
