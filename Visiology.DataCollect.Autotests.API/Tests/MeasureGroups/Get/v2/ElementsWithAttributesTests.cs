using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Visiology.DataCollect.Autotests.API.Infrastructure.Entities;
using Visiology.DataCollect.Autotests.API.Infrastructure.Entities.RequestBody.Filters.MeasureGroup;
using Visiology.DataCollect.Autotests.API.Models.MeasureGroups.Elements;
using Visiology.DataCollect.Autotests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Impl;
using Visiology.DataCollect.Integration.Tests.Models.MeasureGroups.Elements;
using Xunit;

namespace Visiology.DataCollect.Autotests.API.Tests.MeasureGroups.Get.v2
{
    /// <summary>
    /// Класс тестирования метода получения элементов
    /// </summary>
    [XApiVersion("2.0")]
    public class ElementsWithAttributesTests : BaseIntegrationTests<ElementsListDto, ElementDto>
    {
        /// <summary>
        /// Url запроса на получение описаний групп показателей
        /// </summary>
        public override string Url { get; set; }

        /// <summary>
        /// Тип тестируемого метода
        /// </summary>
        public override Method Method { get; set; } = Method.GET;

        public ElementsWithAttributesTests(TokenFixture tokenFixture, RestService restService)
            : base(tokenFixture, restService)
        {
            Url = $"{config.GetValue("ApiUrl")}{config.GetValue("ApiUrlMeasureGroupElementsPath")}";
        }

        /// <summary>
        /// Кейс Заполнено только значение ячейки при наличии у ГП строкогого атрибута
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        //[Fact, Trait("Category", "MeasureGroups2.0")]
       /* public async Task Get_WithFilledValue_FromMGWithStringAttribute()
        {
            var measureGroupId = "measureGroup_Strokovii_atribut";
            var url = this.GetUrl(measureGroupId);

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(new SimpleFilter
            {
                value = 1,
                type = SimpleFilterType.id,
                condition = FilterCondition.equals
            }));

            var response = await _restService.SendRequestAsync(this.Method, url, TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers, filterContent);
            var isSuccess = response.IsSuccessful;
            var message = $"{response.StatusCode} {response.StatusDescription}";

            if (isSuccess)
            {

                var content = await Task.Run(() => JsonConvert.DeserializeObject<ElementsListDto>(response.Content));

                var expectedContent = new List<ElementDto>
                    {
                        new ElementDto
                        {
                            DimensionElements = new List<DimensionElementDto>
                            {
                                new DimensionElementDto { DimensionId = "dim_Day_of_week", ElementId = 1 }
                            },
                            MeasureElements = new List<MeasureElementDto>
                            {
                                new MeasureElementDto { MeasureId = "dim_Measures", ElementId = 1 }
                            },
                            Calendar = new CalendarDto
                            {
                                DateWithGranularity = "2010",
                                Granularity = "Год",
                                Date = DateTime.Parse("2010-01-01T00:00:00")
                            },
                            Attributes = new List<MeasureGroupElementAttributeDto>
                            {
                                new MeasureGroupElementAttributeDto { AttributeId = "attr_Strokovii", Value = null }
                            },
                            SystemInfo = null,
                            Comment = null,
                            Value = 2010,
                            Id = "1"
                        }
                    };

                if (!expectedContent.Equals(content.Entities))
                {
                    isSuccess = false;
                    message += $"Полученное метоописание группы показателей {measureGroupId} не соответствует ожидаемому";
                }
            }

            Assert.True(isSuccess, message);
        }  */ 

        protected override string GetUrl(string measureGroupId)
        {
            return string.Format(this.Url, measureGroupId);
        }

        protected override string GetSearchUrl(string measureGroupId)
        {
            throw new NotImplementedException();
        }
    }
}
