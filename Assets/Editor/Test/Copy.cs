using System;

namespace AW.Test {

	using NUnit.Framework;
	using AW.Framework;
	using AW.Data;

	[TestFixture]
	public class Copy  {

		private NPCConfigData cfg;
		private NPCRuntimeData runtime;

		[SetUp]
		public void Init() {

			cfg = new NPCConfigData() {
				type = LifeNPCType.Build,
				moveable = Moveable.BeStatic,
				skill = new int[]{ 1, 2, 3 },
				crit  = 0.1f,
				ID    = 3,
				healthpoint = 300,
			};

			runtime = new NPCRuntimeData(cfg);
		}


		[Test]
		public void ShallowCopy() {
			NPCConfigData newCfg = cfg.ShallowCopy();

			Assert.IsTrue( newCfg.type == LifeNPCType.Build );
			Assert.IsTrue( newCfg.moveable == Moveable.BeStatic );
			Assert.IsTrue( newCfg.crit == 0.1F );
			Assert.IsTrue( newCfg.ID  == 3);
			Assert.IsTrue( newCfg.skill[1] == 2);
			Assert.IsTrue( newCfg.healthpoint == 300);
		}

		[Test]
		public void RtShallowCopy() {
			NPCRuntimeData newRt = runtime.ShallowCopy();

			Assert.IsTrue( newRt.curHp == 300 );
			Assert.IsTrue( newRt.crit == 0.1F );
		}

	}
}
