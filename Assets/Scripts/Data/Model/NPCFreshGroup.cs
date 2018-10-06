using System;
using System.Collections.Generic;

namespace AW.Data
{
	public class NPCFreshGroup : UniqueBaseData<int> 
	{               
		public int ID;
		public List<Int32> freshGroup {get; set;}     	//NPC编组

		public override string ToString() {
			return 
				"NPCFreshGroup["
					+ "ID=" + ID + ", "
					+ "freshGroup=" + freshGroup + ", "
					+ "]";
		}
				
		/**
		 * 返回主键
		 */				
		public override int getKey() {
			return this.ID;
		}
	}

	[Modle(type = DataSource.FromLocal)]
	public class FreshGroupModel : KVModelBase<int, NPCFreshGroup>
	{
		public FreshGroupModel()
		{

		}

		/*************************************实现接口方法******************************/
		public override bool loadFromConfig()
		{
			return base.load (ConfigType.RefreshGroup);
		}
		/*****************************************************************************/


		public NPCFreshGroup GetFreshGroup(int id)
		{
			NPCFreshGroup freshgrop = null;
			if (instance.TryGetValue (id, out freshgrop))
				return freshgrop;

			ConsoleEx.DebugWarning ("FreshGroupModel :: not find gorup by id ::  " + id);
			return null;
		}
	}
}
