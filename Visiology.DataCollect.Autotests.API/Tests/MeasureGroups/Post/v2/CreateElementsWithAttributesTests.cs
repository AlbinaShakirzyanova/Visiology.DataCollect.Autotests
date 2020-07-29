using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Visiology.DataCollect.Autotests.API.Infrastructure.Entities.RequestBody.Filters.MeasureGroup;
using Visiology.DataCollect.Autotests.API.Infrastructure.Entities.Results.MeasureGroups;
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
    public class CreateElementsWithAttributesTests : BaseIntegrationTests<ElementsListDto, ElementDto>
    {
        /// <summary>
        /// Url запроса на получение описаний групп показателей
        /// </summary>
        public override string Url { get; set; }

        /// <summary>
        /// Тип тестируемого метода
        /// </summary>
        public override Method Method { get; set; } = Method.POST;

        public CreateElementsWithAttributesTests(TokenFixture tokenFixture, RestService restService)
            : base(tokenFixture, restService)
        {
            Url = $"{config.GetValue("ApiUrl")}{config.GetValue("ApiUrlMeasureGroupElementsCreatePath")}";
        }

        /// <summary>
        /// Кейс Заполнено значение ячейки и  SystemInfo при наличии у ГП атрибута типа String
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Create_WithFilledValue_ForMGWithStringAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_string_create";
            var measureGroupName = "string атрибут. Тестирование создания элементов.";

            var url = this.GetUrl(measureGroupId);

            object attrValue = null;
            object commentValue = null;

            var body = new[]
            {
                new
                {
                    MustUpdateComment = false,
                    Comment = commentValue,
                    Value = "2010",
                    Dimensions = new[]
                    {
                        new { Id = "dim_Day_of_week", ElementId = 1 }
                    },
                    Measure = new { Id = "dim_Measures", ElementId = 1 },
                    Calendars = new[] { new { Name = "Год", Id = "cal_God", Date = "2010-01-01" } },
                    Attributes =  new[]
                    {
                        new { id = "attr_string", value = attrValue }
                    }
                 }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var response = await _restService.SendRequestAsync(this.Method, url, TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers, bodyContent);
            var isSuccess = response.IsSuccessful;
            var message = $"{response.StatusCode} {response.StatusDescription}";

            if (isSuccess)
            {

                var content = await Task.Run(() => JsonConvert.DeserializeObject<CreateResult>(response.Content));

                var expectedResult = new CreateResult { created = 1, restricted = 0 };

                // Сначала проверяем корректноть отработки запроса на создание - 
                // Ищем методом Search с фильтром по координам наш элемент               
                if (expectedResult.Equals(content))
                {
                    object calendarFilter = new SimpleFilter { value = "2010-01-01", type = SimpleFilterType.calendar, condition = FilterCondition.equals };
                    var filter = new
                    {
                        operation = "and",
                        filters = new[]
                        {
                             calendarFilter,
                             new NamedFilter { value = 1, type = NamedFilterType.DimensionId, name=  "dim_Day_of_week", condition =  FilterCondition.equals },
                             new NamedFilter { value = 1, type = NamedFilterType.MeasureId, name=  "dim_Measures", condition =  FilterCondition.equals }
                        }
                    };

                    var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

                    var searchResponse = await _restService.SendRequestAsync(this.Method, GetSearchUrl(measureGroupId), TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers, filterContent);
                    
                    isSuccess &= searchResponse.IsSuccessful;
                    message += $"{searchResponse.StatusCode} {searchResponse.StatusDescription}";

                    if (isSuccess)
                    {
                        var searchContent = await Task.Run(() => JsonConvert.DeserializeObject<ElementsListDto>(searchResponse.Content));

                        // Если запрос отработал корректно - получаем наш элемент и смотрим корректность значений
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
                            SystemInfo = "API",
                            Comment = null,
                            Value = 2010.0000000000,
                        }
                    }
                        };

                        if (!expectedContent.Equals(searchContent))
                        {
                            isSuccess = false;
                            message += $"Полученное метоописание элемента группы показателей {measureGroupId} {measureGroupName} не соответствует ожидаемому";
                        }
                    }
                    
                }
                else 
                {
                    isSuccess &= false;
                    message = $"Ошибка при создании элемента группы показателей {measureGroupName}";
                }                            
            }

            Assert.True(isSuccess, message);
        }

        /// <summary>
        /// Кейс Ячейка заполнена полностью при наличии у ГП атрибута типа String
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Create_WithFilledCell_ForMGWithStringAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_string_create";
            var measureGroupName = "string атрибут. Тестирование создания элементов.";

            var url = this.GetUrl(measureGroupId);

            var body = new[]
            {
                new
                {
                    MustUpdateComment = true,
                    Comment = "filled",
                    Value = "2010",
                    Dimensions = new[]
                    {
                        new { Id = "dim_Day_of_week", ElementId = 2 }
                    },
                    Measure = new { Id = "dim_Measures", ElementId = 1 },
                    Calendars = new[] { new { Name = "Год", Id = "cal_God", Date = "2010-01-01" } },
                    Attributes =  new[]
                    {
                        new { id = "attr_string", value = "filled" }
                    }
                 }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var response = await _restService.SendRequestAsync(this.Method, url, TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers, bodyContent);
            var isSuccess = response.IsSuccessful;
            var message = $"{response.StatusCode} {response.StatusDescription}";

            if (isSuccess)
            {

                var content = await Task.Run(() => JsonConvert.DeserializeObject<CreateResult>(response.Content));

                var expectedResult = new CreateResult { created = 1, restricted = 0 };

                // Сначала проверяем корректноть отработки запроса на создание - 
                // Ищем методом Search с фильтром по координам наш элемент               
                if (expectedResult.Equals(content))
                {
                    object calendarFilter = new SimpleFilter { value = "2010-01-01", type = SimpleFilterType.calendar, condition = FilterCondition.equals };
                    var filter = new
                    {
                        operation = "and",
                        filters = new[]
                        {
                             calendarFilter,
                             new NamedFilter { value = 2, type = NamedFilterType.DimensionId, name=  "dim_Day_of_week", condition =  FilterCondition.equals },
                             new NamedFilter { value = 1, type = NamedFilterType.MeasureId, name=  "dim_Measures", condition =  FilterCondition.equals }
                        }
                    };

                    var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

                    var searchResponse = await _restService.SendRequestAsync(this.Method, GetSearchUrl(measureGroupId), TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers, filterContent);

                    isSuccess &= searchResponse.IsSuccessful;
                    message += $"{searchResponse.StatusCode} {searchResponse.StatusDescription}";

                    if (isSuccess)
                    {
                        var searchContent = await Task.Run(() => JsonConvert.DeserializeObject<ElementsListDto>(searchResponse.Content));

                        // Если запрос отработал корректно - получаем наш элемент и смотрим корректность значений
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
                            Value = 2010.0000000000
                        }
                    }
                        };

                        if (!expectedContent.Equals(searchContent))
                        {
                            isSuccess = false;
                            message += $"Полученное метоописание элемента группы показателей {measureGroupId} {measureGroupName} не соответствует ожидаемому";
                        }
                    }

                }
                else
                {
                    isSuccess &= false;
                    message = $"Ошибка при создании элемента группы показателей {measureGroupName}";
                }
            }

            Assert.True(isSuccess, message);
        }

        /// <summary>
        /// Кейс Заполнен комментарий и атрибут при наличии у ГП атрибута типа String 
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Create_WithFilledCommentAndAttribute_ForMGWithStringAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_string_create";
            var measureGroupName = "string атрибут. Тестирование создания элементов.";

            var url = this.GetUrl(measureGroupId);

            object value = null;

            var body = new[]
            {
                new
                {
                    MustUpdateComment = true,
                    Comment = "filled",
                    Value = value,
                    Dimensions = new[]
                    {
                        new { Id = "dim_Day_of_week", ElementId = 3 }
                    },
                    Measure = new { Id = "dim_Measures", ElementId = 1 },
                    Calendars = new[] { new { Name = "Год", Id = "cal_God", Date = "2010-01-01" } },
                    Attributes =  new[]
                    {
                        new { id = "attr_string", value = "filled" }
                    }
                 }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var response = await _restService.SendRequestAsync(this.Method, url, TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers, bodyContent);
            var isSuccess = response.IsSuccessful;
            var message = $"{response.StatusCode} {response.StatusDescription}";

            if (isSuccess)
            {

                var content = await Task.Run(() => JsonConvert.DeserializeObject<CreateResult>(response.Content));

                var expectedResult = new CreateResult { created = 1, restricted = 0 };

                // Сначала проверяем корректноть отработки запроса на создание - 
                // Ищем методом Search с фильтром по координам наш элемент               
                if (expectedResult.Equals(content))
                {
                    object calendarFilter = new SimpleFilter { value = "2010-01-01", type = SimpleFilterType.calendar, condition = FilterCondition.equals };
                    var filter = new
                    {
                        operation = "and",
                        filters = new[]
                        {
                             calendarFilter,
                             new NamedFilter { value = 3, type = NamedFilterType.DimensionId, name=  "dim_Day_of_week", condition =  FilterCondition.equals },
                             new NamedFilter { value = 1, type = NamedFilterType.MeasureId, name=  "dim_Measures", condition =  FilterCondition.equals }
                        }
                    };

                    var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

                    var searchResponse = await _restService.SendRequestAsync(this.Method, GetSearchUrl(measureGroupId), TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers, filterContent);

                    isSuccess &= searchResponse.IsSuccessful;
                    message += $"{searchResponse.StatusCode} {searchResponse.StatusDescription}";

                    if (isSuccess)
                    {
                        var searchContent = await Task.Run(() => JsonConvert.DeserializeObject<ElementsListDto>(searchResponse.Content));

                        // Если запрос отработал корректно - получаем наш элемент и смотрим корректность значений
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
                            SystemInfo = "API",
                            Comment = "filled",
                            Value = null
                        }
                    }
                        };

                        if (!expectedContent.Equals(searchContent))
                        {
                            isSuccess = false;
                            message += $"Полученное метоописание элемента группы показателей {measureGroupId} {measureGroupName} не соответствует ожидаемому";
                        }
                    }

                }
                else
                {
                    isSuccess &= false;
                    message = $"Ошибка при создании элемента группы показателей {measureGroupName}";
                }
            }

            Assert.True(isSuccess, message);
        }

        /// <summary>
        /// Кейс атрибута типа String заполнен спец символами
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Create_WithFilledAttribute_WithрSpecialCharacters_ForMGWithStringAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_string_create";
            var measureGroupName = "string атрибут. Тестирование создания элементов.";

            var url = this.GetUrl(measureGroupId);

            object value = null;
            object commentValue = null;

            var body = new[]
            {
                new
                {
                    MustUpdateComment = false,
                    Comment = commentValue,
                    Value = value,
                    Dimensions = new[]
                    {
                        new { Id = "dim_Day_of_week", ElementId = 4 }
                    },
                    Measure = new { Id = "dim_Measures", ElementId = 1 },
                    Calendars = new[] { new { Name = "Год", Id = "cal_God", Date = "2010-01-01" } },
                    Attributes =  new[]
                    {
                        new { id = "attr_string", value = "!\"№;%:?*()_ + " }
                    }
                 }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var response = await _restService.SendRequestAsync(this.Method, url, TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers, bodyContent);
            var isSuccess = response.IsSuccessful;
            var message = $"{response.StatusCode} {response.StatusDescription}";

            if (isSuccess)
            {

                var content = await Task.Run(() => JsonConvert.DeserializeObject<CreateResult>(response.Content));

                var expectedResult = new CreateResult { created = 1, restricted = 0 };

                // Сначала проверяем корректноть отработки запроса на создание - 
                // Ищем методом Search с фильтром по координам наш элемент               
                if (expectedResult.Equals(content))
                {
                    object calendarFilter = new SimpleFilter { value = "2010-01-01", type = SimpleFilterType.calendar, condition = FilterCondition.equals };
                    var filter = new
                    {
                        operation = "and",
                        filters = new[]
                        {
                             calendarFilter,
                             new NamedFilter { value = 4, type = NamedFilterType.DimensionId, name=  "dim_Day_of_week", condition =  FilterCondition.equals },
                             new NamedFilter { value = 1, type = NamedFilterType.MeasureId, name=  "dim_Measures", condition =  FilterCondition.equals }
                        }
                    };

                    var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

                    var searchResponse = await _restService.SendRequestAsync(this.Method, GetSearchUrl(measureGroupId), TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers, filterContent);

                    isSuccess &= searchResponse.IsSuccessful;
                    message += $"{searchResponse.StatusCode} {searchResponse.StatusDescription}";

                    if (isSuccess)
                    {
                        var searchContent = await Task.Run(() => JsonConvert.DeserializeObject<ElementsListDto>(searchResponse.Content));

                        // Если запрос отработал корректно - получаем наш элемент и смотрим корректность значений
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
                                new MeasureGroupElementAttributeDto { AttributeId = "attr_string", Value = "!\"№;%:?*()_ + " }
                            },
                            SystemInfo = "API",
                            Comment = null,
                            Value = null
                        }
                    }
                        };

                        if (!expectedContent.Equals(searchContent))
                        {
                            isSuccess = false;
                            message += $"Полученное метоописание элемента группы показателей {measureGroupId} {measureGroupName} не соответствует ожидаемому";
                        }
                    }

                }
                else
                {
                    isSuccess &= false;
                    message = $"Ошибка при создании элемента группы показателей {measureGroupName}";
                }
            }

            Assert.True(isSuccess, message);
        }

        /// <summary>
        /// Кейс Заполнено только значение ячейки при наличии у ГП атрибута типа Long
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Create_WithFilledValue_ForMGWithLongAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_long_create";
            var measureGroupName = "long атрибут. Тестирование создания элементов.";

            var url = this.GetUrl(measureGroupId);

            object commentValue = null;
            object attrValue = null;

            var body = new[]
            {
                new
                {
                    MustUpdateComment = false,
                    Comment = commentValue,
                    Value = 2010,
                    Dimensions = new[]
                    {
                        new { Id = "dim_Day_of_week", ElementId = 1 }
                    },
                    Measure = new { Id = "dim_Measures", ElementId = 1 },
                    Calendars = new[] { new { Name = "Год", Id = "cal_God", Date = "2010-01-01" } },
                    Attributes =  new[]
                    {
                        new { id = "attr_long", value = attrValue }
                    }
                 }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var response = await _restService.SendRequestAsync(this.Method, url, TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers, bodyContent);
            var isSuccess = response.IsSuccessful;
            var message = $"{response.StatusCode} {response.StatusDescription}";

            if (isSuccess)
            {

                var content = await Task.Run(() => JsonConvert.DeserializeObject<CreateResult>(response.Content));

                var expectedResult = new CreateResult { created = 1, restricted = 0 };

                // Сначала проверяем корректноть отработки запроса на создание - 
                // Ищем методом Search с фильтром по координам наш элемент               
                if (expectedResult.Equals(content))
                {
                    object calendarFilter = new SimpleFilter { value = "2010-01-01", type = SimpleFilterType.calendar, condition = FilterCondition.equals };
                    var filter = new
                    {
                        operation = "and",
                        filters = new[]
                        {
                             calendarFilter,
                             new NamedFilter { value = 1, type = NamedFilterType.DimensionId, name=  "dim_Day_of_week", condition =  FilterCondition.equals },
                             new NamedFilter { value = 1, type = NamedFilterType.MeasureId, name=  "dim_Measures", condition =  FilterCondition.equals }
                        }
                    };

                    var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

                    var searchResponse = await _restService.SendRequestAsync(this.Method, GetSearchUrl(measureGroupId), TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers, filterContent);

                    isSuccess &= searchResponse.IsSuccessful;
                    message += $"{searchResponse.StatusCode} {searchResponse.StatusDescription}";

                    if (isSuccess)
                    {
                        var searchContent = await Task.Run(() => JsonConvert.DeserializeObject<ElementsListDto>(searchResponse.Content));

                        // Если запрос отработал корректно - получаем наш элемент и смотрим корректность значений
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
                            SystemInfo = "API",
                            Comment = null,
                            Value = 2010
                        }
                    }
                        };

                        if (!expectedContent.Equals(searchContent))
                        {
                            isSuccess = false;
                            message += $"Полученное метоописание элемента группы показателей {measureGroupId} {measureGroupName} не соответствует ожидаемому";
                        }
                    }

                }
                else
                {
                    isSuccess &= false;
                    message = $"Ошибка при создании элемента группы показателей {measureGroupName}";
                }
            }

            Assert.True(isSuccess, message);
        }

        /// <summary>
        /// Кейс Ячейка заполнена полностью при наличии у ГП атрибута типа Long
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Create_WithFilledCell_ForMGWithLongAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_long_create";
            var measureGroupName = "long атрибут. Тестирование создания элементов.";

            var url = this.GetUrl(measureGroupId);

            var body = new[]
            {
                new
                {
                    MustUpdateComment = true,
                    Comment = "filled",
                    Value = 2010,
                    Dimensions = new[]
                    {
                        new { Id = "dim_Day_of_week", ElementId = 2 }
                    },
                    Measure = new { Id = "dim_Measures", ElementId = 1 },
                    Calendars = new[] { new { Name = "Год", Id = "cal_God", Date = "2010-01-01" } },
                    Attributes =  new[]
                    {
                        new { id = "attr_long", value = 2010 }
                    }
                 }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var response = await _restService.SendRequestAsync(this.Method, url, TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers, bodyContent);
            var isSuccess = response.IsSuccessful;
            var message = $"{response.StatusCode} {response.StatusDescription}";

            if (isSuccess)
            {

                var content = await Task.Run(() => JsonConvert.DeserializeObject<CreateResult>(response.Content));

                var expectedResult = new CreateResult { created = 1, restricted = 0 };

                // Сначала проверяем корректноть отработки запроса на создание - 
                // Ищем методом Search с фильтром по координам наш элемент               
                if (expectedResult.Equals(content))
                {
                    object calendarFilter = new SimpleFilter { value = "2010-01-01", type = SimpleFilterType.calendar, condition = FilterCondition.equals };
                    var filter = new
                    {
                        operation = "and",
                        filters = new[]
                        {
                             calendarFilter,
                             new NamedFilter { value = 2, type = NamedFilterType.DimensionId, name=  "dim_Day_of_week", condition =  FilterCondition.equals },
                             new NamedFilter { value = 1, type = NamedFilterType.MeasureId, name=  "dim_Measures", condition =  FilterCondition.equals }
                        }
                    };

                    var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

                    var searchResponse = await _restService.SendRequestAsync(this.Method, GetSearchUrl(measureGroupId), TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers, filterContent);

                    isSuccess &= searchResponse.IsSuccessful;
                    message += $"{searchResponse.StatusCode} {searchResponse.StatusDescription}";

                    if (isSuccess)
                    {
                        var searchContent = await Task.Run(() => JsonConvert.DeserializeObject<ElementsListDto>(searchResponse.Content));

                        // Если запрос отработал корректно - получаем наш элемент и смотрим корректность значений
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
                            Value = 2010
                        }
                    }
                        };

                        if (!expectedContent.Equals(searchContent))
                        {
                            isSuccess = false;
                            message += $"Полученное метоописание элемента группы показателей {measureGroupId} {measureGroupName} не соответствует ожидаемому";
                        }
                    }

                }
                else
                {
                    isSuccess &= false;
                    message = $"Ошибка при создании элемента группы показателей {measureGroupName}";
                }
            }

            Assert.True(isSuccess, message);           
        }

        /// <summary>
        /// Кейс Заполнен комментарий и атрибут при наличии у ГП атрибута типа Long
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Create_WithFilledCommentAndAttribute_ForMGWithLongAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_long_create";
            var measureGroupName = "long атрибут. Тестирование создания элементов.";

            var url = this.GetUrl(measureGroupId);
            object value = null;

            var body = new[]
            {
                new
                {
                    MustUpdateComment = true,
                    Comment = "filled",
                    Value = value,
                    Dimensions = new[]
                    {
                        new { Id = "dim_Day_of_week", ElementId = 3 }
                    },
                    Measure = new { Id = "dim_Measures", ElementId = 1 },
                    Calendars = new[] { new { Name = "Год", Id = "cal_God", Date = "2010-01-01" } },
                    Attributes =  new[]
                    {
                        new { id = "attr_long", value = 2010 }
                    }
                 }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var response = await _restService.SendRequestAsync(this.Method, url, TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers, bodyContent);
            var isSuccess = response.IsSuccessful;
            var message = $"{response.StatusCode} {response.StatusDescription}";

            if (isSuccess)
            {

                var content = await Task.Run(() => JsonConvert.DeserializeObject<CreateResult>(response.Content));

                var expectedResult = new CreateResult { created = 1, restricted = 0 };

                // Сначала проверяем корректноть отработки запроса на создание - 
                // Ищем методом Search с фильтром по координам наш элемент               
                if (expectedResult.Equals(content))
                {
                    object calendarFilter = new SimpleFilter { value = "2010-01-01", type = SimpleFilterType.calendar, condition = FilterCondition.equals };
                    var filter = new
                    {
                        operation = "and",
                        filters = new[]
                        {
                             calendarFilter,
                             new NamedFilter { value = 3, type = NamedFilterType.DimensionId, name=  "dim_Day_of_week", condition =  FilterCondition.equals },
                             new NamedFilter { value = 1, type = NamedFilterType.MeasureId, name=  "dim_Measures", condition =  FilterCondition.equals }
                        }
                    };

                    var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

                    var searchResponse = await _restService.SendRequestAsync(this.Method, GetSearchUrl(measureGroupId), TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers, filterContent);

                    isSuccess &= searchResponse.IsSuccessful;
                    message += $"{searchResponse.StatusCode} {searchResponse.StatusDescription}";

                    if (isSuccess)
                    {
                        var searchContent = await Task.Run(() => JsonConvert.DeserializeObject<ElementsListDto>(searchResponse.Content));

                        // Если запрос отработал корректно - получаем наш элемент и смотрим корректность значений
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
                            SystemInfo = "API",
                            Comment = "filled",
                            Value = null
                        }
                    }
                        };

                        if (!expectedContent.Equals(searchContent))
                        {
                            isSuccess = false;
                            message += $"Полученное метоописание элемента группы показателей {measureGroupId} {measureGroupName} не соответствует ожидаемому";
                        }
                    }

                }
                else
                {
                    isSuccess &= false;
                    message = $"Ошибка при создании элемента группы показателей {measureGroupName}";
                }
            }

            Assert.True(isSuccess, message);           
        }

        /// <summary>
        /// Кейс атрибута типа Long заполнен спец символами
        /// </summary>
        /// <returns>Ожидаемый результат - отрицательный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Create_WithFilledAttribute_WithрSpecialCharacters_ForMGWithLongAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_long_create";
            var measureGroupName = "long атрибут. Тестирование создания элементов.";

            var url = this.GetUrl(measureGroupId);

            object value = null;
            object commentValue = null;
            var attrValue = "!\"№;%:?*()_ + ";

            var body = new[]
            {
                new
                {
                    MustUpdateComment = false,
                    Comment = commentValue,
                    Value = value,
                    Dimensions = new[]
                    {
                        new { Id = "dim_Day_of_week", ElementId = 4 }
                    },
                    Measure = new { Id = "dim_Measures", ElementId = 1 },
                    Calendars = new[] { new { Name = "Год", Id = "cal_God", Date = "2010-01-01" } },
                    Attributes =  new[]
                    {
                        new { id = "attr_long", value = attrValue }
                    }
                 }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var response = await _restService.SendRequestAsync(this.Method, url, TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers, bodyContent);
            var isSuccess = response.IsSuccessful;
            var message = $"{response.StatusCode} {response.StatusDescription}";            

            Assert.True(!isSuccess, message);
        }

        /// <summary>
        /// Кейс атрибута типа Long заполнен дробным числом
        /// </summary>
        /// <returns>Ожидаемый результат - отрицательный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Create_WithFilledAttribute_WithDecimal_ForMGWithLongAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_long_create";
            var measureGroupName = "long атрибут. Тестирование создания элементов.";

            var url = this.GetUrl(measureGroupId);

            object value = null;
            object commentValue = null;
            var attrValue = "1234.56789";

            var body = new[]
            {
                new
                {
                    MustUpdateComment = false,
                    Comment = commentValue,
                    Value = value,
                    Dimensions = new[]
                    {
                        new { Id = "dim_Day_of_week", ElementId = 5 }
                    },
                    Measure = new { Id = "dim_Measures", ElementId = 1 },
                    Calendars = new[] { new { Name = "Год", Id = "cal_God", Date = "2010-01-01" } },
                    Attributes =  new[]
                    {
                        new { id = "attr_long", value = attrValue }
                    }
                 }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var response = await _restService.SendRequestAsync(this.Method, url, TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers, bodyContent);
            var isSuccess = response.IsSuccessful;

            Assert.True(!isSuccess, response.Content);
        }

        /// <summary>
        /// Кейс Заполнено только значение ячейки при наличии у ГП атрибута типа Decimal
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Create_WithFilledValue_ForMGWithDecimalAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_decimal_create";
            var measureGroupName = "decimal атрибут. Тестирование создания элементов.";

            var url = this.GetUrl(measureGroupId);

            object commentValue = null;
            object attrValue = null;

            var body = new[]
            {
                new
                {
                    MustUpdateComment = false,
                    Comment = commentValue,
                    Value = 2010,
                    Dimensions = new[]
                    {
                        new { Id = "dim_Day_of_week", ElementId = 1 }
                    },
                    Measure = new { Id = "dim_Measures", ElementId = 1 },
                    Calendars = new[] { new { Name = "Год", Id = "cal_God", Date = "2010-01-01" } },
                    Attributes =  new[]
                    {
                        new { id = "attr_decimal", value = attrValue }
                    }
                 }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var response = await _restService.SendRequestAsync(this.Method, url, TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers, bodyContent);
            var isSuccess = response.IsSuccessful;
            var message = $"{response.StatusCode} {response.StatusDescription}";

            if (isSuccess)
            {

                var content = await Task.Run(() => JsonConvert.DeserializeObject<CreateResult>(response.Content));

                var expectedResult = new CreateResult { created = 1, restricted = 0 };

                // Сначала проверяем корректноть отработки запроса на создание - 
                // Ищем методом Search с фильтром по координам наш элемент               
                if (expectedResult.Equals(content))
                {
                    object calendarFilter = new SimpleFilter { value = "2010-01-01", type = SimpleFilterType.calendar, condition = FilterCondition.equals };
                    var filter = new
                    {
                        operation = "and",
                        filters = new[]
                        {
                             calendarFilter,
                             new NamedFilter { value = 1, type = NamedFilterType.DimensionId, name=  "dim_Day_of_week", condition =  FilterCondition.equals },
                             new NamedFilter { value = 1, type = NamedFilterType.MeasureId, name=  "dim_Measures", condition =  FilterCondition.equals }
                        }
                    };

                    var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

                    var searchResponse = await _restService.SendRequestAsync(this.Method, GetSearchUrl(measureGroupId), TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers, filterContent);

                    isSuccess &= searchResponse.IsSuccessful;
                    message += $"{searchResponse.StatusCode} {searchResponse.StatusDescription}";

                    if (isSuccess)
                    {
                        var searchContent = await Task.Run(() => JsonConvert.DeserializeObject<ElementsListDto>(searchResponse.Content));

                        // Если запрос отработал корректно - получаем наш элемент и смотрим корректность значений
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
                            SystemInfo = "API",
                            Comment = null,
                            Value = 2010
                        }
                    }
                        };

                        if (!expectedContent.Equals(searchContent))
                        {
                            isSuccess = false;
                            message += $"Полученное метоописание элемента группы показателей {measureGroupId} {measureGroupName} не соответствует ожидаемому";
                        }
                    }

                }
                else
                {
                    isSuccess &= false;
                    message = $"Ошибка при создании элемента группы показателей {measureGroupName}";
                }
            }

            Assert.True(isSuccess, message);
        }

        /// <summary>
        /// Кейс Ячейка заполнена полностью при наличии у ГП атрибута типа Decimal
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Create_WithFilledCell_ForMGWithDecimalAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_decimal_create";
            var measureGroupName = "decimal атрибут. Тестирование создания элементов.";

            var url = this.GetUrl(measureGroupId);

            var body = new[]
            {
                new
                {
                    MustUpdateComment = true,
                    Comment = "filled",
                    Value = 2010,
                    Dimensions = new[]
                    {
                        new { Id = "dim_Day_of_week", ElementId = 2 }
                    },
                    Measure = new { Id = "dim_Measures", ElementId = 1 },
                    Calendars = new[] { new { Name = "Год", Id = "cal_God", Date = "2010-01-01" } },
                    Attributes =  new[]
                    {
                        new { id = "attr_decimal", value = 2010.123 }
                    }
                 }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var response = await _restService.SendRequestAsync(this.Method, url, TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers, bodyContent);
            var isSuccess = response.IsSuccessful;
            var message = $"{response.StatusCode} {response.StatusDescription}";

            if (isSuccess)
            {

                var content = await Task.Run(() => JsonConvert.DeserializeObject<CreateResult>(response.Content));

                var expectedResult = new CreateResult { created = 1, restricted = 0 };

                // Сначала проверяем корректноть отработки запроса на создание - 
                // Ищем методом Search с фильтром по координам наш элемент               
                if (expectedResult.Equals(content))
                {
                    object calendarFilter = new SimpleFilter { value = "2010-01-01", type = SimpleFilterType.calendar, condition = FilterCondition.equals };
                    var filter = new
                    {
                        operation = "and",
                        filters = new[]
                        {
                             calendarFilter,
                             new NamedFilter { value = 2, type = NamedFilterType.DimensionId, name=  "dim_Day_of_week", condition =  FilterCondition.equals },
                             new NamedFilter { value = 1, type = NamedFilterType.MeasureId, name=  "dim_Measures", condition =  FilterCondition.equals }
                        }
                    };

                    var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

                    var searchResponse = await _restService.SendRequestAsync(this.Method, GetSearchUrl(measureGroupId), TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers, filterContent);

                    isSuccess &= searchResponse.IsSuccessful;
                    message += $"{searchResponse.StatusCode} {searchResponse.StatusDescription}";

                    if (isSuccess)
                    {
                        var searchContent = await Task.Run(() => JsonConvert.DeserializeObject<ElementsListDto>(searchResponse.Content));

                        // Если запрос отработал корректно - получаем наш элемент и смотрим корректность значений
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
                                new MeasureGroupElementAttributeDto { AttributeId = "attr_decimal", Value = 2010.123 }
                            },
                            SystemInfo = "API",
                            Comment = "filled",
                            Value = 2010
                        }
                    }
                        };

                        if (!expectedContent.Equals(searchContent))
                        {
                            isSuccess = false;
                            message += $"Полученное метоописание элемента группы показателей {measureGroupId} {measureGroupName} не соответствует ожидаемому";
                        }
                    }

                }
                else
                {
                    isSuccess &= false;
                    message = $"Ошибка при создании элемента группы показателей {measureGroupName}";
                }
            }

            Assert.True(isSuccess, message);
        }

        /// <summary>
        /// Кейс Заполнен комментарий и атрибут при наличии у ГП атрибута типа Decimal
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Create_WithFilledCommentAndAttribute_ForMGWithDecimalAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_decimal_create";
            var measureGroupName = "decimal атрибут. Тестирование создания элементов.";

            var url = this.GetUrl(measureGroupId);
            object value = null;

            var body = new[]
            {
                new
                {
                    MustUpdateComment = true,
                    Comment = "filled",
                    Value = value,
                    Dimensions = new[]
                    {
                        new { Id = "dim_Day_of_week", ElementId = 3 }
                    },
                    Measure = new { Id = "dim_Measures", ElementId = 1 },
                    Calendars = new[] { new { Name = "Год", Id = "cal_God", Date = "2010-01-01" } },
                    Attributes =  new[]
                    {
                        new { id = "attr_decimal", value = 2010.123 }
                    }
                 }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var response = await _restService.SendRequestAsync(this.Method, url, TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers, bodyContent);
            var isSuccess = response.IsSuccessful;
            var message = $"{response.StatusCode} {response.StatusDescription}";

            if (isSuccess)
            {

                var content = await Task.Run(() => JsonConvert.DeserializeObject<CreateResult>(response.Content));

                var expectedResult = new CreateResult { created = 1, restricted = 0 };

                // Сначала проверяем корректноть отработки запроса на создание - 
                // Ищем методом Search с фильтром по координам наш элемент               
                if (expectedResult.Equals(content))
                {
                    object calendarFilter = new SimpleFilter { value = "2010-01-01", type = SimpleFilterType.calendar, condition = FilterCondition.equals };
                    var filter = new
                    {
                        operation = "and",
                        filters = new[]
                        {
                             calendarFilter,
                             new NamedFilter { value = 3, type = NamedFilterType.DimensionId, name=  "dim_Day_of_week", condition =  FilterCondition.equals },
                             new NamedFilter { value = 1, type = NamedFilterType.MeasureId, name=  "dim_Measures", condition =  FilterCondition.equals }
                        }
                    };

                    var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

                    var searchResponse = await _restService.SendRequestAsync(this.Method, GetSearchUrl(measureGroupId), TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers, filterContent);

                    isSuccess &= searchResponse.IsSuccessful;
                    message += $"{searchResponse.StatusCode} {searchResponse.StatusDescription}";

                    if (isSuccess)
                    {
                        var searchContent = await Task.Run(() => JsonConvert.DeserializeObject<ElementsListDto>(searchResponse.Content));

                        // Если запрос отработал корректно - получаем наш элемент и смотрим корректность значений
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
                                new MeasureGroupElementAttributeDto { AttributeId = "attr_decimal", Value = 2010.123 }
                            },
                            SystemInfo = "API",
                            Comment = "filled",
                            Value = null
                        }
                    }
                        };

                        if (!expectedContent.Equals(searchContent))
                        {
                            isSuccess = false;
                            message += $"Полученное метоописание элемента группы показателей {measureGroupId} {measureGroupName} не соответствует ожидаемому";
                        }
                    }

                }
                else
                {
                    isSuccess &= false;
                    message = $"Ошибка при создании элемента группы показателей {measureGroupName}";
                }
            }

            Assert.True(isSuccess, message);
        }

        /// <summary>
        /// Кейс атрибута типа Decimal заполнен спец символами
        /// </summary>
        /// <returns>Ожидаемый результат - отрицательный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Create_WithFilledAttribute_WithрSpecialCharacters_ForMGWithDecimalAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_decimal_create";
            var measureGroupName = "decimal атрибут. Тестирование создания элементов.";

            var url = this.GetUrl(measureGroupId);

            object value = null;
            object commentValue = null;
            var attrValue = "!\"№;%:?*()_ + ";

            var body = new[]
            {
                new
                {
                    MustUpdateComment = false,
                    Comment = commentValue,
                    Value = value,
                    Dimensions = new[]
                    {
                        new { Id = "dim_Day_of_week", ElementId = 4 }
                    },
                    Measure = new { Id = "dim_Measures", ElementId = 1 },
                    Calendars = new[] { new { Name = "Год", Id = "cal_God", Date = "2010-01-01" } },
                    Attributes =  new[]
                    {
                        new { id = "attr_decimal", value = attrValue }
                    }
                 }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var response = await _restService.SendRequestAsync(this.Method, url, TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers, bodyContent);
            var isSuccess = response.IsSuccessful;

            Assert.True(!isSuccess, response.Content);
        }

        /// <summary>
        /// Кейс 1 атрибута типа Boolean
        /// Value	Comment	Attribute	SystemInfo
        ///  empty  empty   no          empty
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Create_Case1_ForMGWithBooleanAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_boolean_create";
            var measureGroupName = "boolean атрибут. Тестирование создания элементов.";

            var url = this.GetUrl(measureGroupId);

            object commentValue = null;
            object value = null;

            var body = new[]
            {
                new
                {
                    MustUpdateComment = false,
                    Comment = commentValue,
                    Value = value,
                    Dimensions = new[]
                    {
                        new { Id = "dim_Day_of_week", ElementId = 1 }
                    },
                    Measure = new { Id = "dim_Measures", ElementId = 1 },
                    Calendars = new[] { new { Name = "Год", Id = "cal_God", Date = "2010-01-01" } },
                    Attributes =  new[]
                    {
                        new { id = "attr_boolean", value = false }
                    }
                 }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var response = await _restService.SendRequestAsync(this.Method, url, TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers, bodyContent);
            var isSuccess = response.IsSuccessful;
            var message = $"{response.StatusCode} {response.StatusDescription}";

            if (isSuccess)
            {

                var content = await Task.Run(() => JsonConvert.DeserializeObject<CreateResult>(response.Content));

                var expectedResult = new CreateResult { created = 1, restricted = 0 };

                // Сначала проверяем корректноть отработки запроса на создание - 
                // Ищем методом Search с фильтром по координам наш элемент               
                if (expectedResult.Equals(content))
                {
                    object calendarFilter = new SimpleFilter { value = "2010-01-01", type = SimpleFilterType.calendar, condition = FilterCondition.equals };
                    var filter = new
                    {
                        operation = "and",
                        filters = new[]
                        {
                             calendarFilter,
                             new NamedFilter { value = 1, type = NamedFilterType.DimensionId, name=  "dim_Day_of_week", condition =  FilterCondition.equals },
                             new NamedFilter { value = 1, type = NamedFilterType.MeasureId, name=  "dim_Measures", condition =  FilterCondition.equals }
                        }
                    };

                    var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

                    var searchResponse = await _restService.SendRequestAsync(this.Method, GetSearchUrl(measureGroupId), TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers, filterContent);

                    isSuccess &= searchResponse.IsSuccessful;
                    message += $"{searchResponse.StatusCode} {searchResponse.StatusDescription}";

                    if (isSuccess)
                    {
                        var searchContent = await Task.Run(() => JsonConvert.DeserializeObject<ElementsListDto>(searchResponse.Content));

                        // Если запрос отработал корректно - получаем наш элемент и смотрим корректность значений
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
                            new MeasureGroupBooleanAttributeDto
                            {
                                Name = "boolean",
                                Id = "attr_boolean",
                                TypeCode = (int)MeasureGroupAttributeType.Boolean,
                                TypeName =  "Boolean",
                                TrueValueText = "Yes",
                                FalseValueText = "No",
                                NullValueText = "Empty"
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
                                new MeasureGroupElementAttributeDto { AttributeId = "attr_boolean", Value = false, DisplayValue = "No" }
                            },
                            SystemInfo = "API",
                            Comment = null,
                            Value = null
                        }
                    }
                        };

                        if (!expectedContent.Equals(searchContent))
                        {
                            isSuccess = false;
                            message += $"Полученное метоописание элемента группы показателей {measureGroupId} {measureGroupName} не соответствует ожидаемому";
                        }
                    }

                }
                else
                {
                    isSuccess &= false;
                    message = $"Ошибка при создании элемента группы показателей {measureGroupName}";
                }
            }

            Assert.True(isSuccess, message);
        }

        /// <summary>
        /// Кейс 2 атрибута типа Boolean
        /// Value Comment Attribute SystemInfo
        /// filled filled  empty    filled
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Create_Case2_ForMGWithBooleanAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_boolean_create";
            var measureGroupName = "boolean атрибут. Тестирование создания элементов.";

            var url = this.GetUrl(measureGroupId);

            object attrValue = null;

            var body = new[]
            {
                new
                {
                    MustUpdateComment = true,
                    Comment = "filled",
                    Value = 2010,
                    Dimensions = new[]
                    {
                        new { Id = "dim_Day_of_week", ElementId = 2 }
                    },
                    Measure = new { Id = "dim_Measures", ElementId = 1 },
                    Calendars = new[] { new { Name = "Год", Id = "cal_God", Date = "2010-01-01" } },
                    Attributes =  new[]
                    {
                        new { id = "attr_boolean", value = attrValue }
                    }
                 }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var response = await _restService.SendRequestAsync(this.Method, url, TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers, bodyContent);
            var isSuccess = response.IsSuccessful;
            var message = $"{response.StatusCode} {response.StatusDescription}";

            if (isSuccess)
            {

                var content = await Task.Run(() => JsonConvert.DeserializeObject<CreateResult>(response.Content));

                var expectedResult = new CreateResult { created = 1, restricted = 0 };

                // Сначала проверяем корректноть отработки запроса на создание - 
                // Ищем методом Search с фильтром по координам наш элемент               
                if (expectedResult.Equals(content))
                {
                    object calendarFilter = new SimpleFilter { value = "2010-01-01", type = SimpleFilterType.calendar, condition = FilterCondition.equals };
                    var filter = new
                    {
                        operation = "and",
                        filters = new[]
                        {
                             calendarFilter,
                             new NamedFilter { value = 2, type = NamedFilterType.DimensionId, name=  "dim_Day_of_week", condition =  FilterCondition.equals },
                             new NamedFilter { value = 1, type = NamedFilterType.MeasureId, name=  "dim_Measures", condition =  FilterCondition.equals }
                        }
                    };

                    var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

                    var searchResponse = await _restService.SendRequestAsync(this.Method, GetSearchUrl(measureGroupId), TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers, filterContent);

                    isSuccess &= searchResponse.IsSuccessful;
                    message += $"{searchResponse.StatusCode} {searchResponse.StatusDescription}";

                    if (isSuccess)
                    {
                        var searchContent = await Task.Run(() => JsonConvert.DeserializeObject<ElementsListDto>(searchResponse.Content));

                        // Если запрос отработал корректно - получаем наш элемент и смотрим корректность значений
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
                            new MeasureGroupBooleanAttributeDto
                            {
                                Name = "boolean",
                                Id = "attr_boolean",
                                TypeCode = (int)MeasureGroupAttributeType.Boolean,
                                TypeName =  "Boolean",
                                TrueValueText = "Yes",
                                FalseValueText = "No",
                                NullValueText = "Empty"
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
                                new MeasureGroupElementAttributeDto { AttributeId = "attr_boolean", Value = null, DisplayValue = "Empty" }
                            },
                            SystemInfo = "API",
                            Comment = "filled",
                            Value = 2010
                        }
                    }
                        };

                        if (!expectedContent.Equals(searchContent))
                        {
                            isSuccess = false;
                            message += $"Полученное метоописание элемента группы показателей {measureGroupId} {measureGroupName} не соответствует ожидаемому";
                        }
                    }

                }
                else
                {
                    isSuccess &= false;
                    message = $"Ошибка при создании элемента группы показателей {measureGroupName}";
                }
            }

            Assert.True(isSuccess, message);
        }

        /// <summary>
        /// Кейс 3 атрибута типа Boolean
        /// Value Comment Attribute SystemInfo
        /// empty filled	yes	    filled
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Create_Case3_ForMGWithBooleanAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_boolean_create";
            var measureGroupName = "boolean атрибут. Тестирование создания элементов.";

            var url = this.GetUrl(measureGroupId);

            object value = null;

            var body = new[]
            {
                new
                {
                    MustUpdateComment = true,
                    Comment = "filled",
                    Value = value,
                    Dimensions = new[]
                    {
                        new { Id = "dim_Day_of_week", ElementId = 3 }
                    },
                    Measure = new { Id = "dim_Measures", ElementId = 1 },
                    Calendars = new[] { new { Name = "Год", Id = "cal_God", Date = "2010-01-01" } },
                    Attributes =  new[]
                    {
                        new { id = "attr_boolean", value = true }
                    }
                 }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var response = await _restService.SendRequestAsync(this.Method, url, TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers, bodyContent);
            var isSuccess = response.IsSuccessful;
            var message = $"{response.StatusCode} {response.StatusDescription}";

            if (isSuccess)
            {

                var content = await Task.Run(() => JsonConvert.DeserializeObject<CreateResult>(response.Content));

                var expectedResult = new CreateResult { created = 1, restricted = 0 };

                // Сначала проверяем корректноть отработки запроса на создание - 
                // Ищем методом Search с фильтром по координам наш элемент               
                if (expectedResult.Equals(content))
                {
                    object calendarFilter = new SimpleFilter { value = "2010-01-01", type = SimpleFilterType.calendar, condition = FilterCondition.equals };
                    var filter = new
                    {
                        operation = "and",
                        filters = new[]
                        {
                             calendarFilter,
                             new NamedFilter { value = 3, type = NamedFilterType.DimensionId, name=  "dim_Day_of_week", condition =  FilterCondition.equals },
                             new NamedFilter { value = 1, type = NamedFilterType.MeasureId, name=  "dim_Measures", condition =  FilterCondition.equals }
                        }
                    };

                    var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

                    var searchResponse = await _restService.SendRequestAsync(this.Method, GetSearchUrl(measureGroupId), TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers, filterContent);

                    isSuccess &= searchResponse.IsSuccessful;
                    message += $"{searchResponse.StatusCode} {searchResponse.StatusDescription}";

                    if (isSuccess)
                    {
                        var searchContent = await Task.Run(() => JsonConvert.DeserializeObject<ElementsListDto>(searchResponse.Content));

                        // Если запрос отработал корректно - получаем наш элемент и смотрим корректность значений
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
                            new MeasureGroupBooleanAttributeDto
                            {
                                Name = "boolean",
                                Id = "attr_boolean",
                                TypeCode = (int)MeasureGroupAttributeType.Boolean,
                                TypeName =  "Boolean",
                                TrueValueText = "Yes",
                                FalseValueText = "No",
                                NullValueText = "Empty"
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
                                new MeasureGroupElementAttributeDto { AttributeId = "attr_boolean", Value = true, DisplayValue = "Yes" }
                            },
                            SystemInfo = "API",
                            Comment = "filled",
                            Value = null
                        }
                    }
                        };

                        if (!expectedContent.Equals(searchContent))
                        {
                            isSuccess = false;
                            message += $"Полученное метоописание элемента группы показателей {measureGroupId} {measureGroupName} не соответствует ожидаемому";
                        }
                    }

                }
                else
                {
                    isSuccess &= false;
                    message = $"Ошибка при создании элемента группы показателей {measureGroupName}";
                }
            }

            Assert.True(isSuccess, message);
        }

        /// <summary>
        /// Кейс 4 атрибута типа Boolean
        /// Value  Comment Attribute SystemInfo
        /// filled filled	no	     filled
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Create_Case4_ForMGWithBooleanAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_boolean_create";
            var measureGroupName = "boolean атрибут. Тестирование создания элементов.";

            var url = this.GetUrl(measureGroupId);

            var body = new[]
            {
                new
                {
                    MustUpdateComment = true,
                    Comment = "filled",
                    Value = 2010,
                    Dimensions = new[]
                    {
                        new { Id = "dim_Day_of_week", ElementId = 4 }
                    },
                    Measure = new { Id = "dim_Measures", ElementId = 1 },
                    Calendars = new[] { new { Name = "Год", Id = "cal_God", Date = "2010-01-01" } },
                    Attributes =  new[]
                    {
                        new { id = "attr_boolean", value = false }
                    }
                 }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var response = await _restService.SendRequestAsync(this.Method, url, TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers, bodyContent);
            var isSuccess = response.IsSuccessful;
            var message = $"{response.StatusCode} {response.StatusDescription}";

            if (isSuccess)
            {

                var content = await Task.Run(() => JsonConvert.DeserializeObject<CreateResult>(response.Content));

                var expectedResult = new CreateResult { created = 1, restricted = 0 };

                // Сначала проверяем корректноть отработки запроса на создание - 
                // Ищем методом Search с фильтром по координам наш элемент               
                if (expectedResult.Equals(content))
                {
                    object calendarFilter = new SimpleFilter { value = "2010-01-01", type = SimpleFilterType.calendar, condition = FilterCondition.equals };
                    var filter = new
                    {
                        operation = "and",
                        filters = new[]
                        {
                             calendarFilter,
                             new NamedFilter { value = 4, type = NamedFilterType.DimensionId, name=  "dim_Day_of_week", condition =  FilterCondition.equals },
                             new NamedFilter { value = 1, type = NamedFilterType.MeasureId, name=  "dim_Measures", condition =  FilterCondition.equals }
                        }
                    };

                    var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

                    var searchResponse = await _restService.SendRequestAsync(this.Method, GetSearchUrl(measureGroupId), TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers, filterContent);

                    isSuccess &= searchResponse.IsSuccessful;
                    message += $"{searchResponse.StatusCode} {searchResponse.StatusDescription}";

                    if (isSuccess)
                    {
                        var searchContent = await Task.Run(() => JsonConvert.DeserializeObject<ElementsListDto>(searchResponse.Content));

                        // Если запрос отработал корректно - получаем наш элемент и смотрим корректность значений
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
                            new MeasureGroupBooleanAttributeDto
                            {
                                Name = "boolean",
                                Id = "attr_boolean",
                                TypeCode = (int)MeasureGroupAttributeType.Boolean,
                                TypeName =  "Boolean",
                                TrueValueText = "Yes",
                                FalseValueText = "No",
                                NullValueText = "Empty"
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
                                new MeasureGroupElementAttributeDto { AttributeId = "attr_boolean", Value = false, DisplayValue = "No" }
                            },
                            SystemInfo = "API",
                            Comment = "filled",
                            Value = 2010
                        }
                    }
                        };

                        if (!expectedContent.Equals(searchContent))
                        {
                            isSuccess = false;
                            message += $"Полученное метоописание элемента группы показателей {measureGroupId} {measureGroupName} не соответствует ожидаемому";
                        }
                    }

                }
                else
                {
                    isSuccess &= false;
                    message = $"Ошибка при создании элемента группы показателей {measureGroupName}";
                }
            }

            Assert.True(isSuccess, message);
        }

        /// <summary>
        /// Кейс атрибута типа Boolean заполнен спец символами
        /// </summary>
        /// <returns>Ожидаемый результат - отрицательный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Create_WithFilledAttribute_WithрSpecialCharacters_ForMGWithBooleanAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_boolean_create";
            var measureGroupName = "boolean атрибут. Тестирование создания элементов.";

            var url = this.GetUrl(measureGroupId);

            object value = null;
            object commentValue = null;
            var attrValue = "!\"№;%:?*()_ + ";

            var body = new[]
            {
                new
                {
                    MustUpdateComment = false,
                    Comment = commentValue,
                    Value = value,
                    Dimensions = new[]
                    {
                        new { Id = "dim_Day_of_week", ElementId = 5 }
                    },
                    Measure = new { Id = "dim_Measures", ElementId = 1 },
                    Calendars = new[] { new { Name = "Год", Id = "cal_God", Date = "2010-01-01" } },
                    Attributes =  new[]
                    {
                        new { id = "attr_boolean", value = attrValue }
                    }
                 }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var response = await _restService.SendRequestAsync(this.Method, url, TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers, bodyContent);
            var isSuccess = response.IsSuccessful;

            Assert.True(!isSuccess, response.Content);
        }

        /// <summary>
        /// Кейс атрибута типа Boolean заполнен 1
        /// </summary>
        /// <returns>Ожидаемый результат - отрицательный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Create_WithFilledAttribute_WithрNumber1_ForMGWithBooleanAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_boolean_create";
            var measureGroupName = "boolean атрибут. Тестирование создания элементов.";

            var url = this.GetUrl(measureGroupId);

            object value = null;
            object commentValue = null;
            var attrValue = 1;

            var body = new[]
            {
                new
                {
                    MustUpdateComment = false,
                    Comment = commentValue,
                    Value = value,
                    Dimensions = new[]
                    {
                        new { Id = "dim_Day_of_week", ElementId = 6 }
                    },
                    Measure = new { Id = "dim_Measures", ElementId = 1 },
                    Calendars = new[] { new { Name = "Год", Id = "cal_God", Date = "2010-01-01" } },
                    Attributes =  new[]
                    {
                        new { id = "attr_boolean", value = attrValue }
                    }
                 }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var response = await _restService.SendRequestAsync(this.Method, url, TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers, bodyContent);
            var isSuccess = response.IsSuccessful;

            Assert.True(!isSuccess, response.Content);
        }

        /// <summary>
        /// Кейс Заполнено только значение ячейки при наличии у ГП атрибута типа Date
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Create_WithFilledValue_ForMGWithDateAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_date_create";
            var measureGroupName = "date атрибут. Тестирование создания элементов.";

            var url = this.GetUrl(measureGroupId);

            object attrValue = null;
            object commentValue = null;

            var body = new[]
            {
                new
                {
                    MustUpdateComment = false,
                    Comment = commentValue,
                    Value = 2010,
                    Dimensions = new[]
                    {
                        new { Id = "dim_Day_of_week", ElementId = 1 }
                    },
                    Measure = new { Id = "dim_Measures", ElementId = 1 },
                    Calendars = new[] { new { Name = "Год", Id = "cal_God", Date = "2010-01-01" } },
                    Attributes =  new[]
                    {
                        new { id = "attr_date", value = attrValue }
                    }
                 }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var response = await _restService.SendRequestAsync(this.Method, url, TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers, bodyContent);
            var isSuccess = response.IsSuccessful;
            var message = $"{response.StatusCode} {response.StatusDescription}";

            if (isSuccess)
            {

                var content = await Task.Run(() => JsonConvert.DeserializeObject<CreateResult>(response.Content));

                var expectedResult = new CreateResult { created = 1, restricted = 0 };

                // Сначала проверяем корректноть отработки запроса на создание - 
                // Ищем методом Search с фильтром по координам наш элемент               
                if (expectedResult.Equals(content))
                {
                    object calendarFilter = new SimpleFilter { value = "2010-01-01", type = SimpleFilterType.calendar, condition = FilterCondition.equals };
                    var filter = new
                    {
                        operation = "and",
                        filters = new[]
                        {
                             calendarFilter,
                             new NamedFilter { value = 1, type = NamedFilterType.DimensionId, name=  "dim_Day_of_week", condition =  FilterCondition.equals },
                             new NamedFilter { value = 1, type = NamedFilterType.MeasureId, name=  "dim_Measures", condition =  FilterCondition.equals }
                        }
                    };

                    var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

                    var searchResponse = await _restService.SendRequestAsync(this.Method, GetSearchUrl(measureGroupId), TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers, filterContent);

                    isSuccess &= searchResponse.IsSuccessful;
                    message += $"{searchResponse.StatusCode} {searchResponse.StatusDescription}";

                    if (isSuccess)
                    {
                        var searchContent = await Task.Run(() => JsonConvert.DeserializeObject<ElementsListDto>(searchResponse.Content));

                        // Если запрос отработал корректно - получаем наш элемент и смотрим корректность значений
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
                                new MeasureGroupDateAttributeDto
                                {
                                    Name = "date",
                                    Id = "attr_date",
                                    TypeCode = (int)MeasureGroupAttributeType.Date,
                                    TypeName =  "Date",
                                    MinDate = "2020-01-01",
                                    MaxDate = "2030-12-31"
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
                                new MeasureGroupElementAttributeDto { AttributeId = "attr_date", Value = null }
                            },
                            SystemInfo = "API",
                            Comment = null,
                            Value = 2010
                        }
                    }
                        };

                        if (!expectedContent.Equals(searchContent))
                        {
                            isSuccess = false;
                            message += $"Полученное метоописание элемента группы показателей {measureGroupId} {measureGroupName} не соответствует ожидаемому";
                        }
                    }

                }
                else
                {
                    isSuccess &= false;
                    message = $"Ошибка при создании элемента группы показателей {measureGroupName}";
                }
            }

            Assert.True(isSuccess, message);
        }

        /// <summary>
        /// Кейс Ячейка заполнена полностью при наличии у ГП атрибута типа Date
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Create_WithFilledCell_ForMGWithDateAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_date_create";
            var measureGroupName = "date атрибут. Тестирование создания элементов.";

            var url = this.GetUrl(measureGroupId);

            var body = new[]
            {
                new
                {
                    MustUpdateComment = true,
                    Comment = "filled",
                    Value = 2010,
                    Dimensions = new[]
                    {
                        new { Id = "dim_Day_of_week", ElementId = 2 }
                    },
                    Measure = new { Id = "dim_Measures", ElementId = 1 },
                    Calendars = new[] { new { Name = "Год", Id = "cal_God", Date = "2010-01-01" } },
                    Attributes =  new[]
                    {
                        new { id = "attr_date", value = "2030-12-31" }
                    }
                 }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var response = await _restService.SendRequestAsync(this.Method, url, TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers, bodyContent);
            var isSuccess = response.IsSuccessful;
            var message = $"{response.StatusCode} {response.StatusDescription}";

            if (isSuccess)
            {

                var content = await Task.Run(() => JsonConvert.DeserializeObject<CreateResult>(response.Content));

                var expectedResult = new CreateResult { created = 1, restricted = 0 };

                // Сначала проверяем корректноть отработки запроса на создание - 
                // Ищем методом Search с фильтром по координам наш элемент  
                //if (expectedResult.Equals(content))
                    if (true)
                {
                    object calendarFilter = new SimpleFilter { value = "2010-01-01", type = SimpleFilterType.calendar, condition = FilterCondition.equals };
                    var filter = new
                    {
                        operation = "and",
                        filters = new[]
                        {
                             calendarFilter,
                             new NamedFilter { value = 2, type = NamedFilterType.DimensionId, name=  "dim_Day_of_week", condition =  FilterCondition.equals },
                             new NamedFilter { value = 1, type = NamedFilterType.MeasureId, name=  "dim_Measures", condition =  FilterCondition.equals }
                        }
                    };

                    var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

                    var searchResponse = await _restService.SendRequestAsync(this.Method, GetSearchUrl(measureGroupId), TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers, filterContent);

                    isSuccess &= searchResponse.IsSuccessful;
                    message += $"{searchResponse.StatusCode} {searchResponse.StatusDescription}";

                    if (isSuccess)
                    {
                        var searchContent = await Task.Run(() => JsonConvert.DeserializeObject<ElementsListDto>(searchResponse.Content));

                        // Если запрос отработал корректно - получаем наш элемент и смотрим корректность значений
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
                                new MeasureGroupDateAttributeDto
                                {
                                    Name = "date",
                                    Id = "attr_date",
                                    TypeCode = (int)MeasureGroupAttributeType.Date,
                                    TypeName =  "Date",
                                    MinDate = "2020-01-01",
                                    MaxDate = "2030-12-31"
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
                                new MeasureGroupElementAttributeDto { AttributeId = "attr_date", Value = "2030-12-31" }
                            },
                            SystemInfo = "API",
                            Comment = "filled",
                            Value = 2010
                        }
                    }
                        };

                        if (!expectedContent.Equals(searchContent))
                        {
                            isSuccess = false;
                            message += $"Полученное метоописание элемента группы показателей {measureGroupId} {measureGroupName} не соответствует ожидаемому";
                        }
                    }

                }
                else
                {
                    isSuccess &= false;
                    message = $"Ошибка при создании элемента группы показателей {measureGroupName}";
                }
            }

            Assert.True(isSuccess, message);            
        }

        /// <summary>
        /// Кейс Заполнен комментарий и атрибут при наличии у ГП атрибута типа Date
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Create_WithFilledCommentAndAttribute_ForMGWithDateAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_date_create";
            var measureGroupName = "date атрибут. Тестирование создания элементов.";

            var url = this.GetUrl(measureGroupId);
            object value = null;

            var body = new[]
            {
                new
                {
                    MustUpdateComment = true,
                    Comment = "filled",
                    Value = value,
                    Dimensions = new[]
                    {
                        new { Id = "dim_Day_of_week", ElementId = 3 }
                    },
                    Measure = new { Id = "dim_Measures", ElementId = 1 },
                    Calendars = new[] { new { Name = "Год", Id = "cal_God", Date = "2010-01-01" } },
                    Attributes =  new[]
                    {
                        new { id = "attr_date", value = "2030-12-31" }
                    }
                 }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var response = await _restService.SendRequestAsync(this.Method, url, TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers, bodyContent);
            var isSuccess = response.IsSuccessful;
            var message = $"{response.StatusCode} {response.StatusDescription}";

            if (isSuccess)
            {
                var content = await Task.Run(() => JsonConvert.DeserializeObject<CreateResult>(response.Content));
                var expectedResult = new CreateResult { created = 1, restricted = 0 };

                // Сначала проверяем корректноть отработки запроса на создание - 
                // Ищем методом Search с фильтром по координам наш элемент               
                if (expectedResult.Equals(content))
                {
                    object calendarFilter = new SimpleFilter { value = "2010-01-01", type = SimpleFilterType.calendar, condition = FilterCondition.equals };
                    var filter = new
                    {
                        operation = "and",
                        filters = new[]
                        {
                             calendarFilter,
                             new NamedFilter { value = 3, type = NamedFilterType.DimensionId, name=  "dim_Day_of_week", condition =  FilterCondition.equals },
                             new NamedFilter { value = 1, type = NamedFilterType.MeasureId, name=  "dim_Measures", condition =  FilterCondition.equals }
                        }
                    };

                    var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

                    var searchResponse = await _restService.SendRequestAsync(this.Method, GetSearchUrl(measureGroupId), TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers, filterContent);

                    isSuccess &= searchResponse.IsSuccessful;
                    message += $"{searchResponse.StatusCode} {searchResponse.StatusDescription}";

                    if (isSuccess)
                    {
                        var searchContent = await Task.Run(() => JsonConvert.DeserializeObject<ElementsListDto>(searchResponse.Content));

                        // Если запрос отработал корректно - получаем наш элемент и смотрим корректность значений
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
                                new MeasureGroupDateAttributeDto
                                {
                                    Name = "date",
                                    Id = "attr_date",
                                    TypeCode = (int)MeasureGroupAttributeType.Date,
                                    TypeName =  "Date",
                                    MinDate = "2020-01-01",
                                    MaxDate = "2030-12-31"
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
                                new MeasureGroupElementAttributeDto { AttributeId = "attr_date", Value = "2030-12-31" }
                            },
                            SystemInfo = "API",
                            Comment = "filled",
                            Value = null
                        }
                    }
                        };

                        if (!expectedContent.Equals(searchContent))
                        {
                            isSuccess = false;
                            message += $"Полученное метоописание элемента группы показателей {measureGroupId} {measureGroupName} не соответствует ожидаемому";
                        }
                    }

                }
                else
                {
                    isSuccess &= false;
                    message = $"Ошибка при создании элемента группы показателей {measureGroupName}";
                }
            }

            Assert.True(isSuccess, message);
        }

        /// <summary>+
        /// Кейс Не заполнена информация только по атрибуту при наличии у ГП атрибута типа Date
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Create_EmptyOnlyAttribute_ForMGWithDateAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_date_create";
            var measureGroupName = "date атрибут. Тестирование создания элементов.";

            var url = this.GetUrl(measureGroupId);

            object value = null;
            object commentValue = null;

            var body = new[]
            {
                new
                {
                    MustUpdateComment = false,
                    Comment = commentValue,
                    Value = value,
                    Dimensions = new[]
                    {
                        new { Id = "dim_Day_of_week", ElementId = 4 }
                    },
                    Measure = new { Id = "dim_Measures", ElementId = 1 },
                    Calendars = new[] { new { Name = "Год", Id = "cal_God", Date = "2010-01-01" } },
                    Attributes =  new[]
                    {
                        new { id = "attr_date", value = "2030-12-31" }
                    }
                 }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var response = await _restService.SendRequestAsync(this.Method, url, TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers, bodyContent);
            var isSuccess = response.IsSuccessful;
            var message = $"{response.StatusCode} {response.StatusDescription}";

            if (isSuccess)
            {

                var content = await Task.Run(() => JsonConvert.DeserializeObject<CreateResult>(response.Content));

                var expectedResult = new CreateResult { created = 1, restricted = 0 };

                // Сначала проверяем корректноть отработки запроса на создание - 
                // Ищем методом Search с фильтром по координам наш элемент               
                if (expectedResult.Equals(content))
                {
                    object calendarFilter = new SimpleFilter { value = "2010-01-01", type = SimpleFilterType.calendar, condition = FilterCondition.equals };
                    var filter = new
                    {
                        operation = "and",
                        filters = new[]
                        {
                             calendarFilter,
                             new NamedFilter { value = 4, type = NamedFilterType.DimensionId, name=  "dim_Day_of_week", condition =  FilterCondition.equals },
                             new NamedFilter { value = 1, type = NamedFilterType.MeasureId, name=  "dim_Measures", condition =  FilterCondition.equals }
                        }
                    };

                    var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

                    var searchResponse = await _restService.SendRequestAsync(this.Method, GetSearchUrl(measureGroupId), TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers, filterContent);

                    isSuccess &= searchResponse.IsSuccessful;
                    message += $"{searchResponse.StatusCode} {searchResponse.StatusDescription}";

                    if (isSuccess)
                    {
                        var searchContent = await Task.Run(() => JsonConvert.DeserializeObject<ElementsListDto>(searchResponse.Content));

                        // Если запрос отработал корректно - получаем наш элемент и смотрим корректность значений
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
                                new MeasureGroupDateAttributeDto
                                {
                                    Name = "date",
                                    Id = "attr_date",
                                    TypeCode = (int)MeasureGroupAttributeType.Date,
                                    TypeName =  "Date",
                                    MinDate = "2020-01-01",
                                    MaxDate = "2030-12-31"
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
                                new MeasureGroupElementAttributeDto { AttributeId = "attr_date", Value = "2030-12-31" }
                            },
                            SystemInfo = "API",
                            Comment = null,
                            Value = null
                        }
                    }
                        };

                        if (!expectedContent.Equals(searchContent))
                        {
                            isSuccess = false;
                            message += $"Полученное метоописание элемента группы показателей {measureGroupId} {measureGroupName} не соответствует ожидаемому";
                        }
                    }

                }
                else
                {
                    isSuccess &= false;
                    message = $"Ошибка при создании элемента группы показателей {measureGroupName}";
                }
            }

            Assert.True(isSuccess, message);
        }

        /// <summary>
        /// Кейс Некорректный формат даты
        /// </summary>
        /// <returns>Ожидаемый результат - отрицательный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Create_WithFilledAttribute_WithрIncorrectFormat_ForMGWithDateAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_date_create";
            var measureGroupName = "date атрибут. Тестирование создания элементов.";

            var url = this.GetUrl(measureGroupId);

            object value = null;
            object commentValue = null;

            var body = new[]
            {
                new
                {
                    MustUpdateComment = false,
                    Comment = commentValue,
                    Value = value,
                    Dimensions = new[]
                    {
                        new { Id = "dim_Day_of_week", ElementId = 5 }
                    },
                    Measure = new { Id = "dim_Measures", ElementId = 1 },
                    Calendars = new[] { new { Name = "Год", Id = "cal_God", Date = "2010-01-01" } },
                    Attributes =  new[]
                    {
                        new { id = "attr_date", value = "2030-31-12" }
                    }
                 }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var response = await _restService.SendRequestAsync(this.Method, url, TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers, bodyContent);
            var isSuccess = response.IsSuccessful;

            Assert.True(!isSuccess, response.Content);
        }       

        protected override string GetUrl(string measureGroupId)
        {
            return string.Format(this.Url, measureGroupId);
        }

        protected override string GetSearchUrl(string measureGroupId)
        {
            return string.Format($"{config.GetValue("ApiUrl")}{config.GetValue("ApiUrlMeasureGroupElementsSearchPath")}", measureGroupId);
        }
    }
}
