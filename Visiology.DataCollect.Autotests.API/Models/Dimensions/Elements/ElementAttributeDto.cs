namespace Visiology.DataCollect.Integration.Tests.Models.Dimensions.Elements
{
    /// <summary>
    /// Описание атрибута элемента измерения
    /// </summary>
    public class ElementAttributeDto
    {
        /// <summary>
        /// Идентификатор атрибута измерения
        /// </summary>
        public string AttributeId { get; set; }

        /// <summary>
        /// Значение атрибута
        /// </summary>
        public object Value { get; set; }
    }
}
