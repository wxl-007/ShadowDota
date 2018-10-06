using System;
using System.Collections;
using System.Collections.Generic;

public class MathHelper
{

	public static float ONE_HUNDRED = 0.01f;

	//四舍五入
	public static int MidpointRounding (float round)
	{
		return (int)(round + 0.5f);
	}

	public static int MidpointRounding (double round)
	{
		return (int)(round + 0.5f);
	}

	//第一个有效位设置为1
	public static int setFirstPosToOne (int num)
	{
		string strNum = num.ToString ();
		strNum = "1" + strNum;
		return int.Parse (strNum);
	}

	//第一个有效位设置为2
	public static int setFirstPosToTwo (int num)
	{
		string strNum = num.ToString ();
		strNum = "2" + strNum;
		return int.Parse(strNum);
	}

	//移除第一个有效位
	public static int rmFirstPos(int num) {
		string strNum = num.ToString ();

		if (!string.IsNullOrEmpty (strNum) && strNum.Length >= 2) {
			strNum = strNum.Remove (0, 1);
			return int.Parse (strNum);
		} else 
			return 0;

	}

	public static bool IsNum(String str)
	{
		for(int i=0 ; i<str.Length ; i++)
		{
			if(!Char.IsNumber(str,i))
				return false;
		}
		return true;
	}

    /// <summary>
    /// 计算有多少位
    /// </summary>
    /// <returns>The many.</returns>
    public static int howMany(int num, List<int> separate) {
        int count = 0;
        string strNum = num.ToString();

        if(!string.IsNullOrEmpty(strNum)) {
            count = strNum.Length;

            if(separate != null) {
                for(int i = 0; i < count; ++ i) {
                    char n = strNum[i];
                    separate.Add( Convert.ToInt32(n) - 48 );
                }
            }

        }
        return count;
    }

    #region 保证数字大于1 or 0
	public static void KeepGreaterOne(ref float value){
		if (value < 1f) value = 1f;
	}
	public static void KeepGreaterOne(ref int value){
		if ( value >= 0 && value < 1) value = 1;
	}

	public static void KeepCreateZero(ref float value) {
		value = ( value <= 0f ? 0f : value );
	}

	public static void KeepCreateZero(ref int value) {
		value = ( value < 0 ? 0 : value );
	}
    #endregion
}