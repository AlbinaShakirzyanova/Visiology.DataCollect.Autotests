using System.Collections.Generic;
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
    }
}
