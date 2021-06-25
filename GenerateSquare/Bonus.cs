using System;
using System.Collections.Generic;

namespace GenerateSquare
{
    public class Bonus
    {
        public enum BonusVariant
        {
            x2,
            x3,
            c2,
            c3,
            empty
        }

        public static Dictionary<Vector2Int, BonusVariant> GenerateBonusPosition(int countRow, int countColumn)
        {
            var dict = new Dictionary<Vector2Int, BonusVariant>();
            // создаем лист из бонусов
            var bonusArray = new List<Bonus.BonusVariant> { Bonus.BonusVariant.c2, Bonus.BonusVariant.c3, Bonus.BonusVariant.x2, Bonus.BonusVariant.x3 };
           
            var random = new Random();

            // заполняем словарь 
            while (dict.Count < 4)
            {
                var vector = new Vector2Int(random.Next(0, countRow), random.Next(0, countColumn));
                if (!dict.ContainsKey(vector))
                {
                    var bonus = bonusArray[random.Next(0, bonusArray.Count)];
                    dict.Add(vector, bonus);    
                    bonusArray.Remove(bonus);
                }
            }

            return dict;
        }
    }
}
