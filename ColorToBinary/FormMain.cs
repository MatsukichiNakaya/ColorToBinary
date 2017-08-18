using System;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColorToBinary
{
    public partial class FormMain : Form
    {
        /// <summary>コンボボックスの選択肢</summary>
        enum EnumMode
        {
            /// <summary>グレースケール</summary>
            Gray,
            /// <summary>2値化　固定閾値</summary>
            BinaryFixed,
            /// <summary>2値化　オーダー</summary>
            BinaryOrdered,
            /// <summary>2値化　差分</summary>
            BinaryDiff,
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FormMain()
        {
            InitializeComponent();
            this.picSrc.AllowDrop = true;
            this.picDst.AllowDrop = true;
            this.txtValue.Text = this.slider.Value.ToString();

            // 選択肢設定
            foreach (EnumMode num in Enum.GetValues(typeof(EnumMode)))
            {
                this.cmbMode.Items.Add(num.ToString());
            }
            this.cmbMode.SelectedIndex = 0;
        }

        /// <summary>
        /// 変換ボタン
        /// </summary>
        private async void BtnConvert_Click(Object sender, EventArgs e)
        {
            if (this.picSrc.Image == null) { return; }
            // 変換が連打されないように操作不可にする。
            this.btnConvert.Enabled = false;
            try
            {
                // フォーム設定値を変数に。
                Int32 idx =  this.cmbMode.SelectedIndex;
                Int32 th = this.slider.Value;
                Bitmap bmp = new Bitmap(this.picSrc.Image);
                Boolean isCsv = this.chkCsv.Checked;
                Boolean isImg = this.chkImage.Checked;

                this.picDst.Image = null;
                // 変換に時間がかかってもOK　バックグラウンドで実行
                Bitmap dstImane = await Task.Run(
                    () => ConvertMethod(idx, th, bmp, isCsv, isImg));

                if (dstImane != null)
                {
                    this.picDst.Image = dstImane;
                }
            }
            finally
            {   // 終了時操作可能に戻す。
                this.btnConvert.Enabled = true;
            }
        }

        /// <summary>
        /// 画像変換タスク
        /// </summary>
        /// <param name="index">コンボボックスの選択肢</param>
        /// <param name="thresold">閾値</param>
        /// <param name="srcImage">ソースとなる画像データ</param>
        /// <param name="isOutCsv">CSV出力をするか？</param>
        /// <param name="isOutImage">画像出力をするか？</param>
        private Bitmap ConvertMethod(Int32 index, Int32 thresold, Bitmap srcImage,
                                   Boolean isOutCsv, Boolean isOutImage)
        {
            Bitmap result = null;
            if (srcImage == null) { return null; }
            try
            {
                switch (index)
                {
                    // グレースケール
                    case (Int32)EnumMode.Gray:
                        result = Project.Drawing.ColorConverter.ToGrayScale(srcImage);
                        break;
                    // 固定閾値
                    case (Int32)EnumMode.BinaryFixed:
                        result = Project.Drawing.ColorConverter.ToBinaryByFixed(
                                                                    srcImage, thresold);
                        break;
                    // オーダー
                    case (Int32)EnumMode.BinaryOrdered:
                        result = Project.Drawing.ColorConverter.ToBinaryByOrdered(srcImage);
                        break;
                    // 差分
                    case (Int32)EnumMode.BinaryDiff:
                        result = Project.Drawing.ColorConverter.ToBinaryByDiff(
                                                                    srcImage, thresold);
                        break;
                }
            }
            catch (Exception) { }

            if (result == null) { return null; }

            // CSVファイル出力を行う
            if (isOutCsv)
            {
                if (index != (Int32)EnumMode.Gray)
                {
                    OutputCSV(result);
                }
            }

            // 画像ファイル出力を行う
            if (isOutImage) { OutputImage(result); }

            return result;
        }

        /// <summary>
        /// CSVファイルの書出しを行う
        /// </summary>
        /// <param name="image"></param>
        private void OutputCSV(Bitmap image)
        {
            try
            {
                var csv = Project.Drawing.ColorConverter.GetBinaryData(image);
                Project.Text.CSV.WriteCell(@".\Binary.csv", Encoding.UTF8, ',', csv);
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 画像ファイルの書出しを行う
        /// </summary>
        /// <param name="image"></param>
        private void OutputImage(Bitmap image)
        {
            try
            {
                (image.Clone() as Bitmap).Save(
                    @".\Binary.png", System.Drawing.Imaging.ImageFormat.Png);
            }
            catch (Exception) { }
        }

        /// <summary>
        /// ドロップされたファイルを表示
        /// </summary>
        private void PicSrc_DragDrop(Object sender, DragEventArgs e)
        {
            var files = e.Data.GetData(DataFormats.FileDrop, false) as String[];
            // 拡張子のフィルタリングを行い対応する画像形式に絞り込む
            var pattern = new Regex(@"\.jpg$|\.jpeg$|\.png$|\.bmp$");

            if(files == null) { return; }

            String file = String.Empty;
            foreach (var f in files)
            {
                if (pattern.IsMatch(f))
                {   // 最初にマッチしたファイルを選択する
                    file = f;
                    break;
                }
            }
            if (String.IsNullOrEmpty(file)) { return; }

            // 画像表示
            this.picSrc.ImageLocation = file;
        }

        /// <summary>
        /// ファイルがドラッグされているときのマウスカーソルアイコン変更
        /// </summary>
        private void Pic_DragEnter(Object sender, DragEventArgs e)
        {
            //コントロール内にドラッグされたとき実行される
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                //ドラッグされたデータ形式を調べ、ファイルのときはコピーとする
                { e.Effect = DragDropEffects.Copy; }
            else
                //ファイル以外は受け付けない
                { e.Effect = DragDropEffects.None; }
        }

        /// <summary>
        /// スライダー動作時のテキストボックスの値変更
        /// </summary>
        private void Slider_Scroll(Object sender, EventArgs e)
        {
            this.txtValue.Text = this.slider.Value.ToString();
        }

        /// <summary>
        /// テキストボックスの値変更時のスライダー値変更
        /// </summary>
        private void TxtValue_KeyDown(Object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Int32 value = 0;
                if (Int32.TryParse(this.txtValue.Text, out value))
                {
                    if(this.slider.Maximum < value) { value = this.slider.Maximum; }
                    if(value < this.slider.Minimum) { value = this.slider.Minimum; }

                    this.slider.Value = value;
                }
                // 上下限を超えている場合など、テキストボックスの値をスライダーに合わせる
                this.txtValue.Text = this.slider.Value.ToString();
            }
        }
    }
}
