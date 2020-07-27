using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Visiology.DataCollect.Autotests.API.Models.MeasureGroups.Elements
{
    /// <summary>
    /// Элемент показателя
    /// </summary>
    public class MeasureElementWithDetailsDto : IEquatable<MeasureElementWithDetailsDto>
    {
        /// <summary>
        /// Уникальное наименование показателя
        /// </summary>
        public string MeasureId { get; set; }

        /// <summary>
        /// Наименование показателя
        /// </summary>
        public string MeasureName { get; set; }

        /// <summary>
        /// Идентификатор элемента
        /// </summary>
        public long ElementId { get; set; }

        /// <summary>
        /// Наименование элемента
        /// </summary>
        public string ElementName { get; set; }

        public override bool Equals(object obj)
        {
            var element = obj as MeasureElementWithDetailsDto;
            return element != null &&
                   MeasureId == element.MeasureId &&
                   ElementId == element.ElementId &&
                   MeasureName == element.MeasureName &&
                   ElementName == element.ElementName;
        }

        public bool Equals([AllowNull] MeasureElementWithDetailsDto other)
        {
            var element = other as MeasureElementWithDetailsDto;
            return element != null &&
                   MeasureId == element.MeasureId &&
                   ElementId == element.ElementId &&
                   MeasureName == element.MeasureName &&
                   ElementName == element.ElementName;
        }

        public override int GetHashCode()
        {
            var hashCode = 1757159580;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MeasureId);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MeasureName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ElementName);
            hashCode = hashCode * -1521134295 + ElementId.GetHashCode();
            return hashCode;
        }
    }
}
