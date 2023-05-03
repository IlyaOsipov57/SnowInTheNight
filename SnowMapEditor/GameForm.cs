using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnowMapEditor
{
    public partial class GameForm : Form
    {
        public GameForm()
        {
            Instance = this;
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += GameForm_KeyDown;
            this.KeyUp += GameForm_KeyUp;
            this.gameScreen.MouseDown += GameForm_MouseDown;
            this.gameScreen.MouseUp += GameForm_MouseUp;
            Meta.GoFullscreen(this);
            this.comboBoxTypePolyline.SelectedIndex = 0;
        }

        void GameForm_MouseUp(object sender, MouseEventArgs e)
        {
            var key = e.Button;
            Meta.InputController.OnKeyUp(key);
        }

        void GameForm_MouseDown(object sender, MouseEventArgs e)
        {
            gameScreen.Focus();
            var key = e.Button;
            Meta.InputController.OnKeyDown(key);
        }

        void GameForm_KeyUp(object sender, KeyEventArgs e)
        {
            var key = e.KeyCode;
            Meta.InputController.OnKeyUp(key);
        }

        void GameForm_KeyDown(object sender, KeyEventArgs e)
        {
            var key = e.KeyCode;
            Meta.InputController.OnKeyDown(key);
        }

        public static int FitWidth = 400;
        public IntPoint GetImageSize()
        {
            if (gameScreen.Width <= 0 || gameScreen.Height <= 0)
                return new IntPoint(1, 1);

            var bounds = gameScreen.Bounds;

            return new IntPoint(bounds.Width,bounds.Height);

        }


        public void UpdateImage(Image image)
        {
            var previousImage = this.gameScreen.Image;
            this.gameScreen.Image = image;
            if (previousImage != null)
                previousImage.Dispose();
        }
        private void GameForm_FormClosing(object sender, FormClosingEventArgs e)
        {
        }
        private static GameForm Instance;

        public static void CloseForm()
        {
            Instance.Close();
        }

        public PanelData UpdateToolPanel (PanelData panelData)
        {
            cursorPositionLabel.Text = panelData.CursorPosition;
            panelData.AllowVertices = checkBoxVertices.Checked;
            panelData.AllowEdges = checkBoxEdges.Checked;
            panelData.FixDistance = (double)numericUpDownDistance.Value;
            panelData.GridStep = (int)numericUpDownStep.Value;
            panelData.CreatingType = comboBoxTypePolyline.SelectedIndex;
            panelData.Speed = (int)numericUpDownSpeed.Value;
            this.MapData = panelData.MapData;
            if (panelData.Comment != null)
            {
                textBoxComment.Text = panelData.Comment;
            }
            panelData.Comment = textBoxComment.Text;
            panelData.useHQ = checkBoxHQ.Checked;
            return panelData;
        }

        internal IntPoint GetCursorPosition()
        {
            return (IntPoint)gameScreen.PointToClient(Cursor.Position);
        }

        private void comboBoxTypeChoise_SelectedIndexChanged(object sender, EventArgs e)
        {
            gameScreen.Focus();
        }
        private String MapData;
        private void buttonGenerate_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(MapData);
        }

        bool UpdateCommentText = true;
        private void textBox1_Enter(object sender, EventArgs e)
        {
            UpdateCommentText = false;
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            UpdateCommentText = true;
        }

        private void GameForm_Load(object sender, EventArgs e)
        {
            Meta.StartMainLoop(this);
        }
    }

    public class PanelData
    {
        public String CursorPosition;
        public bool AllowVertices;
        public bool AllowEdges;
        public double FixDistance;
        public int GridStep;
        public int CreatingType;
        public int Speed;
        public String MapData;
        public String Comment;
        public bool useHQ;
    }
}
