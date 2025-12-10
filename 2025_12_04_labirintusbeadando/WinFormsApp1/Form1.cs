using _2025_12_04_labirintusbeadando.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private Maze maze;
        private Panel panelMaze;
        private TextBox tbSize;
        private TextBox tbExitCount;
        private Button btnGenerate;
        private Button btnStartPath;
        private int cellSize;

        private List<Position> path;
        private int pathIndex;
        private System.Windows.Forms.Timer timer;

        public Form1()
        {
            InitializeComponent();

            // ===== INPUT MEZÕK =====
            Label lblSize = new Label() { Text = "Méret:", Location = new Point(10, 10), AutoSize = true };
            this.Controls.Add(lblSize);

            tbSize = new TextBox() { Location = new Point(lblSize.Right + 10, 10), Width = 50, Text = "21" };
            this.Controls.Add(tbSize);

            Label lblExit = new Label() { Text = "Kijáratok:", Location = new Point(10, lblSize.Bottom + 10), AutoSize = true };
            this.Controls.Add(lblExit);

            tbExitCount = new TextBox() { Location = new Point(lblExit.Right + 10, lblSize.Bottom + 10), Width = 50, Text = "1" };
            this.Controls.Add(tbExitCount);

            // ===== GENERATE GOMB =====
            btnGenerate = new Button() { Text = "Labirintus generálása", Location = new Point(10, 70) };
            btnGenerate.Click += BtnGenerate_Click;
            this.Controls.Add(btnGenerate);

            // ===== START PATH GOMB =====
            btnStartPath = new Button() { Text = "Ut animálása", Location = new Point(200, 70) };
            btnStartPath.Click += BtnStartPath_Click;
            this.Controls.Add(btnStartPath);

            // ===== PANEL =====
            panelMaze = new Panel() { Location = new Point(10, 110), Width = 500, Height = 500, BackColor = Color.LightGray };
            panelMaze.Paint += PanelMaze_Paint;
            this.Controls.Add(panelMaze);

            // ===== TIMER =====
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 100; // ms lépésenként
            timer.Tick += Timer_Tick;
        }

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            int size = int.Parse(tbSize.Text);
            int exitCount = int.Parse(tbExitCount.Text);

            MazeGenerator gen = new MazeGenerator();
            maze = gen.Generate(size, exitCount);

            // cellSize számítása
            cellSize = Math.Min(panelMaze.Width / maze.Size, panelMaze.Height / maze.Size);

            // Reset út animáció
            path = null;
            pathIndex = 0;
            timer.Stop();

            panelMaze.Invalidate();
        }

        private void BtnStartPath_Click(object sender, EventArgs e)
        {
            if (maze == null) return;

            MazeGenerator gen = new MazeGenerator();
            path = gen.FindShortestPath(maze);


            if (path.Count == 0) return;

            pathIndex = 0;
            timer.Start();
        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            pathIndex++;
            panelMaze.Invalidate();

            if (pathIndex >= path.Count)
                timer.Stop();
        }

        private void PanelMaze_Paint(object sender, PaintEventArgs e)
        {
            if (maze == null) return;

            for (int y = 0; y < maze.Size; y++)
            {
                for (int x = 0; x < maze.Size; x++)
                {
                    char c = maze.Grid[y, x];
                    Brush brush = Brushes.White;

                    if (c == '#') brush = Brushes.Black;
                    else if (c == 'S') brush = Brushes.Green;
                    else if (c == 'E') brush = Brushes.Red;

                    e.Graphics.FillRectangle(brush, x * cellSize, y * cellSize, cellSize, cellSize);
                }
            }

            // Útvonal kirajzolása sárgával
            if (path != null)
            {
                for (int i = 0; i <= pathIndex && i < path.Count; i++)
                {
                    var p = path[i];
                    e.Graphics.FillRectangle(Brushes.Yellow, p.X * cellSize, p.Y * cellSize, cellSize, cellSize);
                }
            }
        }
    }
}
