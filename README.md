# FallingBlocks
Tetris style of block game
- The game is developed with C# / Visual Studio 201x
- Created on a WPF –form.
- Game board is a canvas control, 300 x 600 pixels in size
- Each block consists of 30 x 30 pixel squares, thus the board size is 10 x 20 squares
- Game is animated and logic run with a BackgroundWorker –thread, which times the drawing of graphics and    
  animation with a constant framerate (which is increased as the game progresses) at ~900 ms and gradually speeds up to ~150 ms.
- Animation and drawing is done in real-time (canvas is double buffered).
- Tetriminos are rotated around the center axis.
- Game logic is based on two-dimensional tables, which represent the game state. One table for the static –state 
  (i.e. contains all non-moving tetriminos in the table), second one for a dynamic –state, which contains also the moving tetrimino. There is also a temporary table for handling the removal of full rows. 
   * The size of the tables are 12 x 22, i.e. two elements wider and higher than the actual visible gameboard  
     for handling the tetrimino rotation).