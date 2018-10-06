using System;
using System.Collections.Generic;

namespace AW.IO {
	public enum DataType {
		SERVER_TYPE    = 0x01,
		HTTP_COMPLETE  = 0x02,
		CRASH_TYPE     = 0x03,
		USER_CONFIG    = 0x04,
		ACCOUNT_CONFIG = 0x05,
	}

	public class DataPersisterConfig {
		/*
	     * We define file name and typeOf(DataObject)
	     */
		public static readonly Dictionary<DataType, RelationData> PreDefined = new Dictionary<DataType,RelationData>() {
			{ DataType.HTTP_COMPLETE,     new RelationData("Hc.bin",        typeof(HttpData_No))    },
		};

	}
}

