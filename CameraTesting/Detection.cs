using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace CameraTesting
{
    class Detection
    {
        public Point center01 = new Point();
        public Point center02 = new Point();

        public void FindCenter(VideoCapture capture, PictureBox targetVisible, PictureBox targetBinary)
        {
            //Declare "frame" and retrieve it from camera
            Mat m = new Mat();
            capture.Retrieve(m);

            //Gaussian blurred image
            Mat blurFrame = new Mat();
            CvInvoke.GaussianBlur(m, blurFrame, new Size(7,7),0);
            //pictureBox4.Image = blurFrame.ToImage<Hsv, byte>().Bitmap;

            //Convert from BRG to HSV
            Mat hsvFrame = new Mat();
            CvInvoke.CvtColor(blurFrame, hsvFrame, ColorConversion.Bgr2Hsv);
            //pictureBox4.Image = hsvFrame.ToImage<Hsv, byte>().Bitmap;

            //HSV color space parameters
            Hsv lowerLimit = new Hsv(39, 42, 42);
            Hsv upperLimit = new Hsv(77, 208, 255);

            //Binary image based on threshold colors
            Mat mask1 = new Mat();
            CvInvoke.InRange(hsvFrame, new ScalarArray(lowerLimit.MCvScalar), new ScalarArray(upperLimit.MCvScalar), mask1);
            targetBinary.Image = mask1.ToImage<Hsv, byte>().Bitmap;

            //Find contours in the image
            var contours = new VectorOfVectorOfPoint();
            Mat hierarchy = new Mat();
            CvInvoke.FindContours(mask1, contours, hierarchy, RetrType.External, ChainApproxMethod.ChainApproxSimple);

            //Sort Contours - We are only interested in the two biggest contours in terms of area 
            //Don't think it's possible to sort a VectorOfVectorOfPoint(), so have to do it old-school...
            double largestArea1 = 0;
            double largestArea2 = 0;
            int largestContour1Index = 0;
            int largestContour2Index = 0;

            if (contours.Size > 0)
            {
                for (int i = 0; i < contours.Size; i++)
                {
                    double area = CvInvoke.ContourArea(contours[i]);
                    if (area > largestArea1)
                    {
                        largestArea2 = largestArea1;
                        largestArea1 = area;
                        largestContour1Index = i;
                    }
                    else if (area > largestArea2)
                    {
                        largestArea2 = area;
                        largestContour2Index = i;
                    }
                }
                //Declare Points for each contour found (only two biggest contours needed)
                VectorOfPoint c01 = contours[largestContour1Index];
                VectorOfPoint c02 = contours[largestContour2Index];

                //Moments - To calculate center of contour
                var m01 = CvInvoke.Moments(c01);
                var m02 = CvInvoke.Moments(c02);

                int x1 = (int)(m01.M10 / m01.M00);
                int y1 = (int)(m01.M01 / m01.M00);
                int x2 = (int)(m02.M10 / m02.M00);
                int y2 = (int)(m02.M01 / m02.M00);

                //Center of contours
                if (m01.M00 > 0.000001)
                {
                    center01 = new Point(x1, y1);
                }

                if (m02.M00 > 0.000001)
                {
                    center02 = new Point(x2, y2);
                }

                //To make sure that calculated direction vector is facing down
                if (center01.Y > center02.Y)
                {
                    var tmp = center01;
                    center01 = center02;
                    center02 = tmp;
                }
            }
            
            //For debugging, show on screen how many contours have been found
            CvInvoke.PutText(m, "Detected Shapes: " + Convert.ToString(contours.Size), new Point(10, 10), FontFace.HersheySimplex, 0.3, new Bgr(255, 0, 0).MCvScalar);

            //Show circles around each center of contour
            CvInvoke.Circle(m, center01, 10, new Bgr(0, 0, 255).MCvScalar, 1);
            CvInvoke.Circle(m, center02, 10, new Bgr(255, 0, 0).MCvScalar, 1);

            //Show line connecting each center of contour
            CvInvoke.Line(m, center01, center02, new Bgr(0, 255, 0).MCvScalar, 1);

            //Show final picture 
            targetVisible.Image = m.ToImage<Bgr, byte>().Bitmap;
        }


    }
}
