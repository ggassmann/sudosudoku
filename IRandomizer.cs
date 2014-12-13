using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuModel
{
    public interface IRandomizer
    {
        int GetInt(int max);
        int GetInt(int min, int max);
    }
}
