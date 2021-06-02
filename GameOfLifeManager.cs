using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GameOfLife
{
    class GameOfLifeManager : INotifyPropertyChanged
    {

        private const int BOARD_MIN_SIZE = 5;
        private const int BOARD_MAX_SIZE = 50;
        private const int GENERATION_LOWEST_INTERVAL = 2000;
        private const int GENERATION_HIGHEST_INTERVAL = 200;
        private const float ALIVE_CELLS_RATIO = 0.6f;

        private readonly MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        Timer generationTimer = new System.Timers.Timer();

        public List<List<Cell>> Cells { get; } = new List<List<Cell>>();

        private int _boardSize;
        public int BoardSize
        {
            get
            {
                return _boardSize;
            }

            set
            {
                if (value < BOARD_MIN_SIZE)
                {
                    _boardSize = BOARD_MIN_SIZE;
                }
                else if (value > BOARD_MAX_SIZE)
                {
                    _boardSize = BOARD_MAX_SIZE;
                }
                else
                {
                    _boardSize = value;
                }

                OnPropertyChanged();
                Initialize();
            }

        }

        private int _generationTimeInterval;

        public int GenerationTimeInterval
        {
            get
            {
                return _generationTimeInterval;
            }

            set
            {

                if (value < GENERATION_HIGHEST_INTERVAL)
                {
                    _generationTimeInterval = GENERATION_HIGHEST_INTERVAL;
                }
                else if (value > GENERATION_LOWEST_INTERVAL)
                {
                    _generationTimeInterval = GENERATION_LOWEST_INTERVAL;
                }
                else
                {
                    _generationTimeInterval = value;
                }

                generationTimer.Interval = _generationTimeInterval;

                OnPropertyChanged();
            }
        }

        public GameOfLifeManager()
        {
            BoardSize = BOARD_MIN_SIZE;
            GenerationTimeInterval = GENERATION_HIGHEST_INTERVAL;
            generationTimer.Elapsed += TimerOnElapsed;
        }

        public void Initialize()
        {
            var newBoardSizeDiff = BoardSize - mainWindow.GameOfLifeCellsField.ColumnDefinitions.Count;
            var differenceStartIndex = mainWindow.GameOfLifeCellsField.ColumnDefinitions.Count;

            if (newBoardSizeDiff >= 0)
            {
                for (int i = 0; i < newBoardSizeDiff; i++)
                {
                    ColumnDefinition columnDefinition = new ColumnDefinition();
                    RowDefinition rowDefinition = new RowDefinition();

                    mainWindow.GameOfLifeCellsField.ColumnDefinitions.Add(columnDefinition);
                    mainWindow.GameOfLifeCellsField.RowDefinitions.Add(rowDefinition);

                }

                for (int columnIndex = 0; columnIndex < mainWindow.GameOfLifeCellsField.ColumnDefinitions.Count; columnIndex++)
                {
                    int startingRowIndex;

                    if (columnIndex < differenceStartIndex)
                    {
                        startingRowIndex = differenceStartIndex;
                    }
                    else
                    {
                        Cells.Add(new List<Cell>());
                        startingRowIndex = 0;
                    }

                    for (int rowIndex = startingRowIndex; rowIndex < mainWindow.GameOfLifeCellsField.RowDefinitions.Count; rowIndex++)
                    {
                        Button fieldButton = new Button();
                        fieldButton.Style = (Style)mainWindow.FindResource("CellButtonStyle");
                        fieldButton.Background = new SolidColorBrush(Colors.White);
                        mainWindow.GameOfLifeCellsField.Children.Add(fieldButton);
                        fieldButton.SetValue(Grid.ColumnProperty, columnIndex);
                        fieldButton.SetValue(Grid.RowProperty, rowIndex);
                        Cells[columnIndex].Add(new Cell(fieldButton));
                    }
                }

            }

            else
            {
                newBoardSizeDiff = newBoardSizeDiff * (-1);
                for (int i = 0; i < newBoardSizeDiff; i++)
                {
                    mainWindow.GameOfLifeCellsField.ColumnDefinitions.RemoveAt(mainWindow.GameOfLifeCellsField.ColumnDefinitions.Count - 1);
                    mainWindow.GameOfLifeCellsField.RowDefinitions.RemoveAt(mainWindow.GameOfLifeCellsField.RowDefinitions.Count - 1);
                }

                for (int i = 0; i < Cells.Count; i++)
                {
                    var cellsInRow = Cells[i];

                    if (i < Cells.Count - newBoardSizeDiff)
                    {
                        for (int j = 0; j < newBoardSizeDiff; j++)
                        {
                            var cellToRemove = cellsInRow[cellsInRow.Count - 1];
                            mainWindow.GameOfLifeCellsField.Children.Remove(cellToRemove.cellButton);
                            cellsInRow.Remove(cellToRemove);
                        }
                    }
                    else
                    {
                        for (int j = 0; j < Cells.Count; j++)
                        {
                            var cellToRemove = cellsInRow[0];
                            mainWindow.GameOfLifeCellsField.Children.Remove(cellToRemove.cellButton);
                            cellsInRow.Remove(cellToRemove);

                        }

                    }

                }

                for (int i = 0; i < newBoardSizeDiff; i++)
                {
                    Cells.RemoveAt(Cells.Count - 1);
                }
            }

        }

        public void NextGeneration()
        {

            List<List<bool>> statesToApplyInNextGeneration = new List<List<bool>>();

            for (int columnIndex = 0; columnIndex < mainWindow.GameOfLifeCellsField.ColumnDefinitions.Count; columnIndex++)
            {
                statesToApplyInNextGeneration.Add(new List<bool>());

                for (int rowIndex = 0; rowIndex < mainWindow.GameOfLifeCellsField.RowDefinitions.Count; rowIndex++)
                {
                    bool aliveStatusCell = Cells[columnIndex][rowIndex].cellState == CellState.Alive;
                    int countOfAliveCells = GetNumberOfAliveNeighbors(columnIndex, rowIndex);
                    bool shouldCellBeAliveInNextGeneration = false;

                    if (aliveStatusCell && countOfAliveCells < 2)
                    {
                        shouldCellBeAliveInNextGeneration = false;
                    }
                    else if (aliveStatusCell && (countOfAliveCells == 2 || countOfAliveCells == 3))
                    {
                        shouldCellBeAliveInNextGeneration = true;
                    }
                    else if (!aliveStatusCell && countOfAliveCells == 3)
                    {
                        shouldCellBeAliveInNextGeneration = true;
                    }
                    else if (aliveStatusCell && countOfAliveCells == 3)
                    {
                        shouldCellBeAliveInNextGeneration = false;
                    }

                    statesToApplyInNextGeneration[columnIndex].Add(shouldCellBeAliveInNextGeneration);

                }
            }

            for (int columnIndex = 0; columnIndex < mainWindow.GameOfLifeCellsField.ColumnDefinitions.Count; columnIndex++)
            {

                for (int rowIndex = 0; rowIndex < mainWindow.GameOfLifeCellsField.RowDefinitions.Count; rowIndex++)
                {
                    bool aliveStatusCell = Cells[columnIndex][rowIndex].cellState == CellState.Alive;

                    if (aliveStatusCell != statesToApplyInNextGeneration[columnIndex][rowIndex])
                    {
                        Cells[columnIndex][rowIndex].SwichSateOfCell();
                    }
                }
            }
        }


        public void RandomlyGeneratedAliveCells()
        {
            for (int columnIndex = 0; columnIndex < mainWindow.GameOfLifeCellsField.ColumnDefinitions.Count; columnIndex++)
            {
                for (int rowIndex = 0; rowIndex < mainWindow.GameOfLifeCellsField.RowDefinitions.Count; rowIndex++)
                {
                    if (Cells[columnIndex][rowIndex].cellState == CellState.Alive)
                    {
                        Cells[columnIndex][rowIndex].SwichSateOfCell();
                    }
                }
            }

            int numberOfCellsToAlive = (int)(ALIVE_CELLS_RATIO * BoardSize * BoardSize);

            for (int i = 0; i < numberOfCellsToAlive; i++)
            {
                int randomColumnIndex;
                int randomRowIndex;

                do
                {
                    randomColumnIndex = RandomGenerator.Generator.Next(0, BoardSize);
                    randomRowIndex = RandomGenerator.Generator.Next(0, BoardSize);
                }
                while (Cells[randomColumnIndex][randomRowIndex].cellState != CellState.Dead);
                Cells[randomColumnIndex][randomRowIndex].SwichSateOfCell();

            }
        }

        public int GetNumberOfAliveNeighbors(int columnIndex, int rowIndex)
        {
            int countOfAliveCells = 0;


            //State of Cell on the top
            if (rowIndex > 0 && Cells[columnIndex][rowIndex - 1].cellState == CellState.Alive)
            {
                countOfAliveCells++;
            }

            //State of Cell on the top left 
            if (rowIndex > 0 && columnIndex > 0 && Cells[columnIndex - 1][rowIndex - 1].cellState == CellState.Alive)
            {
                countOfAliveCells++;
            }

            //State of Cell on the right
            if (columnIndex < BoardSize - 1 && Cells[columnIndex + 1][rowIndex].cellState == CellState.Alive)
            {
                countOfAliveCells++;
            }

            //State of Cell on the bottom right
            if (columnIndex < BoardSize - 1 && rowIndex < BoardSize - 1 && Cells[columnIndex + 1][rowIndex + 1].cellState == CellState.Alive)
            {
                countOfAliveCells++;
            }

            //State of Cell on the bottom
            if (rowIndex < BoardSize - 1 && Cells[columnIndex][rowIndex + 1].cellState == CellState.Alive)
            {
                countOfAliveCells++;
            }

            //State of Cell on the bottom left
            if (columnIndex > 0 && rowIndex < BoardSize - 1 && Cells[columnIndex - 1][rowIndex + 1].cellState == CellState.Alive)
            {
                countOfAliveCells++;
            }

            //State of Cell on the left
            if (columnIndex > 0 && Cells[columnIndex - 1][rowIndex].cellState == CellState.Alive)
            {
                countOfAliveCells++;
            }

            //State of Cell on the top right
            if (columnIndex < BoardSize - 1 && rowIndex > 0 && Cells[columnIndex + 1][rowIndex - 1].cellState == CellState.Alive)
            {
                countOfAliveCells++;
            }


            return countOfAliveCells;
        }

        public void ClearGameBoard()
        {
            for (int columnIndex = 0; columnIndex < mainWindow.GameOfLifeCellsField.ColumnDefinitions.Count; columnIndex++)
            {
                for (int rowIndex = 0; rowIndex < mainWindow.GameOfLifeCellsField.RowDefinitions.Count; rowIndex++)
                {
                    if (Cells[columnIndex][rowIndex].cellState == CellState.Alive)
                    {
                        Cells[columnIndex][rowIndex].SwichSateOfCell();
                    }
                }
            }
        }

        public void StartCellLifeSimulation()
        {
            generationTimer.Start();
            mainWindow.StartButton.IsEnabled = false;
            mainWindow.StopButton.IsEnabled = true;
        }

        public void StopCellLifeSimulation()
        {
            generationTimer.Stop();
            mainWindow.StartButton.IsEnabled = true;
            mainWindow.StopButton.IsEnabled = false;
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => NextGeneration());
        }

        public void ImportConfig(GameOfLifeConfigurationModel gameOfLifeConfigurationModel)
        {
            BoardSize = gameOfLifeConfigurationModel.BoardSize;
            GenerationTimeInterval = gameOfLifeConfigurationModel.GenerationTimeInterval;
            Initialize();

            for (int columnIndex = 0; columnIndex < mainWindow.GameOfLifeCellsField.ColumnDefinitions.Count; columnIndex++)
            {
                for (int rowIndex = 0; rowIndex < mainWindow.GameOfLifeCellsField.RowDefinitions.Count; rowIndex++)
                {
                    if (gameOfLifeConfigurationModel.StateOfCells[columnIndex][rowIndex] == true)
                    {
                        Cells[columnIndex][rowIndex].SwichSateOfCell();
                    }
                }
            }

        }

        public GameOfLifeConfigurationModel ExportConfig()
        {
            List<List<bool>> stateOfCells = new List<List<bool>>();

            for (int columnIndex = 0; columnIndex < mainWindow.GameOfLifeCellsField.ColumnDefinitions.Count; columnIndex++)
            {
                stateOfCells.Add(new List<bool>());

                for (int rowIndex = 0; rowIndex < mainWindow.GameOfLifeCellsField.RowDefinitions.Count; rowIndex++)
                {
                    stateOfCells[columnIndex].Add(Cells[columnIndex][rowIndex].cellState == CellState.Alive);
                }
            }

            GameOfLifeConfigurationModel gameOfLifeConfigurationModel = new GameOfLifeConfigurationModel(BoardSize, GenerationTimeInterval, stateOfCells);
            return gameOfLifeConfigurationModel;

        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
