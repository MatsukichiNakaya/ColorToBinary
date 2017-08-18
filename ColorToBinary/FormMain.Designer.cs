namespace ColorToBinary
{
    partial class FormMain
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.btnConvert = new System.Windows.Forms.Button();
            this.picSrc = new System.Windows.Forms.PictureBox();
            this.picDst = new System.Windows.Forms.PictureBox();
            this.slider = new System.Windows.Forms.TrackBar();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.cmbMode = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkCsv = new System.Windows.Forms.CheckBox();
            this.chkImage = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.picSrc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDst)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.slider)).BeginInit();
            this.SuspendLayout();
            // 
            // btnConvert
            // 
            this.btnConvert.Location = new System.Drawing.Point(137, 339);
            this.btnConvert.Name = "btnConvert";
            this.btnConvert.Size = new System.Drawing.Size(75, 23);
            this.btnConvert.TabIndex = 6;
            this.btnConvert.Text = "Convert";
            this.btnConvert.UseVisualStyleBackColor = true;
            this.btnConvert.Click += new System.EventHandler(this.BtnConvert_Click);
            // 
            // picSrc
            // 
            this.picSrc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picSrc.Location = new System.Drawing.Point(14, 38);
            this.picSrc.Name = "picSrc";
            this.picSrc.Size = new System.Drawing.Size(320, 120);
            this.picSrc.TabIndex = 1;
            this.picSrc.TabStop = false;
            this.picSrc.DragDrop += new System.Windows.Forms.DragEventHandler(this.PicSrc_DragDrop);
            this.picSrc.DragEnter += new System.Windows.Forms.DragEventHandler(this.Pic_DragEnter);
            // 
            // picDst
            // 
            this.picDst.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picDst.Location = new System.Drawing.Point(14, 164);
            this.picDst.Name = "picDst";
            this.picDst.Size = new System.Drawing.Size(320, 120);
            this.picDst.TabIndex = 2;
            this.picDst.TabStop = false;
            // 
            // slider
            // 
            this.slider.AutoSize = false;
            this.slider.Location = new System.Drawing.Point(73, 290);
            this.slider.Maximum = 255;
            this.slider.Name = "slider";
            this.slider.Size = new System.Drawing.Size(261, 29);
            this.slider.TabIndex = 3;
            this.slider.TabStop = false;
            this.slider.TickFrequency = 10;
            this.slider.Value = 127;
            this.slider.Scroll += new System.EventHandler(this.Slider_Scroll);
            // 
            // txtValue
            // 
            this.txtValue.Location = new System.Drawing.Point(14, 290);
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(53, 19);
            this.txtValue.TabIndex = 2;
            this.txtValue.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtValue_KeyDown);
            // 
            // cmbMode
            // 
            this.cmbMode.FormattingEnabled = true;
            this.cmbMode.Location = new System.Drawing.Point(200, 12);
            this.cmbMode.Name = "cmbMode";
            this.cmbMode.Size = new System.Drawing.Size(134, 20);
            this.cmbMode.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "↓ここに画像をD＆D";
            // 
            // chkCsv
            // 
            this.chkCsv.AutoSize = true;
            this.chkCsv.Checked = true;
            this.chkCsv.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCsv.Location = new System.Drawing.Point(218, 325);
            this.chkCsv.Name = "chkCsv";
            this.chkCsv.Size = new System.Drawing.Size(83, 16);
            this.chkCsv.TabIndex = 4;
            this.chkCsv.Text = "CSV output";
            this.chkCsv.UseVisualStyleBackColor = true;
            // 
            // chkImage
            // 
            this.chkImage.AutoSize = true;
            this.chkImage.Location = new System.Drawing.Point(218, 343);
            this.chkImage.Name = "chkImage";
            this.chkImage.Size = new System.Drawing.Size(96, 16);
            this.chkImage.TabIndex = 5;
            this.chkImage.Text = "Picture output";
            this.chkImage.UseVisualStyleBackColor = true;
            // 
            // FormMain
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(348, 367);
            this.Controls.Add(this.chkImage);
            this.Controls.Add(this.chkCsv);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbMode);
            this.Controls.Add(this.txtValue);
            this.Controls.Add(this.slider);
            this.Controls.Add(this.picDst);
            this.Controls.Add(this.picSrc);
            this.Controls.Add(this.btnConvert);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMain";
            this.Text = "BinaryConvert";
            ((System.ComponentModel.ISupportInitialize)(this.picSrc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDst)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.slider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnConvert;
        private System.Windows.Forms.PictureBox picSrc;
        private System.Windows.Forms.PictureBox picDst;
        private System.Windows.Forms.TrackBar slider;
        private System.Windows.Forms.TextBox txtValue;
        private System.Windows.Forms.ComboBox cmbMode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkCsv;
        private System.Windows.Forms.CheckBox chkImage;
    }
}

