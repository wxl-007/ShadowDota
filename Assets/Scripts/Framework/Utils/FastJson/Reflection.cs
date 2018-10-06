using System;
using System.Collections.Generic;
using System.Text;
#if !AOT0 && !AOT1 && !AOT2
using System.Reflection.Emit;
#endif
#if AOT1
using System.Runtime.Serialization;
#endif
#if AOT2
using System.Linq.Expressions;
#endif


using System.Reflection;
using System.Collections;

namespace fastJSON
{
    internal struct Getters
    {
        public string Name;
        public Reflection.GenericGetter Getter;
        //public Type propertyType;
    }

    internal sealed class Reflection
    {
        public readonly static Reflection Instance = new Reflection();
        private Reflection()
        {
        }

        internal delegate object GenericSetter(object target, object value);
        internal delegate object GenericGetter(object obj);
        private delegate object CreateObject();
		private delegate object ObjectActivator(params object[] args);

        private SafeDictionary<Type, string> _tyname = new SafeDictionary<Type, string>();
        private SafeDictionary<string, Type> _typecache = new SafeDictionary<string, Type>();
		#if !AOT0 && !AOT1 && !AOT2
        private SafeDictionary<Type, CreateObject> _constrcache = new SafeDictionary<Type, CreateObject>();
		#elif AOT1
		private SafeDictionary<Type, ConstructorInfo> _constrcache = new SafeDictionary<Type, ConstructorInfo>();
		#elif AOT2
		private SafeDictionary<Type, ObjectActivator> _constrcache = new SafeDictionary<Type, ObjectActivator>();
		#endif
        private SafeDictionary<Type, Getters[]> _getterscache = new SafeDictionary<Type, Getters[]>();

        #region [   PROPERTY GET SET   ]
        internal string GetTypeAssemblyName(Type t)
        {
            string val = "";
            if (_tyname.TryGetValue(t, out val))
                return val;
            else
            {
                string s = t.AssemblyQualifiedName;
                _tyname.Add(t, s);
                return s;
            }
        }

        internal Type GetTypeFromCache(string typename)
        {
            Type val = null;
            if (_typecache.TryGetValue(typename, out val))
                return val;
            else
            {
                Type t = Type.GetType(typename);
                //if (t == null) // RaptorDB : loading runtime assemblies
                //{
                //    t = Type.GetType(typename, (name) => {
                //        return AppDomain.CurrentDomain.GetAssemblies().Where(z => z.FullName == name.FullName).FirstOrDefault();
                //    }, null, true);
                //}
                _typecache.Add(typename, t);
                return t;
            }
        }

	#if AOT0
		internal object FastCreateInstance(Type objtype)
		{
			return Activator.CreateInstance(objtype);
		}

	#elif AOT1
		internal object FastCreateInstance(Type objtype)
		{
			ConstructorInfo ctor = null;
			if(_constrcache.TryGetValue(objtype, out ctor)) {
				return ctor.Invoke(new object[]{});
			} else {
				ctor = objtype.GetConstructor(Type.EmptyTypes);
				_constrcache.Add(objtype, ctor);
				return ctor.Invoke(new object[]{});
			}
		}

	#elif AOT2

		private static ObjectActivator GetActivator(Type objtype) {
			ConstructorInfo ctor = objtype.GetConstructor(Type.EmptyTypes);
			Type type = ctor.DeclaringType;
			ParameterInfo[] paramsInfo = ctor.GetParameters();                  

			//create a single param of type object[]
			ParameterExpression param =
				Expression.Parameter(typeof(object[]), "args");

			Expression[] argsExp =
				new Expression[paramsInfo.Length];            

			//pick each arg from the params array 
			//and create a typed expression of them
			for (int i = 0; i < paramsInfo.Length; i++)
			{
				Expression index = Expression.Constant(i);
				Type paramType = paramsInfo[i].ParameterType;              

				Expression paramAccessorExp =
					Expression.ArrayIndex(param, index);              

				Expression paramCastExp =
					Expression.Convert (paramAccessorExp, paramType);              

				argsExp[i] = paramCastExp;
			}                  

			//make a NewExpression that calls the
			//ctor with the args we just created
			NewExpression newExp = Expression.New(ctor,argsExp);                  

			//create a lambda with the New
			//Expression as body and our param object[] as arg
			LambdaExpression lambda =
				Expression.Lambda(typeof(ObjectActivator), newExp, param);           

			//compile it
			ObjectActivator compiled = (ObjectActivator)lambda.Compile();
			return compiled;
		}

		internal object FastCreateInstance(Type objtype) {
			ObjectActivator aotObj = null;
			if( !_constrcache.TryGetValue(objtype, out aotObj)) {
				aotObj = GetActivator(objtype);
				_constrcache.Add(objtype, aotObj);
			}
			return aotObj(new object[]{});
		}

	#else

        internal object FastCreateInstance(Type objtype)
        {
            try
            {
                CreateObject c = null;
                if (_constrcache.TryGetValue(objtype, out c))
                {
                    return c();
                }
                else
                {
                    if (objtype.IsClass)
                    {
                        DynamicMethod dynMethod = new DynamicMethod("_", objtype, null);
                        ILGenerator ilGen = dynMethod.GetILGenerator();
                        ilGen.Emit(OpCodes.Newobj, objtype.GetConstructor(Type.EmptyTypes));
                        ilGen.Emit(OpCodes.Ret);
                        c = (CreateObject)dynMethod.CreateDelegate(typeof(CreateObject));
                        _constrcache.Add(objtype, c);
                    }
                    else // structs
                    {
                        DynamicMethod dynMethod = new DynamicMethod("_", typeof(object), null);
                        ILGenerator ilGen = dynMethod.GetILGenerator();
                        var lv = ilGen.DeclareLocal(objtype);
                        ilGen.Emit(OpCodes.Ldloca_S, lv);
                        ilGen.Emit(OpCodes.Initobj, objtype);
                        ilGen.Emit(OpCodes.Ldloc_0);
                        ilGen.Emit(OpCodes.Box, objtype);
                        ilGen.Emit(OpCodes.Ret);
                        c = (CreateObject)dynMethod.CreateDelegate(typeof(CreateObject));
                        _constrcache.Add(objtype, c);
                    }
                    return c();
                }
            }
            catch (Exception exc)
            {
                throw new Exception(string.Format("Failed to fast create instance for type '{0}' from assembly '{1}'",
                    objtype.FullName, objtype.AssemblyQualifiedName), exc);
            }
        }
	#endif

	#if AOT0 || AOT1 || AOT2
		internal static GenericSetter CreateSetField(Type type, FieldInfo fieldInfo)
		{
			return new GenericSetter((target, value) => {fieldInfo.SetValue(target,value); return target;});
		}
	#else

        internal static GenericSetter CreateSetField(Type type, FieldInfo fieldInfo)
        {
            Type[] arguments = new Type[2];
            arguments[0] = arguments[1] = typeof(object);

            DynamicMethod dynamicSet = new DynamicMethod("_", typeof(object), arguments, type);

            ILGenerator il = dynamicSet.GetILGenerator();

            if (!type.IsClass) // structs
            {
                var lv = il.DeclareLocal(type);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Unbox_Any, type);
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldloca_S, lv);
                il.Emit(OpCodes.Ldarg_1);
                if (fieldInfo.FieldType.IsClass)
                    il.Emit(OpCodes.Castclass, fieldInfo.FieldType);
                else
                    il.Emit(OpCodes.Unbox_Any, fieldInfo.FieldType);
                il.Emit(OpCodes.Stfld, fieldInfo);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Box, type);
                il.Emit(OpCodes.Ret);
            }
            else
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                if (fieldInfo.FieldType.IsValueType)
                    il.Emit(OpCodes.Unbox_Any, fieldInfo.FieldType);
                il.Emit(OpCodes.Stfld, fieldInfo);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ret);
            }
            return (GenericSetter)dynamicSet.CreateDelegate(typeof(GenericSetter));
        }
	#endif

	#if AOT0 || AOT1 || AOT2
		internal static GenericSetter CreateSetMethod(Type type, PropertyInfo propertyInfo)
		{
			return new GenericSetter((target, value) => {propertyInfo.SetValue(target,value, null); return target;});
		}
	#else
        internal static GenericSetter CreateSetMethod(Type type, PropertyInfo propertyInfo)
        {
            MethodInfo setMethod = propertyInfo.GetSetMethod();
            if (setMethod == null)
                return null;

            Type[] arguments = new Type[2];
            arguments[0] = arguments[1] = typeof(object);

            DynamicMethod setter = new DynamicMethod("_", typeof(object), arguments);
            ILGenerator il = setter.GetILGenerator();

            if (!type.IsClass) // structs
            {
                var lv = il.DeclareLocal(type);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Unbox_Any, type);
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldloca_S, lv);
                il.Emit(OpCodes.Ldarg_1);
                if (propertyInfo.PropertyType.IsClass)
                    il.Emit(OpCodes.Castclass, propertyInfo.PropertyType);
                else
                    il.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
                il.EmitCall(OpCodes.Call, setMethod, null);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Box, type);
            }
            else
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Castclass, propertyInfo.DeclaringType);
                il.Emit(OpCodes.Ldarg_1);
                if (propertyInfo.PropertyType.IsClass)
                    il.Emit(OpCodes.Castclass, propertyInfo.PropertyType);
                else
                    il.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
                il.EmitCall(OpCodes.Callvirt, setMethod, null);
                il.Emit(OpCodes.Ldarg_0);
            }

            il.Emit(OpCodes.Ret);

            return (GenericSetter)setter.CreateDelegate(typeof(GenericSetter));
        }
	#endif

	#if AOT0 || AOT1 || AOT2
		internal static GenericGetter CreateGetField(Type type, FieldInfo fieldInfo)
		{
			return new GenericGetter((target) => {return fieldInfo.GetValue(target); });
		}
	#else
        internal static GenericGetter CreateGetField(Type type, FieldInfo fieldInfo)
        {
            DynamicMethod dynamicGet = new DynamicMethod("_", typeof(object), new Type[] { typeof(object) }, type);

            ILGenerator il = dynamicGet.GetILGenerator();

            if (!type.IsClass) // structs
            {
                var lv = il.DeclareLocal(type);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Unbox_Any, type);
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldloca_S, lv);
                il.Emit(OpCodes.Ldfld, fieldInfo);
                if (fieldInfo.FieldType.IsValueType)
                    il.Emit(OpCodes.Box, fieldInfo.FieldType);
            }
            else
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, fieldInfo);
                if (fieldInfo.FieldType.IsValueType)
                    il.Emit(OpCodes.Box, fieldInfo.FieldType);
            }

            il.Emit(OpCodes.Ret);

            return (GenericGetter)dynamicGet.CreateDelegate(typeof(GenericGetter));
        }
	#endif


	#if AOT0 || AOT1 || AOT2
		internal static GenericGetter CreateGetMethod(Type type, PropertyInfo propertyInfo)
		{
			return new GenericGetter((target) => {return propertyInfo.GetValue(target, null); });
		}
	#else
        internal static GenericGetter CreateGetMethod(Type type, PropertyInfo propertyInfo)
        {
            MethodInfo getMethod = propertyInfo.GetGetMethod();
            if (getMethod == null)
                return null;

            DynamicMethod getter = new DynamicMethod("_", typeof(object), new Type[] { typeof(object) }, type);

            ILGenerator il = getter.GetILGenerator();

            if (!type.IsClass) // structs
            {
                var lv = il.DeclareLocal(type);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Unbox_Any, type);
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldloca_S, lv);
                il.EmitCall(OpCodes.Call, getMethod, null);
                if (propertyInfo.PropertyType.IsValueType)
                    il.Emit(OpCodes.Box, propertyInfo.PropertyType);
            }
            else
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Castclass, propertyInfo.DeclaringType);
                il.EmitCall(OpCodes.Callvirt, getMethod, null);
                if (propertyInfo.PropertyType.IsValueType)
                    il.Emit(OpCodes.Box, propertyInfo.PropertyType);
            }

            il.Emit(OpCodes.Ret);

            return (GenericGetter)getter.CreateDelegate(typeof(GenericGetter));
        }

	#endif

        internal Getters[] GetGetters(Type type, bool showreadonly)
        {
            Getters[] val = null;
            if (_getterscache.TryGetValue(type, out val))
                return val;

            PropertyInfo[] props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            List<Getters> getters = new List<Getters>();
            foreach (PropertyInfo p in props)
            {
                if (!p.CanWrite && showreadonly == false) continue;

				#if !SILVERLIGHT && USE_XML
                object[] att = p.GetCustomAttributes(typeof(System.Xml.	Serialization.XmlIgnoreAttribute), false);
                if (att != null && att.Length > 0)
                    continue;
				#endif
                GenericGetter g = CreateGetMethod(type, p);
                if (g != null)
                {
                    Getters gg = new Getters();
                    gg.Name = p.Name;
                    gg.Getter = g;
                    //gg.propertyType = p.PropertyType;
                    getters.Add(gg);
                }
            }

            FieldInfo[] fi = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);
            foreach (var f in fi)
            {
				#if !SILVERLIGHT && USE_XML
                object[] att = f.GetCustomAttributes(typeof(System.Xml.Serialization.XmlIgnoreAttribute), false);
                if (att != null && att.Length > 0)
                    continue;
				#endif
                GenericGetter g = CreateGetField(type, f);
                if (g != null)
                {
                    Getters gg = new Getters();
                    gg.Name = f.Name;
                    gg.Getter = g;
                    //gg.propertyType = f.FieldType;
                    getters.Add(gg);
                }
            }
            val = getters.ToArray();
            _getterscache.Add(type, val);
            return val;
        }

        #endregion
    }
}
