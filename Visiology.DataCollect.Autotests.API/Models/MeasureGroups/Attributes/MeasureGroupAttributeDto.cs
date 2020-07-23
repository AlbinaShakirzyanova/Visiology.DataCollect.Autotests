using System.Collections.Generic;
using System.Globalization;

namespace Visiology.DataCollect.Autotests.API.Models.MeasureGroups.Attributes
{
    public abstract class MeasureGroupAttributeDto
    {
        public string Id { get; }

        public string Name { get; }

        public string TypeName { get; }

        public MeasureGroupAttributeType TypeCode
        {
            get;
        }

        protected MeasureGroupAttributeDto(ApiMeasureGroupAttribute attribute)
        {
            Id = attribute.UniqueName;
            Name = attribute.Name;
            TypeName = attribute.Type.ToString();
            TypeCode = attribute.Type;
        }
    }

    public class MeasureGroupDateAttributeDto : MeasureGroupAttributeDto
    {
        public string MinDate { get; }

        public string MaxDate { get; }

        public MeasureGroupDateAttributeDto(ApiMeasureGroupDateAttribute attribute) : base(attribute)
        {
            MinDate = attribute.MinDate.ToString();
            MaxDate = attribute.MaxDate.ToString();
        }
    }

    public class MeasureGroupBooleanAttributeDto : MeasureGroupAttributeDto
    {
        public string TrueValueText { get; }

        public string FalseValueText { get; }

        public string NullValueText { get; }

        public MeasureGroupBooleanAttributeDto(
            ApiMeasureGroupBooleanAttribute booleanAttribute) : base(booleanAttribute)
        {
            TrueValueText = booleanAttribute.TrueValueText;
            FalseValueText = booleanAttribute.FalseValueText;
            NullValueText = booleanAttribute.NullValueText;
        }
    }

    public class MeasureGroupDecimalAttributeDto : MeasureGroupAttributeDto
    {
        public byte Precision { get; }

        public MeasureGroupDecimalAttributeDto(ApiMeasureGroupDecimalAttribute attribute) : base(attribute)
        {
            Precision = attribute.Precision;
        }
    }

    public class MeasureGroupLongAttributeDto : MeasureGroupAttributeDto
    {
        public MeasureGroupLongAttributeDto(ApiMeasureGroupLongAttribute attribute) : base(attribute)
        {
        }
    }

    public class MeasureGroupStringAttributeDto : MeasureGroupAttributeDto
    {
        public MeasureGroupStringAttributeDto(ApiMeasureGroupStringAttribute attribute) : base(attribute)
        {
        }
    }
}

public interface ILocalizer
{
    string L(string key, params object[] values);

    string L(string key, CultureInfo culture);

    string L(string key, CultureInfo culture, params object[] args);
}