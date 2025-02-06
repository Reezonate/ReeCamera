using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using UnityEngine;

namespace ReeCamera {
    public static class ConfigUtils {
        #region Constants

        public static class Categories {
            public const string Hidden = "Hidden";
            public const string Motion = "Motion";
            public const string Effects = "Effects";
            public const string Others = "Others";
        }

        public static class Authors {
            public const string Ree = "Reezonate";
        }

        #endregion

        #region Converters

        public class ReeTransformEulerConverter : JsonConverter<ObservableValue<ReeTransform>> {
            private struct Temp {
                public Vector3 position;
                public Vector3 rotationEuler;
            }

            public override void WriteJson(JsonWriter writer, ObservableValue<ReeTransform> value, JsonSerializer serializer) {
                serializer.Serialize(writer, new Temp {
                    position = value.Value.Position,
                    rotationEuler = value.Value.Rotation.eulerAngles
                });
            }

            public override ObservableValue<ReeTransform> ReadJson(
                JsonReader reader,
                Type objectType,
                ObservableValue<ReeTransform> existingValue,
                bool hasExistingValue,
                JsonSerializer serializer
            ) {
                var temp = serializer.Deserialize<Temp>(reader);
                var value = new ReeTransform(temp.position, Quaternion.Euler(temp.rotationEuler));
                if (!hasExistingValue) return new ObservableValue<ReeTransform>(value);
                existingValue.SetValue(value, existingValue);
                return existingValue;
            }
        }

        #endregion

        #region DefaultSerializer

        public static readonly JsonSerializer DefaultSerializer = new JsonSerializer() {
            MissingMemberHandling = MissingMemberHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new ConfigSerializerContractResolver(),
            Formatting = Formatting.Indented
        };

        private class ConfigSerializerContractResolver : DefaultContractResolver {
            #region GetSerializableMembers

            protected override List<MemberInfo> GetSerializableMembers(Type objectType) {
                var members = base.GetSerializableMembers(objectType);

                for (var i = members.Count - 1; i > 0; i--) {
                    if (ShouldSerialize(objectType, members[i])) continue;
                    members.RemoveAt(i);
                }

                return members;
            }

            #endregion

            #region ShouldSerialize

            private static readonly Dictionary<Type, Func<MemberInfo, bool>> TypeHandlers = new Dictionary<Type, Func<MemberInfo, bool>>() {
                { typeof(Vector2), XYZWOnly },
                { typeof(Vector3), XYZWOnly },
                { typeof(Vector4), XYZWOnly },
                { typeof(Quaternion), XYZWOnly },
                { typeof(Color), RGBAOnly },
                { typeof(Rect), Rect }
            };

            private static bool ShouldSerialize(Type objectType, MemberInfo memberInfo) {
                return !TypeHandlers.ContainsKey(objectType) || TypeHandlers[objectType](memberInfo);
            }

            private static bool RGBAOnly(MemberInfo memberInfo) {
                switch (memberInfo.Name) {
                    case "r":
                    case "g":
                    case "b":
                    case "a": return true;
                    default: return false;
                }
            }

            private static bool XYZWOnly(MemberInfo memberInfo) {
                switch (memberInfo.Name) {
                    case "x":
                    case "y":
                    case "z":
                    case "w": return true;
                    default: return false;
                }
            }

            private static bool Rect(MemberInfo memberInfo) {
                switch (memberInfo.Name) {
                    case "x":
                    case "y":
                    case "width":
                    case "height": return true;
                    default: return false;
                }
            }

            #endregion
        }

        #endregion

        #region Extensions

        public static JObject SerializeObject(object obj) {
            return JObject.FromObject(obj, DefaultSerializer);
        }

        public static T DeserializeOrDefault<T>(this JObject source, string key, Func<T> defaultObject) {
            var token = source.GetValue(key);
            if (token != null) return token.ToObject<T>(DefaultSerializer);
            Plugin.Error($"Unable to deserialize {typeof(T).FullName}!");
            return defaultObject();
        }

        #endregion

        #region FileSystem

        public static void EnsureDirectoryExists(string path) {
            var directory = new FileInfo(path).Directory ?? new DirectoryInfo(path);
            if (!directory.Exists) directory.Create();
        }

        #endregion
    }
}