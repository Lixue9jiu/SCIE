namespace LibPixz
{
	internal class ArraySlice<T>
	{
		private readonly T[,] arr;
		private int firstDimension;

		public ArraySlice(T[,] arr)
		{
			this.arr = arr;
		}

		/*public int FirstDimension
		{
			get { return firstDimension; }
			set { firstDimension = value; }
		}*/

		public T this[int index]
		{
			get { return arr[firstDimension, index]; }
			set { arr[firstDimension, index] = value; }
		}

		/// <summary>
		/// Returns a slice of a 2D array, specified at a index
		/// Note: returned object is the same as input
		/// </summary>
		/// <param name="firstDimension">Index of the first dimension to slice</param>
		/// <returns>Slice of the 2D array</returns>
		public ArraySlice<T> GetSlice(int firstDimension)
		{
			this.firstDimension = firstDimension;
			return this;
		}
	}
}