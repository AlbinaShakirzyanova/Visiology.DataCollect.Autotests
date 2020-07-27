using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Visiology.DataCollect.Autotests.API.Infrastructure.Entities.RequestBody.Filters.MeasureGroup;
using Visiology.DataCollect.Autotests.API.Models.MeasureGroups.Attributes;
using Visiology.DataCollect.Autotests.API.Models.MeasureGroups.Elements;
using Visiology.DataCollect.Autotests.API.Tests.MeasureGroups.Get;
using Visiology.DataCollect.Autotests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Impl;
using Visiology.DataCollect.Integration.Tests.Models.MeasureGroups;
using Visiology.DataCollect.Integration.Tests.Models.MeasureGroups.Elements;
using Xunit;

namespace Visiology.DataCollect.Autotests.API.Tests.MeasureGroups.Get.v2
{
    /// <summary>
    /// Класс тестирования метода поиска элементов группы показателей
    /// </summary>
    [XApiVersion("2.0")]
    public class DetailsElementsTests : BaseTests<ElementsListWithDetailsDto, ElementWithDetailsDto>
    {
        public override string Url { get; set; }

        public override Method Method { get; set; } = Method.GET;


        public DetailsElementsTests(TokenFixture tokenFixture, RestService restService)
            : base(tokenFixture, restService)
        {
            Url = $"{config.GetValue("ApiUrl")}{config.GetValue("ApiUrlMeasureGroupElementsDetailsPath")}";
        }                                    

        /// <summary>
        /// Кейс Тестирования корректного описания группы показателей для группы показателей с атрибутами
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Get_WithNamedAttributeIdFilter_ForDateAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_all_search";
            var measureGroupName = "all. Тестирование получения элементов";

            var url = GetUrl(measureGroupId);

            var response = await _restService.SendRequestAsync(Method, url, TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers);
            var isSuccess = response.IsSuccessful;
            var message = $"{response.StatusCode} {response.StatusDescription}";

            if (isSuccess)
            {
                var content = await Task.Run(() => JsonConvert.DeserializeObject<ElementsListWithDetailsDto>(response.Content));

                var expectedContent = new ElementsListWithDetailsDto
                {
                    MeasureGroup = new MeasureGroupDto
                    {
                        Name = measureGroupName,
                        Id = measureGroupId,
                        Dimensions = new List<ApiMeasureGroupComponent>
                        {
                            new ApiMeasureGroupComponent { Name = "Дни недели", Id = "dim_Day_of_week" }
                        },
                        Measure = new ApiMeasureGroupComponent
                        {
                            Name = "Показатели",
                            Id = "dim_Measures"
                        },
                        Calendar = new ApiMeasureGroupComponent
                        {
                            Name = "Год",
                            Id = "cal_God"
                        },
                        Attributes = new List<MeasureGroupAttributeDto>
                        {
                            new MeasureGroupStringAttributeDto
                            {
                                Name = "string",
                                Id = "attr_string",
                                TypeCode = (int)MeasureGroupAttributeType.String,
                                TypeName =  "String",
                            },
                            new MeasureGroupDateAttributeDto
                            {
                                Name = "date",
                                Id = "attr_date",
                                TypeCode = (int)MeasureGroupAttributeType.Date,
                                TypeName =  "Date",
                                MinDate = "2020-01-01",
                                MaxDate = "2030-12-31"
                            },
                            new MeasureGroupLongAttributeDto
                            {
                                Name = "long",
                                Id = "attr_long",
                                TypeCode = (int)MeasureGroupAttributeType.Long,
                                TypeName =  "Long",
                            },
                            new MeasureGroupDecimalAttributeDto
                            {
                                Name = "decimal",
                                Id = "attr_decimal",
                                TypeCode = (int)MeasureGroupAttributeType.Decimal,
                                TypeName =  "Decimal",
                            },
                             new MeasureGroupBooleanAttributeDto
                            {
                                Name = "boolean",
                                Id = "attr_boolean",
                                TypeCode = (int)MeasureGroupAttributeType.Boolean,
                                TypeName =  "Boolean",
                                FalseValueText = "No",
                                TrueValueText = "Yes",
                                NullValueText = "Empty"
                            }
                        }
                    },
                    Entities = new List<ElementWithDetailsDto>
                    {
                        new ElementWithDetailsDto
                        {
                            DimensionElements = new List<DimensionElementWithDetailsDto>
                            {
                                new DimensionElementWithDetailsDto { DimensionId = "1", ElementId = 1, DimensionName = "Дни недели", ElementName = "Понедельник" }
                            },
                            MeasureElements = new List<MeasureElementWithDetailsDto>
                            {
                                new MeasureElementWithDetailsDto { MeasureId = "4", ElementId = 1, MeasureName = "Показатели", ElementName = "Показатель по умолчанию" }
                            },
                            Calendar = new CalendarDto
                            {
                                DateWithGranularity = "2010",
                                Granularity = "Год",
                                Date = DateTime.Parse("2010-01-01T00:00:00")
                            },
                            Attributes = new List<MeasureGroupElementAttributeDto>
                            {
                                new MeasureGroupElementAttributeDto { AttributeId = "attr_string", Value = "string" },
                                new MeasureGroupElementAttributeDto { AttributeId = "attr_date", Value = "2030-12-30" },
                                new MeasureGroupElementAttributeDto { AttributeId = "attr_long", Value = 2010 },
                                new MeasureGroupElementAttributeDto { AttributeId = "attr_decimal", Value = 2010.21 },
                                new MeasureGroupElementAttributeDto { AttributeId = "attr_boolean", Value = true, DisplayValue = "Yes" },
                            },
                            SystemInfo = null,
                            Comment = null,
                            Value = 2010
                        }
                    }
                };

                if (!expectedContent.Equals(content))
                {
                    isSuccess = false;
                    message += $"Полученное метоописание группы показателей {measureGroupId} {measureGroupName} не соответствует ожидаемому";
                }
            }

            Assert.True(isSuccess, message);
        }

        protected override string GetSearchUrl(string entityId)
        {
            return string.Format($"{config.GetValue("ApiUrl")}{config.GetValue("ApiUrlMeasureGroupElementsSearchPath")}", entityId);
        }

        protected override string GetUrl(string entityId)
        {
            return string.Format(Url, entityId);
        }
    }
}
