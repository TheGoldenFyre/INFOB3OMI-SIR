using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private double CHANCE_INF;
        private double CHANCE_REC;
        private double CHANCE_LIM;
        private Random rnd;
        private Bitmap bmp;

        public OSIRModel(int w, int h, double cinf, double crec, double clim)
        {
            width = w;
            height = h;
            state = new State[width, height];
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    state[i, j] = State.Susceptible;
            obstruction = new bool[w, h, 9];

            CHANCE_INF = cinf;
            CHANCE_REC = crec;
            CHANCE_LIM = clim;

            rnd = new Random();
            bmp = new Bitmap(w * 2, h * 2);

        }

        public void Iterate()
        {
            State[,] output = new State[width, height];
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    output[x,y] = IterateState(x, y);
                }
            }

            state = output;
        }

        private State IterateState(int x, int y)
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

                return (rnd.NextDouble() < Math.Pow(1 - CHANCE_INF, c)) ? State.Susceptible : State.Infectious;
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

        public Bitmap GetBitmap()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j =0; j < height; j++)
                {
                    Color c;
                    switch (state[i, j])
                    {
                        case State.Infectious: c = Color.Red; break;
                        case State.Recovered: c = Color.Green; break;
                        case State.Susceptible: c = Color.Gray; break;
                        default: c = Color.Purple; break;
                    }

                    bmp.SetPixel(2*i, 2*j, c);
                    bmp.SetPixel(2*i, 2*j + 1, c);
                    bmp.SetPixel(2*i + 1, 2*j, c);
                    bmp.SetPixel(2*i + 1, 2*j + 1, c);
                }
            }
            return bmp;
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
