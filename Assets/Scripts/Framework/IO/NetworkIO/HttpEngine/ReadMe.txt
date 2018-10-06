添加一个新的Http Request之前必读

1. 在HttpRequestParam类里面添加一个新的以BaseRequestParam为基类的类，用于创建一个新的请求参数对象
2. 在AllResponse类里面添加一个新的以BaseResponse为基类的相应类。
3. 在HttpRequestFactory里面添加每个请求的ACT
4. 在HttpRequestFactory里面的 public static readonly Dictionary<RequestType, RelationShipReqAndResp> PreDefined = new Dictionary<RequestType, RelationShipReqAndResp>()；
   添加这两者的关系。
5. 如果需要自己转换返回的响应消息，可以在ActionOnReceiveHttpResponse里面的
    public readonly static Dictionary<RequestType, Action> ACTION_LIST = new Dictionary<RequestType, Action>() {

	};
   创建处理的Action。
6. 如果不需要对请求做自增处理的时候，在HttpThread中的NonHttpDataCompleteness添加。