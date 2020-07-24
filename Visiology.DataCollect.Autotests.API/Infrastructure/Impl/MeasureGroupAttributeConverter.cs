using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using Visiology.DataCollect.Autotests.API.Models.MeasureGroups.Attributes;

namespace Visiology.DataCollect.Autotests.API.Infrastructure.Impl
{
    public class BaseSpecifiedConcreteClassConverter : DefaultContractResolver
    {
        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            if (typeof(MeasureGroupAttributeDto).IsAssignableFrom(objectType) && !objectType.IsAbstract)
                return null; // pretend TableSortRuleConvert is not specified (thus avoiding a stack overflow)
            return base.ResolveContractConverter(objectType);
        }
    }

    public class MeasureGroupAttributeConverter : JsonConverter
    {
        static JsonSerializerSettings SpecifiedSubclassConversion = new JsonSerializerSettings() { ContractResolver = new BaseSpecifiedConcreteClassConverter() };

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(MeasureGroupAttributeDto);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            switch (jo["typeCode"].Value<int>())
            {
                case (int)MeasureGroupAttributeType.String:
                    return JsonConvert.DeserializeObject<MeasureGroupStringAttributeDto>(jo.ToString(), SpecifiedSubclassConversion);
                case (int)MeasureGroupAttributeType.Long:
                    return JsonConvert.DeserializeObject<MeasureGroupLongAttributeDto>(jo.ToString(), SpecifiedSubclassConversion);
                case (int)MeasureGroupAttributeType.Decimal:
                    return JsonConvert.DeserializeObject<MeasureGroupDecimalAttributeDto>(jo.ToString(), SpecifiedSubclassConversion);
                case (int)MeasureGroupAttributeType.Boolean:
                    return JsonConvert.DeserializeObject<MeasureGroupBooleanAttributeDto>(jo.ToString(), SpecifiedSubclassConversion);
                case (int)MeasureGroupAttributeType.Date:
                    return JsonConvert.DeserializeObject<MeasureGroupDateAttributeDto>(jo.ToString(), SpecifiedSubclassConversion);
                default:
                    throw new Exception();
            }
            throw new NotImplementedException();
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException(); // won't be called because CanWrite returns false
        }
    }
}
