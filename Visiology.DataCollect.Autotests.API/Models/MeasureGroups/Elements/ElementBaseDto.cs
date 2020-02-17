using Visiology.DataCollect.Integration.Tests.Infrastructure.Interfaces;

namespace Visiology.DataCollect.Integration.Tests.Models.MeasureGroups.Elements
{
    /// <summary>
    /// Элемент группы показателей
    /// </summary>
    public abstract class ElementBaseDto : IResponseContent
    {
        /// <summary>
        /// Идентификатор элемента
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Значение элемента
        /// </summary>
        public object Value { get; set; }
    }
}
