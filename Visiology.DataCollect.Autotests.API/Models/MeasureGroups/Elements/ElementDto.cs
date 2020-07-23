using System.Collections.Generic;
using Visiology.DataCollect.Integration.Tests.Models.MeasureGroups.Elements;

namespace Visiology.DataCollect.Autotests.API.Models.MeasureGroups.Elements
{
    /// <summary>
    /// Элемент группы показателей
    /// </summary>
    public class ElementDto : ElementBaseDto
    {
        /// <summary>
        /// Координаты элемента по измерения
        /// </summary>
        public List<DimensionElementDto> DimensionElements { get; set; }
            = new List<DimensionElementDto>();

        /// <summary>
        /// Координаты элемента по показателями
        /// </summary>
        public List<MeasureElementDto> MeasureElements { get; set; }
            = new List<MeasureElementDto>();

        /// <summary>
        /// Координата по дате
        /// </summary>
        public CalendarDto Calendar { get; set; }

        /// <summary>
        /// Значения атрибутов элемента группы показателей
        /// </summary>
        public List<MeasureGroupElementAttributeDto> Attributes { get; set; }
            = new List<MeasureGroupElementAttributeDto>();

        public override bool Equals(object obj)
        {
            var dto = obj as ElementDto;
            return dto != null &&
                   EqualityComparer<List<DimensionElementDto>>.Default.Equals(DimensionElements, dto.DimensionElements) &&
                   EqualityComparer<List<MeasureElementDto>>.Default.Equals(MeasureElements, dto.MeasureElements) &&
                   EqualityComparer<CalendarDto>.Default.Equals(Calendar, dto.Calendar);
        }

        public override int GetHashCode()
        {
            var hashCode = -2007856361;
            hashCode = hashCode * -1521134295 + EqualityComparer<List<DimensionElementDto>>.Default.GetHashCode(DimensionElements);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<MeasureElementDto>>.Default.GetHashCode(MeasureElements);
            hashCode = hashCode * -1521134295 + EqualityComparer<CalendarDto>.Default.GetHashCode(Calendar);
            return hashCode;
        }
    }
}
