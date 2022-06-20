using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FormsDrawingTemplate.Drawing
{
    public class BufferedDrawing
    {
        private BufferedGraphicsContext context;
        private BufferedGraphics grafx;

        private Form targetForm;
        private Action<Graphics> drawFunc;

        private int frameCount;

        public BufferedDrawing(Form _targetForm, Action<Graphics> _drawFunc)
        {
            targetForm = _targetForm;
            drawFunc = _drawFunc;

            //Bind to form resize event
            targetForm.Resize += Form_Resize;

            context = BufferedGraphicsManager.Current;
            context.MaximumBuffer = new Size(targetForm.Width + 1, targetForm.Height + 1);
            grafx = context.Allocate(targetForm.CreateGraphics(), new Rectangle(0, 0, targetForm.Width, targetForm.Height));
        }

        // Handle resetting graphics object and redraw when form is resized
        private void Form_Resize(object sender, EventArgs e)
        {
            context.MaximumBuffer = new Size(targetForm.Width + 1, targetForm.Height + 1);
            if (grafx != null)
            {
                grafx.Dispose();
                grafx = null;
            }
            grafx = context.Allocate(targetForm.CreateGraphics(),
                new Rectangle(0, 0, targetForm.Width, targetForm.Height));

            DrawToBuffer(grafx.Graphics);
            targetForm.Refresh();
        }

        private void DrawToBuffer(Graphics g)
        {
            //Clear form
            g.FillRectangle(Brushes.Black, 0, 0, targetForm.Width, targetForm.Height);

            //Run drawing function
            drawFunc(g);

            // Draw Frame count.
            g.DrawString($"Frame: {frameCount++}", new Font("Arial", 8), Brushes.White, 10, 10);
        }

        public void Render()
        {
            DrawToBuffer(grafx.Graphics);
            grafx.Render(Graphics.FromHwnd(targetForm.Handle));
        }
    }
}
