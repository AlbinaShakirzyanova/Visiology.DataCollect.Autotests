using Newtonsoft.Json;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Interfaces;

namespace Visiology.DataCollect.Integration.Tests.Models.Dimensions.Attributes
{
    /// <summary>
    /// Описание атрибута измерения
    /// </summary>
    public class AttributeDto : IResponseContent
    {
        /// <summary>
        /// Идентификатор атрибута
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Наименование атрибута
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Флаг, показывающий, что атрибут содержит только уникальные значения
        /// </summary>
        public bool IsUnique { get; set; }

        /// <summary>
        /// Тип атрибута
        /// </summary>
        [JsonIgnore]
        public DimensionAttributeType Type { get; set; }

        /// <summary>
        /// Название типа атрибута
        /// </summary>
        public string TypeName
        {
            get
            {
                return this.Type.ToString();
            }
        }

        /// <summary>
        /// Код типа атрибута
        /// </summary>
        public int TypeCode
        {
            get
            {
                return (int)this.Type;
            }
        }

        public enum DimensionAttributeType
        {
            String = 10,

            Integer = 20,

            Float = 30,

            Link = 40,

            Date = 50
        }
    }
}
