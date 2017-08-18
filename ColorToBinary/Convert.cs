using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Project.Drawing
{
    /// <summary>色変換クラス</summary>
    public class ColorConverter
    {
        /// <summary>グレースケール R オフセット</summary>
        private const Int32 RP = (Int32)(0.298912 * 1024);
        /// <summary>グレースケール G オフセット</summary>
        private const Int32 GP = (Int32)(0.586611 * 1024);
        /// <summary>グレースケール B オフセット</summary>
        private const Int32 BP = (Int32)(0.114478 * 1024);

        /// <summary>
        /// 色変換ベースの関数
        /// </summary>
        /// <param name="bmpBase">元となる画像</param>
        /// <param name="converter">変換式</param>
        private static Bitmap ConvertColor(Bitmap bmpBase, Func<Byte[], Byte[]> converter)
        {
            // 元の画像をARGB形式の画像データに変換
            var rect = new Rectangle(0, 0, bmpBase.Width, bmpBase.Height);
            Bitmap bmp = bmpBase.Clone(rect, PixelFormat.Format32bppArgb);
            bmpBase.Dispose();

            // ARGBの配列に変換
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, bmp.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            Int32 bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            var rgbValues = new Byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            // 変換処理
            converter(rgbValues).CopyTo(rgbValues, 0);

            // 元データに値を設定する
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
            bmp.UnlockBits(bmpData);

            return bmp;
        }

        #region グレースケール化
        /// <summary>
        /// グレースケールに変換(パス指定)
        /// </summary>
        /// <param name="path">読込画像パス</param>
        /// <returns>画像データ</returns>
        public static Bitmap ToGrayScale(String path)
        {
            return ToGrayScale(new Bitmap(path));
        }

        /// <summary>
        /// グレースケールに変換(読込済みの画像指定)
        /// </summary>
        /// <param name="bmpBase">元となる画像</param>
        /// <returns>画像データ</returns>
        public static Bitmap ToGrayScale(Bitmap bmpBase)
        {
            return ConvertColor(bmpBase, rgb => GrayScaleConvert(rgb));
        }

        /// <summary>
        /// グレースケール計算式
        /// </summary>
        /// <param name="rgbValues">色データ</param>
        /// <returns>変換色データ</returns>
        private static Byte[] GrayScaleConvert(Byte[] rgbValues)
        {
            Byte g; 
            // グレースケール化
            for (Int32 i = 0; i < rgbValues.Length; i += 4)
            {
                g = (Byte)( ( BP * rgbValues[i + 0] 
                            + GP * rgbValues[i + 1]
                            + RP * rgbValues[i + 2] ) >> 10);
                // グレーにするために同じ色を設定
                rgbValues[i + 0] = g;      // b
                rgbValues[i + 1] = g;      // g
                rgbValues[i + 2] = g;      // r
            }
            return rgbValues;
        }
        #endregion

        #region 透過処理
        /// <summary>
        /// 画像の透過値を0にして透過画像に置き換える
        /// </summary>
        /// <param name="path">読込画像パス</param>
        /// <returns>画像データ</returns>
        /// <remarks>半透明のドットがある画像は変換注意</remarks>
        public static Bitmap ToTransfarent(String path)
        {
            return ToTransfarent(new Bitmap(path));
        }

        /// <summary>
        /// 画像の透過値を0にして透過画像に置き換える
        /// </summary>
        /// <param name="bmpBase">元となる画像</param>
        /// <returns>画像データ</returns>
        /// <remarks>半透明のドットがある画像は変換注意</remarks>
        public static Bitmap ToTransfarent(Bitmap bmpBase)
        {
            return ConvertColor(bmpBase, rgb => TransfarentConvert(rgb));
        }

        /// <summary>
        /// 透過設定の処理
        /// </summary>
        /// <param name="rgbValues">色データ</param>
        /// <returns>変換色データ</returns>
        private static Byte[] TransfarentConvert(Byte[] rgbValues)
        {
            // 配列の中身が [ b,g,r,a, b,g,r,a, ... ] と続く
            for (Int32 i = 3; i < rgbValues.Length; i += 4)
            {
                rgbValues[i] = 0;   // [a]の値を[0]書き換える
            }
            return rgbValues;
        }


        /// <summary>
        /// 透過形式の画像から元の透過無の画像に復元する
        /// </summary>
        /// <param name="path">読込画像パス</param>
        /// <returns>画像データ</returns>
        /// <remarks>元の画像に半透明があった場合は完全には戻らない</remarks>
        public static Bitmap FromTransfarentDecode(String path)
        {
            return FromTransfarentDecode(new Bitmap(path));
        }

        /// <summary>
        /// 透過式の画像から元の透過無の画像に復元する
        /// </summary>
        /// <param name="bmpBase">元となる画像</param>
        /// <returns>画像データ</returns>
        /// <remarks>元の画像に半透明があった場合は完全には戻らない</remarks>
        public static Bitmap FromTransfarentDecode(Bitmap bmpBase)
        {
            if (bmpBase.PixelFormat == PixelFormat.Format32bppArgb) { return null; }

            return ConvertColor(bmpBase, rgb => TransfarentDecodeConvert(rgb));
        }

        /// <summary>
        /// 透過式の画像から元の透過無の画像に復元する計算
        /// </summary>
        /// <param name="rgbValues">色データ</param>
        /// <returns>変換色データ</returns>
        private static Byte[] TransfarentDecodeConvert(Byte[] rgbValues)
        {
            for (Int32 i = 3; i < rgbValues.Length; i += 4)
            {
                rgbValues[i] = 255;
            }
            return rgbValues;
        }
        #endregion

        #region 2値化
        /// <summary>
        /// 輝度計算
        /// </summary>
        /// <param name="r">赤</param>
        /// <param name="g">緑</param>
        /// <param name="b">青</param>
        /// <returns>輝度</returns>
        /// <remarks>
        /// 引数3つは、順不同でもよい。
        /// 単純にMaxとMinを取得しているだけなので。
        /// </remarks>
        private static Single GetBrightness(Byte r, Byte g, Byte b)
        {
            Single max = r;
            Single min = r;

            if (max < g) { max = g; }
            if (max < b) { max = b; }

            if (g < min) { min = g; }
            if (b < min) { min = b; }

            return ((max / 255.0f) + (min / 255.0f)) / 2;
        }

        /// <summary>
        /// 2値化用変換関数
        /// </summary>
        /// <param name="bmpBase">元となる画像</param>
        /// <param name="threshold">閾値</param>
        /// <param name="converter">変換式</param>
        /// <returns></returns>
        private static Bitmap ConvertBinary(Bitmap bmpBase, Int32 threshold, 
                                            Func<Byte[], Int32, Size, Int32, Byte[]> converter)
        {
            var rect = new Rectangle(0, 0, bmpBase.Width, bmpBase.Height);
            var result = new Bitmap(rect.Width, rect.Height, PixelFormat.Format1bppIndexed);

            Byte[] rgbValues;
            // ベースのARGBデータ取得
            using (Bitmap bmp = bmpBase.Clone(rect, PixelFormat.Format32bppArgb))
            {
                BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, bmp.PixelFormat);
                IntPtr ptr = bmpData.Scan0;
                Int32 bytes = Math.Abs(bmpData.Stride) * bmp.Height;
                rgbValues = new Byte[bytes];
                System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
                bmp.UnlockBits(bmpData);
            }

            // 変換後の2値データを作成
            BitmapData bmpResultData = result.LockBits(rect, ImageLockMode.WriteOnly, result.PixelFormat);
            // データをもとに計算
            var resultValues = converter(rgbValues, threshold, result.Size, bmpResultData.Stride);
            // 計算結果を画像に反映させる
            IntPtr ptrRet = bmpResultData.Scan0;
            System.Runtime.InteropServices.Marshal.Copy(resultValues, 0, ptrRet, resultValues.Length);

            bmpBase.Dispose();
            result.UnlockBits(bmpResultData);
            return result;
        }

        #region 固定閾値
        /// <summary>
        /// 2値化画像に変換(固定閾値法)
        /// </summary>
        /// <param name="bmpBase">画像データ</param>
        /// <param name="threshold">2値化閾値</param>
        /// <returns>画像データ</returns>
        public static Bitmap ToBinaryByFixed(Bitmap bmpBase, Int32 threshold)
        {
            // 指定の閾値を使用して2値化を行う
            return ConvertBinary(bmpBase, threshold,
                                (rgb, th, sz, st) => BinaryFixedConvert(rgb, th, sz, st));
        }

        /// <summary>
        /// 固定閾値法の変換処理
        /// </summary>
        /// <param name="rgbValues">色データ</param>
        /// <param name="threshold">閾値</param>
        /// <param name="bmpSize">画像サイズ</param>
        /// <param name="stride">画像読み込み幅</param>
        /// <returns>変換色データ</returns>
        private static Byte[] BinaryFixedConvert(Byte[] rgbValues, Int32 threshold, 
                                                 Size bmpSize, Int32 stride)
        {
            var br = 0f;
            var th = (threshold / 255.0f);
            // 2値化
            var c = 0;
            var pos = 0;
            var result = new Byte[stride * bmpSize.Height];

            for (Int32 r = 0; r < bmpSize.Height; r++)
            {
                for (c = 0; c < bmpSize.Width; c++)
                {
                    br = GetBrightness(rgbValues[(r * bmpSize.Width * 4 + c * 4) + 2],  // r
                                       rgbValues[(r * bmpSize.Width * 4 + c * 4) + 1],  // g
                                       rgbValues[(r * bmpSize.Width * 4 + c * 4) + 0]); // b
                    // 閾値チェック
                    if (th < br)
                    {   // 色設定
                        pos = (c >> 3) + stride * r;
                        result[pos] |= (Byte)(0x80 >> (c & 0x7));
                    }
                }
            }
            return result;
        }
        #endregion

        #region オーダー
        /// <summary>
        /// 2値化画像に変換(オーダー法)
        /// </summary>
        /// <param name="bmpBase">元となる画像</param>
        /// <returns>画像データ</returns>
        public static Bitmap ToBinaryByOrdered(Bitmap bmpBase)
        {
            return ConvertBinary(bmpBase, 0, 
                                 (rgb, th, sz, st) => BinaryOrderedConvert(rgb, sz, st));
        }

        /// <summary>
        /// オーダードディザリングの変換処理
        /// </summary>
        /// <param name="rgbValues">色データ</param>
        /// <param name="bmpSize">画像サイズ</param>
        /// <param name="stride">画像読み込み幅</param>
        /// <returns>変換色データ</returns>
        private static Byte[] BinaryOrderedConvert(Byte[] rgbValues, Size bmpSize, Int32 stride)
        {
            var br = 0f;
            // 閾値マップを作成する
            var thMap = new Single[4][]
            {
                new Single[4] {1f/17f, 9f/17f, 3f/17f, 11f/17f},
                new Single[4] {13f/17f, 5f/17f, 15f/17f, 7f/17f},
                new Single[4] {4f/17f, 12f/17f, 2f/17f, 10f/17f},
                new Single[4] {16f/17f, 8f/17f, 14f/17f, 6f/17f },
            };
            var c = 0;
            var pos = 0;
            var result = new Byte[stride * bmpSize.Height];

            for (Int32 r = 0; r < bmpSize.Height; r++)
            {
                for (c = 0; c < bmpSize.Width; c++)
                {
                    br = GetBrightness(rgbValues[(r * bmpSize.Width * 4 + c * 4) + 2],  // r
                                       rgbValues[(r * bmpSize.Width * 4 + c * 4) + 1],  // g
                                       rgbValues[(r * bmpSize.Width * 4 + c * 4) + 0]); // b
                    if (thMap[r % 4][c % 4] <= br)  
                    {
                        // 色設定
                        pos = (c >> 3) + stride * r;
                        result[pos] |= (Byte)(0x80 >> (c & 0x7));
                    }
                }
            }
            return result;
        }
        #endregion

        #region 差分
        /// <summary>
        /// 2値化画像に変換(差分法)
        /// </summary>
        /// <param name="bmpBase">元となる画像</param>
        /// <param name="threshold">閾値</param>
        /// <returns>画像データ</returns>
        public static Bitmap ToBinaryByDiff(Bitmap bmpBase, Int32 threshold)
        {
            return ConvertBinary(bmpBase, threshold, 
                                 (rgb, th, sz, st) => BinaryDiffConvert(rgb, th, sz, st));
        }

        /// <summary>
        /// 差分法の変換処理
        /// </summary>
        /// <param name="rgbValues">色データ</param>
        /// <param name="threshold">閾値</param>
        /// <param name="bmpSize">画像サイズ</param>
        /// <param name="stride">画像読み込み幅</param>
        /// <returns>変換色データ</returns>
        private static Byte[] BinaryDiffConvert(Byte[] rgbValues, Int32 threshold,
                                                Size bmpSize, Int32 stride)
        {
            var err = 0.0f;
            var th = threshold / 255.0f;
            var c = 0;
            var pos = 0;
            var result = new Byte[stride * bmpSize.Height];
            //現在の行と次の行の誤差を記憶する配列
            var errors = new Single[2][] {
                            new Single[bmpSize.Width + 1],
                            new Single[bmpSize.Width + 1]
                         };

            for (Int32 r = 0; r < bmpSize.Height; r++)
            {
                for (c = 0; c < bmpSize.Width; c++)
                {
                    err = GetBrightness(rgbValues[(r * bmpSize.Width * 4 + c * 4) + 2], // r
                                        rgbValues[(r * bmpSize.Width * 4 + c * 4) + 1], // g
                                        rgbValues[(r * bmpSize.Width * 4 + c * 4) + 0]) // b
                                        + errors[0][c];
                    if (th <= err)
                    {   
                        // 色設定
                        pos = (c >> 3) + stride * r;
                        result[pos] |= (Byte)(0x80 >> (c & 0x7));
                        //誤差を計算（黒くした時の誤差はerr-0なので、そのまま）
                        err -= 1f;
                    }

                    //誤差を振り分ける
                    errors[0][c + 1] += err * 7f / 16f;
                    if (c > 0)
                    {
                        errors[1][c - 1] += err * 3f / 16f;
                    }
                    errors[1][c] += err * 5f / 16f;
                    errors[1][c + 1] += err * 1f / 16f;
                }
                //誤差を記憶した配列を入れ替える
                errors[0] = errors[1];
                errors[1] = new Single[errors[0].Length];
            }
            return result;
        }
        #endregion

        #region CSV化
        /// <summary>
        /// 2値化画像をもとに0,1のデータを取得
        /// </summary>
        /// <param name="bmpBase">元となる画像</param>
        /// <returns>画像を白:0, 黒:1 と置き換えたCSVデータ</returns>
        public static String[][] GetBinaryData(Bitmap bmpBase)
        {
            // 返値変数初期化
            var result = new String[bmpBase.Height][];
            for (Int32 i = 0; i < result.Length; i++)
            {
                result[i] = new String[bmpBase.Width];
            }

            // ARGB形式の画像データに変換
            var rect = new Rectangle(0, 0, bmpBase.Width, bmpBase.Height);
            Byte[] rgbValues = null;
            using (var bmp = bmpBase.Clone(rect, PixelFormat.Format32bppArgb))
            {
                if (bmp == null) { return null; }
                // ARGBの配列に変換
                BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, bmp.PixelFormat);
                IntPtr ptr = bmpData.Scan0;
                Int32 bytes = Math.Abs(bmpData.Stride) * bmp.Height;
                rgbValues = new Byte[bytes];
                System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
                bmp.UnlockBits(bmpData);
            }

            // 一次元の配列をCSV的な二次元配列へ変換しながら値を格納する
            var r = 0;
            var c = 0;
            for (Int32 i = 0; i < rgbValues.Length; i += 4)
            {
                // ドット中の適当な色データを見て判定する
                result[r][c] =  (rgbValues[i] == 0xFF ? 0 : 1).ToString();
                c++;
                // 改行
                if (result[r].Length <= c)
                {
                    c = 0;
                    r++;
                }
            }
            return result;
        }
        #endregion
        #endregion
    }
}
