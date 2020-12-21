using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Sudoku_Solver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Sudoku sudoku;
        private byte[,] BaseValues = new byte[9, 9];

        public MainWindow()
        {
            InitializeComponent();
        }

        //------------------------------------------------
        //Reads in the content of the textblocks into a 2d byte array
        private byte[,] ReadIn()
        {
            byte[,] output = new byte[9, 9];

            for (int c = 0; c < 9; c++)
            {
                for (int i = 0; i < 9; i++)
                {
                    string textBoxContent = ((((SudokuGrid.Children[c] as Border).Child as Grid).Children[i] as Border).Child as TextBox).Text;
                    if (textBoxContent != "")
                    {
                        output[i / 3 + c / 3 * 3, i % 3 + c % 3 * 3] = Convert.ToByte(textBoxContent);
                    }
                }
            }
            return output;
        }

        //-----------------------------------------------------
        //Updates the content of the textblocks skips 0 values in the byte 2d array
        private void WriteOut(byte[,] input)
        {
            for (int c = 0; c < 9; c++)
            {
                for (int i = 0; i < 9; i++)
                {
                    if (input[i / 3 + c / 3 * 3, i % 3 + c % 3 * 3] != 0)
                        ((((SudokuGrid.Children[c] as Border).Child as Grid).Children[i] as Border).Child as TextBox).Text = input[i / 3 + c / 3 * 3, i % 3 + c % 3 * 3].ToString();
                    else
                        ((((SudokuGrid.Children[c] as Border).Child as Grid).Children[i] as Border).Child as TextBox).Text = "";
                }
            }
        }

        //**********************************************************************
        //Handlers
        //Resets the Sudoku
        private void Resetbtn_Click(object sender, RoutedEventArgs e)
        {
            WriteOut(BaseValues);
        }

        //----------------------------------------------------------------------
        //Solves the sudoku
        private async void solvebtn_Click(object sender, RoutedEventArgs e)
        {
            BaseValues = ReadIn();
            sudoku = new Sudoku(BaseValues);
            byte[,] solvedArray = await sudoku.Solve();
            if (solvedArray == null)
            {
                MessageBox.Show("Impossible to solve");
                return;
            }

            WriteOut(solvedArray);
        }

        //-----------------------------------------------------------------------------
        //Makes so that only numbers are possible to be pressed
        private void OnlyNumber_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if ((int)e.Key > 34 && (int)e.Key <= 43)
                e.Handled = false;
            else
                e.Handled = true;
        }

        //----------------------------------------------------------------------------------------------
        //Empties every textbox's Text value
        private void clearAllbtn_Click(object sender, RoutedEventArgs e)
        {
            BaseValues = new byte[9, 9];
            WriteOut(BaseValues);
        }
    }
}