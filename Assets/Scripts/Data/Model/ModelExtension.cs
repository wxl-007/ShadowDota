
namespace AW.Data {
	// ---数组的扩展类---
	//
	public static class ArrayExtension
	{
		public static T Value<T>(this T[] array, int pos) {
			if(array != null && array.Length > pos ) {
				return array[pos];
			} else {
				return default(T);
			}
		}

		public static bool IsNullOrEmpty (this System.Array array) {
			if(array != null && array.Length > 0)
				return false;
			else 
				return true;
		}

	}
}
