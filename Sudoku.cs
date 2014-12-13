using System;

namespace SudokuModel
{
	public class Sudoku
	{
		// 0 = solve for
		private byte[,] m_sudoku;

		// Maps sub square index to m_sudoku
		private readonly EntryPoint[,] m_subIndex =
			new EntryPoint[,]
			{
				{ new EntryPoint(0,0),new EntryPoint(0,1),new EntryPoint(0,2),new EntryPoint(1,0),new EntryPoint(1,1),new EntryPoint(1,2),new EntryPoint(2,0),new EntryPoint(2,1),new EntryPoint(2,2)},
				{ new EntryPoint(0,3),new EntryPoint(0,4),new EntryPoint(0,5),new EntryPoint(1,3),new EntryPoint(1,4),new EntryPoint(1,5),new EntryPoint(2,3),new EntryPoint(2,4),new EntryPoint(2,5)},
				{ new EntryPoint(0,6),new EntryPoint(0,7),new EntryPoint(0,8),new EntryPoint(1,6),new EntryPoint(1,7),new EntryPoint(1,8),new EntryPoint(2,6),new EntryPoint(2,7),new EntryPoint(2,8)},
				{ new EntryPoint(3,0),new EntryPoint(3,1),new EntryPoint(3,2),new EntryPoint(4,0),new EntryPoint(4,1),new EntryPoint(4,2),new EntryPoint(5,0),new EntryPoint(5,1),new EntryPoint(5,2)},
				{ new EntryPoint(3,3),new EntryPoint(3,4),new EntryPoint(3,5),new EntryPoint(4,3),new EntryPoint(4,4),new EntryPoint(4,5),new EntryPoint(5,3),new EntryPoint(5,4),new EntryPoint(5,5)},
				{ new EntryPoint(3,6),new EntryPoint(3,7),new EntryPoint(3,8),new EntryPoint(4,6),new EntryPoint(4,7),new EntryPoint(4,8),new EntryPoint(5,6),new EntryPoint(5,7),new EntryPoint(5,8)},
				{ new EntryPoint(6,0),new EntryPoint(6,1),new EntryPoint(6,2),new EntryPoint(7,0),new EntryPoint(7,1),new EntryPoint(7,2),new EntryPoint(8,0),new EntryPoint(8,1),new EntryPoint(8,2)},
				{ new EntryPoint(6,3),new EntryPoint(6,4),new EntryPoint(6,5),new EntryPoint(7,3),new EntryPoint(7,4),new EntryPoint(7,5),new EntryPoint(8,3),new EntryPoint(8,4),new EntryPoint(8,5)},
				{ new EntryPoint(6,6),new EntryPoint(6,7),new EntryPoint(6,8),new EntryPoint(7,6),new EntryPoint(7,7),new EntryPoint(7,8),new EntryPoint(8,6),new EntryPoint(8,7),new EntryPoint(8,8)}
		};
		
		// Maps sub square to index
		private readonly byte[,] m_subSquare =
			new byte[,]
			{
				{0,0,0,1,1,1,2,2,2},
				{0,0,0,1,1,1,2,2,2},
				{0,0,0,1,1,1,2,2,2},
				{3,3,3,4,4,4,5,5,5},
				{3,3,3,4,4,4,5,5,5},
				{3,3,3,4,4,4,5,5,5},
				{6,6,6,7,7,7,8,8,8},
				{6,6,6,7,7,7,8,8,8},
				{6,6,6,7,7,7,8,8,8}
		};

        /// <summary>
        /// Sudoku byte[9,9] array
        /// </summary>
        public byte[,] Data
        {
            get
            {
                return m_sudoku.Clone() as byte[,];
            }

            set
            {
                if (value.Rank == 2 && value.GetUpperBound(0) == 8 && value.GetUpperBound(1) == 8)
                    m_sudoku = value.Clone() as byte[,];
                else
                    throw new Exception("Array has wrong size");
            }
        }

        IRandomizer randomizer = new DefaultRandomizer();

        public IRandomizer Randomizer
        {
            get
            {
                return randomizer;
            }
            set
            {
                randomizer = value;
            }
        }

		/// <summary>
		/// Solves the given Sudoku.
		/// </summary>
		/// <returns>Success</returns>
		public bool Solve()
		{
			// Find untouched location with most information
			int xp = 0;
			int yp = 0;
			byte[]	Mp	=	null;
			int cMp = 10;

			for(int y = 0; y < 9; y++)
			{
				for(int x = 0; x < 9; x++)
				{
					// Is this spot unused?
					if(m_sudoku[y,x] == 0)
					{
						// Set M of possible solutions
						byte[] M = {0,1,2,3,4,5,6,7,8,9};

						// Remove used numbers in the vertical direction
						for(int a = 0; a < 9; a++)
							M[m_sudoku[a,x]] = 0;

						// Remove used numbers in the horizontal direction
						for(int b = 0; b < 9; b++)
							M[m_sudoku[y,b]] = 0;

						// Remove used numbers in the sub square.
						int	squareIndex = m_subSquare[y,x];
						for(int c = 0; c < 9; c++)
						{
							EntryPoint p = m_subIndex[squareIndex,c];
							M[m_sudoku[p.x,p.y]] = 0;
						}

						int cM = 0;
						// Calculate cardinality of M
						for(int d = 1; d < 10; d++)
							cM += M[d] == 0 ? 0 : 1;

						// Is there more information in this spot than in the best yet?
						if(cM < cMp)
						{
							cMp = cM;
							Mp = M;
							xp = x;
							yp = y;
						}
					}
				}
			}

			// Finished?
			if(cMp == 10)
				return true;

			// Couldn't find a solution?
			if(cMp == 0)
				return false;

			// Try elements
			for(int i = 1; i < 10; i++)
			{
				if(Mp[i] != 0)
				{
					m_sudoku[yp,xp] = Mp[i];
					if(Solve())
						return true;
				}
			}

			// Restore to original state.
			m_sudoku[yp,xp] = 0;
			return false;
		}

		/// <summary>
		/// Generate a new Sudoku from the template.
		/// </summary>
		/// <param name="spots">Number of set spots in Sudoku.</param>
        /// <param name="numberOfTries">Number of tries before ending generation.</param>
		/// <returns>(Number of tries, success)</returns>
        public Tuple<long, bool> Generate(int spots, int numberOfTries = 1000000)
		{
			// Number of set spots.
			int num = GetNumberSpots();

			if(!IsSudokuFeasible() || num > spots)
			{
				// The supplied data is not feasible.
				// - or -
				// The supplied data has too many spots set.
                return Tuple.Create(0L, false);
			}

			/////////////////////////////////////
			// Randomize spots
			/////////////////////////////////////

            var originalData = Data;

            long tries = 0;
            for (; tries < numberOfTries; tries++)
            {
                // Try to generate spots
                if (Gen(spots - num))
                {
                    // Test if unique solution.
                    if (IsSudokuUnique())
                    {
                        return Tuple.Create(tries, true);
                    }
                }

                // Start over.
                Data = originalData;
            }

            return Tuple.Create(tries, false);
		}

		/// <summary>
		/// Fast test if the data is feasible. 
		/// Does not check if there is more than one solution.
		/// </summary>
		/// <returns>True if feasible</returns>
		public bool IsSudokuFeasible()
		{
			for(int y = 0; y < 9; y++)
			{
				for(int x = 0; x < 9; x++)
				{
					// Set M of possible solutions
					byte[] M = new byte[10];

					// Count used numbers in the vertical direction
					for(int a = 0; a < 9; a++)
						M[m_sudoku[a,x]]++;
					// Sudoku feasible?
					if(!Feasible(M))
						return false;

					M = new byte[10];
					// Count used numbers in the horizontal direction
					for(int b = 0; b < 9; b++)
						M[m_sudoku[y,b]]++;
					if(!Feasible(M))
						return false;

					M = new byte[10];
					// Count used numbers in the sub square.
					int	squareIndex = m_subSquare[y,x];
					for(int c = 0; c < 9; c++)
					{
						EntryPoint p = m_subIndex[squareIndex,c];
						if(p.x != y && p.y != x)
							M[m_sudoku[p.x,p.y]]++;
					}
					if(!Feasible(M))
						return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Test generated Sudoku for solvability.
		/// A true Sudoku has one and only one solution.
		/// </summary>
		/// <returns>True if unique</returns>
		public bool IsSudokuUnique()
		{
			byte[,] m = Data;
			bool b = TestUniqueness() == Ret.Unique;
			Data = m;
			return b;
		}

		// Generate spots
		private bool Gen(int spots)
		{
            for (int i = 0; i < spots; i++)
			{
				int xRand,yRand;

				do
				{
					xRand = Randomizer.GetInt(9);
					yRand = Randomizer.GetInt(9);
				} while(m_sudoku[yRand,xRand] != 0);

				/////////////////////////////////////
				// Get feasible values for spot.
				/////////////////////////////////////

				// Set M of possible solutions
				byte[] M = {0,1,2,3,4,5,6,7,8,9};

				// Remove used numbers in the vertical direction
				for(int a = 0; a < 9; a++)
					M[m_sudoku[a,xRand]] = 0;

				// Remove used numbers in the horizontal direction
				for(int b = 0; b < 9; b++)
					M[m_sudoku[yRand,b]] = 0;

				// Remove used numbers in the sub square.
				int	squareIndex = m_subSquare[yRand,xRand];
				for(int c = 0; c < 9; c++)
				{
					EntryPoint p = m_subIndex[squareIndex,c];
					M[m_sudoku[p.x,p.y]] = 0;
				}

				int cM = 0;
				// Calculate cardinality of M
				for(int d = 1; d < 10; d++)
					cM += M[d] == 0 ? 0 : 1;

				// Is there a number to use?
				if(cM > 0)
				{
					int e = 0;

					do
					{
						// Randomize number from the feasible set M
						e =  Randomizer.GetInt(1,10);
					} while(M[e] == 0);

					// Set number in Sudoku
					m_sudoku[yRand,xRand] = (byte)e;
				}
				else
				{
					// Error
					return false;
				}
			}

			// Successfully generated a feasible set.
			return true;
		}

		private enum Ret { Unique, NotUnique, NoSolution };

		// Is there one and only one solution?
		private Ret TestUniqueness()
		{
			// Find untouched location with most information
			int xp = 0;
			int yp = 0;
			byte[]	Mp	=	null;
			int cMp = 10;

			for(int y = 0; y < 9; y++)
			{
				for(int x = 0; x < 9; x++)
				{
					// Is this spot unused?
					if(m_sudoku[y,x] == 0)
					{
						// Set M of possible solutions
						byte[] M = {0,1,2,3,4,5,6,7,8,9};

						// Remove used numbers in the vertical direction
						for(int a = 0; a < 9; a++)
							M[m_sudoku[a,x]] = 0;

						// Remove used numbers in the horizontal direction
						for(int b = 0; b < 9; b++)
							M[m_sudoku[y,b]] = 0;

						// Remove used numbers in the sub square.
						int	squareIndex = m_subSquare[y,x];
						for(int c = 0; c < 9; c++)
						{
							EntryPoint p = m_subIndex[squareIndex,c];
							M[m_sudoku[p.x,p.y]] = 0;
						}

						int cM = 0;
						// Calculate cardinality of M
						for(int d = 1; d < 10; d++)
							cM += M[d] == 0 ? 0 : 1;

						// Is there more information in this spot than in the best yet?
						if(cM < cMp)
						{
							cMp = cM;
							Mp = M;
							xp = x;
							yp = y;
						}
					}
				}
			}

			// Finished?
			if(cMp == 10)
				return Ret.Unique;

			// Couldn't find a solution?
			if(cMp == 0)
				return Ret.NoSolution;

			// Try elements
			int success = 0;
			for(int i = 1; i < 10; i++)
			{
				if(Mp[i] != 0)
				{
					m_sudoku[yp,xp] = Mp[i];

					switch(TestUniqueness())
					{
						case Ret.Unique:
							success++;
							break;

						case Ret.NotUnique:
							return Ret.NotUnique;

						case Ret.NoSolution:
							break;
					}

					// More than one solution found?
					if(success > 1)
						return Ret.NotUnique;
				}
			}

			// Restore to original state.
			m_sudoku[yp,xp] = 0;

			switch(success)
			{
				case 0:
					return Ret.NoSolution;

				case 1:
					return Ret.Unique;

				default:
					// Won't happen.
					return Ret.NotUnique;
			}
		}

        private bool Feasible(byte[] M)
        {
            for (int d = 1; d < 10; d++)
                if (M[d] > 1)
                    return false;

            return true;
        }

        private int GetNumberSpots()
        {
            int num = 0;

            for (int y = 0; y < 9; y++)
                for (int x = 0; x < 9; x++)
                    num += m_sudoku[y, x] == 0 ? 0 : 1;

            return num;
        }
    }
}
