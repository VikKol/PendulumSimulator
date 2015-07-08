using System.Windows.Forms;

namespace Pendulum
{
    public class MouseCapture
    {
        public int X = 0;
        public int Y = 0;
        public int PreviousX = 0;
        public int PreviousY = 0;
        public bool IsDown = false;
        public MouseButtons Button = MouseButtons.Left;
    }
}
