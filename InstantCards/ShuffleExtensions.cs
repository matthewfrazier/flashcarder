using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protomeme
{
	public static class ShuffleExtensions
	{
		public static void SwapAt<T>(this IList<T> list, int indexA, int indexB)
		{
			var temp = list[indexA];
			list[indexA] = list[indexB];
			list[indexB] = list[indexA];
		}

		public static void ShuffleInPlace<T>(this IList<T> list)
		{
			var indexes = new List<int>(list.Count);
			for(int i = 0; i < indexes.Count; i++)
			{
				indexes[i] = i;
			}
			var shuffled = indexes.Shuffle().ToList();
			for (int i = 0; i < indexes.Count; i++)
			{
				list.SwapAt(i, shuffled[i]);
			}
		}

		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
		{
			Random rand = new Random((int)DateTime.Now.Ticks);
			return source.Select(t =>
				new KeyValuePair<int, T>(rand.Next(), t)).OrderBy(
					pair => (int)pair.Key)
					.Select(pair => (T)pair.Value).ToList();
		}

	}
}
