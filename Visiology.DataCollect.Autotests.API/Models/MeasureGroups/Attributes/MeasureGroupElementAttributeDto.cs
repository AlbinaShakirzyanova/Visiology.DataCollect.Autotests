using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Visiology.DataCollect.Autotests.API.Models.MeasureGroups.Elements
{
    public class MeasureGroupElementAttributeDto : IEquatable<MeasureGroupElementAttributeDto>
    {
        public string AttributeId { get; set; }

        public object Value { get; set; }

        public string? DisplayValue { get; set; }

        public override bool Equals(object obj)
        {
            var attribute = obj as MeasureGroupElementAttributeDto;
            return attribute != null &&
                   AttributeId == attribute.AttributeId &&
                   Value == attribute.Value &&
                   DisplayValue == attribute.DisplayValue;
        }

        public bool Equals([AllowNull] MeasureGroupElementAttributeDto other)
        {
            var attribute = other as MeasureGroupElementAttributeDto;
            return attribute != null &&
                   AttributeId == attribute.AttributeId &&
                   Value == attribute.Value &&
                   DisplayValue == attribute.DisplayValue; ;
        }

        public override int GetHashCode()
        {
            var hashCode = 1757159580;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(AttributeId);
            hashCode = hashCode * -1521134295 + Value.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(DisplayValue);
            return hashCode;
        }
    }   
}
