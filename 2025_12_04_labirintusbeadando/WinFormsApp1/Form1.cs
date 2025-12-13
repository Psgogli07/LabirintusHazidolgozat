using _2025_12_04_labirintusbeadando.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private MazeService mazeService;
        private Panel panelMaze;
        private Panel panelControl;
        private TextBox tbSize;
        private TextBox tbExitCount;
        private Button btnGenerate;
        private Button btnFindShortest;
        private Button btnFindAlternatives;
        private ComboBox cmbExitSelect;
        private ComboBox cmbPathSelect;
        private ComboBox cmbObstacleType;
        private NumericUpDown nudObstacleCost;
        private CheckBox chkIncludeCosts;
        private Label lblStatus;
        private RadioButton[] obstacleRadios;

        private int cellSize;
        private Timer animationTimer;
        private int animationStep;
        private List<Position> currentAnimationPath;
        private bool isPlacingObstacle;
        private CellType selectedObstacleType;

        public Form1()
        {
            InitializeComponent();
            InitializeMazeService();
            SetupUI();
        }

        private void InitializeMazeService()
        {
            mazeService = new MazeService();
            mazeService.MazeChanged += MazeService_MazeChanged;
            mazeService.PathsChanged += MazeService_PathsChanged;
        }

        private void SetupUI()
        {
            this.Text = "Labirintus Generátor";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Control Panel
            panelControl = new Panel
            {
                Location = new Point(10, 10),
                Size = new Size(250, 650),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.LightGray
            };
            this.Controls.Add(panelControl);

            // Maze Panel
            panelMaze = new Panel
            {
                Location = new Point(270, 10),
                Size = new Size(600, 600),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };
            panelMaze.Paint += PanelMaze_Paint;
            panelMaze.MouseClick += PanelMaze_MouseClick;
            this.Controls.Add(panelMaze);

            int yPos = 20;

            // Size input
            var lblSize = CreateLabel("Méret:", 20, yPos);
            panelControl.Controls.Add(lblSize);

            tbSize = new TextBox
            {
                Location = new Point(120, yPos),
                Size = new Size(100, 20),
                Text = "21"
            };
            panelControl.Controls.Add(tbSize);
            yPos += 30;

            // Exit count input
            var lblExit = CreateLabel("Kijáratok:", 20, yPos);
            panelControl.Controls.Add(lblExit);

            tbExitCount = new TextBox
            {
                Location = new Point(120, yPos),
                Size = new Size(100, 20),
                Text = "2"
            };
            panelControl.Controls.Add(tbExitCount);
            yPos += 30;

            // Generate button
            btnGenerate = CreateButton("Labirintus generálása", 20, yPos, 200);
            btnGenerate.Click += BtnGenerate_Click;
            panelControl.Controls.Add(btnGenerate);
            yPos += 40;

            // Exit selection
            var lblExitSelect = CreateLabel("Kijárat választás:", 20, yPos);
            panelControl.Controls.Add(lblExitSelect);
            yPos += 25;

            cmbExitSelect = new ComboBox
            {
                Location = new Point(20, yPos),
                Size = new Size(200, 20),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbExitSelect.SelectedIndexChanged += CmbExitSelect_SelectedIndexChanged;
            panelControl.Controls.Add(cmbExitSelect);
            yPos += 35;

            // Find shortest path button
            btnFindShortest = CreateButton("Legrövidebb út keresése", 20, yPos, 200);
            btnFindShortest.Click += BtnFindShortest_Click;
            panelControl.Controls.Add(btnFindShortest);
            yPos += 35;

            // Include costs checkbox
            chkIncludeCosts = new CheckBox
            {
                Text = "Akadály költségek figyelembe vétele",
                Location = new Point(25, yPos),
                Size = new Size(200, 20),
                Checked = true
            };
            panelControl.Controls.Add(chkIncludeCosts);
            yPos += 30;

            // Find alternatives button
            btnFindAlternatives = CreateButton("Alternatív utak keresése", 20, yPos, 200);
            btnFindAlternatives.Click += BtnFindAlternatives_Click;
            panelControl.Controls.Add(btnFindAlternatives);
            yPos += 40;

            // Path selection
            var lblPathSelect = CreateLabel("Útvonal választás:", 20, yPos);
            panelControl.Controls.Add(lblPathSelect);
            yPos += 25;

            cmbPathSelect = new ComboBox
            {
                Location = new Point(20, yPos),
                Size = new Size(200, 20),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbPathSelect.SelectedIndexChanged += CmbPathSelect_SelectedIndexChanged;
            panelControl.Controls.Add(cmbPathSelect);
            yPos += 40;

            // Obstacle section
            var lblObstacle = CreateLabel("Akadályok:", 20, yPos);
            panelControl.Controls.Add(lblObstacle);
            yPos += 25;

            // Obstacle type selection
            var lblObstacleType = CreateLabel("Akadály típus:", 20, yPos);
            panelControl.Controls.Add(lblObstacleType);
            yPos += 25;

            cmbObstacleType = new ComboBox
            {
                Location = new Point(20, yPos),
                Size = new Size(150, 20),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            foreach (var obstacle in Obstacle.AllObstacles)
            {
                cmbObstacleType.Items.Add(obstacle.Name);
            }
            cmbObstacleType.SelectedIndex = 0;
            cmbObstacleType.SelectedIndexChanged += CmbObstacleType_SelectedIndexChanged;
            panelControl.Controls.Add(cmbObstacleType);
            yPos += 30;

            // Obstacle cost
            var lblObstacleCost = CreateLabel("Akadály költség:", 20, yPos);
            panelControl.Controls.Add(lblObstacleCost);
            yPos += 25;

            nudObstacleCost = new NumericUpDown
            {
                Location = new Point(20, yPos),
                Size = new Size(100, 20),
                Minimum = 1,
                Maximum = 10,
                Value = 2
            };
            nudObstacleCost.ValueChanged += NudObstacleCost_ValueChanged;
            panelControl.Controls.Add(nudObstacleCost);
            yPos += 35;

            // Place obstacle button
            var btnPlaceObstacle = CreateButton("Akadály elhelyezése", 20, yPos, 200);
            btnPlaceObstacle.Click += BtnPlaceObstacle_Click;
            panelControl.Controls.Add(btnPlaceObstacle);
            yPos += 35;

            // Remove obstacle button
            var btnRemoveObstacle = CreateButton("Akadály eltávolítása", 20, yPos, 200);
            btnRemoveObstacle.Click += BtnRemoveObstacle_Click;
            panelControl.Controls.Add(btnRemoveObstacle);
            yPos += 40;

            // Status label
            lblStatus = new Label
            {
                Location = new Point(20, yPos),
                Size = new Size(200, 40),
                Text = "Készültségi állapot: Inicializálva",
                ForeColor = Color.DarkBlue
            };
            panelControl.Controls.Add(lblStatus);

            // Animation timer
            animationTimer = new Timer
            {
                Interval = 50
            };
            animationTimer.Tick += AnimationTimer_Tick;

            UpdateStatus("Kész");
        }

        private Label CreateLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = true,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
        }

        private Button CreateButton(string text, int x, int y, int width)
        {
            return new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, 30),
                Font = new Font("Arial", 9)
            };
        }

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                int size = int.Parse(tbSize.Text);
                int exitCount = int.Parse(tbExitCount.Text);

                if (!mazeService.ValidateExitCount(size, exitCount))
                {
                    MessageBox.Show($"A kijáratok száma nem haladhatja meg a méret 20%-át! Maximum: {mazeService.CalculateMaxExits(size)}");
                    return;
                }

                mazeService.GenerateMaze(size, exitCount);
                UpdateStatus($"Labirintus generálva: {size}x{size}, {exitCount} kijárat");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba: {ex.Message}", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MazeService_MazeChanged(object sender, EventArgs e)
        {
            UpdateExitComboBox();
            UpdateUI();
            panelMaze.Invalidate();
        }

        private void MazeService_PathsChanged(object sender, EventArgs e)
        {
            UpdatePathComboBox();
            UpdateUI();
            panelMaze.Invalidate();
        }

        private void UpdateExitComboBox()
        {
            cmbExitSelect.Items.Clear();

            if (mazeService.CurrentMaze != null && mazeService.CurrentMaze.Exits != null)
            {
                for (int i = 0; i < mazeService.CurrentMaze.Exits.Count; i++)
                {
                    var exit = mazeService.CurrentMaze.Exits[i];
                    // Explicitly get X and Y properties instead of relying on ToString()
                    cmbExitSelect.Items.Add($"Kijárat {i + 1} ({exit.X}, {exit.Y})");
                }

                if (mazeService.CurrentMaze.Exits.Count > 0)
                {
                    cmbExitSelect.SelectedIndex = 0;
                }
            }
        }

        private void UpdatePathComboBox()
        {
            cmbPathSelect.Items.Clear();

            if (mazeService.CurrentPaths != null)
            {
                for (int i = 0; i < mazeService.CurrentPaths.Count; i++)
                {
                    var path = mazeService.CurrentPaths[i];
                    cmbPathSelect.Items.Add($"Út {i + 1} (Hossz: {path.PathLength}, Költség: {path.TotalCost})");
                }

                if (mazeService.CurrentPaths.Count > 0)
                {
                    cmbPathSelect.SelectedIndex = mazeService.SelectedPathIndex;
                }
            }
        }

        private void UpdateUI()
        {
            if (mazeService.CurrentMaze != null)
            {
                cellSize = Math.Min(panelMaze.Width / mazeService.CurrentMaze.Size,
                                   panelMaze.Height / mazeService.CurrentMaze.Size);

                btnFindShortest.Enabled = true;
                btnFindAlternatives.Enabled = true;
                cmbExitSelect.Enabled = true;
                cmbObstacleType.Enabled = true;
                nudObstacleCost.Enabled = true;
            }
            else
            {
                btnFindShortest.Enabled = false;
                btnFindAlternatives.Enabled = false;
                cmbExitSelect.Enabled = false;
                cmbPathSelect.Enabled = false;
                cmbObstacleType.Enabled = false;
                nudObstacleCost.Enabled = false;
            }
        }

        private void CmbExitSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mazeService.CurrentMaze != null &&
                mazeService.CurrentMaze.Exits != null &&
                cmbExitSelect.SelectedIndex >= 0)
            {
                var selectedExit = mazeService.CurrentMaze.Exits[cmbExitSelect.SelectedIndex];
                mazeService.SelectExit(selectedExit);
                UpdateStatus($"Kijárat kiválasztva: ({selectedExit.X}, {selectedExit.Y})");
            }
        }

        private void CmbPathSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPathSelect.SelectedIndex >= 0)
            {
                mazeService.SelectPath(cmbPathSelect.SelectedIndex);

                if (mazeService.SelectedPath != null)
                {
                    StartAnimation(mazeService.SelectedPath.Path);
                    UpdateStatus($"Útvonal {cmbPathSelect.SelectedIndex + 1} kiválasztva");
                }
            }
        }

        private void BtnFindShortest_Click(object sender, EventArgs e)
        {
            if (mazeService.CurrentMaze == null) return;

            mazeService.FindShortestPath();
            UpdateStatus("Legrövidebb út keresve");
        }

        private void BtnFindAlternatives_Click(object sender, EventArgs e)
        {
            if (mazeService.CurrentMaze == null) return;

            mazeService.FindAlternativePaths(3); // Get 3 alternative paths
            UpdateStatus("Alternatív utak keresve");
        }

        private void CmbObstacleType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbObstacleType.SelectedIndex >= 0)
            {
                var obstacle = Obstacle.AllObstacles[cmbObstacleType.SelectedIndex];
                selectedObstacleType = obstacle.Type;
                nudObstacleCost.Value = obstacle.Cost;
            }
        }

        private void NudObstacleCost_ValueChanged(object sender, EventArgs e)
        {
            if (selectedObstacleType != CellType.Wall)
            {
                mazeService.SetObstacleCost(selectedObstacleType, (int)nudObstacleCost.Value);
            }
        }

        private void BtnPlaceObstacle_Click(object sender, EventArgs e)
        {
            isPlacingObstacle = true;
            UpdateStatus("Kattintson a labirintusra az akadály elhelyezéséhez");
        }

        private void BtnRemoveObstacle_Click(object sender, EventArgs e)
        {
            isPlacingObstacle = false;
            UpdateStatus("Kattintson az eltávolítani kívánt akadályra");
        }

        private void PanelMaze_MouseClick(object sender, MouseEventArgs e)
        {
            if (mazeService.CurrentMaze == null) return;

            int x = e.X / cellSize;
            int y = e.Y / cellSize;

            if (x >= 0 && x < mazeService.CurrentMaze.Size &&
                y >= 0 && y < mazeService.CurrentMaze.Size)
            {
                if (isPlacingObstacle)
                {
                    mazeService.PlaceObstacle(x, y, selectedObstacleType);
                    UpdateStatus($"Akadály elhelyezve: ({x}, {y})");
                }
                else
                {
                    mazeService.RemoveObstacle(x, y);
                    UpdateStatus($"Akadály eltávolítva: ({x}, {y})");
                }
            }
        }

        private void StartAnimation(List<Position> path)
        {
            if (path == null || path.Count == 0) return;

            currentAnimationPath = path;
            animationStep = 0;
            animationTimer.Start();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            animationStep++;
            panelMaze.Invalidate();

            if (animationStep >= currentAnimationPath.Count)
            {
                animationTimer.Stop();
            }
        }

        private void PanelMaze_Paint(object sender, PaintEventArgs e)
        {
            if (mazeService.CurrentMaze == null) return;

            var maze = mazeService.CurrentMaze;
            var g = e.Graphics;

            // Draw cells
            for (int y = 0; y < maze.Size; y++)
            {
                for (int x = 0; x < maze.Size; x++)
                {
                    var cellType = maze.Grid[y, x];
                    Brush brush = GetBrushForCell(cellType);

                    g.FillRectangle(brush, x * cellSize, y * cellSize, cellSize, cellSize);
                    g.DrawRectangle(Pens.Gray, x * cellSize, y * cellSize, cellSize, cellSize);

                    // Draw cell type character
                    if (cellType != CellType.Path)
                    {
                        string text = ((char)cellType).ToString();
                        var font = new Font("Arial", cellSize / 3);
                        var textSize = g.MeasureString(text, font);
                        g.DrawString(text, font, Brushes.Black,
                            x * cellSize + (cellSize - textSize.Width) / 2,
                            y * cellSize + (cellSize - textSize.Height) / 2);
                    }
                }
            }

            // Highlight selected exit
            if (maze.SelectedExit != null)
            {
                var pen = new Pen(Color.Yellow, 3);
                g.DrawRectangle(pen,
                    maze.SelectedExit.X * cellSize,
                    maze.SelectedExit.Y * cellSize,
                    cellSize, cellSize);
            }

            // Draw path if animating
            if (currentAnimationPath != null && animationStep > 0)
            {
                for (int i = 0; i < Math.Min(animationStep, currentAnimationPath.Count); i++)
                {
                    var pos = currentAnimationPath[i];
                    g.FillRectangle(Brushes.Orange,
                        pos.X * cellSize + 2,
                        pos.Y * cellSize + 2,
                        cellSize - 4, cellSize - 4);
                }
            }
        }

        private Brush GetBrushForCell(CellType cellType)
        {
            return cellType switch
            {
                CellType.Wall => Brushes.Black,
                CellType.Path => Brushes.White,
                CellType.Start => Brushes.Green,
                CellType.Exit => Brushes.Red,
                CellType.Obstacle1 => Brushes.SandyBrown,  // Mud
                CellType.Obstacle2 => Brushes.LightBlue,   // Water
                CellType.Obstacle3 => Brushes.LightGray,   // Rocks
                _ => Brushes.White
            };
        }

        private void UpdateStatus(string message)
        {
            lblStatus.Text = $"Állapot: {message}";
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            animationTimer?.Stop();
            animationTimer?.Dispose();
        }
    }
}