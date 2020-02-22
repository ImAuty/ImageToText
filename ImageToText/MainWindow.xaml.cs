using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageToText
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string WHITE_STRING = "□";
        private const string BLACK_STRING = "■";
        private const double THRESHOLD = 0.5;
        private const string SEPARATOR = "";

        private string[] filePaths;

        public MainWindow()
        {
            InitializeComponent();
            this.Initialize();
            WhiteStringTextBox.Text = WHITE_STRING;
            BlackStringTextBox.Text = BLACK_STRING;
            ThresholdTextBox.Text = THRESHOLD.ToString();
            SeparatorTextBox.Text = SEPARATOR;
        }

        private void Initialize()
        {
            this.filePaths = null;
            FileOpenLabel.Content = "画像を選択してください";
            ImageToTextButton.IsEnabled = false;
        }

        private void FileOpenButton_Click(object sender, RoutedEventArgs e)
        {
            // ダイアログを設定する
            var dialog = new OpenFileDialog { Filter = "Image Files(*.BMP;*.JPG;*.PNG;*.GIF)|*.BMP;*.JPG;*.PNG;*.GIF" };

            // 複数選択を許可する
            dialog.Multiselect = true;

            // ファイル選択ダイアログを表示する
            var result = dialog.ShowDialog();

            if (!result.GetValueOrDefault())
            { // ファイルが選択されない場合
                this.Initialize();
                return;
            }

            // 選択されたファイルのパスを設定
            this.filePaths = dialog.FileNames;

            // ファイル名を取得する
            FileOpenLabel.Content = String.Join("\r\n", dialog.FileNames.Select(name => name.Split('\\').Last().Replace("_", "__")));
            ImageToTextButton.IsEnabled = true;
            return;
        }

        private void ImageToTextButton_Click(object sender, RoutedEventArgs e)
        {
            var fileNames = this.filePaths;

            foreach (var fileName in fileNames)
            {
                if (!File.Exists(fileName))
                { // ファイルが存在しない場合
                    this.Initialize();
                    return;
                }
            }

            var whiteString = WHITE_STRING;
            if (!String.IsNullOrEmpty(WhiteStringTextBox.Text))
            {
                whiteString = WhiteStringTextBox.Text;
            }
            else
            {
                WhiteStringTextBox.Text = WHITE_STRING;
            }

            var blackString = BLACK_STRING;
            if (!String.IsNullOrEmpty(BlackStringTextBox.Text))
            {
                blackString = BlackStringTextBox.Text;
            }
            else
            {
                BlackStringTextBox.Text = BLACK_STRING;
            }

            var threshold = THRESHOLD;
            if (!String.IsNullOrEmpty(ThresholdTextBox.Text) && Double.TryParse(ThresholdTextBox.Text, out double thresholdResult))
            {
                threshold = thresholdResult;
            }
            else
            {
                ThresholdTextBox.Text = THRESHOLD.ToString();
            }

            var separator = SEPARATOR;
            if (!String.IsNullOrEmpty(SeparatorTextBox.Text))
            {
                separator = SeparatorTextBox.Text;
            }
            else
            {
                SeparatorTextBox.Text = SEPARATOR;
            }

            foreach (var fileName in fileNames)
            {
                // Bitmap画像を作成する
                using (var bitmap = new Bitmap(fileName))
                {
                    var bitmapString = Enumerable.Range(0, bitmap.Height).Select(j =>
                        Enumerable.Range(0, bitmap.Width).Select(i => BitmapUtility.ColorToSpecifiedString(bitmap.GetPixel(i, j), whiteString, blackString, threshold)));

                    var windowText = String.Join("\r\n", bitmapString.Select(x => String.Join(separator, x)));

                    var subWindow = new SubWindow(windowText)
                    {
                        Title = fileName.Split('\\').Last()
                    };
                    subWindow.Show();
                }
            }
        }
    }

    public static class BitmapUtility
    {
        public static string ColorToSpecifiedString(System.Drawing.Color color, string whiteString, string blackString, double threshold)
        {
            if (threshold >= 1.0) return whiteString;
            if (threshold <= 0.0) return blackString;

            if (color.GetBrightness() > threshold) return whiteString;
            else return blackString;
        }
    }
}
