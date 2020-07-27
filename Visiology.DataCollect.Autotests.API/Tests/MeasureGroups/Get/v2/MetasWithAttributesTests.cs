using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Visiology.DataCollect.Autotests.API.Models.MeasureGroups.Attributes;
using Visiology.DataCollect.Autotests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Impl;
using Visiology.DataCollect.Integration.Tests.Models.MeasureGroups;
using Xunit;

namespace Visiology.DataCollect.Autotests.API.Tests.MeasureGroups.Get.v2
{
    /// <summary>
    /// Класс тестирования метода получения метаописаний групп показателей
    /// </summary>
    [XApiVersion("2.0")]
    public class MetasWithAttributesTests : BaseIntegrationTests<MeasureGroupsListDto, MeasureGroupDto>
    {
        /// <summary>
        /// Url запроса на получение описаний групп показателей
        /// </summary>
        public override string Url { get; set; }

        /// <summary>
        /// Тип тестируемого метода
        /// </summary>
        public override Method Method { get; set; } = Method.GET;

        public MetasWithAttributesTests(TokenFixture tokenFixture, RestService restService)
            : base(tokenFixture, restService)
        {
            Url = $"{config.GetValue("ApiUrl")}{config.GetValue("ApiUrlGetMeasureGroupPath")}";
        }

        /// <summary>
        /// Тест получения метаописания группы показателей с атрибутом типа String 
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Get_WithStringAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_string_meta";
            var measureGroupName = "string атрибут. Получение описания группы показателей";

            var url = this.GetUrl(measureGroupId);

            var response = await _restService.SendRequestAsync(this.Method, url, TokenRoleType.UserWithRole, _tokenFixture.Tokens, null, Headers);
            var isSuccess = response.IsSuccessful;
            var message = $"{response.StatusCode} {response.StatusDescription}";

            if (isSuccess)
            {
                try
                {
                    var content = await Task.Run(() => JsonConvert.DeserializeObject<MeasureGroupDto>(response.Content));

                    var expectedContent = new MeasureGroupDto
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
                    };

                    if (!expectedContent.Equals(content))
                    {
                        isSuccess = false;
                        message += $"Полученное метоописание группы показателей {measureGroupName} {measureGroupId} не соответствует ожидаемому";
                    }
                }
                catch (Exception e)
                {
                    isSuccess = false;
                    message += e.Message;
                }
            }

            Assert.True(isSuccess, message);
        }

        /// <summary>
        /// Тест получения метаописания группы показателей с атрибутом типа Long
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Get_WithLongAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_long_meta";
            var measureGroupName = "long атрибут. Получение описания группы показателей";

            var url = this.GetUrl(measureGroupId);

            var response = await _restService.SendRequestAsync(this.Method, url, TokenRoleType.UserWithRole, _tokenFixture.Tokens, null, Headers);
            var isSuccess = response.IsSuccessful;
            var message = $"{response.StatusCode} {response.StatusDescription}";

            if (isSuccess)
            {
                try
                {
                    var content = await Task.Run(() => JsonConvert.DeserializeObject<MeasureGroupDto>(response.Content));

                    var expectedContent = new MeasureGroupDto
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
                    };

                    if (!expectedContent.Equals(content))
                    {
                        isSuccess = false;
                        message += $"Полученное метоописание группы показателей {measureGroupName} {measureGroupId} не соответствует ожидаемому";
                    }
                }
                catch (Exception e)
                {
                    isSuccess = false;
                    message += e.Message;
                }
            }

            Assert.True(isSuccess, message);
        }

        /// <summary>
        /// Тест получения метаописания группы показателей с атрибутом типа Decimal  
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Get_WithDecimalAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_decimal_meta";
            var measureGroupName = "decimal атрибут. Получение описания группы показателей";

            var url = this.GetUrl(measureGroupId);

            var response = await _restService.SendRequestAsync(this.Method, url, TokenRoleType.UserWithRole, _tokenFixture.Tokens, null, Headers);
            var isSuccess = response.IsSuccessful;
            var message = $"{response.StatusCode} {response.StatusDescription}";

            if (isSuccess)
            {
                try
                {
                    var content = await Task.Run(() => JsonConvert.DeserializeObject<MeasureGroupDto>(response.Content));

                    var expectedContent = new MeasureGroupDto
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
                    };

                    if (!expectedContent.Equals(content))
                    {
                        isSuccess = false;
                        message += $"Полученное метоописание группы показателей {measureGroupId} не соответствует ожидаемому";
                    }
                }
                catch (Exception e)
                {
                    isSuccess = false;
                    message += e.Message;
                }
            }

            Assert.True(isSuccess, message);
        }

        /// <summary>
        /// Тест получения метаописания группы показателей с атрибутом типа Boolean  
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Get_WithBooleanAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_boolean_meta";
            var measureGroupName = "boolean атрибут. Получение описания группы показателей";

            var url = this.GetUrl(measureGroupId);

            var response = await _restService.SendRequestAsync(this.Method, url, TokenRoleType.UserWithRole, _tokenFixture.Tokens, null, Headers);
            var isSuccess = response.IsSuccessful;
            var message = $"{response.StatusCode} {response.StatusDescription}";

            if (isSuccess)
            {
                try
                {
                    var content = await Task.Run(() => JsonConvert.DeserializeObject<MeasureGroupDto>(response.Content));

                    var expectedContent = new MeasureGroupDto
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
                    };

                    if (!expectedContent.Equals(content))
                    {
                        isSuccess = false;
                        message += $"Полученное метоописание группы показателей {measureGroupId} не соответствует ожидаемому";
                    }
                }
                catch (Exception e)
                {
                    isSuccess = false;
                    message += e.Message;
                }
            }

            Assert.True(isSuccess, message);
        }

        /// <summary>
        /// Тест получения метаописания группы показателей с атрибутом типа Date  
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Get_WithDateAttribute()
        {
            // Данные группы показателей из дампа
            var measureGroupId = "measureGroup_date_meta";
            var measureGroupName = "date атрибут. Получение описания группы показателей";

            var url = this.GetUrl(measureGroupId);

            var response = await _restService.SendRequestAsync(this.Method, url, TokenRoleType.UserWithRole, _tokenFixture.Tokens, null, Headers);
            var isSuccess = response.IsSuccessful;
            var message = $"{response.StatusCode} {response.StatusDescription}";

            if (isSuccess)
            {
                try
                {
                    var content = await Task.Run(() => JsonConvert.DeserializeObject<MeasureGroupDto>(response.Content));

                    var expectedContent = new MeasureGroupDto
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
                                TypeName = "Date",
                                MaxDate = "2020-01-01",
                                MinDate = "2030-12-31"
                            }
                        }
                    };

                    if (!expectedContent.Equals(content))
                    {
                        isSuccess = false;
                        message += $"Полученное метоописание группы показателей {measureGroupId} не соответствует ожидаемому";
                    }
                }
                catch (Exception e)
                {
                    isSuccess = false;
                    message += e.Message;
                }
            }

            Assert.True(isSuccess, message);
        }

        protected override string GetUrl(string measureGroupId)
        {
            return string.Format(this.Url, measureGroupId);
        }

        protected override string GetSearchUrl(string dimensionId)
        {
            throw new NotImplementedException();
        }
    }
}
