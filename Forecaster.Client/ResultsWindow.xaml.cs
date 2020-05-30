using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;

namespace Forecaster.Client
{
    /// <summary>
    /// Логика взаимодействия для ResultsWindow.xaml
    /// </summary>
    public partial class ResultsWindow : Window
    {
        private List<Dictionary<string, string>> Predictions { get; set; }

        public ResultsWindow()
        {
            InitializeComponent();
        }

        public ResultsWindow(List<Dictionary<string, string>> predictions)
        {
            InitializeComponent();

            Predictions = predictions;

            FillComparsionGrid(grid_tables, predictions);
        }

        private void FillComparsionGrid(Grid grid, List<Dictionary<string, string>> predictions)
        {
            foreach (Dictionary<string, string> prediction in predictions)
                AddResultBlockToGrid(grid, prediction);
        }

        private void AddResultBlockToGrid(Grid grid, Dictionary<string, string> prediction)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            //Grid.SetColumn(grid, grid.ColumnDefinitions.Count - 1);

            Grid resultBlock = CreateBlockGrid();

            DataGrid predictionGrid = new DataGrid
            {
                ItemsSource = prediction
            };

            AddColumn(predictionGrid, "Key", Localization.Strings.Date);
            AddColumn(predictionGrid, "Value", Localization.Strings.Price);

            predictionGrid.AutoGenerateColumns = false;

            resultBlock.Children.Add(predictionGrid);

            predictionGrid.SetValue(Grid.ColumnSpanProperty, 2);

            AddButtonToControlBlock(resultBlock, Localization.Strings.Save, 1, 0);
            AddButtonToControlBlock(resultBlock, Localization.Strings.Remove, 1, 1);

            grid.Children.Add(resultBlock);

            resultBlock.Margin = new Thickness(10);

            resultBlock.SetValue(Grid.ColumnProperty, grid.ColumnDefinitions.Count - 1);
        }

        private Grid CreateBlockGrid()
        {
            Grid resultBlock = new Grid();

            AddRow(resultBlock, 85);
            AddRow(resultBlock, 15);

            for (int i = 0; i < 2; ++i)
            {               
                resultBlock.ColumnDefinitions.Add(new ColumnDefinition());
            }

            return resultBlock;
        }

        private void AddColumn(Grid grid, int percentageWidth)
        {
            ColumnDefinition column = new ColumnDefinition
            {
                Width = new GridLength(percentageWidth, GridUnitType.Star)
            };

            grid.ColumnDefinitions.Add(column);
        }

        private void AddColumn(DataGrid grid, string binding, string header)
        {
            DataGridTextColumn textColumn = new DataGridTextColumn
            {
                Header = header,
                Binding = new Binding(binding)
            };

            grid.Columns.Add(textColumn);
        }

        private void AddRow(Grid grid, int percentageHeight)
        {
            RowDefinition row = new RowDefinition
            {
                Height = new GridLength(percentageHeight, GridUnitType.Star)
            };

            grid.RowDefinitions.Add(row);
        }

        private void AddButtonToControlBlock(Grid controlBlock, string header, int row, int column)
        {
            Button btn = new Button
            {
                Content = header
            };

            btn.Width = 75;
            btn.Height = 25;
            btn.VerticalAlignment = VerticalAlignment.Center;

            controlBlock.Children.Add(btn);

            btn.SetValue(Grid.RowProperty, row);
            btn.SetValue(Grid.ColumnProperty, column);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
