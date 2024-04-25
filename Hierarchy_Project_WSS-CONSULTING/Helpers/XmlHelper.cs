using System.Xml.Serialization;

namespace Hierarchy_Project_WSS_CONSULTING.Helpers
{
    public static class XmlHelper
    {
        //Generic вместо прямого типа, вдруг будем переиспользовать
        public static async Task<string> SerializeAsync<T>(T obj)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StringWriter sw = new StringWriter())
            {
                serializer.Serialize(sw, obj);
                return await Task.FromResult(sw.ToString());
            }
        }

        public static async Task<T> DeserializeAsync<T>(string xml)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (StringReader sr = new StringReader(xml))
                {
                    return (T)await Task.Run(() => serializer.Deserialize(sr));
                }
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine("Ошибка десериализации: " + ex.Message);
                throw;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Неверный аргумент: " + ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка: " + ex.Message);
                throw;
            }
        }
    }
}
