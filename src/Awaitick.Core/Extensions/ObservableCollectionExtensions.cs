using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awaitick.Core.Extensions
{
    public static class ObservableCollectionExtensions
    {
		public static void MergeWith<T>(this ObservableCollection<T> target, IList<T> source, Func<T, T, bool> equalityComparer)
		{
			// 1. Remove any items from target that aren't in source.
			for (int i = target.Count - 1; i >= 0; i--)
			{
				if (!source.Any(s => equalityComparer(s, target[i])))
				{
					target.RemoveAt(i);
				}
			}

			// 2. Ensure items are present and in the same order as source.
			for (int i = 0; i < source.Count; i++)
			{
				var sourceItem = source[i];

				if (i >= target.Count)
				{
					// If target is shorter, simply add the item.
					target.Add(sourceItem);
				}
				else if (!equalityComparer(sourceItem, target[i]))
				{
					// Try to find the item elsewhere in target.
					var itemInTarget = target.FirstOrDefault(item => equalityComparer(sourceItem, item));
					if (itemInTarget is not null)
					{
						int index = target.IndexOf(itemInTarget);
						if (index >= 0)
						{
							// Move the item to the correct position.
							var item = target[index];
							target.Move(index, i);
						}
						else
						{
							// Item doesn't exist in target, so insert it.
							target.Insert(i, sourceItem);
						}
					}
				}
			}
		}
	}
}
