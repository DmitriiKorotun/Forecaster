using Forecaster.Client.MVVM.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Forecaster.Client.MVVM.ViewModels
{
    class PredictionsComparsionViewModel : ArgumentfulViewModel
    {
        public ObservableCollection<ComparsionItemControlData> Data { get; set; }

        public ICommand RemoveRowCommand { get; set; }

        public RelayCommand<ComparsionItemControlData> Command1 { get; set; }

        public RelayCommand<ComparsionItemControlData> Command2 { get; set; }

        public PredictionsComparsionViewModel(List<Dictionary<string, string>> predictions)
        {
            Predictions = predictions;

            IEnumerable<ComparsionItemControlData> dataToShow = CreateComparsionItemControlDataRange(Predictions);

            Data = new ObservableCollection<ComparsionItemControlData>(dataToShow);

            RemoveRowCommand = new RelayCommand(RemoveRow);
            Command1 = new RelayCommand<ComparsionItemControlData>(ExecuteCommand1);
            Command2 = new RelayCommand<ComparsionItemControlData>(ExecuteCommand2);

        }

        private IEnumerable<ComparsionItemControlData> CreateComparsionItemControlDataRange(List<Dictionary<string, string>> predictions)
        {
            List<ComparsionItemControlData> itemControlDataList = new List<ComparsionItemControlData>(predictions.Count);

            foreach (Dictionary<string, string> algorithmResult in predictions)
                itemControlDataList.Add(new ComparsionItemControlData() { Predictions = algorithmResult });

            return itemControlDataList;
        }

        private void RemoveRow()
        {
            Data.Add(new ComparsionItemControlData() { Predictions = new Dictionary<string, string>() { { "lol", "kek2" }, { "lol3", "kek4" }, { "lol5", "kek6" } } });
        }

        private void ExecuteCommand1(ComparsionItemControlData data)
        {
            MessageBox.Show("Command1 - " + data.Predictions["1"]);
        }

        private void ExecuteCommand2(ComparsionItemControlData data)
        {
            MessageBox.Show("Command2 - " + data.Predictions["lol"]);
        }


        private List<Dictionary<string, string>> Predictions { get; set; }

        protected override void AssignArguments()
        {
            if (Args.Count() > 0)
            {
                if (Args.ElementAt(0) is List<Dictionary<string, string>> predictions)
                    Predictions = predictions;
                else
                    throw new ArgumentException("Error with passed arguments order or no predictions' argument at all");
            }
            else
                throw new ArgumentNullException("Passed empty arguments to view-model");
        }

        //private void FillComparsionGrid(Grid grid, List<Dictionary<string, string>> predictions)
        //{
        //    foreach (Dictionary<string, string> prediction in predictions)
        //        AddResultBlockToGrid(grid, prediction);
        //}

        //private void AddResultBlockToGrid(Grid grid, Dictionary<string, string> prediction)
        //{
        //    grid.ColumnDefinitions.Add(new ColumnDefinition());

        //    //Grid.SetColumn(grid, grid.ColumnDefinitions.Count - 1);

        //    Grid resultBlock = CreateBlockGrid();

        //    DataGrid predictionGrid = new DataGrid
        //    {
        //        ItemsSource = prediction
        //    };

        //    AddColumn(predictionGrid, "Key", Localization.Strings.Date);
        //    AddColumn(predictionGrid, "Value", Localization.Strings.Price);

        //    predictionGrid.AutoGenerateColumns = false;

        //    resultBlock.Children.Add(predictionGrid);

        //    predictionGrid.SetValue(Grid.ColumnSpanProperty, 2);

        //    Button btn_Save = AddButtonToControlBlock(resultBlock, Localization.Strings.Save, 1, 0);
        //    Button btn_Remove = AddButtonToControlBlock(resultBlock, Localization.Strings.Remove, 1, 1);

        //    btn_Remove.Click += btn_remove_Click;
        //    btn_Save.Click += btn_save_Click;

        //    grid.Children.Add(resultBlock);

        //    resultBlock.Margin = new Thickness(10);

        //    resultBlock.SetValue(Grid.ColumnProperty, grid.ColumnDefinitions.Count - 1);
        //}

        //private Grid CreateBlockGrid()
        //{
        //    Grid resultBlock = new Grid();

        //    AddRow(resultBlock, 85);
        //    AddRow(resultBlock, 15);

        //    for (int i = 0; i < 2; ++i)
        //    {
        //        resultBlock.ColumnDefinitions.Add(new ColumnDefinition());
        //    }

        //    return resultBlock;
        //}

        //private void AddColumn(Grid grid, int percentageWidth)
        //{
        //    ColumnDefinition column = new ColumnDefinition
        //    {
        //        Width = new GridLength(percentageWidth, GridUnitType.Star)
        //    };

        //    grid.ColumnDefinitions.Add(column);
        //}

        //private void AddColumn(DataGrid grid, string binding, string header)
        //{
        //    DataGridTextColumn textColumn = new DataGridTextColumn
        //    {
        //        Header = header,
        //        Binding = new Binding(binding)
        //    };

        //    grid.Columns.Add(textColumn);
        //}

        //private void AddRow(Grid grid, int percentageHeight)
        //{
        //    RowDefinition row = new RowDefinition
        //    {
        //        Height = new GridLength(percentageHeight, GridUnitType.Star)
        //    };

        //    grid.RowDefinitions.Add(row);
        //}

        //private Button AddButtonToControlBlock(Grid controlBlock, string header, int row, int column)
        //{
        //    Button btn = new Button
        //    {
        //        Content = header
        //    };

        //    btn.Width = 75;
        //    btn.Height = 25;
        //    btn.VerticalAlignment = VerticalAlignment.Center;

        //    controlBlock.Children.Add(btn);

        //    btn.SetValue(Grid.RowProperty, row);
        //    btn.SetValue(Grid.ColumnProperty, column);

        //    return btn;
        //}

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    Close();
        //}

        //private void btn_remove_Click(object sender, RoutedEventArgs e)
        //{
        //    var btn_remove = (Button)sender;

        //    var parentGrid = (Grid)btn_remove.Parent;

        //    var windowGrid = (Grid)parentGrid.Parent;

        //    var columnNum = (int)parentGrid.GetValue(Grid.ColumnProperty);

        //    windowGrid.Children.Remove(parentGrid);

        //    if (windowGrid.ColumnDefinitions.Count > 1)
        //    {
        //        windowGrid.ColumnDefinitions[columnNum].SetValue(ColumnDefinition.WidthProperty, new GridLength(0, GridUnitType.Star));

        //        bool isAllColumnsHidden = true;

        //        foreach (ColumnDefinition column in windowGrid.ColumnDefinitions)
        //            if (column.Width.Value > 0)
        //                isAllColumnsHidden = false;

        //        if (isAllColumnsHidden)
        //            this.Close();
        //    }
        //    else
        //        this.Close();
        //}

        //private void btn_save_Click(object sender, RoutedEventArgs e)
        //{
        //    var btn_remove = (Button)sender;

        //    var parentGrid = (Grid)btn_remove.Parent;

        //    var windowGrid = (DataGrid)parentGrid.Children[0];

        //    var content = ConvertToCsv(windowGrid.Items);

        //    SaveContent(content);
        //}

        //private string ConvertToCsv(ItemCollection dataGridItems)
        //{
        //    string content = "Date,Close\n";

        //    foreach (object item in dataGridItems)
        //    {
        //        var dict = (KeyValuePair<string, string>)item;

        //        content += dict.Key.ToString() + ',' + (double.Parse(dict.Value)).ToString(CultureInfo.InvariantCulture) + '\n';
        //    }

        //    return content;
        //}

        //private void SaveContent(string content)
        //{
        //    SaveFileDialog saveFileDialog = new SaveFileDialog();

        //    if (saveFileDialog.ShowDialog() == true)
        //    {
        //        File.WriteAllText(saveFileDialog.FileName, content);
        //    }
        //}
    }
}
