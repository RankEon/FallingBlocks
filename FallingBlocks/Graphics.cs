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
    class Graphics
    {
        private MainWindow uiThread;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mw">Handle to main window</param>
        public Graphics(MainWindow mw)
        {
            uiThread = mw;
        }

        /// <summary>
        /// Converts cell value to blcok color
        /// </summary>
        /// <param name="cellValue">Value of individual cell to be drawn</param>
        /// <returns>Cell value converted to color string</returns>
        private string ColorEnumToString(int cellValue)
        {
            string color = "black";

            switch (cellValue)
            {
                case (int)BlockColors.BLUE:
                    color = "blue";
                    break;
                case (int)BlockColors.GREEN:
                    color = "green";
                    break;
                case (int)BlockColors.ORANGE:
                    color = "orange";
                    break;
                case (int)BlockColors.RED:
                    color = "red";
                    break;
                case (int)BlockColors.WHITE:
                    color = "white";
                    break;
                case (int)BlockColors.YELLOW:
                    color = "yellow";
                    break;
                case (int)BlockColors.GRAY:
                    color = "gray";
                    break;
            }

            return color;
        }

        /// <summary>
        /// Draws gameboard into canvas
        /// Dispatcher usage based on the MSDN examples.
        /// </summary>
        /// <param name="gameBoardState">Array containing gameboard state</param>
        public void Draw(int[,] gameBoardState)
        {
            lock(this)
            {
                uiThread.Dispatcher.BeginInvoke((Action)delegate
                {
                    uiThread.gameCanvas.Children.Clear();

                    // Draw the game board canvas block by block
                    for (int row = 1; row < gameBoardState.GetLength(0) - 1; row++)
                    {
                        for (int col = 1; col < gameBoardState.GetLength(1) - 1; col++)
                        {
                            BrushConverter bc = new BrushConverter();

                            // Create a 30 x 30 rectangle.
                            Rectangle rect = new Rectangle();
                            rect.Height = 30;
                            rect.Width = 30;

                            // Fill with appropriate color (got from the value of the gameboard cell).
                            // Default is black (empty gameboard slot).
                            if (gameBoardState[row, col] != 0)
                            {
                                rect.Fill = (Brush)bc.ConvertFromString(ColorEnumToString(gameBoardState[row, col]));
                            }
                            else
                            {
                                rect.Fill = (Brush)bc.ConvertFromString("black");
                            }

                            Canvas.SetLeft(rect, (col-1) * 30);
                            Canvas.SetTop(rect, (row-1) * 30);

                            uiThread.gameCanvas.Children.Add(rect);
                        }
                    }
                });
            }
        }

        public void ShowNextBlock(int[,] blockShape)
        {
            lock (this)
            {
                uiThread.Dispatcher.BeginInvoke((Action)delegate
                {
                    uiThread.nextBlockCanvas.Children.Clear();

                    // Draw the game board canvas block by block
                    for (int row = 0; row < blockShape.GetLength(0); row++)
                    {
                        for (int col = 0; col < blockShape.GetLength(1); col++)
                        {
                            BrushConverter bc = new BrushConverter();

                            // Create a 30 x 30 rectangle.
                            Rectangle rect = new Rectangle();
                            rect.Height = 30;
                            rect.Width = 30;

                            // Fill with appropriate color (got from the value of the gameboard cell).
                            // Default is black (empty gameboard slot).
                            if (blockShape[row, col] != 0)
                            {
                                rect.Fill = (Brush)bc.ConvertFromString(ColorEnumToString(blockShape[row, col]));
                            }
                            else
                            {
                                rect.Fill = (Brush)bc.ConvertFromString("black");
                            }

                            Canvas.SetLeft(rect, (col+1) * 30);
                            Canvas.SetTop(rect, (row+1) * 30);

                            uiThread.nextBlockCanvas.Children.Add(rect);
                        }
                    }
                });
            }
        }

        /// <summary>
        /// Draws Game Over text into canvas
        /// </summary>
        public void DrawGameOverText()
        {
            lock (this)
            {
                uiThread.Dispatcher.BeginInvoke((Action)delegate
                {
                    TextBlock txtBlock = new TextBlock();
                    txtBlock.Text = " G A M E  O V E R ";
                    txtBlock.Foreground = Brushes.Violet;
                    txtBlock.Background = Brushes.Thistle;
                    txtBlock.FontSize = 34;

                    Canvas.SetLeft(txtBlock, 15);
                    Canvas.SetTop(txtBlock, 300);
                    uiThread.gameCanvas.Children.Add(txtBlock);
                });
            }
        }
    }
}
