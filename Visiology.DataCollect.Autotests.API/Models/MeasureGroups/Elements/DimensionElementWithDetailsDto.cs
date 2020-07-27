using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Visiology.DataCollect.Integration.Tests.Models.MeasureGroups.Elements;

namespace Visiology.DataCollect.Autotests.API.Models.MeasureGroups.Elements
{
    /// <summary>
    /// Элемент измерения
    /// </summary>
    public class DimensionElementWithDetailsDto : IEquatable<DimensionElementWithDetailsDto>
    {
        /// <summary>
        /// Уникалньое наименование измерения
        /// </summary>
        public string DimensionId { get; set; }

        /// <summary>
        /// Идентификатор элемента
        /// </summary>
        public long ElementId { get; set; }

        /// <summary>
        /// Наименование измерения
        /// </summary>
        public string DimensionName { get; set; }

        /// <summary>
        /// Наименование элемента
        /// </summary>
        public string ElementName { get; set; }

        public override bool Equals(object obj)
        {
            var dto = obj as DimensionElementWithDetailsDto;
            return dto != null &&
                   DimensionId == dto.DimensionId &&
                   ElementId == dto.ElementId &&
                   DimensionName == dto.DimensionName &&
                   ElementName == dto.ElementName;
        }

        public bool Equals([AllowNull] DimensionElementWithDetailsDto other)
        {
            var dto = other as DimensionElementWithDetailsDto;
            return dto != null &&
                   DimensionId == dto.DimensionId &&
                   ElementId == dto.ElementId &&
                   DimensionName == dto.DimensionName &&
                   ElementName == dto.ElementName;
        }

        public override int GetHashCode()
        {
            var hashCode = -1485799208;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(DimensionId);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(DimensionName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ElementName);
            hashCode = hashCode * -1521134295 + ElementId.GetHashCode();
            return hashCode;
        }
    }
}
