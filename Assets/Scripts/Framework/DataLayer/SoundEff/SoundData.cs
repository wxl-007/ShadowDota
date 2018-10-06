using System;

namespace AW.Data {

	/// <summary>
	/// 声音数据的存储，这些数据格式都和具体的游戏不想关
	/// </summary>
	public class SoundData : UniqueBaseData <int> {
		public string name;
		public int ID;

		public override int getKey() {
			return this.ID;
		}

		public SoundData () {
			
		}
	}
}

