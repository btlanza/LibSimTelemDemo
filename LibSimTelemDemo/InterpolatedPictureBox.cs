/* Benjamin Lanza
 * InterpolatedPictureBox.cs
 * October 15th, 2018 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace LibSimTelemDemo
{
    class InterpolatedPictureBox : PictureBox
    {
        public InterpolationMode InterMode { get; set; }

        protected override void OnPaint(PaintEventArgs pe)
        {
            pe.Graphics.InterpolationMode = InterMode;
            base.OnPaint(pe);
        }
    }
}
