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

        public MetasWithAttributesTests(IisFixture iisFixture, TokenFixture tokenFixture, RestService restService)
            : base(iisFixture, tokenFixture, restService)
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
            var measureGroupId = "measureGroup_Strokovii_meta";
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
                        Name = "Строковый атрибут. Получение описания группы показателей",
                        Id = "measureGroup_Strokovii_meta",
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
        /// Тест получения метаописания группы показателей с атрибутом типа Long
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Get_WithLongAttribute()
        {
            var measureGroupId = "measureGroup_TSelochisle_meta";
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
                        Name = "Целочисленный атрибут. Получение описания группы показателей",
                        Id = "measureGroup_TSelochisle_meta",
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
                                Name = "Целочисленный",
                                Id = "attr_TSelochislennii",
                                TypeCode = (int)MeasureGroupAttributeType.Long,
                                TypeName =  "Long"
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
        /// Тест получения метаописания группы показателей с атрибутом типа Decimal  
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups2.0")]
        public async Task Get_WithDecimalAttribute()
        {
            var measureGroupId = "measureGroup_Drobnoe_meta";
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
                        Name = "Дробное атрибут. Получение описания группы показателей",
                        Id = "measureGroup_Drobnoe_meta",
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
                                Name = "Дробное число",
                                Id = "attr_Drobnoe_chislo",
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
            var measureGroupId = "measureGroup_Logicheskii_meta";
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
                        Name = "Логический атрибут. Получение описания группы показателей",
                        Id = "measureGroup_Logicheskii_meta",
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
                                Name = "Логический",
                                Id = "attr_Logicheskii",
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
            var measureGroupId = "measureGroup_Data_atribut_meta";
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
                        Name = "Дата атрибут. Получение описания группы показателей",
                        Id = "measureGroup_Data_atribut_meta",
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
                                Name = "Дата",
                                Id = "attr_Data",
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
