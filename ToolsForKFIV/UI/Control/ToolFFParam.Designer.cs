﻿
namespace ToolsForKFIV.UI.Control
{
    partial class ToolFFParam
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tpeTabView = new System.Windows.Forms.TabControl();
            this.tvPreview = new System.Windows.Forms.TabPage();
            this.ptPageBox = new System.Windows.Forms.ComboBox();
            this.tpeDataGrid = new System.Windows.Forms.DataGridView();
            this.tvExport = new System.Windows.Forms.TabPage();
            this.tpeTabView.SuspendLayout();
            this.tvPreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tpeDataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // tpeTabView
            // 
            this.tpeTabView.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tpeTabView.Controls.Add(this.tvPreview);
            this.tpeTabView.Controls.Add(this.tvExport);
            this.tpeTabView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tpeTabView.Location = new System.Drawing.Point(0, 0);
            this.tpeTabView.Name = "tpeTabView";
            this.tpeTabView.SelectedIndex = 0;
            this.tpeTabView.Size = new System.Drawing.Size(256, 256);
            this.tpeTabView.TabIndex = 0;
            // 
            // tvPreview
            // 
            this.tvPreview.Controls.Add(this.ptPageBox);
            this.tvPreview.Controls.Add(this.tpeDataGrid);
            this.tvPreview.Location = new System.Drawing.Point(4, 27);
            this.tvPreview.Name = "tvPreview";
            this.tvPreview.Padding = new System.Windows.Forms.Padding(3);
            this.tvPreview.Size = new System.Drawing.Size(248, 225);
            this.tvPreview.TabIndex = 0;
            this.tvPreview.Text = "Preview";
            this.tvPreview.UseVisualStyleBackColor = true;
            // 
            // ptPageBox
            // 
            this.ptPageBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ptPageBox.FormattingEnabled = true;
            this.ptPageBox.Location = new System.Drawing.Point(3, 3);
            this.ptPageBox.Name = "ptPageBox";
            this.ptPageBox.Size = new System.Drawing.Size(242, 23);
            this.ptPageBox.TabIndex = 1;
            this.ptPageBox.SelectedIndexChanged += new System.EventHandler(this.ptPageBox_SelectedIndexChanged);
            // 
            // tpeDataGrid
            // 
            this.tpeDataGrid.AllowUserToAddRows = false;
            this.tpeDataGrid.AllowUserToDeleteRows = false;
            this.tpeDataGrid.AllowUserToResizeColumns = false;
            this.tpeDataGrid.AllowUserToResizeRows = false;
            this.tpeDataGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tpeDataGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.tpeDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tpeDataGrid.Location = new System.Drawing.Point(3, 29);
            this.tpeDataGrid.Name = "tpeDataGrid";
            this.tpeDataGrid.RowHeadersWidth = 64;
            this.tpeDataGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.tpeDataGrid.RowTemplate.Height = 25;
            this.tpeDataGrid.Size = new System.Drawing.Size(242, 193);
            this.tpeDataGrid.TabIndex = 0;
            // 
            // tvExport
            // 
            this.tvExport.Location = new System.Drawing.Point(4, 27);
            this.tvExport.Name = "tvExport";
            this.tvExport.Padding = new System.Windows.Forms.Padding(3);
            this.tvExport.Size = new System.Drawing.Size(248, 225);
            this.tvExport.TabIndex = 1;
            this.tvExport.Text = "Export";
            this.tvExport.UseVisualStyleBackColor = true;
            // 
            // ToolFFParam
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tpeTabView);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "ToolFFParam";
            this.Size = new System.Drawing.Size(256, 256);
            this.Resize += new System.EventHandler(this.ToolFFParamEditor_Resize);
            this.tpeTabView.ResumeLayout(false);
            this.tvPreview.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tpeDataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tpeTabView;
        private System.Windows.Forms.TabPage tvPreview;
        private System.Windows.Forms.TabPage tvExport;
        private System.Windows.Forms.DataGridView tpeDataGrid;
        private System.Windows.Forms.ComboBox ptPageBox;
    }
}
