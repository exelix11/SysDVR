using SysDVR.Client.Sources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SysDVR.Client.Core
{
	internal class DisposableCollection<T> : IDisposable, IReadOnlyList<T> where T : class, IDisposable
	{
		record struct Container(T Element, bool Dispose);

		readonly Container[] items;

		public T this[int index] => items[index].Element;
		public int Count => items.Length;

		public DisposableCollection(IEnumerable<T> collection)
		{
			items = collection.Select(x => new Container(x, true)).ToArray();
		}

		public void ExcludeFromDispose(T element)
		{
			var index = items
				.Select((x, i) => (x, i))
				.Where(x => object.ReferenceEquals(x.x.Element, element))
				.First().i;

			items[index].Dispose = false;
		}

		public void Dispose()
		{
			foreach (var item in items)
				if (item.Dispose) 
					item.Element.Dispose();
		}

		public IEnumerator<T> GetEnumerator()
		{
			return items.Select(x => x.Element).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return items.GetEnumerator();
		}
	}
}
