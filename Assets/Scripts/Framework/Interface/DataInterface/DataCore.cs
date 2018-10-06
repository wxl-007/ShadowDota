using System;
using System.Collections;
using System.Collections.Generic;
using AW.Framework;

/// <summary>
/// Model层的数据基本都保存在这里
/// </summary>
namespace AW.Data {

	/// 
	/// 所有的数据分为三种方式：
	/// 0. 服务器端传递过来的数据
	/// 1. 直接从配置表里面读取
	/// 2. 两者的合体
	/// 大部分的情况下都应该是第3种情况
	/// 

	public class DataCore {

		/// 
		/// 分析整个Assembly，找到所有要反射生成的Type
		/// 
		private ImplicitBinder binder;

		/// <summary>
		/// 所有只实现IModelNetwork都在这里
		/// </summary>
		private Dictionary<Type, IModelNetwork> modelNet = null;

		/// <summary>
		/// 所有只继承自ModelBase都在这里
		/// </summary>
		private Dictionary<Type, ModelBase> modelCfg = null;

		/// <summary>
		/// 所有既实现IModelNetwork有继承自ModelBase，在这里
		/// </summary>
		private Dictionary<Type, ModelBase> modelBoth = null;

		public DataCore () {
			binder   = ImplicitBinder.Instance;
			modelNet = new Dictionary<Type, IModelNetwork>();
			modelCfg = new Dictionary<Type, ModelBase>();
			modelBoth= new Dictionary<Type, ModelBase>();

			InitNetLayer();
			InitConfigLayer();
			InitBothLayer();
		}

		/// 
		/// 根据type反射创建只接收网络层数据的对象
		/// 
		void InitNetLayer() {
			int length = binder.IIModelNetwork.Count;
			if(length > 0) {
				for(int i = 0; i < length; ++ i) {
					Type type = binder.IIModelNetwork[i];
					IModelNetwork obj = Activator.CreateInstance(type) as IModelNetwork;
					modelNet.Add(type, obj);
				}

			}
		}

		/// 
		/// 根据type反射创建只读取本地配表数据的对象
		///
		void InitConfigLayer() {
			int length = binder.IIModelConfig.Count;
			if(length > 0) {
				for(int i = 0; i < length; ++ i) {
					Type type = binder.IIModelConfig[i];
					ModelBase obj = Activator.CreateInstance(type) as ModelBase;
					modelCfg.Add(type, obj);
				}
			}
		}

		/// <summary>
		///  根据type反射 所有既实现IModelNetwork有继承自ModelBase的对象
		/// </summary>
		void InitBothLayer() {
			int length = binder.IIModelBoth.Count;
			if(length > 0) {
				for(int i = 0; i < length; ++ i) {
					Type type = binder.IIModelBoth[i];
					ModelBase obj = Activator.CreateInstance(type) as ModelBase;
					modelBoth.Add(type, obj);
				}
			}
		}


		/// <summary>
		/// 返回IModelNetwork数据层
		/// </summary>
		public T getIModelNetwork<T>() where T : IModelNetwork {
			T model = default(T);
			Type type = typeof(T);
			if(modelNet.ContainsKey(type))
				model = (T)modelNet[type];
			return model;
		}

		/// <summary>
		/// 返回ModelBase数据层
		/// </summary>
		public T getIModelConfig<T>() where T: ModelBase {
			T model = default(T);
			Type type = typeof(T);
			if(modelCfg.ContainsKey(type))
				model = (T)modelCfg[type];
			return model;
		}

		/// <summary>
		/// 返回同时满足IModelNetwork和ModelBase的数据层
		/// </summary>
		public T getIModelBoth<T>() where T : ModelBase {
			T model = default(T);
			Type type = typeof(T);
			if(modelBoth.ContainsKey(type))
				model = (T)modelBoth[type];
			return model;
		}

		/// <summary>
		/// EventCenter受到消息后，向数据层分发的统一入口在这里
		/// </summary>
		/// <param name="request">Request.</param>
		public void fullfillByNetwork(HttpTask task) {
			HttpRequest req = task.request as HttpRequest;
			RequestType reqType = req.Type;

			foreach(IModelNetwork netmodel in modelNet.Values) {
				bool isFavoriate = Utils.inArray<RequestType>(reqType, netmodel.getFavoriteRequest());
				if(isFavoriate) {
					try {
						netmodel.fullfillByNetwork(task.request, task.response);
					} catch(System.Exception ex) {
						ConsoleEx.DebugLog(ex.ToString(), ConsoleEx.RED);
					}
				}
			}
		}

		/// <summary>
		/// 读取本地数据, 所有读取本地数据的统一入口
		/// </summary>
		public void readLocalConfig() {
			try {
				foreach(ModelBase model in modelCfg.Values) {
					model.loadFromConfig();
				}

				foreach(ModelBase model in modelBoth.Values) {
					model.loadFromConfig();
				}
			} catch(System.Exception ex) {
				ConsoleEx.DebugLog(ex.ToString(), ConsoleEx.RED);
			}

		}

	}
}
