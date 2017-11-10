using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Windows.Controls;

namespace FallingBlocks
{
    class Block : Board
    {
        private int[,] blockOrientation;
        private bool blockCanMove = true;

        private int[] currentCoordinates = new int[2] { 0, 0 };

        // Each block's size is 30 x 30 px
        private const int blockWidth = 30;
        private const int blockHeight = 30;

        /// <summary>
        /// shape ==
        ///   lshape
        ///         x o o
        ///         x x x
        ///         o o o
        /// 
        ///   lshape_inv
        ///         o o x    
        ///         x x x
        ///         o o o
        ///   
        ///   tshape
        ///         x x x
        ///         o x o
        ///         o o o
        /// 
        ///   cube
        ///         x x o
        ///         x x o
        ///         o o o
        /// 
        ///   zshape
        ///         x o o
        ///         x x o
        ///         o x o
        ///  
        ///   zshape_inv
        ///         o x o
        ///         x x o
        ///         x o o 
        /// 
        ///   bar
        ///         o o o
        ///         1 1 1
        ///         o o o 
        /// </summary>
        /// <param name="shape"></param>
        public Block(string shape, MainWindow mw) :base(mw)
        {
            mainWindow = mw;

            switch (shape)
            {
                case "lshape":
                    blockOrientation = new int[3,3]
                        {
                            {(int) BlockColors.RED, 0,                     0},
                            {(int) BlockColors.RED, (int) BlockColors.RED, (int) BlockColors.RED},
                            {0,                     0,                     0}
                        };
                    break;
                case "lshape_inv":
                    blockOrientation = new int[3, 3]
                        {
                            {0,                      0,                      (int) BlockColors.BLUE},
                            {(int) BlockColors.BLUE, (int) BlockColors.BLUE, (int) BlockColors.BLUE},
                            {0,                      0,                      0}
                        };
                    break;
                case "tshape":
                    blockOrientation = new int[3, 3]
                        {
                            {(int) BlockColors.GREEN, (int) BlockColors.GREEN, (int) BlockColors.GREEN},
                            {0,                       (int) BlockColors.GREEN, 0},
                            {0,                       0,                       0}
                        };
                    break;
                case "cube":
                    blockOrientation = new int[3, 3]
                        {
                            {(int) BlockColors.YELLOW, (int) BlockColors.YELLOW, 0},
                            {(int) BlockColors.YELLOW, (int) BlockColors.YELLOW, 0},
                            {0,                        0,                        0}
                        };
                    break;
                case "zshape":
                    blockOrientation = new int[3, 3]
                        {
                            {0,                        (int) BlockColors.ORANGE, (int) BlockColors.ORANGE},
                            {(int) BlockColors.ORANGE, (int) BlockColors.ORANGE, 0},
                            {0,                        0,                        0}
                        };
                    break;
                case "zshape_inv":
                    blockOrientation = new int[3, 3]
                        {
                            {(int) BlockColors.WHITE, (int) BlockColors.WHITE, 0},
                            {0,                       (int) BlockColors.WHITE, (int) BlockColors.WHITE},
                            {0,                       0,                       0}
                        };
                    break;
                case "bar":
                    blockOrientation = new int[3, 3]
                        {
                            {0,                      0,                       0},
                            {(int) BlockColors.GRAY, (int) BlockColors.GRAY,  (int) BlockColors.GRAY},
                            {0,                      0,                       0}
                        };
                    break;
            }
        }

        /// <summary>
        /// Returns block state, whether it can move.
        /// </summary>
        /// <returns>TRUE = block can move, FALSE = block cannot move</returns>
        public bool getBlockCanMove()
        {
            return blockCanMove;
        }

        /// <summary>
        /// Sets block movable state
        /// </summary>
        /// <param name="state">TRUE = block can move, FALSE = block cannot move</param>
        public void setBlockCanMove(bool state)
        {
            blockCanMove = state;
        }

        /// <summary>
        /// Gets block orientation
        /// </summary>
        /// <returns>Block orientation matrix.</returns>
        public int[,] GetBlockOrientation()
        {
            return blockOrientation;
        }

        /// <summary>
        /// Checks if block matrix's right column is empty.
        /// </summary>
        /// <returns>TRUE = empty, FALSE = not empty</returns>
        public bool IsRightColEmpty()
        {
            return (0 != (blockOrientation[0, 2] | blockOrientation[1, 2] | blockOrientation[2, 2])) ? false : true;
        }

        public bool IsLeftColEmpty()
        {
            return (0 != (blockOrientation[0, 0] | blockOrientation[1, 0] | blockOrientation[2, 0])) ? false : true;
        }

        /// <summary>
        /// Rotate block left
        /// </summary>
        public void RotateLeft()
        {
            int[,] newArr = new int[3, 3];
            int newArr_i = 0;

            // Start from the lowest row.
            for (int i = blockOrientation.GetLength(0) - 1; i >= 0; i--)
            {
                // And move the rightmost column as a top row and so on.
                // j == row
                // i == column
                for (int j = 0; j < blockOrientation.GetLength(1); j++)
                {
                    //Console.Write(arr[j, i] + " ");
                    newArr[newArr_i, j] = blockOrientation[j, i];
                }
                newArr_i++;
                Console.Write("\n");
            }

            // ToDo: Rotation collsion detection
            blockOrientation = newArr;
        }

        /// <summary>
        /// Rotate block right
        /// </summary>
        public void RotateRight()
        {
            int[,] newArr = new int[3, 3];

            // Start from the lowest row.
            for (int i = blockOrientation.GetLength(0) - 1; i >= 0; i--)
            {
                int newArr_i = 0;
                
                // And move the rightmost colum as a low row and so on.
                // j == column
                // i == row
                for (int j = blockOrientation.GetLength(1) - 1; j >= 0; j--)
                {
                    //Console.Write(arr[j, i] + " ");
                    newArr[i, newArr_i++] = blockOrientation[j, i];
                }
                Console.Write("\n");
            }

            // ToDo: Rotation collsion detection
            blockOrientation = newArr;
        }
    }
}

