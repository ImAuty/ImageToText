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
            this.InitializeFileControls(FileOpenLabel, ImagesToTextButton, out this.filePaths);
            this.SetTextControls(WhiteStringTextBox, BlackStringTextBox, ThresholdTextBox, SeparatorTextBox);
        }

        /// <summary>
        /// FileOpenのボタンをクリックしたときの処理<br>
        /// 画像選択ダイアログを表示して、選択された画像のパスを内部に保持する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileOpenButton_Click(object sender, RoutedEventArgs e)
        {
            // ダイアログを設定する
            var dialog = new OpenFileDialog();
            // 画像選択となるようにフィルターを指定する
            dialog.Filter = "Image Files(*.BMP;*.JPG;*.PNG;*.GIF)|*.BMP;*.JPG;*.PNG;*.GIF";
            // 複数選択を許可する
            dialog.Multiselect = true;

            // ファイル選択ダイアログを表示する
            var result = dialog.ShowDialog();
            if (!result.GetValueOrDefault())
            {
                // ファイルが選択されない場合
                this.InitializeFileControls(FileOpenLabel, ImagesToTextButton, out this.filePaths);
                return;
            }

            // ファイルが選択された場合
            // 選択されたファイルのパスを取得して内部に保持する
            this.filePaths = dialog.FileNames;
            // ファイル名を取得して表示する
            FileOpenLabel.Content = String.Join("\r\n", dialog.FileNames.Select(name => name.Split('\\').Last().Replace("_", "__")));
            ImagesToTextButton.IsEnabled = true;
            return;
        }

        /// <summary>
        /// ImagesToTextのボタンを押したときの処理<br>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImagesToTextButton_Click(object sender, RoutedEventArgs e)
        {
            var fileNames = this.filePaths;

            if (fileNames == null)
            {
                this.InitializeFileControls(FileOpenLabel, ImagesToTextButton, out this.filePaths);
                return;
            }

            foreach (var fileName in fileNames)
            {
                if (!File.Exists(fileName))
                { // ファイルが存在しない場合
                    this.InitializeFileControls(FileOpenLabel, ImagesToTextButton, out this.filePaths);
                    return;
                }
            }

            // 入力値を設定する
            this.SetTextControls(WhiteStringTextBox, BlackStringTextBox, ThresholdTextBox, SeparatorTextBox);
            var whiteString = WhiteStringTextBox.Text;
            var blackString = BlackStringTextBox.Text;
            var threshold = double.Parse(ThresholdTextBox.Text);
            var separator = SeparatorTextBox.Text;

            foreach (var fileName in fileNames)
            {
                // Bitmap画像を作成する
                using (var bitmap = new Bitmap(fileName))
                {
                    var bitmapString = Enumerable.Range(0, bitmap.Height).Select(j =>
                        Enumerable.Range(0, bitmap.Width).Select(i => BitmapUtility.ColorToSpecifiedString(bitmap.GetPixel(i, j), whiteString, blackString, threshold)));

                    var windowText = string.Join("\r\n", bitmapString.Select(x => string.Join(separator, x)));

                    var subWindow = new SubWindow(windowText)
                    {
                        Title = fileName.Split('\\').Last()
                    };
                    subWindow.Show();
                }
            }
        }

        private void InitializeFileControls(Label fileOpenLabel, Button imageToTextButton, out string[] filePaths)
        {
            fileOpenLabel.Content = "画像を選択してください";
            imageToTextButton.IsEnabled = false;
            filePaths = null;
        }

        private void SetTextControls(TextBox whiteStringTextBox, TextBox blackStringTextBox, TextBox thresholdTextBox, TextBox separatorTextBox)
        {
            if (string.IsNullOrEmpty(whiteStringTextBox.Text))
            {
                whiteStringTextBox.Text = WHITE_STRING;
            }

            if (string.IsNullOrEmpty(blackStringTextBox.Text))
            {
                blackStringTextBox.Text = BLACK_STRING;
            }

            if (string.IsNullOrEmpty(thresholdTextBox.Text) || !Double.TryParse(thresholdTextBox.Text, out _))
            {
                thresholdTextBox.Text = THRESHOLD.ToString();
            }

            if (string.IsNullOrEmpty(SeparatorTextBox.Text))
            {
                separatorTextBox.Text = SEPARATOR;
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
