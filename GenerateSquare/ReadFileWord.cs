using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GenerateSquare
{
    class ReadFileWord
    {
        // список букв доступных для рендомного выбора, что бы заполнять пустые клетки в поле
        public char[] Letters = new char[] { 'а', 'о', 'у', 'ы', 'э', 'я', 'е', 'ё', 'ю', 'и', 'а', 'о', 'у', 'ы', 'э', 'я', 'е', 'ё', 'ю', 'и', 'б', 'в', 'г', 'д', 'й', 'ж', 'з', 'к', 'л', 'м', 'н', 'п', 'р', 'с', 'т', 'ф', 'х', 'ц', 'ч', 'ш', 'щ' };

        // список всех слов 
        public List<char[]> CharStrings = new List<char[]>();

        // спловарь деревьев в которых хранятся слова в древовидной структуре
        public Dictionary<char, Letter> TreeWord = new Dictionary<char, Letter>();

        // путь до файла где лежит список слов
        private string _path;
        
        public void ReadFileCorutina()
        {
            _path = Directory.GetCurrentDirectory() + @"\words.txt";

            var arrayLine = File.ReadLines(_path, Encoding.UTF8);
            var watch = System.Diagnostics.Stopwatch.StartNew();

            foreach (var word in arrayLine)
            {
                // слова из одной буквы в базу не добавляются
                if (word.Length == 1) continue;

                // добавляем слово в лист
                var charFromStr = word.ToCharArray();
                CharStrings.Add(charFromStr);

                Letter currentLetter = new Letter();

                for (int i = 0; i < charFromStr.Length; i++)
                {
                    // если первая буква то сначала мы работаем не с текущим элементом, а со словарем
                    if (i == 0)
                    {
                        if (!TreeContainsLetter(charFromStr[i], TreeWord))
                            TreeWord.Add(charFromStr[i], new Letter(charFromStr[i]));

                        currentLetter = TreeWord[charFromStr[i]];
                    }

                    // идем по дереву, если ветви со следующей буквой нет, то создаем ее
                    if (i > 0 && i < charFromStr.Length)
                    {
                        if (!TreeContainsLetter(charFromStr[i], currentLetter.NextLetter))
                            currentLetter.NextLetter.Add(charFromStr[i], new Letter(charFromStr[i]));

                        currentLetter = currentLetter.NextLetter[charFromStr[i]];
                    }

                    // на последней букве слово закончено, значит можем сохранить его
                    if (i == charFromStr.Length - 1)
                        currentLetter.Word = charFromStr;
   
                }
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("Чтение заняло - " + elapsedMs);
            Console.WriteLine("Кол-во слов - " + CharStrings.Count);

            CharStrings = CharStrings.OrderBy(i => Guid.NewGuid()).ToList();
        }


        private bool TreeContainsLetter(char v, Dictionary<char, Letter> dict)
        {
            return dict.ContainsKey(v);
        }

    }
}
