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
            CvInvoke.GaussianBlur(m, blurFrame, new Size(3, 3), 0);
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
            }

            //For debugging, show on screen how many contours have been found
            CvInvoke.PutText(m, "Detected Shapes: " + Convert.ToString(contours.Size), new Point(10, 10), FontFace.HersheySimplex, 0.3, new Bgr(255, 0, 0).MCvScalar);

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
            center01 = new Point(x1, y1);
            center02 = new Point(x2, y2);

            //To make sure that calculated direction vector is facing down
            if (center01.Y > center02.Y)
            {
                var tmp = center01;
                center01 = center02;
                center02 = tmp;
            }

            //Show circles around each center of contour
            CvInvoke.Circle(m, center01, 10, new Bgr(0, 0, 255).MCvScalar, 1);
            CvInvoke.Circle(m, center02, 10, new Bgr(255, 0, 0).MCvScalar, 1);

            //Show line connecting each center of contour
            CvInvoke.Line(m, center01, center02, new Bgr(0, 255, 0).MCvScalar, 1);

            //Show final picture 
            targetVisible.Image = m.ToImage<Bgr, byte>().Bitmap;
        }

        //Homogeneous Transformation Matrix - Passing just translation vector
        public static Matrix<double> TransMat(Matrix<double> txx)
        {
            var t = new Matrix<double>(4, 4);
            t[0,0] = 1;
            t[0,1] = 0;
            t[0,2] = 0;
            t[0, 3] = txx[0, 0];
            t[1, 0] = 0;
            t[1, 1] = 1;
            t[1, 2] = 0;
            t[1, 3] = txx[0, 1];
            t[2, 0] = 0;
            t[2, 1] = 0;
            t[2, 2] = 1;
            t[2, 3] = txx[0, 2];
            t[3, 0] = 0;
            t[3, 1] = 0;
            t[3, 2] = 0;
            t[3, 3] = 1;

            return t;
        }

        //Display Matrix - Method overload for double[,] (matrix)
        public static void DisplayMatrix(double[,] matrix, TextBox targetTextBox)
        {
            var matrixString = "";
            for (var i = 0; i < matrix.GetLength(0); i++)
            {
                for (var j = 0; j < matrix.GetLength(1); j++)
                {
                    matrixString += matrix[i, j].ToString();
                    matrixString += " ";
                }

                matrixString += Environment.NewLine;
            }

            targetTextBox.Text = matrixString;
        }

        //Display Matrix - Method overload for double[] (vector)
        public static void DisplayMatrix(double[] matrix, TextBox targetTextBox)
        {
            var matrixString = "";
            for (var i = 0; i < matrix.GetLength(0); i++)
            {
                matrixString += matrix[i].ToString();
                matrixString += " ";
                matrixString += Environment.NewLine;
            }

            targetTextBox.Text = matrixString;
        }

        //Display Matrix - Method overload for Matrix
        public static void DisplayMatrix(Matrix<double> matrix, TextBox targetTextBox)
        {
            var matrixString = "";
            for (var i = 0; i < matrix.Rows; i++)
            {
                for (var j = 0; j < matrix.Cols; j++)
                {
                    matrixString += matrix[i,j].ToString();
                    matrixString += " ";
                }

                matrixString += Environment.NewLine;
            }

            targetTextBox.Text = matrixString;
        }

        //Camera Matrices Calculations
        public static void CameraMatrices(TextBox p0TextBox, TextBox p1TextBox, TextBox p2TextBox)
        {
            //Camera Calibration Matrices - Intrinsic Calibration
            var k0 = new Matrix<double>(3, 3);
            k0[0, 0] = 6.1755e2;
            k0[0, 1] = 0;
            k0[0, 2] = 3.2615e2;
            k0[1, 0] = 0;
            k0[1, 1] = 6.1863e2;
            k0[1, 2] = 2.2548e2;
            k0[2, 0] = 0;
            k0[2, 1] = 0;
            k0[2, 2] = 1;

            var k1 = new Matrix<double>(3, 3);
            k1[0, 0] = 6.1139e2;
            k1[0, 1] = 0;
            k1[0, 2] = 3.1687e2;
            k1[1, 0] = 0;
            k1[1, 1] = 6.1191e2;
            k1[1, 2] = 2.3119e2;
            k1[2, 0] = 0;
            k1[2, 1] = 0;
            k1[2, 2] = 1;

            var k2 = new Matrix<double>(3, 3);
            k2[0, 0] = 6.1793e2;
            k2[0, 1] = 0;
            k2[0, 2] = 3.2971e2;
            k2[1, 0] = 0;
            k2[1, 1] = 6.2355e2;
            k2[1, 2] = 2.2776e2;
            k2[2, 0] = 0;
            k2[2, 1] = 0;
            k2[2, 2] = 1;

            //Translation Between Cameras - Extrinsic Calibration
            var t31 = new Matrix<double>(1, 3);
            t31[0, 0] = -1.5132e2;
            t31[0, 1] = 6.7363e-1;
            t31[0, 2] = 4.47169;

            var t21 = new Matrix<double>(1, 3);
            t21[0, 0] = 1.4760e2;
            t21[0, 1] = 3.0827e-1;
            t21[0, 2] = -6.70344;

            var t11 = new Matrix<double>(1, 3);
            t11[0, 0] = 0;
            t11[0, 1] = 0;
            t11[0, 2] = 0;


            var pII = new Matrix<double>(3, 4);
            pII[0, 0] = 1;
            pII[1, 1] = 1;
            pII[2, 2] = 1;
            //If we used Mat(), we could use Identity matrix implicitly. To be checked
            //var pII = Mat.Eye(3,4, DepthType.Cv32F,1);

            //Homogeneous Transformation Matrix

            var transMat21 = Detection.TransMat(t21);
            var transMat31 = Detection.TransMat(t31);
            var transMat11 = Detection.TransMat(t11);

            //Camera Matrices

            var p0 = (k0 * pII) * transMat11;
            var p1 = (k1 * pII) * transMat21;
            var p2 = (k2 * pII) * transMat31;

            Detection.DisplayMatrix(p0, p0TextBox);
            Detection.DisplayMatrix(p1, p1TextBox);
            Detection.DisplayMatrix(p2, p2TextBox);


        }



    }
}
