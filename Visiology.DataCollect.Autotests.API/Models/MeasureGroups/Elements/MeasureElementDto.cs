using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Visiology.DataCollect.Integration.Tests.Models.MeasureGroups.Elements
{
    /// <summary>
    /// Элемент показателя
    /// </summary>
    public class MeasureElementDto : IEquatable<MeasureElementDto>
    {
        /// <summary>
        /// Уникальное наименование показателя
        /// </summary>
        public string MeasureId { get; set; }

        /// <summary>
        /// Идентификатор элемента
        /// </summary>
        public long ElementId { get; set; }

        public override bool Equals(object obj)
        {
            var element = obj as MeasureElementDto;
            return element != null &&
                   MeasureId == element.MeasureId &&
                   ElementId == element.ElementId;
        }

        public bool Equals([AllowNull] MeasureElementDto other)
        {
            var element = other as MeasureElementDto;
            return element != null &&
                   MeasureId == element.MeasureId &&
                   ElementId == element.ElementId;
        }

        public override int GetHashCode()
        {
            var hashCode = 1757159580;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MeasureId);
            hashCode = hashCode * -1521134295 + ElementId.GetHashCode();
            return hashCode;
        }
    }
}
