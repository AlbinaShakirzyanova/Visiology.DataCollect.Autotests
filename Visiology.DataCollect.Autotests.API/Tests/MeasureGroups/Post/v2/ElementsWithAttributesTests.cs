using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Visiology.DataCollect.Autotests.API.Infrastructure.Entities;
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

        public ElementsWithAttributesTests(TokenFixture tokenFixture, RestService restService)
            : base(tokenFixture, restService)
        {
            Url = $"{config.GetValue("ApiUrl")}{config.GetValue("ApiUrlMeasureGroupElementsPath")}";
        }

        /// <summary>
        /// Кейс Заполнено только значение ячейки при наличии у ГП атрибута типа String
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Get_WithFilledValue_ForMGWithStringAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_string_search";
            var measureGroupName = "string атрибут. Тестирование получения элементов.";

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
                                new MeasureGroupElementAttributeDto { AttributeId = "attr_string", Value = null }
                            },
                            SystemInfo = null,
                            Comment = null,
                            Value = 2010.0000000000,
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

        /// <summary>
        /// Кейс Ячейка заполнена полностью при наличии у ГП атрибута типа String
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Get_WithFilledCell_ForMGWithStringAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_string_search";
            var measureGroupName = "string атрибут. Тестирование получения элементов.";

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
                                new MeasureGroupElementAttributeDto { AttributeId = "attr_string", Value = "filled" }
                            },
                            SystemInfo = "API",
                            Comment = "filled",
                            Value = 2010.0100000000
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

        /// <summary>
        /// Кейс Заполнен комментарий и атрибут при наличии у ГП атрибута типа String
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Get_WithFilledCommentAndAttribute_ForMGWithStringAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_string_search";
            var measureGroupName = "string атрибут. Тестирование получения элементов.";

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
                                new MeasureGroupElementAttributeDto { AttributeId = "attr_string", Value = "filled" }
                            },
                            SystemInfo = null,
                            Comment = "filled",
                            Value = null
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

        /// <summary>+
        /// Кейс Не заполнена информация только по атрибуту при наличии у ГП атрибута типа String
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Get_EmptyOnlyAttribute_ForMGWithStringAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_string_search";
            var measureGroupName = "string атрибут. Тестирование получения элементов.";

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
                                new MeasureGroupElementAttributeDto { AttributeId = "attr_string", Value = null }
                            },
                            SystemInfo = "API",
                            Comment = "filled",
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

        /// <summary>
        /// Кейс Заполнено только значение ячейки при наличии у ГП атрибута типа Long
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Get_WithFilledValue_ForMGWithLongAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_long_search";
            var measureGroupName = "long атрибут. Тестирование получения элементов.";

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
                            new MeasureGroupLongAttributeDto
                            {
                                Name = "long",
                                Id = "attr_long",
                                TypeCode = (int)MeasureGroupAttributeType.Long,
                                TypeName =  "Long"
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
                                new MeasureGroupElementAttributeDto { AttributeId = "attr_long", Value = null }
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
                    message += $"Полученное метоописание группы показателей {measureGroupId} {measureGroupName} не соответствует ожидаемому";
                }
            }

            Assert.True(isSuccess, message);
        }

        /// <summary>
        /// Кейс Ячейка заполнена полностью при наличии у ГП атрибута типа Long
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Get_WithFilledCell_ForMGWithLongAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_long_search";
            var measureGroupName = "long атрибут. Тестирование получения элементов.";

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
                            new MeasureGroupLongAttributeDto
                            {
                                Name = "long",
                                Id = "attr_long",
                                TypeCode = (int)MeasureGroupAttributeType.Long,
                                TypeName =  "Long"
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
                                new MeasureGroupElementAttributeDto { AttributeId = "attr_long", Value = 2010 }
                            },
                            SystemInfo = "API",
                            Comment = "filled",
                            Value = 2010.0000000000
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
        /// Кейс Заполнен комментарий и атрибут при наличии у ГП атрибута типа Long
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Get_WithFilledCommentAndAttribute_ForMGWithLongAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_long_search";
            var measureGroupName = "long атрибут. Тестирование получения элементов.";

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
                            new MeasureGroupLongAttributeDto
                            {
                                Name = "long",
                                Id = "attr_long",
                                TypeCode = (int)MeasureGroupAttributeType.Long,
                                TypeName =  "Long"
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
                                new MeasureGroupElementAttributeDto { AttributeId = "attr_long", Value = 2010 }
                            },
                            SystemInfo = null,
                            Comment = "filled",
                            Value = null
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
        /// Кейс Не заполнена информация только по атрибуту при наличии у ГП атрибута типа Long
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Get_EmptyOnlyAttribut_ForMGWithLongAttribute()
        {
            var measureGroupId = "measureGroup_Long_atribut_Test";
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
                        Name = "Long атрибут. Тестирование получения элементов.",
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
                            new MeasureGroupLongAttributeDto
                            {
                                Name = "long",
                                Id = "attr_long",
                                TypeCode = (int)MeasureGroupAttributeType.Long,
                                TypeName =  "Long"
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
                                new MeasureGroupElementAttributeDto { AttributeId = "attr_long", Value = null }
                            },
                            SystemInfo = "API",
                            Comment = "filled",
                            Value = 2010
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
        /// Кейс Заполнено только значение ячейки при наличии у ГП атрибута типа Decimal
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Get_WithFilledValue_ForMGWithDecimalAttribute()
        {
            var measureGroupId = "measureGroup_Decimal_atribut_T";
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
                        Name = "Decimal атрибут. Тестирование получения элементов.",
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
                            new MeasureGroupDecimalAttributeDto
                            {
                                Name = "decimal",
                                Id = "attr_decimal",
                                TypeCode = (int)MeasureGroupAttributeType.Decimal,
                                TypeName =  "Decimal",
                                Precision = 2
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
                                new MeasureGroupElementAttributeDto { AttributeId = "attr_decimal", Value = null }
                            },
                            SystemInfo = null,
                            Comment = null,
                            Value = 2010.0000000000
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
        /// Кейс Ячейка заполнена полностью при наличии у ГП атрибута типа Decimal
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Get_WithFilledCell_ForMGWithDecimalAttribute()
        {
            var measureGroupId = "measureGroup_Decimal_atribut_T";
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
                        Name = "Decimal атрибут. Тестирование получения элементов.",
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
                            new MeasureGroupDecimalAttributeDto
                            {
                                Name = "decimal",
                                Id = "attr_decimal",
                                TypeCode = (int)MeasureGroupAttributeType.Decimal,
                                TypeName =  "Decimal",
                                Precision = 2
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
                                new MeasureGroupElementAttributeDto { AttributeId = "attr_decimal", Value = 2010.21 }
                            },
                            SystemInfo = "API",
                            Comment = "API",
                            Value = 2010.0000000000
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
        /// Кейс Заполнен комментарий и атрибут при наличии у ГП атрибута типа Decimal
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
         public async Task Get_WithFilledCommentAndAttribute_ForMGWithDecimalAttribute()
         {
             var measureGroupId = "measureGroup_Decimal_atribut_T";
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
                         Name = "Decimal атрибут. Тестирование получения элементов.",
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
                             new MeasureGroupDecimalAttributeDto
                             {
                                 Name = "decimal",
                                 Id = "attr_decimal",
                                 TypeCode = (int)MeasureGroupAttributeType.Decimal,
                                 TypeName =  "Decimal",
                                 Precision = 2
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
                                 new MeasureGroupElementAttributeDto { AttributeId = "attr_decimal", Value = 2010.21 }
                             },
                             SystemInfo = null,
                             Comment = "2010.21",
                             Value = null
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
        /// Кейс Не заполнена информация только по атрибуту при наличии у ГП атрибута типа Decimal
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Get_EmptyOnlyAttribut_ForMGWithDecimalAttribute()
         {
             var measureGroupId = "measureGroup_Decimal_atribut_T";
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
                         Name = "Decimal атрибут. Тестирование получения элементов.",
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
                             new MeasureGroupDecimalAttributeDto
                             {
                                 Name = "decimal",
                                 Id = "attr_decimal",
                                 TypeCode = (int)MeasureGroupAttributeType.Decimal,
                                 TypeName =  "Decimal",
                                 Precision = 2
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
                                 new MeasureGroupElementAttributeDto { AttributeId = "attr_decimal", Value = null }
                             },
                             SystemInfo = "API",
                             Comment = "filled",
                             Value = 2010
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
        /// Кейс 1 атрибута типа Boolean
        /// Value	Comment	Attribute	SystemInfo
        ///  empty  empty   no          empty
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        //[Fact, Trait("Category", "MeasureGroups2.0")]
        /*public async Task Get_EmptyOnlyAttribut_ForMGWithDecimalAttribute()
        {
            var measureGroupId = "measureGroup_Decimal_atribut_T";
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
                        Name = "Decimal атрибут. Тестирование получения элементов.",
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
                             new MeasureGroupDecimalAttributeDto
                             {
                                 Name = "decimal",
                                 Id = "attr_decimal",
                                 TypeCode = (int)MeasureGroupAttributeType.Decimal,
                                 TypeName =  "Decimal",
                                 Precision = 2
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
                                 new MeasureGroupElementAttributeDto { AttributeId = "attr_decimal", Value = null }
                             },
                             SystemInfo = "API",
                             Comment = "filled",
                             Value = 2010
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
        }*/

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
