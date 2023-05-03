namespace SnowMapEditor
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
            this.toolPanel = new System.Windows.Forms.Panel();
            this.checkBoxHQ = new System.Windows.Forms.CheckBox();
            this.textBoxComment = new System.Windows.Forms.TextBox();
            this.buttonGenerate = new System.Windows.Forms.Button();
            this.comboBoxTypePolyline = new System.Windows.Forms.ComboBox();
            this.numericUpDownSpeed = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownStep = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownDistance = new System.Windows.Forms.NumericUpDown();
            this.checkBoxEdges = new System.Windows.Forms.CheckBox();
            this.checkBoxVertices = new System.Windows.Forms.CheckBox();
            this.cursorPositionLabel = new System.Windows.Forms.Label();
            this.gameScreen = new SnowInTheNight.CustomPictureBox();
            this.toolPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStep)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDistance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gameScreen)).BeginInit();
            this.SuspendLayout();
            // 
            // toolPanel
            // 
            this.toolPanel.Controls.Add(this.checkBoxHQ);
            this.toolPanel.Controls.Add(this.textBoxComment);
            this.toolPanel.Controls.Add(this.buttonGenerate);
            this.toolPanel.Controls.Add(this.comboBoxTypePolyline);
            this.toolPanel.Controls.Add(this.numericUpDownSpeed);
            this.toolPanel.Controls.Add(this.numericUpDownStep);
            this.toolPanel.Controls.Add(this.numericUpDownDistance);
            this.toolPanel.Controls.Add(this.checkBoxEdges);
            this.toolPanel.Controls.Add(this.checkBoxVertices);
            this.toolPanel.Controls.Add(this.cursorPositionLabel);
            this.toolPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.toolPanel.Location = new System.Drawing.Point(517, 0);
            this.toolPanel.Name = "toolPanel";
            this.toolPanel.Size = new System.Drawing.Size(164, 486);
            this.toolPanel.TabIndex = 1;
            // 
            // checkBoxHQ
            // 
            this.checkBoxHQ.AutoSize = true;
            this.checkBoxHQ.Checked = true;
            this.checkBoxHQ.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxHQ.Location = new System.Drawing.Point(17, 281);
            this.checkBoxHQ.Name = "checkBoxHQ";
            this.checkBoxHQ.Size = new System.Drawing.Size(72, 17);
            this.checkBoxHQ.TabIndex = 7;
            this.checkBoxHQ.Text = "качество";
            this.checkBoxHQ.UseVisualStyleBackColor = true;
            // 
            // textBoxComment
            // 
            this.textBoxComment.Location = new System.Drawing.Point(13, 179);
            this.textBoxComment.Name = "textBoxComment";
            this.textBoxComment.Size = new System.Drawing.Size(135, 20);
            this.textBoxComment.TabIndex = 6;
            this.textBoxComment.Enter += new System.EventHandler(this.textBox1_Enter);
            this.textBoxComment.Leave += new System.EventHandler(this.textBox1_Leave);
            // 
            // buttonGenerate
            // 
            this.buttonGenerate.Location = new System.Drawing.Point(11, 309);
            this.buttonGenerate.Name = "buttonGenerate";
            this.buttonGenerate.Size = new System.Drawing.Size(55, 22);
            this.buttonGenerate.TabIndex = 5;
            this.buttonGenerate.Text = "GEN";
            this.buttonGenerate.UseVisualStyleBackColor = true;
            this.buttonGenerate.Click += new System.EventHandler(this.buttonGenerate_Click);
            // 
            // comboBoxTypePolyline
            // 
            this.comboBoxTypePolyline.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.comboBoxTypePolyline.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBoxTypePolyline.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTypePolyline.FormattingEnabled = true;
            this.comboBoxTypePolyline.Items.AddRange(new object[] {
            "Забор",
            "Следы",
            "Дорога",
            "Объект",
            "Стена"});
            this.comboBoxTypePolyline.Location = new System.Drawing.Point(13, 152);
            this.comboBoxTypePolyline.Name = "comboBoxTypePolyline";
            this.comboBoxTypePolyline.Size = new System.Drawing.Size(120, 21);
            this.comboBoxTypePolyline.TabIndex = 4;
            this.comboBoxTypePolyline.SelectedIndexChanged += new System.EventHandler(this.comboBoxTypeChoise_SelectedIndexChanged);
            // 
            // numericUpDownSpeed
            // 
            this.numericUpDownSpeed.Location = new System.Drawing.Point(13, 28);
            this.numericUpDownSpeed.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownSpeed.Name = "numericUpDownSpeed";
            this.numericUpDownSpeed.Size = new System.Drawing.Size(53, 20);
            this.numericUpDownSpeed.TabIndex = 3;
            this.numericUpDownSpeed.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // numericUpDownStep
            // 
            this.numericUpDownStep.Location = new System.Drawing.Point(13, 54);
            this.numericUpDownStep.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownStep.Name = "numericUpDownStep";
            this.numericUpDownStep.Size = new System.Drawing.Size(53, 20);
            this.numericUpDownStep.TabIndex = 3;
            this.numericUpDownStep.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // numericUpDownDistance
            // 
            this.numericUpDownDistance.Location = new System.Drawing.Point(11, 126);
            this.numericUpDownDistance.Name = "numericUpDownDistance";
            this.numericUpDownDistance.Size = new System.Drawing.Size(65, 20);
            this.numericUpDownDistance.TabIndex = 2;
            this.numericUpDownDistance.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // checkBoxEdges
            // 
            this.checkBoxEdges.AutoSize = true;
            this.checkBoxEdges.Checked = true;
            this.checkBoxEdges.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxEdges.Location = new System.Drawing.Point(13, 103);
            this.checkBoxEdges.Name = "checkBoxEdges";
            this.checkBoxEdges.Size = new System.Drawing.Size(57, 17);
            this.checkBoxEdges.TabIndex = 1;
            this.checkBoxEdges.Text = "Ребра";
            this.checkBoxEdges.UseVisualStyleBackColor = true;
            // 
            // checkBoxVertices
            // 
            this.checkBoxVertices.AutoSize = true;
            this.checkBoxVertices.Checked = true;
            this.checkBoxVertices.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxVertices.Location = new System.Drawing.Point(13, 80);
            this.checkBoxVertices.Name = "checkBoxVertices";
            this.checkBoxVertices.Size = new System.Drawing.Size(73, 17);
            this.checkBoxVertices.TabIndex = 1;
            this.checkBoxVertices.Text = "Вершины";
            this.checkBoxVertices.UseVisualStyleBackColor = true;
            // 
            // cursorPositionLabel
            // 
            this.cursorPositionLabel.AutoSize = true;
            this.cursorPositionLabel.Location = new System.Drawing.Point(10, 10);
            this.cursorPositionLabel.Name = "cursorPositionLabel";
            this.cursorPositionLabel.Size = new System.Drawing.Size(0, 13);
            this.cursorPositionLabel.TabIndex = 0;
            // 
            // gameScreen
            // 
            this.gameScreen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gameScreen.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.gameScreen.Location = new System.Drawing.Point(0, 0);
            this.gameScreen.Name = "gameScreen";
            this.gameScreen.Size = new System.Drawing.Size(517, 486);
            this.gameScreen.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.gameScreen.TabIndex = 0;
            this.gameScreen.TabStop = false;
            // 
            // GameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(681, 486);
            this.Controls.Add(this.gameScreen);
            this.Controls.Add(this.toolPanel);
            this.Name = "GameForm";
            this.Text = "GameForm";
            this.Load += new System.EventHandler(this.GameForm_Load);
            this.toolPanel.ResumeLayout(false);
            this.toolPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStep)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDistance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gameScreen)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private SnowInTheNight.CustomPictureBox gameScreen;
        private System.Windows.Forms.Panel toolPanel;
        private System.Windows.Forms.Label cursorPositionLabel;
        private System.Windows.Forms.CheckBox checkBoxEdges;
        private System.Windows.Forms.CheckBox checkBoxVertices;
        private System.Windows.Forms.NumericUpDown numericUpDownDistance;
        private System.Windows.Forms.NumericUpDown numericUpDownStep;
        private System.Windows.Forms.ComboBox comboBoxTypePolyline;
        private System.Windows.Forms.NumericUpDown numericUpDownSpeed;
        private System.Windows.Forms.Button buttonGenerate;
        private System.Windows.Forms.TextBox textBoxComment;
        private System.Windows.Forms.CheckBox checkBoxHQ;
    }
}