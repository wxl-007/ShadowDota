using System;
using System.Collections.Generic;

namespace AW.Data {

	[Modle(type = DataSource.FromLocal)]
	public class NPCAttributeModel : KVModelBase<int, AttrbuteConfig> {

		public override bool loadFromConfig() {
			return base.load(ConfigType.NPCAttribute);
		}

	}

	/**
	* generate by zConvert, never try to change it
	*/
	public class AttrbuteConfig : UniqueBaseData <int>
	{
		public int      id {get; set;}                 //编号                 

		public string   attrbute{get; set;}            //属性                 

		public string   type{get; set;}                //类型                 

		public int      defaultValue{get; set;}        //默认值              

		public string   note{get; set;}                //注释(客户端使用）                 

		public override string ToString() {
			return 
				"Attrbute["
				+ "id=" + id + ", "
				+ "attrbute=" + attrbute + ", "
				+ "type=" + type + ", "
				+ "defaultValue=" + defaultValue + ", "
				+ "note=" + note + ", "
				+ "]";
		}

		/**
		 * 返回主键
		 */				
		public override Int32 getKey() {
			return this.id;
		}
	}

}

