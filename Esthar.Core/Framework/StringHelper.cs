using System;

namespace Esthar.Core
{
    public static class StringHelper
    {
        public static int IndexOfAny(char[] chars, int offset, int left, out string finded, params string[] subStrings)
        {
            int[] counters = new int[subStrings.Length];

            if (left > 0)
            {
                for (int i = offset; i < chars.Length && left > 0; i++, left--)
                {
                    for (int k = 0; k < subStrings.Length; k++)
                    {
                        string str = subStrings[k];
                        if (chars[i] != str[counters[k]++])
                            counters[k] = 0;
                        else if (counters[k] == str.Length)
                        {
                            finded = str;
                            return i - str.Length + 1;
                        }
                    }
                }
            }
            else
            {
                for (int i = offset; i >= 0 && left < 0; i--, left++)
                {
                    for (int k = 0; k < subStrings.Length; k++)
                    {
                        string str = subStrings[k];
                        if (chars[i] != str[str.Length - 1 - counters[k]++])
                            counters[k] = 0;
                        else if (counters[k] == str.Length)
                        {
                            finded = str;
                            return i - str.Length + 1;
                        }
                    }
                }
            }

            finded = null;
            return -1;
        }

        public static int IndexOfAny(string chars, int offset, int left, out string finded, params string[] subStrings)
        {
            int[] counters = new int[subStrings.Length];

            if (left > 0)
            {
                for (int i = offset; i < chars.Length && left > 0; i++, left--)
                {
                    for (int k = 0; k < subStrings.Length; k++)
                    {
                        string str = subStrings[k];
                        if (chars[i] != str[counters[k]++])
                            counters[k] = 0;
                        else if (counters[k] == str.Length)
                        {
                            finded = str;
                            return i - str.Length + 1;
                        }
                    }
                }
            }
            else
            {
                for (int i = offset; i >= 0 && left < 0; i--, left++)
                {
                    for (int k = 0; k < subStrings.Length; k++)
                    {
                        string str = subStrings[k];
                        if (chars[i] != str[str.Length - 1 - counters[k]++])
                            counters[k] = 0;
                        else if (counters[k] == str.Length)
                        {
                            finded = str;
                            return i - str.Length + 1;
                        }
                    }
                }
            }

            finded = null;
            return -1;
        }
    }
}