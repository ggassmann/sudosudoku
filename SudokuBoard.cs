using SudokuModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Sudosuduko
{
    public class SudokuBoard
    {
        public bool solved;
        public SudokuBoard parent;
        public Sudoku sudo = new Sudoku();
        public Byte[,] solvedBoard;
        public Byte[,] answers;
        private Difficulty _difficulty;
        public SudokuBoard[,] boards;
        public Difficulty difficulty
        {
            get
            {
                return _difficulty;
            }
        }
        public SudokuBoard getBoard(Point p)
        {
            if (boards[(int)p.X, (int)p.Y] == null)
            {
                boards[(int)p.X, (int)p.Y] = new SudokuBoard(this, difficulty.nextDifficulty());
            }
            return boards[(int)p.X, (int)p.Y];
        }
        public SudokuBoard(SudokuBoard parent, Difficulty difficulty)
        {
            this.parent = parent;
            boards = new SudokuBoard[9, 9];
            this._difficulty = difficulty;
            sudo.Data = new Byte[,] {
                {0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0}
            };
            answers = new Byte[9, 9];
            if (difficulty != Difficulty.ROOT)
            {
                sudo.Generate((int)difficulty);
                var solvedSu = new Sudoku();
                solvedSu.Data = (Byte[,])sudo.Data.Clone();
                solvedSu.Solve();
                solvedBoard = solvedSu.Data;
            }
            else
            {
                sudo.Generate(30);
                sudo.Solve();
                solvedBoard = (Byte[,])sudo.Data.Clone();
                sudo.Data = new Byte[,] {
                    {0,0,0,0,0,0,0,0,0},
                    {0,0,0,0,0,0,0,0,0},
                    {0,0,0,0,0,0,0,0,0},
                    {0,0,0,0,0,0,0,0,0},
                    {0,0,0,0,0,0,0,0,0},
                    {0,0,0,0,0,0,0,0,0},
                    {0,0,0,0,0,0,0,0,0},
                    {0,0,0,0,0,0,0,0,0},
                    {0,0,0,0,0,0,0,0,0}
                };
            }
        }
    }
}
