using System;
using System.Windows.Forms;

namespace LabirintusGUI
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (var sizeForm = new MazeSizeForm())
            {
                if (sizeForm.ShowDialog() == DialogResult.OK)
                {
                    Application.Run(new Form1(sizeForm.WidthValue, sizeForm.HeightValue));
                }
            }
        }
    }
}
