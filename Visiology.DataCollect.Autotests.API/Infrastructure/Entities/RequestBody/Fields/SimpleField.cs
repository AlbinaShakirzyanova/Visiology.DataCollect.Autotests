using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Visiology.DataCollect.Integration.Tests.Infrastructure.Entities.Fields
{
    /// <summary>
    /// Класс, описывающий простое поле в теле запроса
    /// </summary>
    public class SimpleField
    {
        /// <summary>
        /// Новое значение для поля
        /// </summary>
        public object value { get; set; }

        /// <summary>
        /// Тип
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public SimpleFieldType type { get; set; }
    }
}
