using System;


/*
 * 需要释放内存的类都需要继承自此接口
 */

public interface IDispose {
	/// <summary>
	/// Releases all resource used by the object.
	/// </summary>
	void Dispose();
}