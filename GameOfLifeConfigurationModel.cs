using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    class GameOfLifeConfigurationModel
    {
        public int BoardSize { get; }
        public int GenerationTimeInterval {get; }

        public List<List<bool>> StateOfCells { get; }

        public GameOfLifeConfigurationModel(int BoardSize, int GenerationTimeInterval, List<List<bool>> StateOfCells)
        {
            this.BoardSize = BoardSize;
            this.GenerationTimeInterval = GenerationTimeInterval;
            this.StateOfCells = StateOfCells; 
        }

    }
}
