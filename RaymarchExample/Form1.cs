using FormsDrawingTemplate.Drawing;
using RaymarchExample.Drawing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RaymarchExample
{
    public partial class MainForm : Form
    {
        BufferedDrawing drawObj;
        Raymarcher raymarcher;
        public MainForm()
        {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

            drawObj = new BufferedDrawing(this, Render);
            raymarcher = new Raymarcher(ClientSize.Width, ClientSize.Height);
            raymarcher.SdfObjs.Add(new SphereSdf(new Vector3(5, 0, 0), 1));
            raymarcher.SdfObjs.Add(new BoxSdf(new Vector3(6, 0, -3), new Vector3(1,1,1)));
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            
        }

        private void Render(Graphics g)
        {
            //Parallel.For(0, ClientRectangle.Width, (x) =>
            for(int x = 0; x < ClientRectangle.Width; ++x)
            {
                for (int y = 0; y < ClientRectangle.Height; ++y)
                {
                    Vector3 result = raymarcher.March(x, y);
                    int red = (int)Math.Round(result.X * 255);
                    int green = (int)Math.Round(result.Y * 255);
                    int blue = (int)Math.Round(result.Z * 255);
                    g.FillRectangle(new SolidBrush(Color.FromArgb(red, green, blue)), new Rectangle(x, y, 1, 1));
                }
            }//);
        }

        private void tmrRender_Tick(object sender, EventArgs e)
        {
            drawObj.Render();
        }
    }
}
