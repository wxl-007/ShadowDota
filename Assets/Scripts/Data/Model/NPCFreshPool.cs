using System;
using System.Collections.Generic;


namespace AW.Data
{
	public class NPCFreshPool : UniqueBaseData<int>
	{          
		public int ID;
		public List<Int32> freshPool {get; set;}    	//npc刷新池      

		public override string ToString() {
			return 
				"NPCFreshPool["
					+ "ID=" + ID + ", "
					+ "freshPool=" + freshPool + ", "
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
	public class FreshPoolModel : KVModelBase<int, NPCFreshPool>
	{

		public FreshPoolModel()
		{

		}

		/*************************************实现接口方法******************************/
		public override bool loadFromConfig()
		{
			return base.load (ConfigType.RefreshPool);
		}
		/*****************************************************************************/


		public NPCFreshPool GetNPCFreshPool(int id)
		{
			NPCFreshPool pool = null;
			if (instance.TryGetValue (id, out pool))
				return pool;

			ConsoleEx.DebugWarning ("FreshPoolModel :: not find fresh pool by id :: " + id);
			return null;
		}
	}
}