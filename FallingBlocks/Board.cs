using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FallingBlocks
{
    class Board
    {
        // Board size is 20 x 10, but we have one row and two columns over
        // the edge so that empty edges in left, right and bottom -side of
        // the tetrimino can be put outside the board.
        private const int gameBoardWidth = 12;
        private const int gameBoardHeight = 22;

        // Originally Tetris board was rows x cols = 20 x 10

        // Dynamic gameboard, this is where the block is moved
        private int[,] gameBoardDynamicState = new int[gameBoardHeight, gameBoardWidth];

        // Static gameboard, stores the static state of the gameboard (i.e. fallen blocks,
        // falling block if not collision detected).
        private int[,] gameBoardStaticState = new int[gameBoardHeight, gameBoardWidth];

        // Temporary gameboard, used when checking for completed rows.
        private int[,] gameBoardTemp = new int[gameBoardHeight, gameBoardWidth];

        // Gameboard visible dimensions in pixels
        private const int gameBoardWidthPx = 300;
        private const int gameBoardHeightPx = 600;

        // Reference to the UI Thread
        protected MainWindow mainWindow;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mw"></param>
        public Board(MainWindow mw)
        {
            mainWindow = mw;
            ClearGameBoardStates();
        }

        /// <summary>
        /// Clears and initializes gameboard with default state
        /// </summary>
        /// <param name="board">Reference to the gameboard</param>
        private void ClearGameBoard(ref int[,] board)
        {
            for (int row = 0; row < gameBoardHeight; row++)
            {
                for (int col = 0; col < gameBoardWidth; col++)
                {
                    board[row, col] = (row == 21) ? 1 : 0;
                    board[row, col] = (col == 0) ? 1 : 0;
                    board[row, col] = (col == gameBoardWidth - 1) ? 1 : 0;
                }
            }
        }

        /// <summary>
        /// Copies the status of static gameboard to the dynamic gameboard.
        /// </summary>
        private void CopyStaticStateToDynamicState()
        {
            ClearGameBoard(ref gameBoardDynamicState);

            for (int row = 0; row < gameBoardHeight; row++)
            {
                for (int col = 0; col < gameBoardWidth; col++)
                {
                    gameBoardDynamicState[row, col] = gameBoardStaticState[row, col];
                }
            }
        }

        /// <summary>
        /// Clears and initializes both gameboards (static and dynamic) with
        /// default state.
        /// </summary>
        public void ClearGameBoardStates()
        {
            ClearGameBoard(ref gameBoardDynamicState);
            ClearGameBoard(ref gameBoardStaticState);
        }

        /// <summary>
        /// Returns dynamic gameboard
        /// </summary>
        /// <returns>Dynamic gameboard array</returns>
        public int[,] GetGameBoardDynamicState()
        {
            return gameBoardDynamicState;
        }

        /// <summary>
        /// Returns static gameboard
        /// </summary>
        /// <returns>Static gameboard array</returns>
        public int[,] GetGameBoardStaticState()
        {
            return gameBoardStaticState;
        }

        /// <summary>
        /// Updates static gameboard with the block placement on the
        /// dynamic board.
        /// </summary>
        public void UpdateStaticGameBoard()
        {
            for (int blockRow = 0; blockRow < gameBoardDynamicState.GetLength(0); blockRow++)
            {
                for (int blockCol = 0; blockCol < gameBoardDynamicState.GetLength(1); blockCol++)
                {
                    gameBoardStaticState[blockRow, blockCol] = gameBoardDynamicState[blockRow, blockCol];
                }
            }

            // Static state is copied to dynamic board state as we cannot move
            // the block anymore.
            CopyStaticStateToDynamicState();
        }

        /// <summary>
        /// Updates static gameboard with completed rows (which are removed).
        /// </summary>
        private void UpdateStaticGameBoardCompletedRows()
        {
            for (int row = 0; row < gameBoardTemp.GetLength(0); row++)
            {
                for (int col = 0; col < gameBoardTemp.GetLength(1); col++)
                {
                    gameBoardStaticState[row, col] = gameBoardTemp[row, col];
                }
            }
        }

        /// <summary>
        /// Updates dynamic gameboard with current block state and orientation.
        /// </summary>
        /// <param name="blockState">Current block state and orientation</param>
        /// <param name="col">Current row (top left corner of the block)</param>
        /// <param name="row">Current column (top left corner of the block)</param>
        public void UpdateDynamicGameBoard(int[,] blockState, int col, int row)
        {
            try
            {
                //gameBoardDynamicState = gameBoardStaticState.co;
                CopyStaticStateToDynamicState();

                for (int blockRow = 0; blockRow < blockState.GetLength(0); blockRow++)
                {
                    for (int blockCol = 0; blockCol < blockState.GetLength(1); blockCol++)
                    {
                        if (blockState[blockRow, blockCol] != 0)
                        {
                            gameBoardDynamicState[row + blockRow, col + blockCol] = blockState[blockRow, blockCol];
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // Ignore
                if (col < 0)
                {
                    ;
                }
            }
        }

        /// <summary>
        /// Checks if the block collides to gameboard borders or with another blocks
        /// in the gameboard.
        /// </summary>
        /// <param name="blockState">Current block state and orientation</param>
        /// <param name="direction">The block's direction of movement</param>
        /// <param name="col">Current row (top left corner of the block)</param>
        /// <param name="row">Current column (top left corner of the block)</param>
        /// <returns></returns>
        public bool CheckBlockCollision(int[,] blockState, Directions direction, int col, int row)
        {
            // ToDo: Add try-catch to prevent illegal block moves when block is partially visible.

            bool collision = false;

            switch (direction)
            {
                case Directions.DOWN:

                    if ((row >= 20 && IsBlockBoxLowRowEmpty(blockState)) ||
                        (row >= 19 && !IsBlockBoxLowRowEmpty(blockState)))
                    {
                        return true;
                    }

                    int gameboardCol = 0;

                    for (int blockRow = 0; blockRow < blockState.GetLongLength(0); blockRow++)
                    {
                        for (int blockCol = 0; blockCol < blockState.GetLongLength(1); blockCol++)
                        {
                            gameboardCol = col + blockCol;

                            if (blockState[blockRow, blockCol] != 0 && gameBoardStaticState[row + blockRow, gameboardCol] != 0)
                            {
                                collision = true;
                            }
                        }
                    }
                    break;

                case Directions.RIGHT:
                    if (col > 8 && 1 == (blockState[0, 2] | blockState[1, 2] | blockState[2, 2]))
                    {
                        return true;
                    }

                    /*
                    for (int i = 0; i < 3; i++)
                    {
                        if (blockState[i, 2] != 0 && gameBoardStaticState[row + i, col + 2] != 0)
                        {
                            collision = true;
                        }
                    }*/

                    for(int iRow = 0; iRow < 3; iRow++)
                    {
                        for (int iCol = 0; iCol < 3; iCol++)
                        {
                            if (blockState[iRow, iCol] != 0 && gameBoardStaticState[row + iRow, col + iCol] != 0)
                            {
                                collision = true;
                            }
                        }
                    }

                    break;

                case Directions.LEFT:
                    if (col < 1 && 1 == (blockState[0, 0] | blockState[1, 0] | blockState[2, 0]))
                    {
                        return true;
                    }

                    for (int iRow = 0; iRow < 3; iRow++)
                    {
                        for (int iCol = 0; iCol < 3; iCol++)
                        {
                            if (blockState[iRow, iCol] != 0 && gameBoardStaticState[row + iRow, col + iCol] != 0)
                            {
                                collision = true;
                            }

                        }
                    }

                    break;

                case Directions.ROTATION_LEFT:
                    if (col < 1)
                        return true;

                    for (int blockRow = 0; blockRow < blockState.GetLongLength(0); blockRow++)
                    {
                        for (int blockCol = 0; blockCol < blockState.GetLongLength(1); blockCol++)
                        {
                            gameboardCol = col + blockCol;

                            if (blockState[blockRow, blockCol] != 0 && gameBoardStaticState[row + blockRow, gameboardCol] != 0)
                            {
                                collision = true;
                            }
                        }
                    }
                    break;
            }
            return collision;
        }

        /// <summary>
        /// Checks whether there are completed horizontal rows in the gameboard
        /// and removes any complete rows, calculates the amount of rows and
        /// updates the gameboard state.
        /// </summary>
        /// <returns>Number of completed rows.</returns>
        public int CheckAndRemoveCompleteRows()
        {
            // Number of complete rows
            int numRows = 0;

            // List of complete row numbers.
            List<int> completeRowIds = new List<int>();

            // Look for completed rows from the static gameboard.
            for (int row = 0; row < gameBoardStaticState.GetLength(0) - 1; row++)
            {
                bool rowComplete = true;

                for (int col = 1; col < gameBoardStaticState.GetLength(1) - 1; col++)
                {
                    if (gameBoardStaticState[row, col] == 0)
                    {
                        rowComplete = false;
                    }
                }

                if (rowComplete == true)
                {
                    completeRowIds.Add(row);
                    numRows++;
                }
            }

            // If we found completed rows, collect the completed rows to a temporary
            // array (from static gameboard) and update the states.
            if (numRows != 0)
            {
                ClearGameBoard(ref gameBoardTemp);
                int gameBoardTempRow = gameBoardTemp.GetLength(0) - 1;

                for (int row = gameBoardStaticState.GetLength(0) - 1; row > 0 ; row--)
                {
                    if (!completeRowIds.Contains(row))
                    {
                        for (int col = 1; col < gameBoardStaticState.GetLength(1) - 1; col++)
                        {
                            gameBoardTemp[gameBoardTempRow, col] = gameBoardStaticState[row, col];
                        }
                        gameBoardTempRow--;
                    }
                }

                // Update static and dynamic gameboards.
                UpdateStaticGameBoardCompletedRows();
                CopyStaticStateToDynamicState();
            }

            return numRows;
        }

        /// <summary>
        /// Checks whether block matrix's low row is empty
        /// </summary>
        /// <param name="block">Block state matrix.</param>
        /// <returns>TRUE = low row is empty, FALSE = low row is not empty</returns>
        public bool IsBlockBoxLowRowEmpty(int[,] block)
        {
            return (0 != (block[2, 0] | block[2, 1] | block[2, 2])) ? false : true;
        }
    }
}

