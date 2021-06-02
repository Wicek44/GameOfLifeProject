using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GameOfLife
{
    class SaveAndLoadManager
    {
       
        public void SaveGameBoard(GameOfLifeConfigurationModel gameOfLifeConfigurationModel)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "gol files (*.gol)|*.gol";
            if (saveFileDialog.ShowDialog() == true)
            {
                File.Delete(saveFileDialog.FileName);
                File.AppendAllText(saveFileDialog.FileName, gameOfLifeConfigurationModel.BoardSize.ToString() + "\n");
                File.AppendAllText(saveFileDialog.FileName, gameOfLifeConfigurationModel.GenerationTimeInterval.ToString() + "\n");

                for (int columnIndex = 0; columnIndex < gameOfLifeConfigurationModel.BoardSize; columnIndex++)
                {

                    for (int rowIndex = 0; rowIndex < gameOfLifeConfigurationModel.BoardSize; rowIndex++)
                    {
                        if (gameOfLifeConfigurationModel.StateOfCells[columnIndex][rowIndex])
                        {
                            File.AppendAllText(saveFileDialog.FileName, "1");
                        }
                        else
                        {
                            File.AppendAllText(saveFileDialog.FileName, "0");
                        }
                    }

                    File.AppendAllText(saveFileDialog.FileName, "\n");

                }

            }
        }

        public GameOfLifeConfigurationModel LoadGameBoard()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "gol files (*.gol)|*.gol";
            if (openFileDialog.ShowDialog() == true)
            {
                var linesOfText = File.ReadLines(openFileDialog.FileName).ToList();
                int boardSize = Int32.Parse(linesOfText[0]);
                int generationTimeInterval = Int32.Parse(linesOfText[1]);

                List<List<bool>> stateOfCells = new List<List<bool>>();

                for (int i = 2; i < boardSize + 2; i++)
                {
                    stateOfCells.Add(new List<bool>());

                    var currentRowOfGameBoard = linesOfText[i];

                    for (int j = 0; j < boardSize; j++)
                    {
                        bool isCellAlive = ConvertToBool(currentRowOfGameBoard[j]);
                        stateOfCells[i - 2].Add(isCellAlive);
                    }
                }

                GameOfLifeConfigurationModel gameOfLifeConfigurationModel = new GameOfLifeConfigurationModel(boardSize, generationTimeInterval, stateOfCells);

                return gameOfLifeConfigurationModel;
            }
            return null;
        }


        private bool ConvertToBool(char letter)
        {
            if (letter == '1')
            {
                return true;
            }
            return false;
        }

    }
}
