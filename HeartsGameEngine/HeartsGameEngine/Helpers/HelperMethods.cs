using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeartsGameEngine.Helpers
{
    class HelperMethods
    {
        // http://eforum.idg.se/topic/325610-slumpa-ett-tal-mellan-1-och-10-i-c/?p=1523024
        public static int GetRandomNumber(int min, int max)
        {
            if (min > max)
                throw new ArgumentException();

            if (min == max)
                return min;

            System.Security.Cryptography.RandomNumberGenerator rnd = System.Security.Cryptography.RandomNumberGenerator.Create();
            byte[] data = new byte[1];
            rnd.GetNonZeroBytes(data);
            int n = data[0] % (max - min + 1);
            return min + n;
        }

        public static void Shuffle<T>(IList<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                //Get random number
                int rnd = GetRandomNumber(0, list.Count - 1);
                //Swap
                T tmp = list[i];
                list[i] = list[rnd];
                list[rnd] = tmp;
            }
        }

        public static int Rotate(int from, int positions, int max)
        {
            // Check if rotate backwards
            if (positions < 0)
                positions = max - ((-positions) % max);

            return (from + positions) % max;
        }

        public static void UpdateList<T>(IList<T> list, IList<T> newValues)
        {
            int listCount = list.Count();
            int newValsCount = newValues.Count();

            int count = (listCount > newValsCount) ? listCount : newValsCount;

            for (int i = 0; i < count; i++ )
            {
                if(i < newValsCount && i < listCount)
                {
                    bool equal = EqualityComparer<T>.Default.Equals(list[i], newValues[i]);

                    if (!equal)
                    {
                        list[i] = newValues[i];
                    }
                }
                else if (i >= listCount)
                {
                    list.Add(newValues[i]);
                }
                else if (i >= newValsCount)
                {
                    list.RemoveAt(i);
                }
            }
        }
    }
}
