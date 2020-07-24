using System.Collections.Generic;
using System.Linq;
using Visiology.DataCollect.Autotests.API.Models.MeasureGroups.Attributes;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Interfaces;

namespace Visiology.DataCollect.Integration.Tests.Models.MeasureGroups
{
    /// <summary>
    /// Описание группы показателей
    /// </summary>
    public class MeasureGroupDto : IResponseContent
    {
        /// <summary>
        /// Имя группы показателей
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Идентификатор группы показателей
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Присоединенные измерения группы показателей
        /// </summary>
        public IList<ApiMeasureGroupComponent> Dimensions { get; set; }

        /// <summary>
        /// Показатель группы показателей
        /// </summary>
        public ApiMeasureGroupComponent Measure { get; set; }

        /// <summary>
        /// Календарь группы показателей
        /// </summary>
        public ApiMeasureGroupComponent Calendar { get; set; }

        public IList<MeasureGroupAttributeDto> Attributes { get; set; }

        public override bool Equals(object obj)
        {
            var dto = obj as MeasureGroupDto;
            return dto != null &&
                   Dimensions.SequenceEqual(dto.Dimensions) &&
                   Measure.Equals(dto.Measure) &&
                   Calendar.Equals(dto.Calendar) &&
                   Attributes.SequenceEqual(dto.Attributes) &&
                   Name == dto.Name &&
                   Id == dto.Id;
        }

        public override int GetHashCode()
        {
            var hashCode = -2007856361;
            hashCode = hashCode * -1521134295 + EqualityComparer<IList<ApiMeasureGroupComponent>>.Default.GetHashCode(Dimensions);
            hashCode = hashCode * -1521134295 + EqualityComparer<ApiMeasureGroupComponent>.Default.GetHashCode(Measure);
            hashCode = hashCode * -1521134295 + EqualityComparer<ApiMeasureGroupComponent>.Default.GetHashCode(Calendar);
            hashCode = hashCode * -1521134295 + EqualityComparer<IList<MeasureGroupAttributeDto>>.Default.GetHashCode(Attributes);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Id);
            return hashCode;
        }
    }
}
