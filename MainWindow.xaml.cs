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

namespace GameOfLife
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GameOfLifeManager gameOfLifeManager;
        SaveAndLoadManagerJson<GameOfLifeConfigurationModel> saveAndLoadManager;  

        public MainWindow()
        {
            InitializeComponent();
            gameOfLifeManager = new GameOfLifeManager();
            saveAndLoadManager = new SaveAndLoadManagerJson<GameOfLifeConfigurationModel>();
            DataContext = gameOfLifeManager;
            gameOfLifeManager.Initialize(); 
        }

        private void ClearGameBoardButton_Click(object sender, RoutedEventArgs e)
        {
            gameOfLifeManager.ClearGameBoard();
        }

        private void NextGenerationButton_Click(object sender, RoutedEventArgs e)
        {
            gameOfLifeManager.NextGeneration();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            gameOfLifeManager.StartCellLifeSimulation();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            gameOfLifeManager.StopCellLifeSimulation();
        }

        private void RandomlyButton_Click(object sender, RoutedEventArgs e)
        {
            gameOfLifeManager.RandomlyGeneratedAliveCells();
        }

        private void SaveGameField_Click(object sender, RoutedEventArgs e)
        {
            saveAndLoadManager.Save(gameOfLifeManager.ExportConfig());
        }

        private void LoadGameField_Click(object sender, RoutedEventArgs e)
        {
            var gameOfLifeConfigurationModel = saveAndLoadManager.Load();
            gameOfLifeManager.ImportConfig(gameOfLifeConfigurationModel);
        }

        private void ExitGameField_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); 
        }
    }
}
