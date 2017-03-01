using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Game_of_Life_2017
{
    public class Cell
    {
        int x;
        int y;
        int board_size_x;
        int board_size_y;
        int alive_neighbours;
        public char symbol;
        public bool alive;
        public Cell[] Neighbours;

        public Cell(int pos_x, int pos_y, int x_max, int y_max, bool state)
        {
            x = pos_x;
            y = pos_y;
            board_size_x = x_max;
            board_size_y = y_max;
            alive = state;
            setSymbol();
        }

        private void setSymbol()
        {
            symbol = alive ? '■' : ' ';
        }        

        public void countAliveNeighbours()
        {
            int num_alive = 0;
            for (int i = 0; i < 9; i++)
            {
                if (Neighbours[i] != null)
                {
                    if (Neighbours[i].alive)
                    {
                        num_alive++;
                    }
                }
            }
            alive_neighbours = num_alive;
        }

        public void nextState()
        {
            if (alive_neighbours < 2)
            {
                alive = false;
            }
            else if (alive_neighbours == 2 || alive_neighbours == 3)
            {
                if (!alive && alive_neighbours == 3)
                {
                    alive = true;
                }
            }
            else if (alive_neighbours > 3)
            {
                alive = false;
            }
            setSymbol();
        }
    }

    public class Board
    {
        int print_delay;
        public int width;
        public int height;
        Cell[,] board;

        public Board(bool file)
        {
            print_delay = 500;
            if (file)
            {
                initFile();
            }
            else
            {
                initRandom();
            }
            setRadiusCells();
            printBoard(); 
        }

        public void initRandom()
        {
            width = 25;
            height = 25;
            Random life = new Random();
            board = new Cell[height, width];
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    if (life.Next(0, 2) == 1)
                    {
                        board[j, i] = new Cell(i, j, width, height, true);
                    }
                    else
                    {
                        board[j, i] = new Cell(i, j, width, height, false);
                    }
                }
            }
        }


        public void initTemplate()
        {
            width = 3;
            height = 3;
            board = new Cell[height, width];
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    if ((i >= 0) && (i <= 2) && (j == 1))
                    {
                        board[j, i] = new Cell(i, j, width, height, true);
                    }
                    else
                    {
                        board[j, i] = new Cell(i, j, width, height, false);
                    }
                }
            }
        }

        public void initFile()
        {
            string text = "";
            char value;
            width = 0;
            height = 1;
            try
            {   //change the filepath to the custom file you want to load. 
                //the file must only contain 1's (for alive state) and 0's (for dead state)
                var fileStream = new FileStream(@"C:\Users\kgonl\Documents\C#\AllAlivePattern.txt", FileMode.Open, FileAccess.Read);
                using (var streamReader = new StreamReader(fileStream))
                {
                    while (!streamReader.EndOfStream)
                    {
                        value = (char)streamReader.Read();
                        if (value == '\n')
                        {
                            height = height + 1;
                            width = 0;
                            text = text + "*";
                        }
                        else if (value != '\r')
                        {
                            width = width + 1;
                            text = text + value.ToString();
                        }
                    }
                }
                board = new Cell[height, width];
                int len = text.Length;
                int counter = 0;
                int x = 0;
                int y = 0;
                while (len > counter)
                {
                    if (text[counter] == '*')
                    {
                        y = y + 1;
                        x = 0;
                    }
                    else
                    {
                        board[y, x] = (text[counter] == '1') ? new Cell(x, y, width, height, true) : new Cell(x, y, width, height, false);
                        x = x + 1;
                    }
                    counter = counter + 1;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

        public void setRadiusCells()
        {
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    board[j, i].Neighbours = new Cell[9];
                    board[j, i].Neighbours[0] = (j - 1 < 0 || i - 1 < 0) ? null : board[j - 1, i - 1];
                    board[j, i].Neighbours[1] = (j - 1 < 0 || i < 0) ? null : board[j - 1, i];
                    board[j, i].Neighbours[2] = (j - 1 < 0 || i + 1 >= width) ? null : board[j - 1, i + 1];
                    board[j, i].Neighbours[3] = (j < 0 || i - 1 < 0) ? null : board[j, i - 1];
                    board[j, i].Neighbours[4] = null;
                    board[j, i].Neighbours[5] = (j < 0 || i + 1 >= width) ? null : board[j, i + 1];
                    board[j, i].Neighbours[6] = (j + 1 >= height || i - 1 < 0) ? null : board[j + 1, i - 1];
                    board[j, i].Neighbours[7] = (j + 1 >= height || i < 0) ? null : board[j + 1, i];
                    board[j, i].Neighbours[8] = (j + 1 >= height || i + 1 >= width) ? null : board[j + 1, i + 1];
                }
            }
        }

        public void printBoard()
        {            
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    Console.Write(" ");
                    Console.Write(board[j, i].symbol);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            System.Threading.Thread.Sleep(print_delay);
        }

        public void loopBoard(int l)
        {            
            for (int k = 1; k < l; k++)
            {
                for (int j = 0; j < height; j++)
                {
                    for (int i = 0; i < width; i++)
                    {
                        board[j, i].countAliveNeighbours();
                    }
                }
                for (int j = 0; j < height; j++)
                {
                    for (int i = 0; i < width; i++)
                    {
                        board[j, i].nextState();
                    }
                }
                Console.Clear();
                Console.WriteLine("\nGeneration Number: " + k + "\n");
                printBoard();                
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            bool answer = false;
            bool file = true;
            Console.WriteLine("CONWAY'S GAME OF LIFE! by Kevin George\n");
            Console.WriteLine("Would you like load a file?");
            string input = Console.ReadLine();
            while (!answer)
            {
                if (input.Equals("yes", StringComparison.InvariantCultureIgnoreCase))
                {
                    answer = true;
                    Console.Clear();
                    file = true;
                }
                else if (input.Equals("no", StringComparison.InvariantCultureIgnoreCase))
                {
                    answer = true;
                    Console.Clear();
                    file = false;
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("Please write yes or no");
                    input = Console.ReadLine();
                }
            }
            Console.WriteLine("\nIntial State\n");
            Board board = new Board(file);
            board.loopBoard(9999);
        }
    }
}
