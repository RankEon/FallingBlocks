using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.ComponentModel;
using System.Diagnostics;


namespace FallingBlocks
{
    // Keypress directions
    enum Directions
    {
        LEFT,
        RIGHT,
        DOWN,
        ROTATION_LEFT,
        ROTATION_RIGHT,
        NONE
    }

    // Block color enumerations
    enum BlockColors
    {
        RED    = 1,
        BLUE   = 2,
        GREEN  = 3,
        YELLOW = 4,
        ORANGE = 5,
        WHITE  = 6,
        GRAY   = 7
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Block gameBlock;
        Board gameBoard;
        Graphics gameGraphics;
        
        // Gameloop is run as backgroundworker.
        BackgroundWorker gameLoop = new BackgroundWorker();

        // Game score object.
        Score score;

        // Keypress direction
        Directions keypressDirection = Directions.NONE;

        // Flag to indicate game state
        bool gameRunning = false;

        // Flag for pausing the game
        bool gamePaused = false;

        // Block x and y coordinates (top left corner of
        // the block matrix.
        int block_x = 0;
        int block_y = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            score = new Score { GameScore = Convert.ToString(0) };
            this.DataContext = score;

            labelPause.Content = "";

            // Initialize gameloop
            gameLoop.DoWork += gameLoop_DoWork;
            gameLoop.RunWorkerCompleted += gameLoop_RunWorkerCompleted;
            gameLoop.WorkerReportsProgress = true;
            gameLoop.WorkerSupportsCancellation = true;
            gameBoard = new Board(this);

            //
#if !DEBUG
            debug_textBox.FontFamily = new FontFamily("Courier New");
            debug_textBox.AppendText("Controls:\r\n");
            debug_textBox.AppendText("=========================\r\n");
            debug_textBox.AppendText("Move right  : Right arrow\r\n");
            debug_textBox.AppendText("Move left   : Left arrow\r\n");
            debug_textBox.AppendText("Quick down  : Down arrow\r\n");
            debug_textBox.AppendText("Rotate right: X\r\n");
            debug_textBox.AppendText("Rotate left : Z\r\n");
            debug_textBox.AppendText("Pause game  : P\r\n");
#endif
        }

        /// <summary>
        /// Gameloop
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Work events</param>
        private void gameLoop_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            // Initialize randomizer
            Randomizer blockRandomizer = new Randomizer();

            // Record current time and elapsed time
            DateTime start = DateTime.Now;
            TimeSpan elapsedTime; // = currentTime - lastTime;
            int dropSpeed = 900;  // Block drop speed, in ms
            gameRunning = true;   // Set gameRunning flag to true.

            // Create Graphics object
            gameGraphics = new Graphics(this);

            // Create block object
            gameBlock = new Block(blockRandomizer.RandomizeBlock(), this);

            // Next block
            Block nextBlock = new Block(blockRandomizer.RandomizeBlock(), this);
            gameGraphics.ShowNextBlock(nextBlock.GetBlockOrientation());
            
            // Initial coordinates
            int initial_x = 4;
            int initial_y = -1;
            block_x = initial_x;
            block_y = initial_y;

            int previousScore = 0;

            // Gameloop
            while (gameRunning == true)
            {
                if (!gamePaused)
                {
                    // Record elapsed time.
                    elapsedTime = DateTime.Now - start;

                    // Handle keypresses immediately.
                    // Else update gamestate
                    if (keypressDirection != Directions.NONE && gameBlock.getBlockCanMove())
                    {
                        if (gameBoard.CheckBlockCollision(gameBlock.GetBlockOrientation(), keypressDirection, block_x, block_y))
                        {
                            switch (keypressDirection)
                            {
                                case Directions.ROTATION_LEFT:
                                    gameBlock.RotateRight();
                                    break;
                                case Directions.RIGHT:
                                    if(block_x > 0)
                                        block_x--;
                                    break;
                                case Directions.LEFT:
                                    if(block_x < 10)
                                        block_x++;
                                    break;
                            }
                        }
                        else
                        {
                            gameBoard.UpdateDynamicGameBoard(gameBlock.GetBlockOrientation(), block_x, block_y);
                        }
                        gameGraphics.Draw(gameBoard.GetGameBoardDynamicState());
                        keypressDirection = Directions.NONE;
                    }

                    // Drop the block once the update timer has triggered.
                    if (elapsedTime.Milliseconds >= dropSpeed)
                    {
                        block_y++;

                        // Check collision
                        if (gameBlock.getBlockCanMove() &&
                            gameBoard.CheckBlockCollision(gameBlock.GetBlockOrientation(), Directions.DOWN, block_x, block_y))
                        {
                            gameBoard.UpdateStaticGameBoard();
                            gameBlock.setBlockCanMove(false);
                            int rows = gameBoard.CheckAndRemoveCompleteRows();

                            // If full horizontal rows are detected, add into score.
                            if (rows > 0)
                            {
                                // If more than one horizontal rows were completed, multiply
                                // the score with the number of rows as a bonus points.
                                if (rows > 1)
                                {
                                    rows *= rows;
                                }
                                int totalscore = Convert.ToInt16(score.GameScore) + rows;
                                score.GameScore = Convert.ToString(totalscore);

                                if (previousScore == 0 && totalscore > 15)
                                {
                                    dropSpeed -= 150;
                                    previousScore = totalscore;
                                }
                                else if ((totalscore - previousScore > 15) && dropSpeed >= 200)
                                {
                                    dropSpeed -= 150;
                                    previousScore = totalscore;
                                }
                            }

                            // Game over?
                            if (block_y <= 0)
                            {
                                gameGraphics.DrawGameOverText();
                                gameRunning = false;
                                break;
                            }
                        }
                        else if (gameBlock.getBlockCanMove())
                        {
                            // Updating dynamic board, provided that the block can move.
                            gameBoard.UpdateDynamicGameBoard(gameBlock.GetBlockOrientation(), block_x, block_y);
                        }
                        else if (!gameBlock.getBlockCanMove())
                        {
                            // Block has collided, thus reset block coordinates and randomize a new block.
                            block_x = initial_x;
                            block_y = initial_y;

                            gameBlock = nextBlock; // new Block(blockRandomizer.RandomizeBlock(), this);

                            nextBlock = new Block(blockRandomizer.RandomizeBlock(), this);
                            gameGraphics.ShowNextBlock(nextBlock.GetBlockOrientation());

                            gameBlock.setBlockCanMove(true);
                        }

                        // Draw gameboard and reset timer.
                        gameGraphics.Draw(gameBoard.GetGameBoardDynamicState());
                        start = DateTime.Now;
                    }

                    // Check if we are exiting the game while running
                    if (gameLoop.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }
                } // if(!gamePaused)

                // 1ms sleep to save cpu cycles
                System.Threading.Thread.Sleep(1);
            }
        }

        private void gameLoop_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            buttonStartGame.IsEnabled = true;
        }

        /// <summary>
        /// Handles "Start game" -button click.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event arguments</param>
        private void button_Click(object sender, RoutedEventArgs e)
        {
            buttonStartGame.IsEnabled = false;
            score.GameScore = Convert.ToString(0);
            gameBoard.ClearGameBoardStates();
            gameLoop.RunWorkerAsync();
        }

        /// <summary>
        /// Handles keypress events.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event arguments.</param>
        private void main_KeyDownEventHandler(object sender, KeyEventArgs e)
        {
            if (!gameBlock.getBlockCanMove())
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Right:
                    if (block_x < 8 || block_x == 8 && gameBlock.IsRightColEmpty())
                    {
                        block_x++;
                    }

                    keypressDirection = Directions.RIGHT;
#if DEBUG
                    debug_textBox.AppendText("Key: Right Arrow (" + block_x + ")\r\n");
#endif
                    break;

                case Key.Left:
                    if (block_x > 1 || (block_x == 1 && true == gameBlock.IsLeftColEmpty()))
                    {
                        block_x--;
                    }

                    keypressDirection = Directions.LEFT;
#if DEBUG
                    debug_textBox.AppendText("Key: Left Arrow (" + block_x + ")\r\n");
#endif
                    break;

                case Key.Z:
                    if (block_y < 0)
                    {
                        return;
                    }

                    gameBlock.RotateLeft();
                    keypressDirection = Directions.ROTATION_LEFT;
#if DEBUG
                    debug_textBox.AppendText("Key: Rotate Left (Z)\r\n");
#endif
                    break;

                case Key.X:
                    if (block_y < -1)
                    {
                        return;
                    }

                    gameBlock.RotateRight();
                    keypressDirection = Directions.ROTATION_RIGHT;
#if DEBUG
                    debug_textBox.AppendText("Key: Rotate Right (X)\r\n");
#endif
                    break;

                case Key.Down:
                    block_y++;
                    keypressDirection = Directions.DOWN;
#if DEBUG
                    debug_textBox.AppendText("Key: Down Arrow\r\n");
#endif
                    break;

                case Key.P:
                    gamePaused = (gamePaused) ? false : true;

                    if (gamePaused)
                    {
                        labelPause.Content = "Game Paused ...";
                    }
                    else
                    {
                        labelPause.Content = "";
                    }
#if DEBUG
                    debug_textBox.AppendText("Key: Game pause (P)\r\n");
#endif
                    break;
            }

            debug_textBox.ScrollToEnd();
        }


        /// <summary>
        /// Event handler for closing of game window
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event arguments</param>
        private void applicationClosing(object sender, CancelEventArgs e)
        {
            debug_textBox.AppendText("exiting");
            if (gameLoop != null)
            {
                gameLoop.CancelAsync();
            }
        }
    }
}
