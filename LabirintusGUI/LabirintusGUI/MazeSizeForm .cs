using System;
using System.Windows.Forms;

namespace LabirintusGUI
{
    public partial class MazeSizeForm : Form
    {
        public int WidthValue { get; private set; } = 10;
        public int HeightValue { get; private set; } = 10;

        private NumericUpDown widthInput;
        private NumericUpDown heightInput;
        private Button okButton;

        public MazeSizeForm()
        {
            this.Text = "Labirintus mérete";
            this.ClientSize = new System.Drawing.Size(250, 120);

            var widthLabel = new Label { Text = "Szélesség:", Location = new System.Drawing.Point(10, 10), AutoSize = true };
            this.Controls.Add(widthLabel);
            widthInput = new NumericUpDown { Minimum = 5, Maximum = 50, Value = 10, Location = new System.Drawing.Point(100, 10) };
            this.Controls.Add(widthInput);

            var heightLabel = new Label { Text = "Magasság:", Location = new System.Drawing.Point(10, 40), AutoSize = true };
            this.Controls.Add(heightLabel);
            heightInput = new NumericUpDown { Minimum = 5, Maximum = 50, Value = 10, Location = new System.Drawing.Point(100, 40) };
            this.Controls.Add(heightInput);

            okButton = new Button { Text = "OK", Location = new System.Drawing.Point(80, 70), Width = 80 };
            okButton.Click += OkButton_Click;
            this.Controls.Add(okButton);
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if((int)widthInput.Value % 2 == 0)
            {
                WidthValue = ((int)widthInput.Value + 1);
            }
            if ((int)heightInput.Value % 2 == 0)
            {
                HeightValue = ((int)heightInput.Value + 1);
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
