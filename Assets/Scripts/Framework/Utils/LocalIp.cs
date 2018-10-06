using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Net.NetworkInformation;


namespace AW.IO {
	public class LocalIp {

		private static IPAddress mLocalAddress;
		public static IPAddress localAddress {
			get {

				if (mLocalAddress == null) {

					#if UNITY_EDITOR

					mLocalAddress = GetAllUnicastAddresses_New();

					#else

					#if UNITY_IPHONE

					NetworkInterface[] nis = NetworkInterface.GetAllNetworkInterfaces();

					foreach (NetworkInterface ni in nis) {

						IPInterfaceProperties IPInterfaceProperties = ni.GetIPProperties();
						UnicastIPAddressInformationCollection UnicastIPAddressInformationCollection = IPInterfaceProperties.UnicastAddresses;

						foreach (UnicastIPAddressInformation UnicastIPAddressInformation in UnicastIPAddressInformationCollection) {

							if (UnicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork) {

								mLocalAddress = UnicastIPAddressInformation.Address;
								break;

							}

						}

					}

					#else

					mLocalAddress = IPAddress.Loopback;

					try {
						IPHostEntry ent = Dns.GetHostEntry(Dns.GetHostName());

						foreach (IPAddress ip in ent.AddressList) {
							if (ip.AddressFamily == AddressFamily.InterNetwork) {
								mLocalAddress = ip;
								break;
							}
						}
					} catch (System.Exception ex) {
						ConsoleEx.DebugLog("LocalAddress: " + ex.Message);
					}

					#endif


					#endif


				}

				return mLocalAddress;
			}

		}

		public static IPAddress GetAllUnicastAddresses_New() {

            // This works on both Mono and .NET , but there is a difference: it also
			// includes the LocalLoopBack so we need to filter that one out
			IPAddress localAddress = null;
			// Obtain a reference to all network interfaces in the machine
			NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
			foreach (NetworkInterface adapter in adapters)
			{
				IPInterfaceProperties properties = adapter.GetIPProperties();
			    foreach (IPAddressInformation uniCast in properties.UnicastAddresses)
		        {
		            // Ignore loop-back addresses & IPv6
					if (!IPAddress.IsLoopback(uniCast.Address) && uniCast.Address.AddressFamily!= AddressFamily.InterNetworkV6)
					if(isValidateLocalIp(uniCast.Address)) {
						localAddress = uniCast.Address;
						break;
					}
						
		        }
			     
			}
			return localAddress;
        }

		/// <summary>
		/// 开发使用，错误的判定逻辑
		/// 真实的有效网络地址是不确定的
		/// </summary>
		/// <returns><c>true</c>, if validate local ip was ised, <c>false</c> otherwise.</returns>
		/// <param name="addr">Address.</param>
		static bool isValidateLocalIp (IPAddress addr) {
			if(addr.ToString().StartsWith("192.168"))
				return true;
			else 
				return false;
		}


	}
}