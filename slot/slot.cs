using System.Globalization;

namespace ConsoleApp1
{
    /// <summary>
    /// Slot Machine class
    /// </summary>
    public static class SlotMachine
    {
        private const int REEL_WIDTH = 3;
        private const int REEL_HEIGHT = 3;

        // Use dateTime to Randomise the seed
        private static readonly Random random = new(DateTime.Now.Millisecond);

        private static readonly string[] symbols = ["A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M",];

        private static readonly Dictionary<string, double> payouts = new()
    {
            { "AAA", 1.00 }, // Payout for three cherries
            { "BBB", 1.20 }, // and so on down the line
            { "CCC", 2.00 },
            { "DDD", 2.40 },
            { "EEE", 3.00 },
            { "FFF", 4.00 },
            { "GGG", 5.00 },
            { "HHH", 7.00 },
            { "III", 10.00 },
            { "JJJ", 15.00 },
            { "KKK", 20.00 },
            { "LLL", 40.00 },
            { "MMM", 75.00 }
        };

        // Used for the number of times the reel motor turns
        private static readonly int[] ReelMotor = [0, 0, 0];

        // Only used if symbols on the reel are static like a mechanical slot
        private static readonly int[] ReelIndex = [0, 0, 0];

        // Used for the 3x3 grid of symbols
        private static readonly string[,] reels = new string[REEL_WIDTH, REEL_HEIGHT];

        // Total Payout to Customer
        private static double totalPayout = 0;

        /// <summary>
        /// Calculate Slot Machine RTP (Return to Player)
        /// </summary>
        static public void CalculatePayout()
        {
            // 1 million spins
            const int NumSpins = 1000000;

            int NumWins = 0;

            int NumLoses = NumSpins;
            double CalcRTP;

            // Initialise the reels with random data
            InitialiseReels();

            // Clear console window
            Console.Clear();

            // Hide the cursor
            Console.CursorVisible = false;

            for (int s = 0; s < NumSpins; s++)
            {
                Console.SetCursorPosition(0, 0);
                Console.WriteLine("Number of spins : " + s + 1);

                NumWins = SpinReels() ? NumWins + 1 : NumWins;
            }

            NumLoses -= NumWins;
            
            // 10 is used for 10p a spin instead of £1
            CalcRTP = (totalPayout / (NumSpins / 10)) * 100;

            const double house = NumSpins / 10;

            string strHouse = house.ToString("C", new CultureInfo("en-GB"));
            string strCash = totalPayout.ToString("C", new CultureInfo("en-GB"));

            Console.WriteLine("Number of wins  : " + NumWins);
            Console.WriteLine("Number of loses : " + NumLoses);
            Console.WriteLine("House           : " + strHouse);
            Console.WriteLine("Cash            : " + strCash);
            Console.WriteLine("Calculated RTP  : " + Math.Round(CalcRTP, 2) + "%");
        }

        /// <summary>
        /// Initialise the reels with random data
        /// </summary>
        static private void InitialiseReels()
        {
            // Generate the 3 reels with random data (used for new type slot machine)
            // if using mechanical type, then just get 3 random reel index, then add 1 and 2 for the other symbols in the reel
            for (int i = 0; i < REEL_WIDTH; i++)
            {
                for (int j = 0; j < REEL_HEIGHT; j++)
                {
                    int rnd = random.Next(int.MaxValue) % symbols.Length;
                    reels[i, j] = symbols[rnd];
                    ReelIndex[i] = rnd;
                }
            }
        }

        /// <summary>
        /// Spin the reels
        /// </summary>
        /// <returns></returns>
        static private bool SpinReels()
        {
            // Fire up the 3 Spin Motors
            ReelMotors();

            // Loop until all motors have stopped
            while (ReelMotor[0] > 0 || ReelMotor[1] > 0 || ReelMotor[2] > 0)
            {
                // Spin reel based on motor distance
                for (int r = 0; r < REEL_WIDTH; r++)
                {
                    if (ReelMotor[r] > 0)
                    {
                        SpinReel(r);
                        ReelMotor[r]--;
                    }
                }

                // Remove following to speed up calculations
                //PrintReels();
            }

            return CheckWin();
        }

        /// <summary>
        /// Generate reel motors spins
        /// </summary>
        static private void ReelMotors()
        {
            ReelMotor[0] = random.Next(0, 10) + 15;
            ReelMotor[1] = random.Next(0, 10) + ReelMotor[0];
            ReelMotor[2] = random.Next(0, 10) + ReelMotor[1];
        }

        /// <summary>
        /// Spin the specified reel
        /// </summary>
        /// <param name="r"></param>
        static private void SpinReel(int r)
        {
            reels[r, 2] = reels[r, 1];
            reels[r, 1] = reels[r, 0];

            // Use this to get the next symbol from the reel (like a mechanical slot machine one)
            //ReelIndex[r] = (ReelIndex[r] + 1) % symbols.Length;
            //reels[r, 0] = symbols[ReelIndex[r]];

            // Use this to get a random symbol (like a modern slot machine does)
            int rnd = random.Next(int.MaxValue) % symbols.Length;
            ReelIndex[r] = rnd;
            reels[r, 0] = symbols[rnd];
        }

        /// <summary>
        /// Print the reels to the console
        /// </summary>
        static private void PrintReels()
        {
            // set cursor position to top left
            Console.SetCursorPosition(0, 2);

            for (int i = 0; i < REEL_WIDTH; i++)
            {
                for (int j = 0; j < REEL_HEIGHT; j++)
                {
                    Console.Write(reels[i, j] + " ");
                }

                Console.WriteLine();
            }
        }

        /// <summary>
        /// Check to see if the player has won
        /// </summary>
        /// <returns></returns>
        static private bool CheckWin()
        {
            // Check the winning line (middle row) for 3 symbols the same
            if (reels[0, 1] == reels[1, 1] && reels[1, 1] == reels[2, 1])
            {
                string result = reels[0, 1] + reels[1, 1] + reels[2, 1];
                totalPayout += payouts[result];
                return true;
            }

            return false;
        }
    }
}
