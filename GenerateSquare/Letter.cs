using System;
using System.Collections.Generic;

namespace GenerateSquare
{
    class Letter
    {
        // буква 
        private char _symbol;
        // словарь букв которые следуют после этой в словах
        // словарь сделан для удобства можно было и листом обойтись
        private Dictionary<char, Letter> _nextLetter;
        // слово последняя буква в котором == letters
        private char[] _word;

        public char Symbol { get => _symbol; set => _symbol = value; }
        public Dictionary<char, Letter> NextLetter { get => _nextLetter; set => _nextLetter = value; }
        public char[] Word { get => _word; set => _word = value; }

        public Letter()
        {
            Symbol = '0';
            NextLetter = new Dictionary<char, Letter>();
            Word = Array.Empty<char>();
        }

        public Letter(char letter, Dictionary<char, Letter> nextLetter, char[] word)
        {
            Symbol = letter;
            NextLetter = nextLetter;
            Word = word;
        }

        public Letter(char letter)
        {
            Symbol = letter;
            NextLetter = new Dictionary<char, Letter>();
            Word = null;
        }
    }
}
