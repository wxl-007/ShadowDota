using System;
using System.Collections;
using AW.IO;

public class CrashReport : DataObject {
	public int gameId;
	public int platformId;
	public int serverId;
	public int serverName;
	public string errorInfo;
	public int lastAction;


	public void record(LocalIOManager persist) {
		if(persist != null) {
			mType = DataType.CRASH_TYPE;
			persist.AppendToLocalFileSystem(this, NO_ENCRYPT);
		}
	}

	public static CrashReport getReport(string error) {
		CrashReport report = new CrashReport();
		report.errorInfo = error;
		//collect information
		//.. to do ...
		return report;
	}

}

/*
 * All Exception will be recorded and send to the server.
 */
public class DragonException : ApplicationException
{

	public DragonException() : base() {

    }

	public DragonException(string message) : base(message) {

    }

	public DragonException(string message, Exception innerException) : base(message, innerException) {

    }

	//we should send error info to server and save to local file system.
	public void report() {
		// tell user to send to the server

		// save to local file system
		// and use Thread-Pool
		AsyncTask.RunAsync(() => CrashReport.getReport(Message.ToString()).record(Core.DPM));
	}

    //define All kinds of exception
    public const short INVALIDATE_ARGUMENT = 0x00;


    public static string[] Exception_Message = {
		"invalidate argument",
	};
}