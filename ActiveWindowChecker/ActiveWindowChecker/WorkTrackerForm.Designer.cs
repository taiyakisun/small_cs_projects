namespace ActiveWindowChecker
{
    partial class WorkTrackerForm
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
            this.components = new System.ComponentModel.Container();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.workInfoDataBindingSource2 = new System.Windows.Forms.BindingSource(this.components);
            this.workInfoDataBindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.workInfoDataBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.btnSave = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.workInfoDataBindingSource2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.workInfoDataBindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.workInfoDataBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(13, 40);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 21;
            this.dataGridView1.Size = new System.Drawing.Size(568, 345);
            this.dataGridView1.TabIndex = 0;
            // 
            // workInfoDataBindingSource2
            // 
            this.workInfoDataBindingSource2.DataSource = typeof(WorkInfoData);
            // 
            // workInfoDataBindingSource1
            // 
            this.workInfoDataBindingSource1.DataSource = typeof(WorkInfoData);
            // 
            // workInfoDataBindingSource
            // 
            this.workInfoDataBindingSource.DataSource = typeof(WorkInfoData);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(12, 11);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // WorkTrackerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(593, 397);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.dataGridView1);
            this.Name = "WorkTrackerForm";
            this.Text = "Taiyaki Work Tracker";
            this.Load += new System.EventHandler(this.WorkTrackerForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.workInfoDataBindingSource2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.workInfoDataBindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.workInfoDataBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.BindingSource workInfoDataBindingSource;
        private System.Windows.Forms.BindingSource workInfoDataBindingSource1;
        private System.Windows.Forms.BindingSource workInfoDataBindingSource2;
        private System.Windows.Forms.Button btnSave;
    }
}

