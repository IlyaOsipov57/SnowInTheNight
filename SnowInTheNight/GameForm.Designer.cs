namespace SnowInTheNight
{
    partial class GameForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameForm));
            this.fpsLabel = new System.Windows.Forms.Label();
            this.gameScreen = new SnowInTheNight.CustomPictureBox();
            this.bottomFraming = new SnowInTheNight.CustomPictureBox();
            this.topFraming = new SnowInTheNight.CustomPictureBox();
            this.testLabel = new SnowInTheNight.CustomLabel();
            ((System.ComponentModel.ISupportInitialize)(this.gameScreen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bottomFraming)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.topFraming)).BeginInit();
            this.SuspendLayout();
            // 
            // fpsLabel
            // 
            this.fpsLabel.BackColor = System.Drawing.Color.Black;
            this.fpsLabel.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.fpsLabel.ForeColor = System.Drawing.Color.White;
            this.fpsLabel.Location = new System.Drawing.Point(0, 0);
            this.fpsLabel.Name = "fpsLabel";
            this.fpsLabel.Size = new System.Drawing.Size(46, 41);
            this.fpsLabel.TabIndex = 1;
            this.fpsLabel.Visible = false;
            // 
            // gameScreen
            // 
            this.gameScreen.BackColor = System.Drawing.Color.Black;
            this.gameScreen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gameScreen.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.gameScreen.Location = new System.Drawing.Point(0, 0);
            this.gameScreen.Name = "gameScreen";
            this.gameScreen.Size = new System.Drawing.Size(266, 265);
            this.gameScreen.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.gameScreen.TabIndex = 0;
            this.gameScreen.TabStop = false;
            // 
            // bottomFraming
            // 
            this.bottomFraming.BackColor = System.Drawing.Color.Black;
            this.bottomFraming.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomFraming.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
            this.bottomFraming.Location = new System.Drawing.Point(0, 265);
            this.bottomFraming.Name = "bottomFraming";
            this.bottomFraming.Size = new System.Drawing.Size(266, 0);
            this.bottomFraming.TabIndex = 5;
            this.bottomFraming.TabStop = false;
            // 
            // topFraming
            // 
            this.topFraming.BackColor = System.Drawing.Color.Black;
            this.topFraming.Dock = System.Windows.Forms.DockStyle.Top;
            this.topFraming.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
            this.topFraming.Location = new System.Drawing.Point(0, 0);
            this.topFraming.Name = "topFraming";
            this.topFraming.Size = new System.Drawing.Size(266, 0);
            this.topFraming.TabIndex = 4;
            this.topFraming.TabStop = false;
            // 
            // testLabel
            // 
            this.testLabel.AutoSize = true;
            this.testLabel.Location = new System.Drawing.Point(1, 0);
            this.testLabel.Name = "testLabel";
            this.testLabel.Size = new System.Drawing.Size(0, 16);
            this.testLabel.TabIndex = 2;
            this.testLabel.UseCompatibleTextRendering = true;
            this.testLabel.Visible = false;
            // 
            // GameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(266, 265);
            this.Controls.Add(this.fpsLabel);
            this.Controls.Add(this.gameScreen);
            this.Controls.Add(this.bottomFraming);
            this.Controls.Add(this.topFraming);
            this.Controls.Add(this.testLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "GameForm";
            this.Text = "SnowInTheNight";
            this.Activated += new System.EventHandler(this.GameForm_Activated);
            this.Deactivate += new System.EventHandler(this.GameForm_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GameForm_FormClosing);
            this.Load += new System.EventHandler(this.GameForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gameScreen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bottomFraming)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.topFraming)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CustomPictureBox gameScreen;
        private System.Windows.Forms.Label fpsLabel;
        private CustomLabel testLabel;
        private CustomPictureBox topFraming;
        private CustomPictureBox bottomFraming;
    }
}

