using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Visiology.DataCollect.Autotests.API.Infrastructure.Impl;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Interfaces;

namespace Visiology.DataCollect.Autotests.API.Models.MeasureGroups.Attributes
{
    [JsonConverter(typeof(MeasureGroupAttributeConverter))]
    public abstract class MeasureGroupAttributeDto : IResponseContent, IEquatable<MeasureGroupAttributeDto>
    {
        public string Id { get; set; }

        public string Name { get; set;  }

        /// <summary>
        /// Название типа атрибута
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Код типа атрибута
        /// </summary>
        public int TypeCode { get; set; }

        public override bool Equals(object obj)
        {
            var attribute = obj as MeasureGroupAttributeDto;
            return attribute != null && Name == attribute.Name &&
                   Id == attribute.Id &&
                   TypeName == attribute.TypeName &&
                   TypeCode == attribute.TypeCode;
        }

        public bool Equals([AllowNull] MeasureGroupAttributeDto other)
        {
            var attribute = other as MeasureGroupAttributeDto;
            return attribute != null && Name == attribute.Name &&
                   Id == attribute.Id &&
                   TypeName == attribute.TypeName &&
                   TypeCode == attribute.TypeCode;
        }

        public override int GetHashCode()
        {
            var hashCode = -992789481;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Id);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(TypeName);
            hashCode = hashCode * -1521134295 + TypeCode.GetHashCode();
            return hashCode;
        }
    }

    public class MeasureGroupDateAttributeDto : MeasureGroupAttributeDto
    {
        public string MinDate { get; set; }

        public string MaxDate { get; set; }

        public override bool Equals(object obj)
        {
            var baseAttr = obj as MeasureGroupAttributeDto;
            var attribute = obj as MeasureGroupDateAttributeDto;

            return baseAttr != null &&
                this.Equals(baseAttr) &&
                attribute != null &&
                MinDate == attribute.MinDate &&
                MaxDate == attribute.MaxDate;
        }

        public bool Equals([AllowNull] MeasureGroupDateAttributeDto other)
        {
            var baseAttr = other as MeasureGroupAttributeDto;
            var attribute = other as MeasureGroupDateAttributeDto;

            return baseAttr != null &&
                this.Equals(baseAttr) &&
                attribute != null &&
                MinDate == attribute.MinDate &&
                MaxDate == attribute.MaxDate;
        }

        public override int GetHashCode()
        {
            var hashCode = base.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MinDate);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MaxDate);

            return hashCode;
        }
    }

    public class MeasureGroupBooleanAttributeDto : MeasureGroupAttributeDto
    {
        public string TrueValueText { get; set; }

        public string FalseValueText { get; set; }

        public string NullValueText { get; set; }

        public override bool Equals(object obj)
        {
            var baseAttr = obj as MeasureGroupAttributeDto;
            var attribute = obj as MeasureGroupBooleanAttributeDto;

            return baseAttr != null &&
                this.Equals(baseAttr) &&
                attribute != null &&
                TrueValueText == attribute.TrueValueText &&
                FalseValueText == attribute.FalseValueText &&
                NullValueText == attribute.NullValueText;
        }

        public bool Equals([AllowNull] MeasureGroupBooleanAttributeDto other)
        {
            var baseAttr = other as MeasureGroupAttributeDto;
            var attribute = other as MeasureGroupBooleanAttributeDto;

            return baseAttr != null &&
                this.Equals(baseAttr) &&
                attribute != null &&
                TrueValueText == attribute.TrueValueText &&
                FalseValueText == attribute.FalseValueText &&
                NullValueText == attribute.NullValueText;
        }

        public override int GetHashCode()
        {
            var hashCode = base.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(TrueValueText);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FalseValueText);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(NullValueText);

            return hashCode;
        }
    }

    public class MeasureGroupDecimalAttributeDto : MeasureGroupAttributeDto
    {
        public byte Precision { get; set; }

        public override bool Equals(object obj)
        {
            var baseAttr = obj as MeasureGroupAttributeDto;
            var attribute = obj as MeasureGroupDecimalAttributeDto;

            return baseAttr != null &&
                this.Equals(baseAttr) &&
                attribute != null &&
                Precision == attribute.Precision;
        }

        public bool Equals([AllowNull] MeasureGroupDecimalAttributeDto other)
        {
            var baseAttr = other as MeasureGroupAttributeDto;
            var attribute = other as MeasureGroupDecimalAttributeDto;

            return baseAttr != null &&
                this.Equals(baseAttr) &&
                attribute != null &&
                Precision == attribute.Precision;
        }

        public override int GetHashCode()
        {
            var hashCode = base.GetHashCode();
            hashCode = hashCode * -1521134295 + Precision.GetHashCode();

            return hashCode;
        }
    }

    public class MeasureGroupLongAttributeDto : MeasureGroupAttributeDto
    {
    }

    public class MeasureGroupStringAttributeDto : MeasureGroupAttributeDto
    {
    }
}