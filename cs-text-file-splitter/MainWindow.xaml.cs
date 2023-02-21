using System;
using System.Collections.Generic;
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
//using System.Windows.Shapes;

namespace cs_text_file_splitter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            ComboBoxEncoding.Items.Add("Shift-JIS");
            ComboBoxEncoding.Items.Add("UTF-8");
            ComboBoxEncoding.SelectedIndex = 0;

            EnableDragDrop(this);
        }
        private void EnableDragDrop(Control control)
        {
            //ドラッグ＆ドロップ許可
            control.AllowDrop = true;

            //ドラッグ開始イベント
            control.PreviewDragOver += (s, e) =>
            {
                // カーソルをドラッグ中のアイコンに変更し、そうでない場合は何もしない。
                e.Effects = (e.Data.GetDataPresent(DataFormats.FileDrop)) ? DragDropEffects.Copy : e.Effects = DragDropEffects.None;
                e.Handled = true;
            };

            //ドラッグ＆ドロップが完了した時の処理（ファイル名を取得し、ファイルの中身をTextプロパティに代入）
            control.PreviewDrop += (s, e) =>
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop)) // ドロップされたものがファイルかどうか確認する。
                {
                    string[] paths = ((string[])e.Data.GetData(DataFormats.FileDrop));
                    //--------------------------------------------------------------------
                    // ここに、ドラッグ＆ドロップ受付時の処理を記述する
                    //--------------------------------------------------------------------
                    SplitFile(paths);
                }
            };
        }
        private void SplitFile(string[] paths)
        {
            int maxLineCount = int.Parse(TextBoxMaxLineCount.Text);

            foreach (var path in paths)
            {
                using (var reader = new StreamReader(path, Encoding.GetEncoding(ComboBoxEncoding.SelectedItem as string)))
                {
                    for (int fileCount = 0; ; fileCount++)
                    {
                        var line = null as string;
                        var lines = new List<string>();
                        for (; ; )
                        {
                            line = reader.ReadLine();
                            if (line == null) { break; }
                            lines.Add(line);
                            if (lines.Count() >= maxLineCount) { break; }
                        }
                        var outputPath = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path) + string.Format("_{0}", fileCount) + Path.GetExtension(path));
                        if (lines.Count() > 0) { File.WriteAllLines(outputPath, lines); }
                        if (line == null) { break; }
                    }
                }
            }
        }
    }
}
