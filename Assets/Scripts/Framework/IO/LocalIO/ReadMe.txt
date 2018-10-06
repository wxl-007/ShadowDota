如何添加本地存储的方法：

1.继承自DataObject类，定义自己的数据结构，
2.在DataPersisterConfig里面的DataType枚举中定义自己的类型，

3. 在DataPersisterConfig里面的public static readonly Dictionary<DataType, RelationData> PreDefined = new Dictionary<DataType,RelationData>() {
		{ DataType.SERVER_TYPE,       new RelationData("Server.bin",    typeof(DataObject)) },
		{ DataType.HTTP_COMPLETE,     new RelationData("Hc.bin",    typeof(DataObject))     },
};添加新的定义。
