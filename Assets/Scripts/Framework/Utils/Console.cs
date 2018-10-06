using UnityEngine;
#if UNITY_4_5
using System;
using System.Collections.Generic;
#endif

public class ConsoleEx {
	#if DEBUG
	public static bool DebugMode = true;
	#else
	public static bool DebugMode = false;
	#endif

	public const string RED   = "red";
	public const string GREEN = "green";
	public const string YELLOW = "yellow";

	public static void Write (string de, string color = "green") {

		#if UNITY_4_5_3
		SplitLog(de, DebugMode, color);
		#else
		if(DebugMode) Debug.Log( string.Format(@"<color={0}>{1}</color>", color, de)); 
		#endif
	}

	public static void DebugLog (string de, string color = "green") {
        #if DEBUG

		#if UNITY_4_5
		SplitLog(de, true, color);
		#else
		if(DebugMode) Debug.Log( string.Format(@"<color={0}>{1}</color>", color, de)); 
		#endif
        
		#endif
	}

	/// <summary>
	/// 警告log
	/// </summary>
	/// <param name="strText">String text.</param>
	public static void DebugWarning(string strText)
	{
		#if DEBUG
		if(DebugMode) Debug.LogWarning(strText); 
		#endif
	}

	/// <summary>
	/// 错误log
	/// </summary>
	/// <param name="strText">String text.</param>
	public static void DebugError(string strText)
	{
		#if DEBUG
		if(DebugMode) Debug.LogError(strText); 
		#endif
	}

	#if UNITY_4_5
	static void SplitLog (string de, bool debugMd, string color) {
		const int MAX_WORD_LENGTH = 1024;

		if(debugMd) {
			int length = de.Length;

			if(length <= MAX_WORD_LENGTH) {
				Debug.Log( string.Format(@"<color={0}>{1}</color>", color, de)); 
			} else {
				try {
					List<string> LogList = new List<string>();

					int i = 0, cutLen = MAX_WORD_LENGTH;
					do {

						if((i * MAX_WORD_LENGTH + MAX_WORD_LENGTH) > length)  {
							cutLen = 0;
						} else {
							cutLen = MAX_WORD_LENGTH;
						}

						if(cutLen != 0) {
							LogList.Add(de.Substring(i * MAX_WORD_LENGTH, MAX_WORD_LENGTH));
						} else {
							LogList.Add(de.Substring(i * MAX_WORD_LENGTH));
						}

						i ++;
					} while(i * MAX_WORD_LENGTH <= length);

					int logLen = LogList.Count;
					for(int j = 0; j < logLen; ++ j) {
						if(j == 0) {
							Debug.Log(string.Format(@"<color={0}>{1}</color>", color, LogList[j]));
						} else if(j == (logLen - 1)) {
							Debug.Log(string.Format(@"<color={0}>{1}</color>", color, LogList[j]));
						} else {
							Debug.Log(string.Format(@"<color={0}>{1}</color>", color, LogList[j]));
						}
					}
				} catch(Exception ex) {
					ConsoleEx.DebugLog(ex.ToString());
				}

			}

		}
	}
	#endif

	//check obj
	public static void Asset(object obj) {
		if (obj == null) {
			throw new DragonException( " Ex : " + obj.ToString() + " is null." );
		}
	}
	//check flag
	public static void Asset(bool flag) {
		if(flag) throw new DragonException("Asset Ex : can't be true." );
	}
}
