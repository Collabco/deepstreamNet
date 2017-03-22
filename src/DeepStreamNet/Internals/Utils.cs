﻿using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace DeepStreamNet
{
    static class Utils
    {
        public static string BuildCommand(Topic topic, Action action, params object[] args)
        {
            var sb = new StringBuilder(4 + (args.Length * 2));

            sb.Append(topic);
            sb.Append(Constants.RecordSeperator);
            sb.Append(action);
            sb.Append(Constants.RecordSeperator);

            for (int i = 0; i < args.Length; i++)
            {
                sb.Append(args[i]);
                sb.Append(Constants.RecordSeperator);
            }

            sb[sb.Length - 1] = Constants.GroupSeperator;

            return sb.ToString();
        }

        public static string ConvertAndPrefixData<T>(T data)
        {
            if (data is string)
            {
                return Constants.Types.STRING + data.ToString();
            }
            else if (data is bool)
            {
                if (bool.Parse(data.ToString()))
                    return Constants.Types.TRUE.ToString();
                else
                    return Constants.Types.FALSE.ToString();
            }
            else if (data is int || data is double || data is float || data is decimal)
            {
                return Constants.Types.NUMBER + data.ToString().Replace(',', '.');
            }
            else if (!(data is object))
            {
                return Constants.Types.NULL.ToString();
            }
            else
            {
                try
                {
                    return Constants.Types.OBJECT + JsonConvert.SerializeObject(data);
                }
                catch
                {
                    return Constants.Types.UNDEFINED.ToString();
                }
            }
        }

        public static KeyValuePair<Type, JToken> ConvertPrefixedData(string dataWithTypePrefix)
        {
            var evtData = dataWithTypePrefix.Substring(1);

            switch (dataWithTypePrefix[0])
            {
                case Constants.Types.STRING:
                    return new KeyValuePair<Type, JToken>(typeof(string), JToken.Parse(evtData));

                case Constants.Types.NUMBER:
                    return new KeyValuePair<Type, JToken>(typeof(double), JToken.Parse(evtData));

                case Constants.Types.TRUE:
                    return new KeyValuePair<Type, JToken>(typeof(bool), JToken.FromObject(true));

                case Constants.Types.FALSE:
                    return new KeyValuePair<Type, JToken>(typeof(bool), JToken.FromObject(false));

                case Constants.Types.NULL:
                    return new KeyValuePair<Type, JToken>(typeof(object), JValue.CreateNull());

                case Constants.Types.OBJECT:
                    return new KeyValuePair<Type, JToken>(typeof(object), JToken.Parse(evtData));

                default:
                    return new KeyValuePair<Type, JToken>(typeof(string), JToken.Parse(evtData));
            }
        }

        public static bool IsNumeric(Type type)
        {
            if (type == null)
                return false;

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                case TypeCode.Object:
                    if (IsGenericTypeEx(type) && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        return IsNumeric(Nullable.GetUnderlyingType(type));
                    }
                    return false;
            }
            return false;
        }

        static bool IsGenericTypeEx(Type type)
        {
#if COREFX
            return type.GetTypeInfo().IsGenericType;
#else
            return type.IsGenericType;
#endif
        }

        public static string CreateUid()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}