using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// 优化hash和BitArray
/// </summary>
public class BadWordsFilter
{
		//private Hash hash = new HashSet();
		private HashSet<string> hash = new HashSet<string>();
		private byte[] fastCheck = new byte[char.MaxValue + 1];
		private byte[] fastLength = new byte[char.MaxValue + 1];
		private bool[] charCheck = new bool[char.MaxValue + 1];
		private bool[] endCheck = new bool[char.MaxValue + 1];
		private int maxWordLength = 0;
		private int minWordLength = int.MaxValue;


		public void AddKey(string word)
		{
			maxWordLength = Math.Max(maxWordLength, word.Length);
			minWordLength = Math.Min(minWordLength, word.Length);

			for (int i = 0; i < 7 && i < word.Length; i++)
			{
				fastCheck[word[i]] |= (byte)(1 << i);
			}

			for (int i = 7; i < word.Length; i++)
			{
				fastCheck[word[i]] |= 0x80;
			}

			if (word.Length == 1)
			{
				charCheck[word[0]] = true;
			}
			else
			{
				fastLength[word[0]] |= (byte)(1 << (Math.Min(7, word.Length - 2)));
				endCheck[word[word.Length - 1]] = true;
				hash.Add(word);
			}
		}

		public string Filter(string text, string mask)
		{
			throw new NotImplementedException();
		}

		public bool HasBadWord(string text)
		{
			int index = 0;

			while (index < text.Length)
			{
				int count = 1;

				if (index > 0 || (fastCheck[text[index]] & 1) == 0)
				{
					while (index < text.Length - 1 && (fastCheck[text[++index]] & 1) == 0) ;
				}

				char begin = text[index];

				if (minWordLength == 1 && charCheck[begin])
				{
					return true;
				}

				for (int j = 1; j <= Math.Min(maxWordLength, text.Length - index - 1); j++)
				{
					char current = text[index + j];

					if ((fastCheck[current] & 1) == 0)
					{
						++count;
					}

					if ((fastCheck[current] & (1 << Math.Min(j, 7))) == 0)
					{
						break;
					}

					if (j + 1 >= minWordLength)
					{
						if ((fastLength[begin] & (1 << Math.Min(j - 1, 7))) > 0 && endCheck[current])
						{
							if (hash.Contains( text.Substring(index, j + 1) ) )
							{
								return true;
							}
						}
					}
				}

				index += count;
			}

			return false;
		}
}
