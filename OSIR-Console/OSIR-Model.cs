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

            rnd = new Random();
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
                            if (state[x + dx, y + dy] == State.Infectious && !obstruction[x, y, (dx + 1) * 3 + dy + 1]) c++;
                        }
                    }
                }

                double chance = Math.Pow(1 - CHANCE_INF, c);
                if (rnd.NextDouble() < chance)
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

        // Blocks off a certain percentage of connections
        public void AddObstructions(double p)
        {
            if (p < 0.5)
            {
                int c = (int)((3 * width * height - 2 * width - 2 * height + 1) * p);
                AddObstructions(c);
            }
            else
            {
                int c = (int)((3 * width * height - 2 * width - 2 * height + 1) * (1 - p));
                for (int x = 0; x < width; x++)
                    for (int y = 0; y < height; y++)
                        for (int b = 0; b < 9; b++)
                            obstruction[x, y, b] = true;
                RemoveObstructions(c);
            }
        }

        private void RemoveObstructions(int c)
        {
            for (int i = 0; i < c; i++)
            {
                while (true)
                {
                    int rndX = (int)(rnd.NextDouble() * width);
                    int rndY = (int)(rnd.NextDouble() * height);
                    int rndN = (int)(rnd.NextDouble() * 8);
                    if (rndN > 3) rndN += 1; // We want to skip the center tile, so +1.

                    if (obstruction[rndX, rndY, rndN])
                    {
                        obstruction[rndX, rndY, rndN] = false;
                        // we must now also find the neighbour that we need to set to true: 
                        int dx = rndN % 3 - 1;
                        int dy = rndN / 3 - 1;
                        if (rndX + dx >= 0 && rndY + dy >= 0 && rndX + dx < width && rndY + dy < height)
                        {
                            obstruction[rndX + dx, rndY + dy, 8 - rndN] = false;
                            break;
                        }
                    }
                }
            }
        }

        public void AddObstructions(int c)
        {
            for (int i = 0; i < c; i++)
            {
                while (true)
                {
                    int rndX = (int)(rnd.NextDouble() * width);
                    int rndY = (int)(rnd.NextDouble() * height);
                    int rndN = (int)(rnd.NextDouble() * 8);
                    if (rndN > 3) rndN += 1; // We want to skip the center tile, so +1.

                    if (!obstruction[rndX, rndY, rndN])
                    {
                        obstruction[rndX, rndY, rndN] = true;
                        // we must now also find the neighbour that we need to set to true: 
                        int dx = rndN % 3 - 1;
                        int dy = rndN / 3 - 1;
                        if (rndX + dx >= 0 && rndY + dy >= 0 && rndX + dx < width && rndY + dy < height)
                        {
                            obstruction[rndX+dx, rndY + dy, 8-rndN] = true;
                            break;
                        }
                    }
                }
            }
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

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(width * height + height - 1);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    string s;
                    switch (state[x, y])
                    {
                        case State.Infectious:  s = "I"; break;
                        case State.Recovered:   s = "R"; break;
                        case State.Susceptible: s = "S"; break;
                        default:                s = "X"; break;
                    }
                    sb.Append(s);
                }
                sb.Append("\n");
            }
            return sb.ToString();
        }
    }
}
