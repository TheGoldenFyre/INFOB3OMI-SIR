using System;
using System.Text;

namespace OSIR
{
    public class OSIRModel
    {
        private enum State
        {
            NotSet,
            Susceptible,
            Infectious,
            Recovered
        }

        private State[,] state;
        private bool[,,] obstruction; //per xy, go check wether neighbours are visible.

        private int width;
        private int height;
        private int SDIM;
        private int connectionCount;
        private double CHANCE_INF;
        private double CHANCE_REC;
        private double CHANCE_LIM;
        private Random rnd;

        public OSIRModel(int w, int h, double cinf, double crec, double clim)
        {
            width = w;
            height = h;
            SDIM = 7;
            state = new State[width, height];
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    state[i, j] = State.Susceptible;
            obstruction = new bool[w, h, 9];

            CHANCE_INF = cinf;
            CHANCE_REC = crec;
            CHANCE_LIM = clim;

            connectionCount = ConnectionCount();

             rnd = new Random();
        }

        // Determines the amount of total connections between specimens on the field.
        private int ConnectionCount()
        {
            return (width - 1) * height + (height - 1) * width + 2 * (width - 1) * (height - 1);
        }

        public int Iterate()
        {
            State[,] output = new State[width, height];
            int infectionCount = 0;
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    output[x,y] = IterateState(x, y, ref infectionCount);
                }
            }

            state = output;
            return infectionCount;
        }

        private State IterateState(int x, int y, ref int count)
        {
            if (state[x, y] == State.Susceptible)
            {
                int c = 0;
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        // Bound check
                        if (x + dx >= 0 && y + dy >= 0 && x + dx < width && y + dy < height)
                        {
                            if (state[x + dx, y + dy] == State.Infectious && !obstruction[x, y, (dy + 1) * 3 + dx + 1]) c++;
                        }
                    }
                }

                double chance = Math.Pow(1 - CHANCE_INF, c);
                if (c == 0 || rnd.NextDouble() < chance)
                    return State.Susceptible;
                else
                {
                    count++;
                    return State.Infectious;
                }
            }
            else if (state[x, y] == State.Infectious)
            {
                return (rnd.NextDouble() < CHANCE_REC) ? State.Recovered : State.Infectious;
            }
            else if (state[x, y] == State.Recovered)
            {
                return (rnd.NextDouble() < CHANCE_LIM) ? State.Susceptible : State.Recovered;
            }

            return state[x, y];
        }

        public void AddObstructions(double p)
        {
            if (p < 0) p = 0;
            if (p > 1) p = 1;

            int c = (int)(connectionCount * p);
            AddObstructions(c);


            //for (int x = 0; x < width; x++)
            //    for (int y = 0; y < height; y++)
            //        for (int z = 0; z < 9; z++)
            //            if (z != 4 && !obstruction[x, y, z]) Console.WriteLine($"x: {x}, y: {y}, z: {z}");
        }

        public void AddObstructions(int c)
        {
            int totalRemaining = connectionCount;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (x + 1 < width) TryAddObstruction(x, y, 5, ref c, ref totalRemaining);
                    if (y + 1 < height) TryAddObstruction(x, y, 7, ref c, ref totalRemaining);
                    if (x + 1 < width && y + 1 < height) TryAddObstruction(x, y, 8, ref c, ref totalRemaining);
                    if (x - 1 >= 0 && y + 1 < height) TryAddObstruction(x, y, 6, ref c, ref totalRemaining);
                }
            }
        }

        private void TryAddObstruction(int x, int y, int neighbour, ref int leftToPick, ref int totalLeft)
        {
            if (rnd.NextDouble() < leftToPick / (double)totalLeft)
            {
                // We add the connection
                leftToPick--;
                obstruction[x, y, neighbour] = true;
                int dx = neighbour % 3 - 1;
                int dy = neighbour / 3 - 1;
                obstruction[x + dx, y + dy, 8 - neighbour] = true;
            }

            totalLeft--;
        }

        // Infects a certain percentage of the population
        public void AddInfections(double p)
        {
            int c = (int)(width * height * p);
            AddInfections(c);
        }

        // Infects a certain number of specimens within the population
        public void AddInfections(int c)
        {
            for (int i = 0; i < c; i++)
            {
                while (true)
                {
                    int rndX = (int)(rnd.NextDouble() * width);
                    int rndY = (int)(rnd.NextDouble() * height);

                    if (state[rndX, rndY] != State.Infectious)
                    {
                        state[rndX, rndY] = State.Infectious;
                        break;
                    }
                }
            }
        }

        //public override string ToString()
        //{
        //    StringBuilder sb = new StringBuilder(width * height + height - 1);
        //    for (int y = 0; y < height; y++)
        //    {
        //        for (int x = 0; x < width; x++)
        //        {
        //            string s;
        //            switch (state[x, y])
        //            {
        //                case State.Infectious:  s = "I"; break;
        //                case State.Recovered:   s = "R"; break;
        //                case State.Susceptible: s = "S"; break;
        //                default:                s = "X"; break;
        //            }
        //            sb.Append(s);
        //        }
        //        sb.Append("\n");
        //    }
        //    return sb.ToString();
        //}
    }
}
