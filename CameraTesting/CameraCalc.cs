using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;

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

            var transMat21 = Detection.TransMat(t21);
            var transMat31 = Detection.TransMat(t31);
            var transMat11 = Detection.TransMat(t11);

            //Camera Matrices

            //the fields are not being updated with this values, so when I use p0 in another context is still zero..???
            p0 = (k0 * pII) * transMat11;
            p1 = (k1 * pII) * transMat21;
            p2 = (k2 * pII) * transMat31;

            Detection.DisplayMatrix(p0, p0TextBox);
            Detection.DisplayMatrix(p1, p1TextBox);
            Detection.DisplayMatrix(p2, p2TextBox);


        }

        public void DLT(Point center01, Point center02, Point center11, Point center12, Point center21, Point center22, TextBox debugTextBox)
        {
            var test = p1.GetRow(2);
            Detection.DisplayMatrix(test, debugTextBox);

            //Direct Linear Triangulation

            //CvInvoke.SVD()
            //uL, sL, vL = linalg.svd(
            //np.array
            //([np.dot(c0[0], P0[2,:]) - P0[0,:],
            //  np.dot(c0[1], P0[2,:]) - P0[1,:],
            //  np.dot(c1[0], P1[2,:]) - P1[0,:],
            //  np.dot(c1[1], P1[2,:]) - P1[1,:],
            //  np.dot(c2[0], P2[2,:]) - P2[0,:],
            //  np.dot(c2[1], P2[2,:]) - P2[1,:]]))

        }

    }
}
