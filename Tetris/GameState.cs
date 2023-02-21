﻿namespace Tetris
{
    public class GameState
    {
        private Block currentBlock;

        public Block CurrentBlock
        {
            get => currentBlock;
            private set
            {
                currentBlock = value;
                currentBlock.Reset();

                for (int i = 0; i < 2; i++)
                {
                    currentBlock.Move(1, 0);

                    if (!BlockFits())
                    {
                        currentBlock.Move(-1, 0);
                    }
                }
            }
        }

        public GameGrid GameGrid { get; }
        public BlockQueue BlockQueue { get; }
        public bool GameOver { get; private set; }
        public int Score { get; private set; }
        public Block HeldBlock { get; private set; }
        public bool CanHold { get; private set; }
        public int delayBeforePlace { get; set; }
        public int delayBeforePlaceCounter { get; set; }
        public bool delayBeforePlaceStarted { get; set; }

        public GameState()
        {
            GameGrid = new GameGrid(22, 10);
            BlockQueue = new BlockQueue();
            CurrentBlock = BlockQueue.GetAndUpdate();
            CanHold = true;
            delayBeforePlace = 1; // Delay before the block is permanently placed, you can still move it a little bit around.
            delayBeforePlaceCounter = 0;
            delayBeforePlaceStarted = false;
        }

        private bool BlockFits()
        {
            foreach (Position p in CurrentBlock.TilePositions())
            {
                if (!GameGrid.IsEmpty(p.Row, p.Column))
                {
                    return false;
                }
            }

            return true;
        }

        public void HoldBlock()
        {
            if (!CanHold)
            {
                return;
            }

            if (HeldBlock == null)
            {
                HeldBlock = CurrentBlock;
                CurrentBlock = BlockQueue.GetAndUpdate();
            }
            else
            {
                Block tmp = CurrentBlock;
                CurrentBlock = HeldBlock;
                HeldBlock = tmp;
            }

            CanHold = false;

            // reset possible delay before place
            delayBeforePlace = 1;
        }

        public void RotateBlockCW()
        {
            CurrentBlock.RotateCW();

            if (!BlockFits())
            {
                CurrentBlock.RotateCCW();
            }
            else{
                // If the block is on the bottem and you move it the place delay gets bigger.
                if (delayBeforePlaceStarted){ AddDelayBeforePlace(); }
            }
        }

        public void RotateBlockCCW()
        {
            CurrentBlock.RotateCCW();

            if (!BlockFits())
            {
                CurrentBlock.RotateCW();
            }
            else{
                // If the block is on the bottem and you move it the place delay gets bigger.
                if (delayBeforePlaceStarted){ AddDelayBeforePlace();}
            }
        }

        public void MoveBlockLeft()
        {
            CurrentBlock.Move(0, -1);

            if (!BlockFits())
            {
                CurrentBlock.Move(0, 1);
            }
            else{
                // If the block is on the bottem and you move it the place delay gets bigger.
                if (delayBeforePlaceStarted){ AddDelayBeforePlace();}
            }
        }

        public void MoveBlockRight()
        {
            CurrentBlock.Move(0, 1);


            if (!BlockFits())
            {
                CurrentBlock.Move(0, -1);
            }
            else{
                // If the block is on the bottem and you move it the place delay gets bigger.
                if (delayBeforePlaceStarted){ AddDelayBeforePlace();}
            }
        }

        private bool IsGameOver()
        {
            return !(GameGrid.IsRowEmpty(0) && GameGrid.IsRowEmpty(1));
        }

        private void PlaceBlock()
        {
            foreach (Position p in CurrentBlock.TilePositions())
            {
                GameGrid[p.Row, p.Column] = CurrentBlock.Id;
            }

            Score += GameGrid.ClearFullRows();

            if (IsGameOver())
            {
                GameOver = true;
            }
            else
            {
                CurrentBlock = BlockQueue.GetAndUpdate();
                CanHold = true;
            }
        }

        public void MoveBlockDown()
        {
            CurrentBlock.Move(1, 0);

            if (!BlockFits())
            {
                CurrentBlock.Move(-1, 0);
                delayBeforePlaceStarted = true;
                if(delayBeforePlaceCounter >= delayBeforePlace)
                {
                    PlaceBlock();
                    delayBeforePlaceCounter = 0;
                    delayBeforePlace = 1;
                    delayBeforePlaceStarted = false;
                }
                else
                {
                    delayBeforePlaceCounter++;
                }
            }
        }

        private int TileDropDistance(Position p)
        {
            int drop = 0;

            while (GameGrid.IsEmpty(p.Row + drop + 1, p.Column))
            {
                drop++;
            }

            return drop;
        }

        public int BlockDropDistance()
        {
            int drop = GameGrid.Rows;

            foreach (Position p in CurrentBlock.TilePositions())
            {
                drop = System.Math.Min(drop, TileDropDistance(p));
            }

            return drop;
        }

        public void DropBlock()
        {
            CurrentBlock.Move(BlockDropDistance(), 0);
            PlaceBlock();
        }

        public void AddDelayBeforePlace()
        {
            if(delayBeforePlace <= 3)
            {
                delayBeforePlace += 1;
            }
        }
    }
}
