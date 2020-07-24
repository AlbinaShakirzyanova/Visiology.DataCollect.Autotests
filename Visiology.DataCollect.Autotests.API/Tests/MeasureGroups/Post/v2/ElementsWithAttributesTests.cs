using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Visiology.DataCollect.Autotests.API.Infrastructure.Entities.RequestBody.Filters.MeasureGroup;
using Visiology.DataCollect.Autotests.API.Models.MeasureGroups.Attributes;
using Visiology.DataCollect.Autotests.API.Models.MeasureGroups.Elements;
using Visiology.DataCollect.Autotests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Impl;
using Visiology.DataCollect.Integration.Tests.Models.MeasureGroups;
using Visiology.DataCollect.Integration.Tests.Models.MeasureGroups.Elements;
using Xunit;

namespace Visiology.DataCollect.Autotests.API.Tests.MeasureGroups.Post.v2
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
        public override Method Method { get; set; } = Method.POST;

        public ElementsWithAttributesTests(IisFixture iisFixture, TokenFixture tokenFixture, RestService restService)
            : base(iisFixture, tokenFixture, restService)
        {
            Url = $"{config.GetValue("ApiUrl")}{config.GetValue("ApiUrlMeasureGroupElementsPath")}";
        }

        /// <summary>
        /// Кейс Заполнено только значение ячейки при наличии у ГП строкогого атрибута
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Get_WithFilledValue_ForMGWithStringAttribute()
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

                var expectedContent = new ElementsListDto
                {
                    MeasureGroup = new MeasureGroupDto
                    {
                        Name = "Строковый атрибут. Тестирование получения элементов.",
                        Id = "measureGroup_Strokovii_atribut",
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
                                Name = "Строковый",
                                Id = "attr_Strokovii",
                                TypeCode = (int)MeasureGroupAttributeType.String,
                                TypeName =  "String"
                            }
                        }
                    },
                    Entities = new List<ElementDto>
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
                            Value = 2010.0000000000,
                            Id = "1"
                        }
                    }
                };

                if (!expectedContent.Equals(content))
                {
                    isSuccess = false;
                    message += $"Полученное метоописание группы показателей {measureGroupId} не соответствует ожидаемому";
                }
            }

            Assert.True(isSuccess, message);
        }

        /// <summary>
        /// Кейс Ячейка заполнена полностью при наличии у ГП строкогого атрибута
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Get_WithFilledCell_ForMGWithStringAttribute()
        {

            var measureGroupId = "measureGroup_Strokovii_atribut";
            var url = this.GetUrl(measureGroupId);

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(new SimpleFilter
            {
                value = 2,
                type = SimpleFilterType.id,
                condition = FilterCondition.equals
            }));

            var response = await _restService.SendRequestAsync(this.Method, url, TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers, filterContent);
            var isSuccess = response.IsSuccessful;
            var message = $"{response.StatusCode} {response.StatusDescription}";

            if (isSuccess)
            {

                var content = await Task.Run(() => JsonConvert.DeserializeObject<ElementsListDto>(response.Content));

                var expectedContent = new ElementsListDto
                {
                    MeasureGroup = new MeasureGroupDto
                    {
                        Name = "Строковый атрибут. Тестирование получения элементов.",
                        Id = "measureGroup_Strokovii_atribut",
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
                                Name = "Строковый",
                                Id = "attr_Strokovii",
                                TypeCode = (int)MeasureGroupAttributeType.String,
                                TypeName =  "String"
                            }
                        }
                    },
                    Entities = new List<ElementDto>
                    {
                        new ElementDto
                        {
                            DimensionElements = new List<DimensionElementDto>
                            {
                                new DimensionElementDto { DimensionId = "dim_Day_of_week", ElementId = 2 }
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
                                new MeasureGroupElementAttributeDto { AttributeId = "attr_Strokovii", Value = "filled" }
                            },
                            SystemInfo = "API - filled",
                            Comment = "filled",
                            Value = 2010.0100000000,
                            Id = "2"
                        }
                    }
                };

                if (!expectedContent.Equals(content))
                {
                    isSuccess = false;
                    message += $"Полученное метоописание группы показателей {measureGroupId} не соответствует ожидаемому";
                }
            }

            Assert.True(isSuccess, message);
        }

        /// <summary>
        /// Кейс Заполнен комментарий и системная информация при наличии у ГП строкогого атрибута
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Get_WithFilledCommentAndAttribute_ForMGWithStringAttribute()
        {

            var measureGroupId = "measureGroup_Strokovii_atribut";
            var url = this.GetUrl(measureGroupId);

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(new SimpleFilter
            {
                value = 3,
                type = SimpleFilterType.id,
                condition = FilterCondition.equals
            }));

            var response = await _restService.SendRequestAsync(this.Method, url, TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers, filterContent);
            var isSuccess = response.IsSuccessful;
            var message = $"{response.StatusCode} {response.StatusDescription}";

            if (isSuccess)
            {

                var content = await Task.Run(() => JsonConvert.DeserializeObject<ElementsListDto>(response.Content));

                var expectedContent = new ElementsListDto
                {
                    MeasureGroup = new MeasureGroupDto
                    {
                        Name = "Строковый атрибут. Тестирование получения элементов.",
                        Id = "measureGroup_Strokovii_atribut",
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
                                Name = "Строковый",
                                Id = "attr_Strokovii",
                                TypeCode = (int)MeasureGroupAttributeType.String,
                                TypeName =  "String"
                            }
                        }
                    },
                    Entities = new List<ElementDto>
                    {
                        new ElementDto
                        {
                            DimensionElements = new List<DimensionElementDto>
                            {
                                new DimensionElementDto { DimensionId = "dim_Day_of_week", ElementId = 3 }
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
                                new MeasureGroupElementAttributeDto { AttributeId = "attr_Strokovii", Value = "filled" }
                            },
                            SystemInfo = null,
                            Comment = "filled",
                            Value = null,
                            Id = "3"
                        }
                    }
                };

                if (!expectedContent.Equals(content))
                {
                    isSuccess = false;
                    message += $"Полученное метоописание группы показателей {measureGroupId} не соответствует ожидаемому";
                }
            }

            Assert.True(isSuccess, message);
        }

        /// <summary>+
        /// Кейс Не заполнена информация только по атрибуту при наличии у ГП строкогого атрибута
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Get_EmptyOnlyAttribute_ForMGWithStringAttribute()
        {

            var measureGroupId = "measureGroup_Strokovii_atribut";
            var url = this.GetUrl(measureGroupId);

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(new SimpleFilter
            {
                value = 4,
                type = SimpleFilterType.id,
                condition = FilterCondition.equals
            }));

            var response = await _restService.SendRequestAsync(this.Method, url, TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers, filterContent);
            var isSuccess = response.IsSuccessful;
            var message = $"{response.StatusCode} {response.StatusDescription}";

            if (isSuccess)
            {

                var content = await Task.Run(() => JsonConvert.DeserializeObject<ElementsListDto>(response.Content));

                var expectedContent = new ElementsListDto
                {
                    MeasureGroup = new MeasureGroupDto
                    {
                        Name = "Строковый атрибут. Тестирование получения элементов.",
                        Id = "measureGroup_Strokovii_atribut",
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
                                Name = "Строковый",
                                Id = "attr_Strokovii",
                                TypeCode = (int)MeasureGroupAttributeType.String,
                                TypeName =  "String"
                            }
                        }
                    },
                    Entities = new List<ElementDto>
                    {
                        new ElementDto
                        {
                            DimensionElements = new List<DimensionElementDto>
                            {
                                new DimensionElementDto { DimensionId = "dim_Day_of_week", ElementId = 4 }
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
                            SystemInfo = "API - filled",
                            Comment = "filled",
                            Value = 2010,
                            Id = "4"
                        }
                    }
                };

                if (!expectedContent.Equals(content))
                {
                    isSuccess = false;
                    message += $"Полученное метоописание группы показателей {measureGroupId} не соответствует ожидаемому";
                }
            }

            Assert.True(isSuccess, message);
        }

        protected override string GetUrl(string measureGroupId)
        {
            return string.Format($"{config.GetValue("ApiUrl")}{config.GetValue("ApiUrlMeasureGroupElementsSearchPath")}", measureGroupId);
        }

        protected override string GetSearchUrl(string measureGroupId)
        {
            throw new NotImplementedException();
        }
    }
}
