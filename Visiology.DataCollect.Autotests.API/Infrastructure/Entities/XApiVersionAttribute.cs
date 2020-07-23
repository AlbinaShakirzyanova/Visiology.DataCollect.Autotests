using System;

namespace Visiology.DataCollect.Autotests.Infrastructure.Entities
{
    public class XApiVersionAttribute : Attribute
    {
        public string Value { get; set; }

        public XApiVersionAttribute(string value)
        {
            Value = value;
        }
    }
}
