using System;

namespace GenerateSquare
{
    class Program
    {
        static void Main(string[] args)
        {
            var read = new ReadFileWord();
            read.ReadAndBuildTree();

            var generator = new GenerateSquare(read,5,5);
            var chars = generator.MaxCountGenerateBox();

            var bonus = Bonus.GenerateBonusPosition(5, 5);

            for (int i = 0; i < generator.CountRow; i++)
            {
                for (int j = 0; j < generator.CountColumn; j++)
                {
                    Console.Write(chars[i * generator.CountRow + j] + " ");
                }
                Console.WriteLine(" ");
            }

            foreach (var word in generator.BetterAllWord)
            {
                Console.WriteLine(word);
            }

            foreach (var b in bonus)
            {
                Console.WriteLine($"{b.Key} - {b.Value}");
            }
        }
    }
}
