using System;

public class DateHelper
{

    public const long ONE_DAY = 86400;

    private DateHelper()
    {
    }

    public static string getWeekOfDay()
    {
        string week = DateTime.Now.DayOfWeek.ToString();
        return week;
    }

    //data format : 2012年9月4日
    public static string getLongDate()
    {
        return DateTime.Now.ToLongDateString();
    }

    //data format : 12-9-4
    public static string getShortDate()
    {
        return DateTime.Now.ToShortDateString();
    }

    public static string getTomorrowShortDate()
    {
        return DateTime.Now.AddDays(1).ToShortDateString();
    }

    //fromTime's data format is something like : 12:00
    // toTime is similar to fromTime
    public static bool isDateBetween(string fromTime, string toTime)
    {
        bool isBetween = false;
        try
        {
            DateTime finalFromTime = DateTime.Parse(fromTime);
            DateTime finalToTime = DateTime.Parse(toTime);

            return isDateBetween(finalFromTime, finalToTime);
        }
        catch (Exception ex)
        {
            ConsoleEx.DebugLog("format error : " + ex.Message);
        }
        return isBetween;
    }

    public static bool isDateBetween(DateTime fromTime, DateTime toTime)
    {
        bool isBetween = false;

        if (DateTime.Compare(fromTime, toTime) >= 0)
            return isBetween;

        // get current time
        DateTime currentTime = new DateTime();
        currentTime = System.DateTime.Now;

        int comp = DateTime.Compare(fromTime, currentTime);
        if (comp <= 0)
        { // fromTime <= currentTime

            comp = DateTime.Compare(currentTime, toTime);
            if (comp <= 0)
            { // currentTime <= totime
                isBetween = true;
            }
            else
            {
                isBetween = false;
            }
        }
        else
        {
            isBetween = false;
        }


        return isBetween;
    }

    public static bool isPassedOneDay(long lastLogin, long curLogin)
    {
        int lastDay, curDay;

        lastDay = UnixTimeStampToDateTime(lastLogin).Day;
        curDay = UnixTimeStampToDateTime(curLogin).Day;

        if (curDay != lastDay)
            return true;
        else
            return false;
    }

    public static void dateSubtract(DateTime curTime, DateTime deadline, ref int day, ref int hour, ref int minute)
    {
        TimeSpan ts = curTime.Subtract(deadline);
        day = ts.Days;
        hour = ts.Hours;
        minute = ts.Minutes;
    }


    public static DateTime UnixTimeStampToDateTime(double unixTimeStamp, bool localTime = false)
    {
        // Unix timestamp is seconds past epoch
        DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        if (localTime)
        {
            if (unixTimeStamp < 0)
                dtDateTime = DateTime.Now;
            else
                dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        }
        else
        {
#if China
			if(unixTimeStamp < 0)
				dtDateTime = DateTime.Now;
			else 
				dtDateTime = dtDateTime.AddSeconds (unixTimeStamp).ToLocalTime ();
#else
            if (unixTimeStamp < 0)
                dtDateTime = DateTime.UtcNow;
            else
                dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToUniversalTime();
#endif
        }


        return dtDateTime;
    }

    public static long DateTimeToUnixTimeStamp(DateTime dateTime)
    {
#if China 
		return Convert.ToInt64 ((dateTime - new DateTime (1970, 1, 1).ToLocalTime ()).TotalSeconds);
#else
        return Convert.ToInt64((dateTime - new DateTime(1970, 1, 1).ToUniversalTime()).TotalSeconds);
#endif
    }

    public static long LocalDateTimeToUnixTimeStamp(DateTime dateTime)
    {
        return Convert.ToInt64((dateTime - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds);
    }



    //supposing that in one hour.
    public static string generateCounting(int minute, int second)
    {
        string format = string.Format("{0:D2}:{1:D2}", minute, second);
        return format;
    }

    //supposing that 
	public static string generateCounting(long now, long to, IStringManager strConfig)
    {
        if (now <= to)
        {
            DateTime dt_now = UnixTimeStampToDateTime(now);
            DateTime dt_to = UnixTimeStampToDateTime(to);

            TimeSpan diff = dt_to - dt_now;
			return generateLeftTiming(diff, strConfig);
        }
        else
            return string.Empty;

    }

	public static string generateLeftTiming(TimeSpan span, IStringManager strConfig, bool withoutHourIfZero = false)
    {
        string timing = string.Empty;

        int totalDay = span.Days;
        int totalHour = span.Hours;
        int totalMinite = span.Minutes;



#if US || Korea
		if(totalDay > 0) {
			timing = string.Format( "{0:D2} " + strConfig.getString(99), totalDay);
		} else if(totalHour > 0) {
			timing = string.Format( "{0:D2} " + strConfig.getString(98) + " {1:D2} " + strConfig.getString(97), totalHour, totalMinite );
		} else {
			timing = string.Format( "{0:D2} " + strConfig.getString(98) + " {1:D2} " + strConfig.getString(97), 0 , totalMinite );
		}
#else
		if(totalDay > 0) {
			timing = string.Format( "{0:D2}" + strConfig.getString(99), totalDay);
		} else if(totalHour > 0) {
			timing = string.Format( "{0:D2}" + strConfig.getString(98) + "{1:D2}" + strConfig.getString(97), totalHour, totalMinite );
		} else {
			if(withoutHourIfZero)
				timing = string.Format( "{0:D2}" + strConfig.getString(97), totalMinite );
			else
				timing = string.Format( "{0:D2}" + strConfig.getString(98) + "{1:D2}" + strConfig.getString(97), 0 , totalMinite );
		}
#endif
        return timing;
    }


	/// <summary>
	/// 获取当天剩余的时间，每天凌晨5点开始算作游戏的一天开始
	/// </summary>
	/// <returns>The left timing at day changed.</returns>
	public static long getLeftTimingAtDayChanged (long sysTime) {
		long TickOfDayEnd = 0, LeftOfDayEnd = 0;
		try {
			DateTime loginSys = DateHelper.UnixTimeStampToDateTime(sysTime);

			if(loginSys.Hour >= 5) {
				DateTime DayOfEnd = DateTime.Parse(loginSys.ToShortDateString() + " 23:59:59");
				TickOfDayEnd = DateHelper.DateTimeToUnixTimeStamp(DayOfEnd) + 1;

				LeftOfDayEnd = (TickOfDayEnd - sysTime) + 3600 * 5;
			} else {
				DateTime DayOfEnd = DateTime.Parse(loginSys.ToShortDateString() + " 05:00:00");
				TickOfDayEnd = DateHelper.DateTimeToUnixTimeStamp(DayOfEnd);

				LeftOfDayEnd = TickOfDayEnd - sysTime;
			}

		} catch(Exception ex) {
			ConsoleEx.DebugLog(ex.Message);
		} 

		return LeftOfDayEnd;
	}

	/// <summary>
	/// 获取当天剩余的时间，每天凌晨0点开始算作游戏的一天开始
	/// </summary>
	/// <returns>The left timingbefore day changed.</returns>
	public static long getLeftTimingbeforeDayChanged (long sysTime) {
		long TickOfDayEnd = 0, LeftOfDayEnd = 0;
		try {
			DateTime loginSys = DateHelper.UnixTimeStampToDateTime(sysTime);

			DateTime DayOfEnd = DateTime.Parse(loginSys.ToShortDateString() + " 23:59:59");
			TickOfDayEnd = DateHelper.DateTimeToUnixTimeStamp(DayOfEnd) + 60;

			LeftOfDayEnd = (TickOfDayEnd - sysTime);
		}
		catch(Exception ex) {
			ConsoleEx.DebugLog(ex.Message);
		} 

		return LeftOfDayEnd;
	}

	/// <summary>
	/// 获取当天晚上9点的剩余时间
	/// </summary>
	public static long getNineNight(long sysTime) {
		long TickOfDayEnd = 0, LeftOfDayEnd = 0;
		try {
			DateTime loginSys = DateHelper.UnixTimeStampToDateTime(sysTime);

			DateTime DayOfEnd = DateTime.Parse(loginSys.ToShortDateString() + " 21:00:00");
			TickOfDayEnd = DateHelper.DateTimeToUnixTimeStamp(DayOfEnd) + 60;

			LeftOfDayEnd = (TickOfDayEnd - sysTime);
		}
		catch(Exception ex) {
			ConsoleEx.DebugLog(ex.Message);
		} 

		return LeftOfDayEnd;
	}

	/// <summary>
	/// 从long 转换为 00:00:00 hh:mm:ss
	/// </summary>
	/// <returns>The date format from long.</returns>
	/// <param name="hss">Hss.</param>
	public static string getDateFormatFromLong(long totalSeconds)
	{
		int hours = (int)(totalSeconds / 60 / 60);
		int minutes = (int)(totalSeconds / 60 % 60);
		int seconds = (int)(totalSeconds % 60);
		string str = hours.ToString("00") + ":" + minutes.ToString("00") + ":" + seconds.ToString("00");
		return str;
	}

}