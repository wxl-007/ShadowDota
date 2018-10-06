using System;

public interface IFilter {
	/// <summary>
    /// 检测Filter,并判定是否过滤某个条件
	/// </summary>
    bool check();
}
