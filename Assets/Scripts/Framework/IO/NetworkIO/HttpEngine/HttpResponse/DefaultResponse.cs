using System;

public class ExceptionResponse : BaseResponse {
    public string HttpError = string.Empty;
}

public class ThirdPartyResponse : BaseResponse {
	public string rawData;

	public ThirdPartyResponse(string response) {
		rawData = response;
	}

}

public class HttpDownloadResponse : BaseResponse {

}