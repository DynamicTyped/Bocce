using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;

namespace Bocce
{
	/// <summary>
	/// Implementation of IResourceReader required to retrieve a dictionary
	/// of resource values for implicit localization. 
	/// </summary>
	internal sealed class DbResourceReader : IResourceReader
	{
		private IEnumerable<KeyValuePair<string, string>> _resources;

		public DbResourceReader(IEnumerable<KeyValuePair<string, string>> resources)
		{
			if (resources == null) { throw new ArgumentNullException("resources"); }

			_resources = resources;
		}

		private IDictionaryEnumerator GetEnumeratorInternal()
		{
			if (_resources == null) { throw new ObjectDisposedException("DBResourceReader"); }

			return new MyDictionaryEnumerator(_resources.GetEnumerator());
		}

		#region IResourceReader Members

		public void Close()
		{
			((IDisposable)this).Dispose();
		}

		public IDictionaryEnumerator GetEnumerator()
		{
			return GetEnumeratorInternal();
		}

		#endregion IResourceReader Members

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumeratorInternal();
		}

		#endregion IEnumerable Members

		#region IDisposable Members

		void IDisposable.Dispose()
		{
			var resources = _resources as IDisposable;

			if (resources != null)
			{
				resources.Dispose();
			}

			_resources = null;
		}

		#endregion

		private sealed class MyDictionaryEnumerator : IDictionaryEnumerator
		{
			private readonly IEnumerator<KeyValuePair<string, string>> _underlyingEnumerator;
			private DictionaryEntry _current;

			public MyDictionaryEnumerator(IEnumerator<KeyValuePair<string, string>> underlyingEnumerator)
			{
				if (underlyingEnumerator == null) { throw new ArgumentNullException("underlyingEnumerator"); }

				_underlyingEnumerator = underlyingEnumerator;
			}

			public DictionaryEntry Entry
			{
				get { return _current; }
			}

			public object Key
			{
				get { return _current.Key; }
			}

			public object Value
			{
				get { return _current.Value; }
			}

			public object Current
			{
				get { return _current; }
			}

			public bool MoveNext()
			{
				if (_underlyingEnumerator.MoveNext())
				{
					var underlyingCurrent = _underlyingEnumerator.Current;
					_current = new DictionaryEntry(underlyingCurrent.Key, underlyingCurrent.Value);
					return true;
				}
			    
                _current = new DictionaryEntry();
			    return false;
			}

			public void Reset()
			{
				_underlyingEnumerator.Reset();
				_current = new DictionaryEntry();
			}
		}
	}
}