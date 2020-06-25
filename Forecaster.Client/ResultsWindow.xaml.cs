using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
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

            Button btn_Save = AddButtonToControlBlock(resultBlock, Localization.Strings.Save, 1, 0);
            Button btn_Remove = AddButtonToControlBlock(resultBlock, Localization.Strings.Remove, 1, 1);

            btn_Remove.Click += btn_remove_Click;
            btn_Save.Click += btn_save_Click;

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

        private Button AddButtonToControlBlock(Grid controlBlock, string header, int row, int column)
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

            return btn;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btn_remove_Click(object sender, RoutedEventArgs e)
        {
            var btn_remove = (Button)sender;

            var parentGrid = (Grid)btn_remove.Parent;

            var windowGrid = (Grid)parentGrid.Parent;

            var columnNum = (int)parentGrid.GetValue(Grid.ColumnProperty);

            windowGrid.Children.Remove(parentGrid);

            if (windowGrid.ColumnDefinitions.Count > 1)
            {
                windowGrid.ColumnDefinitions[columnNum].SetValue(ColumnDefinition.WidthProperty, new GridLength(0, GridUnitType.Star));

                bool isAllColumnsHidden = true;

                foreach (ColumnDefinition column in windowGrid.ColumnDefinitions)
                    if (column.Width.Value > 0)
                        isAllColumnsHidden = false;

                if(isAllColumnsHidden)
                    this.Close();
            }
            else
                this.Close();
        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            var btn_remove = (Button)sender;

            var parentGrid = (Grid)btn_remove.Parent;

            var windowGrid = (DataGrid)parentGrid.Children[0];

            var content = ConvertToCsv(windowGrid.Items);

            SaveContent(content);
        }

        private string ConvertToCsv(ItemCollection dataGridItems)
        {
            string content = "Date,Close\n";

            foreach (object item in dataGridItems)
            {
                var dict = (KeyValuePair<string, string>)item;

                content += dict.Key.ToString() + ',' + (double.Parse(dict.Value)).ToString(CultureInfo.InvariantCulture) + '\n';
            }

            return content;
        }

        private void SaveContent(string content)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            if(saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, content);
            }
        }
    }
}
