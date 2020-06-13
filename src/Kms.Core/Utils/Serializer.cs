using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Kms.Core.Utils
{
    /// <summary>
    /// Serializer
    /// </summary>
    public static class Serializer
    {
        /// <summary>
        /// Object to JSON
        /// </summary>
        /// <param name="entity">object</param>
        /// <returns>JSON String</returns>
        public static string ToJson(object entity)
        {
            return JsonConvert.SerializeObject(entity);
        }

        /// <summary>
        /// Object to Camel case JSON
        /// </summary>
        /// <param name="entity">object</param>
        /// <returns>JSON String</returns>
        public static string ToJsonCamel(object entity)
        {
            var camelCaseFormatter = new JsonSerializerSettings();
            camelCaseFormatter.ContractResolver = new CamelCasePropertyNamesContractResolver();
            return JsonConvert.SerializeObject(entity, camelCaseFormatter);
        }

        /// <summary>
        /// Create object from json string
        /// </summary>
        /// <typeparam name="T">Object Type</typeparam>
        /// <param name="json">Json String</param>
        /// <returns>Object</returns>
        public static T FromJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Generate XML from an object
        /// </summary>
        /// <typeparam name="T">Object's type</typeparam>
        /// <param name="obj">object</param>
        /// <param name="isRemoveNamespace">Remove namespace or not</param>
        /// <returns>XML</returns>
        public static string ToXml<T>(T obj, bool isRemoveNamespace = false)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var stream = new MemoryStream())
            {
                if (isRemoveNamespace)
                {
                    XmlSerializerNamespaces xsNamespaces = new XmlSerializerNamespaces(new XmlQualifiedName[] { new XmlQualifiedName(string.Empty, string.Empty) });
                    serializer.Serialize(stream, obj, xsNamespaces);
                }
                else
                {
                    serializer.Serialize(stream, obj);
                }

                stream.Position = 0;
                var reader = new StreamReader(stream);
                var xml = reader.ReadToEnd();
                reader.Close();
                return xml;
            }
        }

        /// <summary>
        /// Generate a XML file from an object
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="obj">Object</param>
        /// <param name="filePath">Target file path</param>
        /// <param name="isRemoveNamespace">Remove namespace or not</param>
        /// <returns>true(Success)/false(Fail)</returns>
        public static bool ToXmlFile<T>(T obj, string filePath, bool isRemoveNamespace = false)
        {
            bool isSuccess = true;

            XmlSerializer writer = new XmlSerializer(typeof(T));

            using (var file = new StreamWriter(filePath))
            {
                if (isRemoveNamespace)
                {
                    XmlSerializerNamespaces xsNamespaces = new XmlSerializerNamespaces(
                        new XmlQualifiedName[] { new XmlQualifiedName(string.Empty, string.Empty) });
                    writer.Serialize(file, obj, xsNamespaces);
                }
                else
                {
                    writer.Serialize(file, obj);
                }

                file.Close();
            }

            return isSuccess;
        }

        /// <summary>
        /// Create an object from XML String
        /// </summary>
        /// <typeparam name="T">Object's type</typeparam>
        /// <param name="xml">XML</param>
        /// <returns>Object</returns>
        public static T FromXml<T>(string xml)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var stream = new MemoryStream())
            {
                var writer = new StreamWriter(stream);
                writer.Write(xml);
                writer.Flush();
                stream.Position = 0;
                T instance = (T)serializer.Deserialize(stream);
                writer.Close();
                return instance;
            }
        }
    }
}
