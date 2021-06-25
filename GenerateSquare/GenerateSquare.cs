using System;
using System.Collections.Generic;
using System.Linq;

namespace GenerateSquare
{
    class GenerateSquare
    {
        // Параметры отвечающие за генерацию лучшего результата
        // количество попыток генерации
        public int MaxIteration = 5;
        // минимальное количество слов при котором генерация считается успешной
        public int MinCountWord = 100;

        // здесь хранится список слов полученный при лучшей генерации 
        public List<string> BetterAllWord = new List<string>();

        // ссылка на дерево 
        public Dictionary<char, Letter> treeWord;

        // Size Cell
        private int _countColumn;
        private int _countRow;

        private ReadFileWord _read;
      
        // все найденые слова 
        private List<string> _allWord = new List<string>();

        // сгенерированный квадрат из букв
        private char[,] _cellLetter;

        // список из лучших сгенерированных букв
        private char[] _betterArrayLetter;

        public int CountColumn { get => _countColumn; set => _countColumn = value; }
        public int CountRow { get => _countRow; set => _countRow = value; }

        public GenerateSquare(ReadFileWord readFileWord, int countColumn, int countRow)
        {
            _read = readFileWord;
            CountColumn = countColumn;
            CountRow = countRow;
        }

        public char[] MaxCountGenerateBox()
        {
            BetterAllWord.Clear();

            var watch = System.Diagnostics.Stopwatch.StartNew();

            // начало генерации вариантов
            for (int i = 0; i < MaxIteration; i++)
            {
                GenerateBox();
                if (BetterAllWord.Count >= MinCountWord)
                    break;
            }

            BetterAllWord.Sort();
            watch.Stop();

            Console.WriteLine("Count word = " + BetterAllWord.Count);
            Console.WriteLine("Time = " + watch.ElapsedMilliseconds);

            return _betterArrayLetter;
        }

        public void GenerateBox()
        {
            var countLetter = CountColumn * CountRow;
            var arrayRandomLetters = new char[countLetter];
            // заполнение квадрата алгоритмом схожим с генерацией кросворда
            FillCrosswordLettersArray(arrayRandomLetters);
            // преобразуем из одномерного в двумерный массив
            _cellLetter = GetCell(arrayRandomLetters);
            // кешируем дерево
            treeWord = _read.TreeWord;
            // поиск слов в сгенерированном поле
            FindAllWord();

            if (_allWord.Count > BetterAllWord.Count)
            {
                var s = new string[_allWord.Count];
                _allWord.CopyTo(s);
                BetterAllWord = s.ToList<string>();

                _betterArrayLetter = new char[arrayRandomLetters.Length];
                arrayRandomLetters.CopyTo(_betterArrayLetter, 0);
            }
        }

        private char[,] GetCell(char[] arrayLetters)
        {
            var cellChar = new char[CountRow, CountColumn];

            for (int i = 0; i < CountRow; i++)
            {
                for (int j = 0; j < CountColumn; j++)
                {
                    cellChar[i, j] = arrayLetters[i * CountRow + j];
                }
            }

            return cellChar;
        }

        private void FillCrosswordLettersArray(char[] arrayRandomLetters)
        {
            var Row = CountRow;
            var Column = CountColumn;
            var cell = new char[Row, Column];
            var iteration = 0;
            var random = new Random();

            // пытаемся разместить рендомные слова в квадрате 100 раз
            do
            {
                var x = random.Next(0, Row);
                var y = random.Next(0, Column);

                FillWord(cell, new Vector2Int(x, y));

                iteration++;

            } while (iteration < 100);

            // заполняем пустые места
            for (int x = 0; x < Row; x++)
            {
                for (int y = 0; y < Column; y++)
                {
                    if (cell[x, y].Equals('\0'))
                    {
                        arrayRandomLetters[x * Row + y] = _read.Letters[random.Next(0, _read.Letters.Length)];
                    }
                    else
                    {
                        arrayRandomLetters[x * Row + y] = cell[x, y];
                    }
                }
            }
        }

        // алгоритм очень простой и мало походит на алгоритм построения кроссворда
        // но работает достаточно эффективно
        private void FillWord(char[,] cell, Vector2Int position)
        {
            var random = new Random();

            List<Vector2Int> posiiblePos = new List<Vector2Int>();
            Vector2Int currentPos = position;
            // выбираем слуучайное слово
            var str = _read.CharStrings[random.Next(0, _read.CharStrings.Count)];
            // попытка размещения слово в рендомном направлении
            for (int i = 0; i < str.Length; i++)
            {
                if (cell[currentPos.X, currentPos.Y].Equals('\0'))
                {
                    posiiblePos.Add(currentPos);
                    currentPos = GetPossiblePos(cell, currentPos);
                    if (currentPos == Vector2Int.Zero)
                    {
                        return;
                    }
                }
            }
            var j = 0;
            foreach (var pos in posiiblePos)
            {
                cell[pos.X, pos.Y] = str[j];
                j++;
            }
        }

        // получаем возможное место расположения следующей буквы
        private Vector2Int GetPossiblePos(char[,] cell, Vector2Int currentPos)
        {
            List<Vector2Int> direction = new List<Vector2Int> { Vector2Int.Up, Vector2Int.Down, Vector2Int.Left, Vector2Int.Right };
            foreach (var dir in direction)
            {
                var posDir = currentPos + dir;
                if (posDir.X >= 0 && posDir.X < cell.GetLength(0))
                {
                    if (posDir.Y >= 0 && posDir.Y < cell.GetLength(1))
                    {
                        if (cell[posDir.X, posDir.X].Equals('\0'))
                        {
                            return posDir;
                        }
                    }
                }
            }
            return Vector2Int.Zero;
        }

        private void FindAllWord()
        {
            _allWord.Clear();
            Dictionary<Vector2Int, List<Vector2Int>> dict = new Dictionary<Vector2Int, List<Vector2Int>>();

            for (int i = 0; i < CountColumn; i++)
            {
                for (int j = 0; j < CountRow; j++)
                {
                    var point = new Vector2Int(i, j);
                    dict.Add(point, GetNextStep(point, new Vector2Int(CountColumn, CountRow)));
                }
            }

            // запускаем алгоритм поиска всех путей из одной точки в другую
            var watch = System.Diagnostics.Stopwatch.StartNew();
            foreach (var point in dict.Keys)
            {
                FindPath(dict, point);
            }
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("Проход по дереву занял - " + elapsedMs);
        }

        public void FindPath(Dictionary<Vector2Int, List<Vector2Int>> dict, Vector2Int start)
        {
            // помечаем пройденные клетки
            var marks = new Dictionary<Vector2Int, bool>();
            // тут содержится текущий путь
            var path = new Stack<Vector2Int>();

            foreach (var i in dict.Keys)
                marks.Add(i, false);

            var count = 0;

            foreach (var i in dict.Keys)
            {
                if (i != start)
                {
                    DFS(dict, marks, start, i, ref count, path);
                }
            }
        }

        // алгоритм поиска в глубину направлен на поиск всевозможных путей из одной точки в другую
        public void DFS(Dictionary<Vector2Int, List<Vector2Int>> dict, Dictionary<Vector2Int, bool> marks, Vector2Int currentPoint, Vector2Int target, ref int count, Stack<Vector2Int> path)
        {
            path.Push(currentPoint);
            // цель достигнута
            if (currentPoint == target)
            {
                count++;
                path.Pop();
                return;
            }

            Letter currentLetter = null;
            var currentWord = new List<char>();

           
            foreach (var i in path.Reverse()) 
            {
                // прекращаем поиск если видим что слов начинающихся с найденной последовательности символов не существует
                if (currentLetter == null)
                {
                    if (treeWord.ContainsKey(_cellLetter[i.X, i.Y]))
                    {
                        currentLetter = treeWord[_cellLetter[i.X, i.Y]];
                        currentWord.Add(currentLetter.Symbol);
                    }
                    else
                    {
                        path.Pop();
                        return;
                    }
                }
                else
                {
                    if (currentLetter.NextLetter.ContainsKey(_cellLetter[i.X, i.Y]))
                    {
                        currentLetter = currentLetter.NextLetter[_cellLetter[i.X, i.Y]];
                        currentWord.Add(currentLetter.Symbol);
                    }
                    else
                    {
                        path.Pop();
                        return;
                    }
                }

                // если слово найдено то запоминаем его
                if (currentLetter != null && currentLetter.Word != null)
                {
                    var aray = currentWord.ToArray();
                    // проверка на совпадение найденной последовательности со словом которе содержится в букве
                    if (Enumerable.SequenceEqual(currentLetter.Word, aray))
                    {
                        var str = new string(currentLetter.Word);
                        if (!_allWord.Contains(str))
                            _allWord.Add(str);
                    }
                }
            }


            marks[currentPoint] = true;
            foreach (var point in dict[currentPoint])
            {
                if (marks[point] == false)
                {
                    DFS(dict, marks, point, target, ref count, path);
                }
            }
            marks[currentPoint] = false;
            path.Pop();
        }

        // получаем возможные переходы из данной точки
        private List<Vector2Int> GetNextStep(Vector2Int currentPos, Vector2Int sizeCell, List<Vector2Int> ignorPoints)
        {
            var result = new List<Vector2Int>();

            int[] xArray = new int[] { 1, 0, -1 };
            int[] yArray = new int[] { 1, 0, -1 };

            foreach (var x in xArray)
            {
                foreach (var y in yArray)
                {
                    if (currentPos.X + x >= 0 && currentPos.X + x < sizeCell.X)
                    {
                        if (currentPos.Y + y >= 0 && currentPos.Y + y < sizeCell.Y)
                        {
                            if (!(x == 0 && y == 0))
                            {
                                var v = new Vector2Int(currentPos.X + x, currentPos.Y + y);
                                if (!ignorPoints.Contains(v))
                                    result.Add(v);
                            }
                        }
                    }
                }
            }
            return result;
        }

        private List<Vector2Int> GetNextStep(Vector2Int currentPos, Vector2Int sizeCell)
        {
            return GetNextStep(currentPos, sizeCell, new List<Vector2Int>());
        }
    }
}
