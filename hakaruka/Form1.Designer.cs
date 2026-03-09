using System.Windows.Forms;
using System.Drawing;

namespace hakaruka
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // Labels and inputs
            lblSerial = new Label();
            txtSerial = new TextBox();
            lblUnit = new Label();
            cmbUnit = new ComboBox();
            lblWeight = new Label();
            txtWeight = new TextBox();
            lblResult = new Label();
            lblResultValue = new Label();

            // Buttons and status/log
            btnScan = new Button();
            btnSave = new Button();
            btnReset = new Button();
            lblStatus = new Label();
            txtLog = new TextBox();

            // Form
            this.SuspendLayout();
            this.ClientSize = new Size(600,520);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Text = "FANモジュール重量測定システム";

            // lblSerial
            lblSerial.AutoSize = true;
            lblSerial.Location = new Point(20,18);
            lblSerial.Name = "lblSerial";
            lblSerial.Size = new Size(80,20);
            lblSerial.Text = "シリアル番号:";

            // txtSerial
            txtSerial.Location = new Point(110,14);
            txtSerial.Name = "txtSerial";
            txtSerial.Size = new Size(300,27);
            txtSerial.TabIndex =0;
            txtSerial.KeyDown += TxtSerial_KeyDown;

            // btnScan
            btnScan.Location = new Point(430,12);
            btnScan.Name = "btnScan";
            btnScan.Size = new Size(120,30);
            btnScan.Text = "スキャン/測定";
            btnScan.TabIndex =1;
            btnScan.UseVisualStyleBackColor = true;
            btnScan.Click += BtnScan_Click;

            // lblUnit
            lblUnit.AutoSize = true;
            lblUnit.Location = new Point(20,54);
            lblUnit.Name = "lblUnit";
            lblUnit.Size = new Size(70,20);
            lblUnit.Text = "測定単位:";

            // cmbUnit
            cmbUnit.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbUnit.Location = new Point(110,50);
            cmbUnit.Name = "cmbUnit";
            cmbUnit.Size = new Size(200,27);
            cmbUnit.Items.AddRange(new object[] { "段ボール1箱", "1シート", "部品1つ" });
            cmbUnit.SelectedIndex =0;

            // lblWeight
            lblWeight.AutoSize = true;
            lblWeight.Font = new Font("Yu Gothic UI",14F, FontStyle.Bold);
            lblWeight.Location = new Point(20,100);
            lblWeight.Name = "lblWeight";
            lblWeight.Size = new Size(90,25);
            lblWeight.Text = "重量( g ):";

            // txtWeight
            txtWeight.Font = new Font("Yu Gothic UI",28F, FontStyle.Bold);
            txtWeight.Location = new Point(20,130);
            txtWeight.Name = "txtWeight";
            txtWeight.ReadOnly = true;
            txtWeight.Size = new Size(390,64);
            txtWeight.Text = "0";
            txtWeight.TextAlign = HorizontalAlignment.Right;

            // lblResult
            lblResult.AutoSize = true;
            lblResult.Font = new Font("Yu Gothic UI",12F);
            lblResult.Location = new Point(20,210);
            lblResult.Name = "lblResult";
            lblResult.Size = new Size(50,21);
            lblResult.Text = "判定:";

            // lblResultValue
            lblResultValue.AutoSize = true;
            lblResultValue.Font = new Font("Yu Gothic UI",36F, FontStyle.Bold);
            lblResultValue.Location = new Point(110,200);
            lblResultValue.Name = "lblResultValue";
            lblResultValue.Size = new Size(140,65);
            lblResultValue.Text = "--";

            // btnSave
            btnSave.Location = new Point(430,130);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(120,40);
            btnSave.Text = "保存";
            btnSave.TabIndex =2;
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += BtnSave_Click;

            // btnReset
            btnReset.Location = new Point(430,180);
            btnReset.Name = "btnReset";
            btnReset.Size = new Size(120,40);
            btnReset.Text = "リセット";
            btnReset.TabIndex =3;
            btnReset.UseVisualStyleBackColor = true;
            btnReset.Click += BtnReset_Click;

            // lblStatus
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(20,280);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(150,20);
            lblStatus.Text = "待機中...";

            // txtLog
            txtLog.Location = new Point(20,310);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.Size = new Size(530,180);

            // Add controls
            this.Controls.Add(lblSerial);
            this.Controls.Add(txtSerial);
            this.Controls.Add(btnScan);
            this.Controls.Add(lblUnit);
            this.Controls.Add(cmbUnit);
            this.Controls.Add(lblWeight);
            this.Controls.Add(txtWeight);
            this.Controls.Add(lblResult);
            this.Controls.Add(lblResultValue);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnReset);
            this.Controls.Add(lblStatus);
            this.Controls.Add(txtLog);

            this.Load += Form1_Load;

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Label lblSerial;
        private TextBox txtSerial;
        private Label lblUnit;
        private ComboBox cmbUnit;
        private Label lblWeight;
        private TextBox txtWeight;
        private Label lblResult;
        private Label lblResultValue;
        private Button btnScan;
        private Button btnSave;
        private Button btnReset;
        private Label lblStatus;
        private TextBox txtLog;
    }
}
