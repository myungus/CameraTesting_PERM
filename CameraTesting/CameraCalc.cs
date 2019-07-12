using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Configuration;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;

namespace CameraTesting
{
    class CameraCalc
    {
        public Matrix<double> p0 = new Matrix<double>(3, 4);
        public Matrix<double> p1 = new Matrix<double>(3, 4);
        public Matrix<double> p2 = new Matrix<double>(3, 4);

        //Camera Matrices Calculations
        public void CameraMatrices(TextBox p0TextBox, TextBox p1TextBox, TextBox p2TextBox)
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

            var transMat21 = TransMat(t21);
            var transMat31 = TransMat(t31);
            var transMat11 = TransMat(t11);

            //Camera Matrices

            p0 = (k0 * pII) * transMat11;
            p1 = (k1 * pII) * transMat21;
            p2 = (k2 * pII) * transMat31;

            Display.DisplayMatrix(p2, p0TextBox);
            Display.DisplayMatrix(p0, p1TextBox);
            Display.DisplayMatrix(p1, p2TextBox);

        }

        //Homogeneous Transformation Matrix - Passing just translation vector
        public static Matrix<double> TransMat(Matrix<double> txx)
        {
            var t = new Matrix<double>(4, 4);
            t[0, 0] = 1;
            t[0, 1] = 0;
            t[0, 2] = 0;
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

        //Direct Linear Triangulation
        public Matrix<double> DLT(Point center0, Point center1, Point center2)
        {

            var svdRow0 = center0.X * p0.GetRow(2)-p0.GetRow(0);
            var svdRow1 = center0.Y * p0.GetRow(2)-p0.GetRow(1);
            var svdRow2 = center1.X * p1.GetRow(2)-p1.GetRow(0);
            var svdRow3 = center1.Y * p1.GetRow(2)-p1.GetRow(1);
            var svdRow4 = center2.X * p2.GetRow(2)-p2.GetRow(0);
            var svdRow5 = center2.Y * p2.GetRow(2)-p2.GetRow(1);

            //used for debug with zero values instead of arguments
//            var svdRow0 = 0 * p0.GetRow(2)-p0.GetRow(0);
//            var svdRow1 = 0 * p0.GetRow(2)-p0.GetRow(1);
//            var svdRow2 = 0 * p1.GetRow(2)-p1.GetRow(0);
//            var svdRow3 = 0 * p1.GetRow(2)-p1.GetRow(1);
//            var svdRow4 = 0 * p2.GetRow(2)-p2.GetRow(0);
//            var svdRow5 = 0 * p2.GetRow(2)-p2.GetRow(1);

            var svdMatrix = svdRow0.ConcateVertical(svdRow1).ConcateVertical(svdRow2).ConcateVertical(svdRow3).ConcateVertical(svdRow4).ConcateVertical(svdRow5);
            
            var uL = new Matrix<double>(6,6);
            var sL = new Matrix<double>(1,4);
            var vL = new Matrix<double>(4,4);


            CvInvoke.SVDecomp(svdMatrix,sL,uL,vL, SvdFlag.Default);

            //For some reason SVDecomp returns vL matrix with wrong sign on all rows except the third. The following swaps the signals of the matrix to match expected result
            vL = -1*vL;
            var swapRowSign = -1*vL.GetRow(2);
            //missing .conj() - "The complex conjugate of a complex number, obtained by changing the sign of its imaginary part." Is this needed???
            vL = vL.GetRows(0, 2,1).ConcateVertical(swapRowSign).ConcateVertical(vL.GetRow(3));
            vL = vL.Transpose();
            var resultDLT = new Matrix<double>(4,1);
            resultDLT = vL.GetRow(3).Transpose();

            if (Math.Abs(vL[3, 3]) > 0.000000000000000001)
            {
                resultDLT = vL.GetRow(3).Transpose() / vL[3, 3];
            }

            return resultDLT;
            
        }

        //FindLine - Between two centers after triangulation is performed (in camera coordinates)
        public void FindLine(Point center01, Point center02, Point center11, Point center12, Point center21, Point center22, double slewAngle, TextBox debugTextBox)
        {
            var x1 = DLT(center01, center11, center21);
            var x2 = DLT(center02, center12, center22);
            var lc0 = x2.GetRows(0, 3, 1) - x1.GetRows(0, 3, 1);

            //Convert to inertial coordinates
            var rotMatrix = new Matrix<double>(3, 3);
            rotMatrix[0, 0] = -Math.Cos(slewAngle);
            rotMatrix[0, 1] = 0;
            rotMatrix[0, 2] = Math.Sin(slewAngle);
            rotMatrix[1, 0] = Math.Sin(slewAngle);
            rotMatrix[1, 1] = 0;
            rotMatrix[1, 2] = Math.Cos(slewAngle);
            rotMatrix[2, 0] = 0;
            rotMatrix[2, 1] = 1;
            rotMatrix[2, 2] = 0;

            var lvec = rotMatrix * lc0;
            var lvecNorm = CvInvoke.Norm(lvec);

            if (lvecNorm > 0.00001)
            {
                lvec = lvec / lvecNorm;
            }
            
            Display.DisplayMatrix(lvec, debugTextBox);

        }
    }
}
