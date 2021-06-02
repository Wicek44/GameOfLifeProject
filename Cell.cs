using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GameOfLife
{
    class Cell
    {
        public readonly Button cellButton;

        public CellState cellState { get; private set; }

        public Cell(Button argCellButton)
        {
            cellButton = argCellButton;
            cellState = CellState.Dead;
            cellButton.Click += CellButton_Click;
        }

        private void CellButton_Click(object sender, RoutedEventArgs e)
        {
            SwichSateOfCell();
        }

        public void SwichSateOfCell()
        {
            if (cellState == CellState.Alive)
            {
                cellState = CellState.Dead;
                cellButton.Background = new SolidColorBrush(Colors.White);
            }
            else if (cellState == CellState.Dead)
            {
                cellState = CellState.Alive;
                cellButton.Background = new SolidColorBrush(Colors.Black);
            }
        }


    }
}
