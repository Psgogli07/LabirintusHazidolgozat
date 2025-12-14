using MazeLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LabirintusGUI
{
    public partial class Form1 : Form
    {
        private Maze maze;
        private PathFinder pathFinder;
        private Button[,] buttons;
        private int cellSize = 40;

        private ComboBox exitCombo;
        private ComboBox obstacleTypeCombo;
        private NumericUpDown obstacleCost;
        private Button findPathBtn;
        private Button findAltPathsBtn;
        private Label exitLabel;
        private Button removeWallsBtn;

        private int mazeWidth;
        private int mazeHeight;

        public Form1(int width, int height)
        {
            mazeWidth = width;
            mazeHeight = height;
            InitializeComponent();
            this.Text = "Labirintus";
            InitializeMaze();
        }

        private void InitializeMaze()
        {
            maze = new Maze(mazeWidth, mazeHeight);
            maze.Generate();
            pathFinder = new PathFinder(maze);

            buttons = new Button[maze.Width, maze.Height];
            this.ClientSize = new Size(maze.Width * cellSize + 20, maze.Height * cellSize + 120);

            for (int x = 0; x < maze.Width; x++)
            {
                for (int y = 0; y < maze.Height; y++)
                {
                    var btn = new Button();
                    btn.Location = new Point(x * cellSize, y * cellSize);
                    btn.Size = new Size(cellSize, cellSize);
                    btn.Tag = maze.Grid[x, y];
                    btn.BackColor = GetColor(maze.Grid[x, y]);
                    btn.Click += Cell_Click;
                    buttons[x, y] = btn;
                    this.Controls.Add(btn);
                }
            }

            exitLabel = new Label();
            exitLabel.Text = "Válassz kijárat:";
            exitLabel.Location = new Point(10, maze.Height * cellSize + 10);
            exitLabel.AutoSize = true;
            this.Controls.Add(exitLabel);

            exitCombo = new ComboBox();
            exitCombo.Location = new Point(120, maze.Height * cellSize + 10);
            exitCombo.Width = 100;
            exitCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            exitCombo.DataSource = maze.Exits.Select(e => $"({e.X},{e.Y})").ToList();
            exitCombo.SelectedIndexChanged += ExitCombo_SelectedIndexChanged;
            this.Controls.Add(exitCombo);

            obstacleTypeCombo = new ComboBox();
            obstacleTypeCombo.Location = new Point(10, maze.Height * cellSize + 40);
            obstacleTypeCombo.Width = 100;
            obstacleTypeCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            obstacleTypeCombo.Items.AddRange(new string[] { "Gödör", "Csapda", "Zárt ajtó" });
            obstacleTypeCombo.SelectedIndex = 0;
            this.Controls.Add(obstacleTypeCombo);

            obstacleCost = new NumericUpDown();
            obstacleCost.Location = new Point(120, maze.Height * cellSize + 40);
            obstacleCost.Minimum = 1;
            obstacleCost.Maximum = 10;
            obstacleCost.Value = 3;
            this.Controls.Add(obstacleCost);

            findPathBtn = new Button();
            findPathBtn.Text = "Legrövidebb út";
            findPathBtn.Location = new Point(230, maze.Height * cellSize + 10);
            findPathBtn.Click += FindPathBtn_Click;
            this.Controls.Add(findPathBtn);

            findAltPathsBtn = new Button();
            findAltPathsBtn.Text = "Alternatív utak";
            findAltPathsBtn.Location = new Point(340, maze.Height * cellSize + 10);
            findAltPathsBtn.Click += FindAltPathsBtn_Click;
            this.Controls.Add(findAltPathsBtn);

            removeWallsBtn = new Button();
            removeWallsBtn.Text = "Több út nyitása";
            removeWallsBtn.Location = new Point(450, maze.Height * cellSize + 10);
            removeWallsBtn.AutoSize = true;
            removeWallsBtn.Click += RemoveWallsBtn_Click;
            this.Controls.Add(removeWallsBtn);

            RefreshButtons();
        }

        private void ExitCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshButtons();
        }

        private void Cell_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            var cell = (Cell)btn.Tag;

            if (cell.Type == CellType.Empty)
            {
                string type = obstacleTypeCombo.SelectedItem.ToString();
                int cost = (int)obstacleCost.Value;
                Obstacle obs = null;
                if (type == "Gödör") obs = Obstacle.Hole(cost);
                else if (type == "Csapda") obs = Obstacle.Trap(cost);
                else if (type == "Zárt ajtó") obs = Obstacle.LockedDoor(cost);

                maze.AddObstacle(cell.X, cell.Y, obs);
            }
            else if (cell.Type == CellType.Obstacle)
            {
                cell.Type = CellType.Empty;
                cell.Obstacle = null;
            }

            RefreshButtons();
        }

        private void FindPathBtn_Click(object sender, EventArgs e)
        {
            int idx = exitCombo.SelectedIndex;
            if (idx >= 0)
            {
                RefreshButtons();
                var path = pathFinder.FindShortestPath(maze.Exits[idx]);
                DrawPath(path, Color.Yellow);
            }
        }

        private void FindAltPathsBtn_Click(object sender, EventArgs e)
        {
            int idx = exitCombo.SelectedIndex;
            if (idx >= 0)
            {
                RefreshButtons();
                var alternatives = pathFinder.FindAlternativePaths(maze.Exits[idx], 3);
                Color[] colors = { Color.Yellow, Color.LightGreen, Color.LightBlue, Color.Pink, Color.Orange };
                int colorIndex = 0;
                foreach (var path in alternatives)
                {
                    Color c = colors[colorIndex % colors.Length];
                    DrawPath(path, c);
                    colorIndex++;
                }
            }
        }

        private void RemoveWallsBtn_Click(object sender, EventArgs e)
        {
            maze.RemoveRandomWalls(10);
            RefreshButtons();
        }

        private void DrawPath(List<Cell> path, Color color)
        {
            if (path == null) return;
            foreach (var cell in path)
            {
                // Start és Exit színe mindig látszódjon
                if (cell.Type == CellType.Start || cell.Type == CellType.Exit)
                    continue;
                buttons[cell.X, cell.Y].BackColor = color;
            }
        }

        private void RefreshButtons()
        {
            for (int x = 0; x < maze.Width; x++)
            {
                for (int y = 0; y < maze.Height; y++)
                {
                    buttons[x, y].BackColor = GetColor(maze.Grid[x, y]);
                }
            }

            // Külön jelöljük a kijáratot kék színnel a combobox alapján
            int idx = exitCombo.SelectedIndex;
            if (idx >= 0)
            {
                var exitCell = maze.Exits[idx];
                buttons[exitCell.X, exitCell.Y].BackColor = Color.Red;
            }

            // Start mindig zöld
            var start = maze.Start;
            buttons[start.X, start.Y].BackColor = Color.Green;
        }

        private Color GetColor(Cell cell)
        {
            if (cell.Type == CellType.Start)
                return Color.Green;
            else if (cell.Type == CellType.Exit)
                return Color.Red;
            else if (cell.Type == CellType.Wall)
                return Color.Black;
            else if (cell.Type == CellType.Obstacle)
                return Color.Orange;
            else
                return Color.White;
        }
    }
}
