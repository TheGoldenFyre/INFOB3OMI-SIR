using OSIR;

class Program
{
    static int WIDTH = 80;
    static int HEIGHT = 80;
    static double INFECTION_CHANCE = 0.03;
    static double RECOVERY_CHANCE = 0.10;
    static double LOSE_IMMUNITY_CHANCE = 0.03;

    static double START_INFECTION_PERC = 0.1;
    static double START_BARRIER_PERC = 0.0;

    static int ITERATION_COUNT = 1000;
    static int TEST_COUNT = 100;

    static int THREAD_COUNT = 12;

    public static void Main(string[] args)
    {
        // Determine the time, and use the file name to save information about the tests
        string fileName = DateTime.Now.ToString("MM-dd HH.mm.ss") + $" W{WIDTH} H{HEIGHT} I{INFECTION_CHANCE} R{RECOVERY_CHANCE} L{LOSE_IMMUNITY_CHANCE} SI{START_INFECTION_PERC} SB{START_BARRIER_PERC}.csv";
        int[][] data = new int[TEST_COUNT][];
        bool[] threadsDone = new bool[THREAD_COUNT];

        for (int i = 0; i < THREAD_COUNT; i++)
        {
            Console.WriteLine($"Starting thread {i}");
            int u = i;
            Thread x = new Thread(_ => Run(u, ref data, ref threadsDone));
            x.Start();
            
        }

        // Spin untill threads are done
        while (!threadsDone.All(x => x)) ;

        string[] output = new string[ITERATION_COUNT + 1];
        string header = "";
        for (int i = 0; i < TEST_COUNT; i++) header += (i == TEST_COUNT - 1) ? $"Run {i}" : $"Run {i}, ";
        output[0] = header;

        for (int i = 0; i < ITERATION_COUNT; i++)
        {
            string s = "";
            for (int j = 0; j < TEST_COUNT; j++)
            {
                s += (j == TEST_COUNT-1) ? $"{data[j][i]}" : $"{data[j][i]}, ";
            }
            output[i+1] = s;
        }

        File.WriteAllLines($"./output/{fileName}", output);
    }

    public static void Run(int index, ref int[][] data, ref bool[] done)
    {
        for (int i = index; i < TEST_COUNT; i += THREAD_COUNT)
        {
            Console.WriteLine($"Starting run {(i + 1).ToString(new string('0', TEST_COUNT.ToString().Length))}/{TEST_COUNT}");
            OSIRModel model = new OSIRModel(WIDTH, HEIGHT, INFECTION_CHANCE, RECOVERY_CHANCE, LOSE_IMMUNITY_CHANCE);

            model.AddInfections(START_INFECTION_PERC);
            model.AddObstructions(START_BARRIER_PERC);

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

