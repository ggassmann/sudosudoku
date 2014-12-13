using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sudosuduko
{
    public enum Difficulty
    {
        EXTREME=25,
        HARD=30,
        MEDIUM=35,
        EASY=40,
        ROOT=0,
        None=1
    }
    public static class DifficultyExtensions {
        public static Difficulty nextDifficulty(this Difficulty d) {
            if (d == Difficulty.ROOT)
            {
                return Difficulty.EXTREME;
            }
            else if (d == Difficulty.EXTREME)
            {
                return Difficulty.HARD;
            }
            else if (d == Difficulty.HARD)
            {
                return Difficulty.MEDIUM;
            }
            else if (d == Difficulty.MEDIUM)
            {
                return Difficulty.EASY;
            }
            return Difficulty.None;
        }
    }
}
