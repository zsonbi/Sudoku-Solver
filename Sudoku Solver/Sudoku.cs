using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sudoku_Solver
{
    internal class Sudoku
    {
        //One cell of the sudoku
        private class cell
        {
            public byte value { get; private set; } //The current value of the cell
            public List<byte> possibleNumbers { get; private set; }

            //--------------------------------------------------
            //Constructor
            public cell()
            {
                possibleNumbers = new List<byte>();
                for (byte i = 1; i <= 9; i++)
                    possibleNumbers.Add(i);
            }

            public cell(cell parent)
            {
                this.value = parent.value;
                this.possibleNumbers = new List<byte>();
                for (byte i = 0; i < parent.possibleNumbers.Count; i++)
                    possibleNumbers.Add(parent.possibleNumbers[i]);
            }

            //---------------------------------------------------
            //Sets the value to the cell
            public void Set(byte value)
            {
                if (this.value != 0)
                    throw new Exception("Already Occupied");
                else if (!possibleNumbers.Contains(value))
                    throw new Exception("Not a possibility");
                else
                    this.value = value;
                possibleNumbers.Clear();
            }
        }

        //************************************************************

        //Varriables
        private cell[,] cells;

        private readonly Random rnd = new Random();

        //----------------------------------------------------------
        //Constructor
        public Sudoku(byte[,] cellValues)
        {
            cells = new cell[9, 9];

            for (byte i = 0; i < 9; i++)
            {
                for (byte j = 0; j < 9; j++)
                {
                    cells[i, j] = new cell();
                }
            }

            //Sets the numbers for the cells
            for (byte i = 0; i < 9; i++)
            {
                for (byte j = 0; j < 9; j++)
                {
                    if (cellValues[i, j] != 0)
                        SetCell(cells, i, j, cellValues[i, j]);
                }
            }
        }

        //***********************************************************************
        //Private Methods
        //Sets the value for a cell
        private void SetCell(cell[,] board, byte row, byte col, byte number)
        {
            board[row, col].Set(number);
            //Updates the row's possibleNumbers
            for (int i = 0; i < 9; i++)
                board[row, i].possibleNumbers.Remove(number);

            //Updates the column's possibleNumbers
            for (int i = 0; i < 9; i++)
                board[i, col].possibleNumbers.Remove(number);

            byte rowIndex = (byte)(row - row % 3);
            byte colIndex = (byte)(col - col % 3);
            //Updates the column's possibleNumbers
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    board[rowIndex + i, colIndex + j].possibleNumbers.Remove(number);
                }
            }
        }

        //------------------------------------------------------
        //Checks if the current board is impossible
        private bool IsImpossible(cell[,] board)
        {
            for (byte i = 0; i < 9; i++)
            {
                for (byte j = 0; j < 9; j++)
                {
                    if (board[i, j].value == 0 && board[i, j].possibleNumbers.Count == 0)
                        return true;
                }
            }
            return false;
        }

        //-------------------------------------------------------
        //Find the guaranteed values for the cells
        private void FindGuaranteed(cell[,] board)
        {
            bool found;

            do
            {
                found = false;
                for (byte i = 0; i < 9; i++)
                {
                    for (byte j = 0; j < 9; j++)
                    {
                        if (board[i, j].possibleNumbers.Count == 1)
                        {
                            SetCell(board, i, j, board[i, j].possibleNumbers.First());
                            found = true;
                        }
                    }
                }
            } while (found);
        }

        //----------------------------------------------------
        //Makes a copy of the board then make a guess on the new board then return the new board if there is no possible guess left return null
        private cell[,] Guess(cell[,] board)
        {
            cell[,] newBoard = new cell[9, 9]; //deep copy of the board
            //Makes a deep copy of the board
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    newBoard[i, j] = new cell(board[i, j]);
                }
            }

            //Finds the best possible guess
            byte[] bestCords = new byte[2] { 0, 0 };
            byte leastCount = 10;
            for (byte i = 0; i < 9; i++)
            {
                for (byte j = 0; j < 9; j++)
                {
                    if (newBoard[i, j].value == 0 && newBoard[i, j].possibleNumbers.Count < leastCount)
                    {
                        leastCount = (byte)newBoard[i, j].possibleNumbers.Count;
                        bestCords[0] = i;
                        bestCords[1] = j;
                    }
                }
            }
            //If there was no valid guess posible return null
            if (leastCount == 10 || board[bestCords[0], bestCords[1]].possibleNumbers.Count == 0)
                return null;
            byte guessedNumber = board[bestCords[0], bestCords[1]].possibleNumbers[rnd.Next(0, board[bestCords[0], bestCords[1]].possibleNumbers.Count)];
            //Removes that possibility from the parent board
            board[bestCords[0], bestCords[1]].possibleNumbers.Remove(guessedNumber);
            //Sets the value on the newBoard
            SetCell(newBoard, bestCords[0], bestCords[1], guessedNumber);
            return newBoard;
        }

        //-----------------------------------------------------
        //Checks if everything is done
        private bool IsDone(cell[,] board)
        {
            for (sbyte i = 8; i >= 0; i--)
            {
                for (sbyte j = 8; j >= 0; j--)
                {
                    if (board[i, j].value == 0)
                        return false;
                }
            }
            return true;
        }

        //-------------------------------------------------------
        //This algorithm will solve the sudoku
        private cell[,] SolvingAlgorithm(cell[,] board)
        {
            //Finds the guaranteed spots on the board
            FindGuaranteed(board);
#if DEBUG
            PrintDebug(board);
#endif
            if (IsDone(board))
                return board;
            //Goes into guessing
            do
            {
                //If it's evident with the guessing that it's impossible return null
                if (IsImpossible(board))
                    return null;

                cell[,] temp = Guess(board);
                //if there is no more possible guesses break the do while
                if (temp == null)
                    break;
                else
                {
#if DEBUG
                    PrintDebug(temp);
#endif
                    //Calls the method recursively
                    temp = SolvingAlgorithm(temp);
                    //If it returned null it means that it's impossible with that number
                    if (temp == null)
                        continue;
                    //else return the board which means the sudoku is solved
                    else
                        return temp;
                }
            } while (true);
            //If it's impossible
            return null;
        }

        //--------------------------------------------------------
        //Prints out the board to the console
        private void PrintDebug(cell[,] board)
        {
            Console.WriteLine("**************************************");
            Console.WriteLine("--------------------------------------");
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Console.Write(board[i, j].value + (j % 3 == 2 ? " |  " : "   "));
                }
                Console.WriteLine();
                if (i % 3 == 2)
                    Console.WriteLine("--------------------------------------");
            }
        }

        //*********************************************************************
        //Public Methods
        /// <summary>
        /// Solves the sudoku which was inputted in the constructor
        /// </summary>
        /// <returns>The solved value if it's impossible it returns null</returns>
        public async Task<byte[,]> Solve()
        {
            byte[,] output = new byte[9, 9];
            cells = await Task.Run(() => SolvingAlgorithm(cells));
            if (cells == null)
                return null;
            //Converts the cells into a byte 2d array
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    output[j, i] = cells[j, i].value;
                }
            }
            return output;
        }
    }
}