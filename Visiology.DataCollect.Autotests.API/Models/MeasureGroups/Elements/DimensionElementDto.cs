using System.Collections.Generic;

namespace Visiology.DataCollect.Integration.Tests.Models.MeasureGroups.Elements
{
    /// <summary>
    /// Элемент измерения
    /// </summary>
    public class DimensionElementDto
    {
        /// <summary>
        /// Уникалньое наименование измерения
        /// </summary>
        public string DimensionId { get; set; }

        /// <summary>
        /// Идентификатор элемента
        /// </summary>
        public long ElementId { get; set; }

        public override bool Equals(object obj)
        {
            var dto = obj as DimensionElementDto;
            return dto != null &&
                   DimensionId == dto.DimensionId &&
                   ElementId == dto.ElementId;
        }

        public override int GetHashCode()
        {
            var hashCode = -1485799208;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(DimensionId);
            hashCode = hashCode * -1521134295 + ElementId.GetHashCode();
            return hashCode;
        }
    }
}
