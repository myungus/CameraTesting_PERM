using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;

namespace CameraTesting
{
    class Display
    {
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
                    matrixString += matrix[i, j].ToString();
                    matrixString += " ";
                }

                matrixString += Environment.NewLine;
            }

            targetTextBox.Text = matrixString;
        }
    }
}
