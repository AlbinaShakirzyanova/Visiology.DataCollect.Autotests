using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Visiology.DataCollect.Integration.Tests.Models.MeasureGroups
{
    /// <summary>
    /// Описание компонента группы показателей - измерений, показателя и календаря
    /// </summary>
    public class ApiMeasureGroupComponent : IEquatable<ApiMeasureGroupComponent>
    {
        /// <summary>
        /// Имя компонента
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Идентификатор компонента
        /// </summary>
        public string Id { get; set; }

        public bool Equals(object obj)
        {
            var component = obj as ApiMeasureGroupComponent;
            return component != null &&
                   Name == component.Name &&
                   Id == component.Id;
        }

        public bool Equals([AllowNull] ApiMeasureGroupComponent other)
        {
            var component = other as ApiMeasureGroupComponent;
            return component != null &&
                   Name == component.Name &&
                   Id == component.Id;
        }

        public override int GetHashCode()
        {
            var hashCode = -992789481;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Id);
            return hashCode;
        }
    }
}
