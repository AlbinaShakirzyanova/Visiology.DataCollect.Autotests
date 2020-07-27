using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Visiology.DataCollect.Autotests.API.Infrastructure.Entities;
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

namespace Visiology.DataCollect.Autotests.API.Tests.MeasureGroups.Post.v2
{
    /// <summary>
    /// Класс тестирования метода поиска элементов группы показателей
    /// </summary>
    [XApiVersion("2.0")]
    public class SearchElementsTests : BaseTests<ElementsListDto, ElementDto>
    {
        public override string Url { get; set; }

        public override Method Method { get; set; } = Method.POST;

        /// <summary>
        /// Группа показателей для тестирования получения элементов 
        /// В дампе - "Группа показателей для тестирования получения элементов"
        /// </summary>
        private const string measureGroupId = "measureGroup_Gruppa_pokazatel_";

        public SearchElementsTests(TokenFixture tokenFixture, RestService restService)
            : base(tokenFixture, restService)
        {
            Url = string.Format($"{config.GetValue("ApiUrl")}{config.GetValue("ApiUrlMeasureGroupElementsSearchPath")}", measureGroupId);
        }

        /// <summary>
        /// Кейс Тестирование простого фильтра типа ID -  фильтрация по идентификатору элемента
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Search_Get_WithSimpleIdFilter()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_all_search";
            var measureGroupName = "all. Тестирование получения элементов";

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

        /// <summary>
        /// Кейс Тестирование простого фильтра типа formInstance -  фильтрацияф по идентификатору элемента
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Search_Get_WithSimpleFormInstanceFilter()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_all_search";
            var measureGroupName = "all. Тестирование получения элементов";

            var url = this.GetUrl(measureGroupId);

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(new SimpleFilter
            {
                value = "ft6",
                type = SimpleFilterType.formInstance,
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

        /// <summary>
        /// Кейс Тестирование именованного фильтра типа AttributeName для атрибута типа string
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Get_WithNamedAttributeNameFilter_ForStringAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_all_search";
            var measureGroupName = "all. Тестирование получения элементов";

            var url = this.GetUrl(measureGroupId);

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(new NamedFilter
            {
                value = "string",
                type = NamedFilterType.AttributeName,
                name = "string",
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

        /// <summary>
        /// Кейс Тестирование именованного фильтра типа AttributeId для атрибута типа string
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Get_WithNamedAttributeIdFilter_ForStringAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_all_search";
            var measureGroupName = "all. Тестирование получения элементов";

            var url = this.GetUrl(measureGroupId);

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(new NamedFilter
            {
                value = "string",
                type = NamedFilterType.AttributeId,
                name = "attr_string",
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

        /// <summary>
        /// Кейс Тестирование именованного фильтра типа AttributeName для атрибута типа long
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Get_WithNamedAttributeNameFilter_ForLongAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_all_search";
            var measureGroupName = "all. Тестирование получения элементов";

            var url = this.GetUrl(measureGroupId);

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(new NamedFilter
            {
                value = 2010,
                type = NamedFilterType.AttributeName,
                name = "long",
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

        /// <summary>
        /// Кейс Тестирование именованного фильтра типа AttributeId для атрибута типа long
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Get_WithNamedAttributeIdFilter_ForLongAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_all_search";
            var measureGroupName = "all. Тестирование получения элементов";

            var url = this.GetUrl(measureGroupId);

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(new NamedFilter
            {
                value = 2010,
                type = NamedFilterType.AttributeId,
                name = "attr_long",
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

        /// <summary>
        /// Кейс Тестирование именованного фильтра типа AttributeName для атрибута типа decimal
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Get_WithNamedAttributeNameFilter_ForDecimalAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_all_search";
            var measureGroupName = "all. Тестирование получения элементов";

            var url = this.GetUrl(measureGroupId);

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(new NamedFilter
            {
                value = 2010,
                type = NamedFilterType.AttributeName,
                name = "long",
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

        /// <summary>
        /// Кейс Тестирование именованного фильтра типа AttributeId для атрибута типа decimal
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Get_WithNamedAttributeIdFilter_ForDecimalAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_all_search";
            var measureGroupName = "all. Тестирование получения элементов";

            var url = this.GetUrl(measureGroupId);

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(new NamedFilter
            {
                value = 2010.21,
                type = NamedFilterType.AttributeId,
                name = "attr_decimal",
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

        /// <summary>
        /// Кейс Тестирование именованного фильтра типа AttributeName для атрибута типа boolean
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Get_WithNamedAttributeNameFilter_ForBooleanAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_all_search";
            var measureGroupName = "all. Тестирование получения элементов";

            var url = this.GetUrl(measureGroupId);

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(new NamedFilter
            {
                value = true,
                type = NamedFilterType.AttributeName,
                name = "boolean",
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

        /// <summary>
        /// Кейс Тестирование именованного фильтра типа AttributeId для атрибута типа boolean
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Get_WithNamedAttributeIdFilter_ForBooleanAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_all_search";
            var measureGroupName = "all. Тестирование получения элементов";

            var url = this.GetUrl(measureGroupId);

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(new NamedFilter
            {
                value = true,
                type = NamedFilterType.AttributeId,
                name = "attr_boolean",
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

        /// <summary>
        /// Кейс Тестирование именованного фильтра типа AttributeName для атрибута типа date
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Get_WithNamedAttributeNameFilter_ForDateAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_all_search";
            var measureGroupName = "all. Тестирование получения элементов";

            var url = this.GetUrl(measureGroupId);

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(new NamedFilter
            {
                value = "2030-12-30",
                type = NamedFilterType.AttributeName,
                name = "date",
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

        /// <summary>
        /// Кейс Тестирование именованного фильтра типа AttributeId для атрибута типа dare
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Get_WithNamedAttributeIdFilter_ForDateAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_all_search";
            var measureGroupName = "all. Тестирование получения элементов";

            var url = this.GetUrl(measureGroupId);

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(new NamedFilter
            {
                value = "2030-12-30",
                type = NamedFilterType.AttributeId,
                name = "attr_date",
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
            return string.Format($"{config.GetValue("ApiUrl")}{config.GetValue("ApiUrlMeasureGroupElementsSearchPath")}", entityId);
        }
    }
}
