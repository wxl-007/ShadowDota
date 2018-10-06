using System;
using System.Collections;
using System.Collections.Generic;

namespace AW.Framework {
	public static class Utils {

		public static bool inArray<Telement>(Telement element, Telement[] array) {
			bool found = false;
			if (array != null) {
				foreach (Telement e in array)
				{
					if(e.Equals(element)){
						found = true;
						break;
					}
				}
			}
			return found;
		}

		//安全的释放，
		public static void safeFree<T> (this T list) where T : IList {
			if(list != null) {
				list.Clear();
				list = default(T);
			}
		}

		//安全的释放
		public static void safeClear<T> (this T list) where T : IList {
			if(list != null) {
				list.Clear();
			}
		}

		public static void saftyFree<T> (this T Dic) where T : IDictionary {
			if(Dic != null) {
				Dic.Clear();
				Dic = default(T);
			}
		}

		public static void Assert(bool flag, string message = "") {
			if(flag) throw new DragonException("obj is null." + message);
		}


		public static bool checkJsonFormat (string json)
		{
			if (json == null || json == string.Empty) {
				return false;
			} else {
				char head, tail;
				head = json [0];
				tail = json [json.Length - 1];
				if (head == '{' && tail == '}')
					return true;
				else 
					return false;
			}
		}

	}
}




