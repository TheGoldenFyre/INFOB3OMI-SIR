using OSIR;
using System;
using System.IO;
using System.Linq;
using System.Threading;

class Program
{
    static int WIDTH = 80;
    static int HEIGHT = 80;
    static double INFECTION_CHANCE = 0.03;
    static double RECOVERY_CHANCE = 0.10;
    static double LOSE_IMMUNITY_CHANCE = 0.03;

    static double START_INFECTION_PERC = 0.1;
    static double START_BARRIER_PERC = 0;

    static int ITERATION_COUNT = 1000;
    static int TEST_COUNT = 100;

    static int THREAD_COUNT = 12;

    public static void Main(string[] args)
    {
        for (int y = 0; y < 37; y++)
        {
            // Determine the time, and use the file name to save information about the tests
            string fileName = DateTime.Now.ToString("MM-dd HH.mm.ss") + $" W{WIDTH} H{HEIGHT} I{INFECTION_CHANCE} R{RECOVERY_CHANCE} L{LOSE_IMMUNITY_CHANCE} SI{START_INFECTION_PERC} SB{START_BARRIER_PERC}.csv";
            int[][] data = new int[TEST_COUNT][];
            bool[] threadsDone = new bool[THREAD_COUNT];

            for (int i = 0; i < THREAD_COUNT; i++)
            {
                Console.WriteLine($"Starting thread {i}");
                int u = i;
                Thread x = new Thread(_ => Run(START_BARRIER_PERC, u, ref data, ref threadsDone));
                x.Start();
            }

           // Run(START_BARRIER_PERC, 0, ref data, ref threadsDone);

            // Spin untill threads are done
            while (!threadsDone.All(x => x)) ;

            string[] output = new string[ITERATION_COUNT + 1];
            string header = "";
            for (int i = 0; i < TEST_COUNT; i++) header += $"Run {i}; ";
            header += "AVG";
            output[0] = header;

            for (int i = 0; i < ITERATION_COUNT; i++)
            {
                string s = "";
                long total = 0;
                for (int j = 0; j < TEST_COUNT; j++)
                {
                    total += data[j][i];
                    s += $"{data[j][i]}; ";
                }
                s += total / (double)TEST_COUNT;
                output[i + 1] = s;
            }

            File.WriteAllLines($"./output/{fileName}", output);

            if (y < 20) START_BARRIER_PERC += 0.01;
            else START_BARRIER_PERC += 0.05;
        }
    }

    public static void Run(double bp, int index, ref int[][] data, ref bool[] done)
    {
        for (int i = index; i < TEST_COUNT; i += THREAD_COUNT)
        {
            Console.WriteLine($"Starting run {(i + 1).ToString(new string('0', TEST_COUNT.ToString().Length))}/{TEST_COUNT}");
            OSIRModel model = new OSIRModel(WIDTH, HEIGHT, INFECTION_CHANCE, RECOVERY_CHANCE, LOSE_IMMUNITY_CHANCE);

            model.AddInfections(START_INFECTION_PERC);
            model.AddObstructions(bp);

            int[] infectionsPerIteration = new int[ITERATION_COUNT];

            for (int j = 0; j < ITERATION_COUNT; j++)
            {
                infectionsPerIteration[j] = model.Iterate();
            }

            data[i] = infectionsPerIteration;
        }

        done[index] = true;
    }
}

