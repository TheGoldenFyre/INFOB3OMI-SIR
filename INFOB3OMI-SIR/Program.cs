
class Program
{
    enum EntityState
    {
        Not_Set, 
        Susceptible,
        Infectious,
        Recovered,
        Obstruction
    }

    static Random rnd = new Random();
    static double INF_CHANCE = 0.05;
    static double REC_CHANCE = 0.06;
    static double LIM_CHANCE = 0.04;

    static void Main(String[] args)
    {
        Console.CursorVisible = false;

        EntityState[,] states = new EntityState[50,10];
        for (int i = 0; i < states.GetLength(0); i++)
        {
            for (int j = 0; j < states.GetLength(1); j++)
            {
                states[i,j] = EntityState.Susceptible;

            }
        }

        states[12, 0]=EntityState.Infectious;
        states[24, 0]=EntityState.Obstruction;
        states[24, 1]=EntityState.Obstruction;
        states[24, 2]=EntityState.Obstruction;
        states[24, 3]=EntityState.Obstruction;
        states[24, 4]=EntityState.Obstruction;
        states[25, 4]=EntityState.Obstruction;
        states[26, 4]=EntityState.Obstruction;
        states[27, 4]=EntityState.Obstruction;
        //states[24, 5]=EntityState.Obstruction;
        states[24, 6]=EntityState.Obstruction;
        states[25, 6]=EntityState.Obstruction;
        states[26, 6]=EntityState.Obstruction;
        states[27, 6]=EntityState.Obstruction;
        states[24, 7]=EntityState.Obstruction;
        states[24, 8]=EntityState.Obstruction;
        states[24, 9]=EntityState.Obstruction;
        //states[36, 0]=EntityState.Infectious;


        while (true)
        {
            Iterate(ref states);
            DisplayStates(states);
            Console.ReadKey(true);
            //Console.Clear();
        }

    }

    static void Iterate(ref EntityState[,] es)
    {
        EntityState[,] output = new EntityState[es.GetLength(0), es.GetLength(1)];

        for (int i = 0;i < es.GetLength(0); i++)
        {
            for (int j = 0; j < es.GetLength(1); j++)
            {
                
                if (es[i, j] == EntityState.Susceptible)
                {
                    int infectedNeighbours = 0;
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            if (i + dx >= 0 && j + dy >= 0 && i + dx < es.GetLength(0) && j + dy < es.GetLength(1))
                            {
                                infectedNeighbours += es[i+dx, j+dy] == EntityState.Infectious ? 1 : 0;
                            }
                        }
                    }

                    if (rnd.NextDouble() < infectedNeighbours * INF_CHANCE)
                    {
                        output[i,j] = EntityState.Infectious;
                    }
                }

                if (es[i,j] == EntityState.Infectious)
                {
                    if (rnd.NextDouble() < REC_CHANCE) output[i,j] = EntityState.Recovered;
                }

                if (es[i,j] == EntityState.Recovered)
                {
                    if (rnd.NextDouble() < LIM_CHANCE) output[i,j] = EntityState.Susceptible;
                }
            }
        }


        for (int i = 0; i < es.GetLength(0); i++)
            for (int j = 0; j < es.GetLength(1); j++)
                if (output[i,j] == EntityState.Not_Set) output[i,j] = es[i,j];

        es = output;
    }

    static void DisplayStates(EntityState[,] es)
    {
        for (int i = 0; i < es.GetLength(1); i++)
        {
            for (int j = 0; j < es.GetLength(0); j++)
            {
                string s = "";
                switch (es[j, i])
                {
                    case EntityState.Susceptible:
                        s = "S";
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case EntityState.Infectious:
                        s = "I";
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    case EntityState.Recovered:
                        s = "R";
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                    case EntityState.Obstruction:
                        s = "X";
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                        break;
                }
                Console.SetCursorPosition(j, i);
                Console.Write(s);
            }
           // Console.Write("\n");
        }
    }
}