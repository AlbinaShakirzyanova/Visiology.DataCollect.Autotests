using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Entities.Fields;

namespace Visiology.DataCollect.Integration.Tests.Infrastructure.Entities.RequestBody.Fields
{
    /// <summary>
    /// Класс, описывающий именованное поле в теле запроса
    /// </summary>
    public class NamedField
    {
        /// <summary>
        /// Новое значение для поля
        /// </summary>
        public object value { get; set; }

        /// <summary>
        /// Тип
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public NamedFieldType type { get; set; }

        /// <summary>
        /// Наименование поля
        /// </summary>
        public string name { get; set; }
    }
}
