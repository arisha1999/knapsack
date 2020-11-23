using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Numerics;

namespace knapsack
{
    class Program
    {
        public static int n;
        public static BigInteger M;
        public static List<Knapsack> knapsacks;
        public static string answer;
        private static Random rnd;
        public static int d = 0;
        static Program()
        {
            rnd = new Random();
        }

        public static void ReadFile(string filename)
        {
            try
            {
                using (StreamReader sr = new StreamReader(filename, System.Text.Encoding.Default))
                {
                    n = Convert.ToInt32(sr.ReadLine());
                    M = BigInteger.Parse(sr.ReadLine());
                    string line;
                    BigInteger price, weight;
                    double k;
                    knapsacks = new List<Knapsack>();
                    for(int i = 0; i < n; i++)
                    {
                        line = sr.ReadLine();
                        string[] numbers = line.Split(' ');
                        weight = BigInteger.Parse(numbers[0]);
                        price = BigInteger.Parse(numbers[1]);
                        k = Math.Exp(BigInteger.Log(price)- BigInteger.Log(weight));
                        knapsacks.Add(new Knapsack(i, price, weight, k));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }


        }
        public static void WriteFile(string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename, false, System.Text.Encoding.Default))
            {
                answer = answer.Replace("@", System.Environment.NewLine);
                sw.WriteLine(answer);
                Console.WriteLine(answer);
            }
        }

        public static void RecursiveAlgorithm()
        {
            BigInteger max_price = 0, max_weight = 0;
            string best = "";
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            Rec(0,"",0,0,ref max_weight,ref max_price,ref best);
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            AddAnswer("рекурсивный полный перебор", best, max_price, max_weight, ts);
        }

        public static void Rec(int num, string s,BigInteger weight,BigInteger price, ref BigInteger max_weight,ref BigInteger max_price, ref string best)
        {
            if (num == n)
            {
                return;
            }
            Rec(num + 1,s, weight,price,ref max_weight,ref max_price, ref best);
            weight = BigInteger.Add(weight, knapsacks[num].weight);
            price = BigInteger.Add(price, knapsacks[num].price);
            s = s + (knapsacks[num].id) + ' ';
            if (price > max_price && weight <= M)
            {
                best = s;
                max_weight = weight;
                max_price = price;
            }
            Rec(num + 1,s, weight, price,ref max_weight,ref max_price, ref best);
        }

        public static void IterativeAlgorithm()
        {
            BigInteger max_price = 0, max_weight = 0, weight, price;
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            List<int> best_group = new List<int>();
            for (int k = 0; k < 1<<n; k++)
            {
                d++;
                weight = 0;
                price = 0;
                List<int> group = new List<int>();
                for (int t = 0; t < n; t++)
                {
                    if ((((1 << t) & k) != 0))
                    {
                        weight = BigInteger.Add(weight, knapsacks[t].weight);
                        price = BigInteger.Add(price, knapsacks[t].price);
                        group.Add(t);
                    }
                }
                if (price > max_price && weight <= M)
                {
                    best_group = group;
                    max_weight = weight;
                    max_price = price;
                }
            }
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string line = "";
            for (int i = 0; i < best_group.Count; i++)
            {
                line += (best_group[i] + " ");
            }
            AddAnswer("итерационный полный перебор", line, max_price, max_weight, ts);
        }

        public static void GreedyAlgorithm()
        {
            BigInteger max_price = 0, max_weight = 0;
            List<int> usedKnapsacks = new List<int>();
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            knapsacks = knapsacks.OrderByDescending(a => a.k).ToList();
            for (int i = 0; i < n; i++)
            {
                if (BigInteger.Add(max_weight,knapsacks[i].weight) <= M)
                {
                    max_price = BigInteger.Add(max_price,knapsacks[i].price);
                    max_weight = BigInteger.Add(max_weight,knapsacks[i].weight);
                    usedKnapsacks.Add(knapsacks[i].id);
                }
            }
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            usedKnapsacks.Sort();
            string line = "";
            for(int i = 0; i < usedKnapsacks.Count; i++)
            {
                line += (usedKnapsacks[i] + " ");
            }
            AddAnswer("жадный", line, max_price, max_weight, ts);
        }

        public static void GenerateAutoTest()
        {
            BigInteger price, weight;
            double k;
            knapsacks = new List<Knapsack>();
            for (int i = 0; i < n; i++)
            {
                price = rnd.Next(1, 100);
                weight = NextBigInteger(0,M);
                //Console.WriteLine(weight + " "+price);
                k = Math.Exp(BigInteger.Log(price) - BigInteger.Log(weight));
                knapsacks.Add(new Knapsack(i, price, weight, k));
            }
        }

        public static BigInteger NextBigInteger(int bitLength)
        {
            if (bitLength < 1) return BigInteger.Zero;

            int bytes = bitLength / 8;
            int bits = bitLength % 8;
            byte[] bs = new byte[bytes + 1];
            rnd.NextBytes(bs);
            byte mask = (byte)(0xFF >> (8 - bits));
            bs[bs.Length - 1] &= mask;
            return new BigInteger(bs);
        }

        public static BigInteger NextBigInteger(BigInteger start, BigInteger end)
        {
            if (start == end) return start;
            BigInteger res = end;
            if (start > end)
            {
                end = start;
                start = res;
                res = end - start;
            }
            else
                res -= start;
            Random rnd = new Random();
            byte[] bs = res.ToByteArray();
            int bits = 8;
            byte mask = 0x7F;
            while ((bs[bs.Length - 1] & mask) == bs[bs.Length - 1])
            {
                bits--;
                mask >>= 1;
            }
            bits += 8 * bs.Length;
            return ((NextBigInteger(bits + 1) * res) / BigInteger.Pow(2, bits + 1)) + start;
        }

        public static void AddAnswer(string name_algorithm, string line, BigInteger max_price, BigInteger max_weight, TimeSpan ts)
        {
            answer += ("Алгоритм: "+name_algorithm+"@");
            answer += "Оптимальное решение @";
            answer += ("Берем предметы: " + line + "@");
            answer += ("Стоимость: " + max_price + "@");
            answer += ("Масса: " + max_weight + "@");
            answer += ("Время выполнения: " + ts + "@@");
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Выберите способ введения данных: ");
            Console.WriteLine("1. Автогенерирующиеся тесты");
            Console.WriteLine("2. Тесты из файла");
            int type = Convert.ToInt32(Console.ReadLine());
            switch (type)
            {
                case 1:
                    {
                        Console.WriteLine("Введите количество входных данных для генерации теста");
                        n = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("Введите максимальную массу рюкзака");
                        M = BigInteger.Parse(Console.ReadLine());
                        GenerateAutoTest();
                        break;
                    }
                case 2:
                    {
                        Console.WriteLine("Введите название файла: ");
                        ReadFile(Console.ReadLine());
                        break;
                    }
                default: Console.WriteLine("Неверный код способа");
                    break;
            }
            RecursiveAlgorithm();
            IterativeAlgorithm();
            GreedyAlgorithm();
            WriteFile("OUTPUT.txt");
            Console.ReadKey();
        }
    }
}
