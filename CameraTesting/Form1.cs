using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;


using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace CameraTesting
{
    public partial class Form1 : Form
    {
        VideoCapture captureL;
        VideoCapture captureC;
        VideoCapture captureR;
        Detection m_det_captureL;
        Detection m_det_captureC;
        Detection m_det_captureR;
        CameraCalc cameraCalculations;

        public Form1()
        {
            InitializeComponent();
            m_det_captureL= new Detection();
            m_det_captureC = new Detection();
            m_det_captureR = new Detection();
            cameraCalculations = new CameraCalc();
            

            //Initialize CameraMatrices calculations and shows results in selected textBox
            Detection.CameraMatrices(textBox1, textBox2, textBox3);
        }

        private void StartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (captureL==null)
            {
                captureL = new Emgu.CV.VideoCapture(2);
                //captureL.SetCaptureProperty(CapProp.FrameWidth, 1280);
                //captureL.SetCaptureProperty(CapProp.FrameHeight, 1024);
            }
            captureL.ImageGrabbed += Capture_ImageGrabbed1;
            captureL.Start();
        }

        private void Capture_ImageGrabbed1(object sender, EventArgs e)
        {
            try
            {
                m_det_captureL.FindCenter(captureL, pictureBox1, pictureBox4);
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message);
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
                m_det_captureC.FindCenter(captureC, pictureBox2, pictureBox5);
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
                m_det_captureR.FindCenter(captureR, pictureBox3, pictureBox6);
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

        private void StartColorMaskToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (captureL == null)
            {
                captureL = new Emgu.CV.VideoCapture(2);
                //captureL.SetCaptureProperty(CapProp.FrameWidth, 1280);
                //captureL.SetCaptureProperty(CapProp.FrameHeight, 1024);
            }
            captureL.ImageGrabbed += Capture_ImageGrabbed1;
            captureL.Start();

            if (captureC == null)
            {
                captureC = new Emgu.CV.VideoCapture(3);
            }
            captureC.ImageGrabbed += Capture_ImageGrabbed2;
            captureC.Start();

            if (captureR == null)
            {
                captureR = new Emgu.CV.VideoCapture(0);
            }
            captureR.ImageGrabbed += CaptureR_ImageGrabbed3;
            captureR.Start();

        }

        private void Button2_Click(object sender, EventArgs e)
        {
            cameraCalculations.DLT(m_det_captureC.center01, m_det_captureC.center02, m_det_captureR.center01, m_det_captureR.center02, m_det_captureL.center01, m_det_captureL.center02, textBox4);
        }
    }
}
