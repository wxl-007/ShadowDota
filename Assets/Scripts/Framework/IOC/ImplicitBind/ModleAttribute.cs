using System;
using AW.Entity;
using AW.Data;
/// 
/// 所有的数据层都必须有这个特性，
/// 当Type等于下面属性的时候：
/// 0. 服务器端传递过来的数据
/// 1. 直接从配置表里面读取
/// 2. 两者的合体
/// 
[AttributeUsage(AttributeTargets.Class)]
public class ModleAttribute : Attribute {
	//定义类型
	public DataSource type { get; set;}
}

///
/// 所有的控制层都必须有这个特性，
/// 此特性用来决定控制层是具体的控制哪个
/// 
[AttributeUsage(AttributeTargets.Class)]
public class ControllerAttribute : Attribute {
	//定义是哪个的控制层
	public LogicalType ctrlType;
}
