namespace chessGame
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
            this.timer = new System.Windows.Forms.Timer();
            this.currentMoveInfoLabel = new System.Windows.Forms.Label();
            this.inCheckLabel = new System.Windows.Forms.Label();

            this.currentMoveInfoLabel.Name = "currentMoveInfoLabel";
            this.currentMoveInfoLabel.Size = new System.Drawing.Size(100, 24);
            this.currentMoveInfoLabel.Text = "";
            this.currentMoveInfoLabel.Location = new System.Drawing.Point(1, 1);

            this.inCheckLabel.Name = "inCheckLabel";
            this.inCheckLabel.Size = new System.Drawing.Size(100, 24);
            this.inCheckLabel.Text = "";
            this.inCheckLabel.Location = new System.Drawing.Point(519, 1);

            this.timer.Interval = 50;
            this.timer.Enabled = true;
            this.timer.Tick += new System.EventHandler(this.mainLoop);
            this.timer.Start();

            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(620, 600);
            this.Text = "Form1";
            this.DoubleBuffered = true;
            this.BackColor = System.Drawing.Color.Gray;
            this.MouseClick += new MouseEventHandler(this.mouseHandler);
            this.Paint += new PaintEventHandler(this.paintHandler);
            this.Controls.Add(this.currentMoveInfoLabel);
            this.Controls.Add(this.inCheckLabel);
        }

        #endregion
        System.Windows.Forms.Timer timer;
        System.Windows.Forms.Label inCheckLabel;
        System.Windows.Forms.Label currentMoveInfoLabel;
    }
}