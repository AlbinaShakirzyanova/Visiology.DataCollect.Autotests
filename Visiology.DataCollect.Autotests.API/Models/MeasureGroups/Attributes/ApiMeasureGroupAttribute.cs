using System;

namespace Visiology.DataCollect.Autotests.API.Models.MeasureGroups.Attributes
{
    public abstract class ApiMeasureGroupAttribute
    {
        public long Id { get; set; }

        public string UniqueName { get; set; }

        public string Name { get; set; }

        public bool IsValue { get; set; }

        public int SortOrder { get; set; }

        public abstract MeasureGroupAttributeType Type { get; }
    }

    public class ApiMeasureGroupLongAttribute : ApiMeasureGroupAttribute
    {
        public override MeasureGroupAttributeType Type => MeasureGroupAttributeType.Long;
    }

    public class ApiMeasureGroupDecimalAttribute : ApiMeasureGroupAttribute
    {
        public override MeasureGroupAttributeType Type => MeasureGroupAttributeType.Decimal;

        public byte Precision { get; set; }
    }

    public class ApiMeasureGroupBooleanAttribute : ApiMeasureGroupAttribute
    {
        public override MeasureGroupAttributeType Type => MeasureGroupAttributeType.Boolean;

        public string TrueValueText { get; set; }

        public string FalseValueText { get; set; }

        public string NullValueText { get; set; }
    }

    public class ApiMeasureGroupStringAttribute : ApiMeasureGroupAttribute
    {
        public override MeasureGroupAttributeType Type => MeasureGroupAttributeType.String;
    }

    public class ApiMeasureGroupDateAttribute : ApiMeasureGroupAttribute
    {
        public override MeasureGroupAttributeType Type => MeasureGroupAttributeType.Date;

        public DateTime MinDate { get; set; }

        public DateTime MaxDate { get; set; }

        public CalendarGranularity Granularity { get; set; }
    }
}
