using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.CV.Structure;

namespace CameraTesting
{
    public partial class Form1 : Form
    {
        VideoCapture captureL;
        VideoCapture captureC;
        VideoCapture captureR;
        public Form1()
        {
            InitializeComponent();
        }

        private void StartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (captureL==null)
            {
                captureL = new Emgu.CV.VideoCapture(2);
            }
            captureL.ImageGrabbed += Capture_ImageGrabbed1;
            captureL.Start();
        }

        private void Capture_ImageGrabbed1(object sender, EventArgs e)
        {
            try
            {
                Mat m = new Mat();
                captureL.Retrieve(m);
                pictureBox1.Image = m.ToImage<Bgr, byte>().Bitmap;
            }
            catch (Exception)
            {

            }
        }

        private void StopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (captureL!=null)
            {
                captureL = null;
            }
        }

        private void PauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (captureL!=null)
            {
                captureL.Pause();
            }
        }

        private void CameraToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void StartToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (captureC == null)
            {
                captureC = new Emgu.CV.VideoCapture(3);
            }
            captureC.ImageGrabbed += Capture_ImageGrabbed2;
            captureC.Start();
       
        }

        private void Capture_ImageGrabbed2(object sender, EventArgs e)
        {
            try
            {
                Mat n = new Mat();
                captureC.Retrieve(n);
                pictureBox2.Image = n.ToImage<Bgr, byte>().Bitmap;
            }
            catch (Exception)
            {

            }
        }

        private void StopToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (captureC != null)
            {
                captureC = null;
            }
        }

        private void PauseToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (captureC != null)
            {
                captureC.Pause();
            }
        }

        private void StartToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (captureR == null)
            {
                captureR = new Emgu.CV.VideoCapture(0);
            }
            captureR.ImageGrabbed += CaptureR_ImageGrabbed3;
            captureR.Start();
        }

        private void CaptureR_ImageGrabbed3(object sender, EventArgs e)
        {
            try
            {
                Mat o = new Mat();
                captureR.Retrieve(o);
                pictureBox3.Image = o.ToImage<Bgr, byte>().Bitmap;
            }
            catch (Exception)
            {

            }
        }

        private void StopToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (captureR != null)
            {
                captureR = null;
            }
        }

        private void PauseToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (captureR != null)
            {
                captureR.Pause();
            }
        }
    }
}
