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
using System.Windows.Shapes;

namespace ImageToText
{
    /// <summary>
    /// OrderWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class OrderWindow : Window
    {
        public string[] orderFileNames;

        public OrderWindow(string[] fileNames)
        {
            InitializeComponent();

            this.orderFileNames = fileNames;

            foreach (var (fileName, index) in fileNames.Select((value, index) => (value, index.ToString())))
            {
                var label = new Label
                {
                    Content = fileName.Split('\\').Last().Replace("_", "__"),
                    Width = this.Width / 2,
                    Height = 30
                };

                var textBox = new TextBox
                {
                    Name = "FileNameTextBox_" + index,
                    Text = index,
                    Width = 30,
                    Height = 30
                };

                LabelsStackPanel.Children.Add(label);
                OrdersStackPanel.Children.Add(textBox);
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            var orderList = new List<int>();
            foreach (var child in OrdersStackPanel.Children)
            {
                var textBox = (TextBox)child;
                if (int.TryParse(textBox.Text, out int result)){
                    orderList.Add(result);
                }
                else
                {
                    orderList.Add(OrdersStackPanel.Children.Count);
                }
            }

            this.orderFileNames = orderList.Select((value, index) => (value, orderFileNames[index])).OrderBy(v => v.value).Select(ordered=>ordered.Item2).ToArray();

            this.DialogResult = true;
            this.Close();
        }
    }
}
