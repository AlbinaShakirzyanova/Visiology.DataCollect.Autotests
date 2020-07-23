namespace Visiology.DataCollect.Autotests.API.Models.MeasureGroups.Elements
{
    public class MeasureGroupElementAttributeDto
    {
        public string AttributeId { get; }

        public object Value { get; }

        public MeasureGroupElementAttributeDto(string attributeId, object value)
        {
            Value = value;
            AttributeId = attributeId;
        }
    }

    public class MeasureGroupElementDecimalAttributeDto : MeasureGroupElementAttributeDto
    {
        public MeasureGroupElementDecimalAttributeDto(string attributeId, object value) : base(attributeId, value)
        {
        }
    }

    public class MeasureGroupElementLongAttributeDto : MeasureGroupElementAttributeDto
    {
        public MeasureGroupElementLongAttributeDto(string attributeId, object value) : base(attributeId, value)
        {
        }
    }

    public class MeasureGroupElementStringAttributeDto : MeasureGroupElementAttributeDto
    {
        public MeasureGroupElementStringAttributeDto(string attributeId, object value) : base(attributeId, value)
        {
        }
    }

    public class MeasureGroupElementDateAttributeDto : MeasureGroupElementAttributeDto
    {
        public MeasureGroupElementDateAttributeDto(string attributeId, object value) : base(attributeId, value)
        {
        }
    }

    public class MeasureGroupElementBooleanAttributeDto : MeasureGroupElementAttributeDto
    {
        public string DisplayValue { get; }

        public MeasureGroupElementBooleanAttributeDto(string attributeId, object value, string displayValue) : base(attributeId, value)
        {
            DisplayValue = displayValue;
        }
    }
}
