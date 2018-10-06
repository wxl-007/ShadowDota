using System;
using xClient;
using SuperSocket.ClientEngine;
using System.Collections.Generic;

/// <summary>
/// 由工具生成该类
/// </summary>
namespace xClient.Action{

	public class UserAction : IAction {

		public void _userLogin_1001(UserLoginParams param, NonBlockingConnection conn){
			//TODO Your Code Here
			ConsoleEx.DebugLog( string.Format("The UserName is {0}", param.userName) );
			ConsoleEx.DebugLog( string.Format("The Password is {0}", param.passwd) );
			ConsoleEx.DebugLog( string.Format("The Firends size is {0}", param.firends.Count) );
		}

	}
		//======================
	public class UserLoginParams : IFuncParam {

		/*
		 * ÓÃ»§ºÃÓÑ
		 */
		public List<String> firends{ get; set; }

		/*
		 * ÓÃ»§Ãû
		 */
		public String userName{ get; set; }

		/*
		 * ÓÃ»§ÃÜÂë
		 */
		public String passwd{ get; set; }

		public UserLoginParams(){}
		public UserLoginParams(List<String> firends, String userName, String passwd){
			this.firends = firends;
			this.userName = userName;
			this.passwd = passwd;
		}

		public Dictionary<string, object> ToDictionay() {
			Dictionary<string, object> dic = new Dictionary<string, object>();
			dic["firends"]	=	this.firends;
			dic["userName"]	=	this.userName;
			dic["passwd"]	=	this.passwd;

			return dic;
		}

		public void FromDictionay(Dictionary<string, object> dic) {
			this.firends	=	dic["firends"]	as	List<String>;
			this.userName	=	dic["userName"]	as	String;
			this.passwd	    =	dic["passwd"]	as	String;
		}

		public IFuncParam clone() {
			return new UserLoginParams();
		}
	}

}