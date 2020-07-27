using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;
using Visiology.DataCollect.Autotests.API.Infrastructure.Entities;
using Visiology.DataCollect.Autotests.API.Tests.Dimensions.Get;
using Visiology.DataCollect.Autotests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Entities.RequestBody.Filters.Dimensions;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Impl;
using Visiology.DataCollect.Integration.Tests.Models.Dimensions.Elements;
using Xunit;

namespace Visiology.DataCollect.Autotests.API.Tests.Dimensions.Post.v2
{
    /// <summary>
    /// Класс тестирования метода писка элементов измерения
    /// </summary>
    [XApiVersion("2.0")]
    public class SearchElementsTests : BaseTests<ElementsListDto, ElementDto>
    {
        /// <summary>
        /// Измерение для тестирования получения элементов
        /// В дампе - "Тестовое измерение для тестирования фильтров получения элементов"
        /// </summary>
        private const string dimensionId = "dim_Testovoe_izmerenie_forFi01";

        /// <summary>
        /// Тип тестируемого метода
        /// </summary>
        public override Method Method { get; set; } = Method.POST;

        /// <summary>
        /// Url тестируемого метода
        /// </summary>
        public override string Url { get; set; }

        public SearchElementsTests(TokenFixture tokenFixture, RestService restService)
            : base(tokenFixture, restService, new DimensionElementsVerifier())
        {
            Url = GetUrl(dimensionId);
        }

        /// <summary>
        /// Тест получения элементов измерения при невалидном или несуществующем идентификаторе измерения
        /// </summary>
        /// <param name="dimensionId">Идентификато измерения</param>
        /// <returns>Ожидаемый результат - отрицательный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TokenRoleType.UserAdmin, null)]
        public async Task GetAll_WithInvalidDimensionId(TokenRoleType token, string dimensionId)
        {
            Url = GetUrl(dimensionId);
            var result = await ExecuteGet(token, 0);

            Assert.True(!result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест получения элементов измерения при отсутствии у роли пользователя разрешения на это измерение
        /// </summary>
        /// <param name="tokenRoleType">Тип токена пользоателя(по роли)</param>
        /// <returns>Ожидаемый результат - отрицательный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TokenRoleType.UserWithFakeRole)]
        public async Task GetAll_WithInvalidTokenType(TokenRoleType token)
        {
            var result = await ExecuteGet(token, 0);

            Assert.True(!result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест получения элементов измерения с параметром getAll
        /// </summary>
        /// <param name="getAll">Параметр getAll</param>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TokenRoleType.UserWithRole, true)]
        [InlineData(TokenRoleType.UserWithRole, false)]
        public async Task GetAll_WithParamGetAll(TokenRoleType token, bool getAll)
        {
            var parameters = new Dictionary<string, object>
            {
                    { "getAll", getAll }
            };

            // Количество элементов в измерении "Тестовое измерение для тестирования фильтров получения элементов"
            var elementsCountInDimension = 16;

            var result = await ExecuteGet(token, elementsCountInDimension, parameters);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест получения элементов измерения без тела
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TokenRoleType.UserWithRole)]
        public async Task GetAll_WithoutBody(TokenRoleType token)
        {
            // Количество элементов в измерении "Тестовое измерение для тестирования фильтров получения элементов"
            var elementsCountInDimension = 16;

            var result = await ExecuteGet(token, elementsCountInDimension);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест получения элементов измерения с параметром limit
        /// </summary>
        /// <param name="limit">Параметр limit</param>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TokenRoleType.UserWithRole, TestValues.MoreThanZeroValue)]
        public async Task GetAll_WithParamLimit(TokenRoleType token, int limit)
        {
            var parameters = new Dictionary<string, object>
            {
                    { Parameters.Limit, limit }
            };

            var result = await ExecuteGet(token, limit, parameters);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест получения элементов измерения с параметром skip
        /// </summary>
        /// <param name="skip">Параметр skip</param>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TokenRoleType.UserWithRole, TestValues.ZeroValue)]
        [InlineData(TokenRoleType.UserWithRole, TestValues.MoreThanZeroValue)]
        [InlineData(TokenRoleType.UserWithRole, 17)]
        public async Task GetAll_WithParamSkip(TokenRoleType token, int skip)
        {
            var parameters = new Dictionary<string, object>
            {
                    { Parameters.Skip, skip }
            };

            // Количество элементов в измерении "Тестовое измерение для тестирования фильтров получения элементов"
            var elementsCountInDimension = 16;
            var elementsCount = skip > elementsCountInDimension ? 0 : elementsCountInDimension - skip;

            var result = await ExecuteGet(token, elementsCount, parameters);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест простого фильтра типа "id"
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TokenRoleType.UserWithRole)]
        public async Task Get_WithSimpleIdFilter(TokenRoleType token)
        {
            var filter = new SimpleFilter
            {
                value = 4,
                type = SimpleFilterType.id,
                condition = FilterCondition.equals
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 4.ToString(),
                        Name = "Москва",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Московская область"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 4000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Msk" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value =  4000000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-01" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    }
                };

            var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionId), token, null, Headers, filterContent);
            var result = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест простого фильтра типа "name" (получение одного элемента)
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Get_WithSimpleNameFilter()
        {
            var filter = new SimpleFilter
            {
                value = "Мытищи",
                type = SimpleFilterType.name,
                condition = FilterCondition.equals
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 5.ToString(),
                        Name = "Мытищи",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Московская область"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 1111111 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "MTSHU" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 1111111.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-21" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    }
                };

            var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionId), TokenRoleType.UserWithRole, null, Headers, filterContent);
            var result = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра типа "level"
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Get_WithNamedLevelFilter()
        {
            var filter = new NamedFilter
            {
                value = "Татарстан",
                type = NamedFilterType.level,
                name = "Регион",
                condition = FilterCondition.equals
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 1.ToString(),
                        Name = "Казань",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Татарстан"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 3000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Name" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 10000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-23" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                    new ElementDto
                    {
                        Id = 2.ToString(),
                        Name = "Бугульма",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Татарстан"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 3000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Name" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 10000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-23" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                    new ElementDto
                    {
                        Id = 3.ToString(),
                        Name = "Карабаш",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Татарстан"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 3000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Name" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 10000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-23" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 2 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    }
                };

            var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionId), TokenRoleType.UserWithRole, null, Headers, filterContent);
            var result = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "level" по конечному уровню
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Get_WithNamedLevelFilterForLastLevel()
        {
            var filter = new NamedFilter
            {
                value = "Сигнахи",
                type = NamedFilterType.level,
                name = "Город",
                condition = FilterCondition.equals
            };
            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 6.ToString(),
                        Name = "Сигнахи",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Грузия"},
                            new ElementFolderDto { FolderName = "Кахетия"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 1000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Sgn" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 1000000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-22" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    }
                };

            var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionId), TokenRoleType.UserWithRole, null, Headers, filterContent);
            var result = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "attribute", где атрибут типа int
        /// Тест-кейс проверки условия фильтрации Equals
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Get_WithNamedAttributeFilter_ForIntAttribute_EqualsCondition()
        {
            var filter = new NamedFilter
            {
                value = "-1",
                type = NamedFilterType.attribute,
                name = "Численность населения",
                condition = FilterCondition.equals
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 7.ToString(),
                        Name = "int attribute",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Фильтры"},
                            new ElementFolderDto { FolderName = "По атрибутам"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = -1 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = "<p>int attribute<br></p>" }
                        }
                    }
                };

            var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionId), TokenRoleType.UserWithRole, null, Headers, filterContent);
            var result = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "attribute", где атрибут типа int
        /// Тест-кейс проверки условия фильтрации GreaterOrEquals
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Success_Get_WithNamedAttributeFilter_ForIntAttribute_GreaterOrEqualsCondition()
        {
            var filter = new NamedFilter
            {
                value = "3000000",
                type = NamedFilterType.attribute,
                name = "Численность населения",
                condition = FilterCondition.greaterorequals
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 1.ToString(),
                        Name = "Казань",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Татарстан"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 3000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Name" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 10000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-23" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                     new ElementDto
                    {
                        Id = 2.ToString(),
                        Name = "Бугульма",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Татарстан"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 3000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Name" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 10000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-23" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                      new ElementDto
                    {
                        Id = 3.ToString(),
                        Name = "Карабаш",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Татарстан"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 3000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Name" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 10000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-23" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 2 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                       new ElementDto
                    {
                        Id = 4.ToString(),
                        Name = "Москва",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Московская область"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 4000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Msk" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 4000000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-01" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    }
                };

            var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionId), TokenRoleType.UserWithRole, null, Headers, filterContent);
            var result = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "attribute", где атрибут типа int
        /// Тест-кейс проверки условия фильтрации Greater
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Get_WithNamedAttributeFilter_ForIntAttribute_GreaterCondition()
        {
            var filter = new NamedFilter
            {
                value = "3000000",
                type = NamedFilterType.attribute,
                name = "Численность населения",
                condition = FilterCondition.greater
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedElementsInfo = new List<ElementDto>
                {
                       new ElementDto
                    {
                        Id = 4.ToString(),
                        Name = "Москва",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Московская область"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 4000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Msk" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 4000000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-01" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    }
                };

            var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionId), TokenRoleType.UserWithRole, null, Headers, filterContent);
            var result = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "attribute", где атрибут типа int
        /// Тест-кейс проверки условия фильтрации Less
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Get_WithNamedAttributeFilter_ForIntAttribute_LessCondition()
        {
            var filter = new NamedFilter
            {
                value = "1111111",
                type = NamedFilterType.attribute,
                name = "Численность населения",
                condition = FilterCondition.less
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 6.ToString(),
                        Name = "Сигнахи",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Грузия"},
                            new ElementFolderDto { FolderName = "Кахетия"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 1000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Sgn" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 1000000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-22" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                    new ElementDto
                    {
                        Id = 7.ToString(),
                        Name = "int attribute",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Фильтры"},
                            new ElementFolderDto { FolderName = "По атрибутам"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = -1 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = "<p>int attribute<br></p>" }
                        }
                    }
            };

            var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionId), TokenRoleType.UserWithRole, null, Headers, filterContent);
            var result = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "attribute", где атрибут типа int
        /// Тест-кейс проверки условия фильтрации LessOrEquals
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Get_WithNamedAttributeFilter_ForIntAttribute_LessOrEqualsCondition()
        {
            var filter = new NamedFilter
            {
                value = "1111111",
                type = NamedFilterType.attribute,
                name = "Численность населения",
                condition = FilterCondition.lessorequals
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 5.ToString(),
                        Name = "Мытищи",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Московская область"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 1111111 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "MTSHU" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 1111111.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-21" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                    new ElementDto
                    {
                        Id = 6.ToString(),
                        Name = "Сигнахи",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Грузия"},
                            new ElementFolderDto { FolderName = "Кахетия"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 1000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Sgn" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 1000000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-22" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                    new ElementDto
                    {
                        Id = 7.ToString(),
                        Name = "int attribute",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Фильтры"},
                            new ElementFolderDto { FolderName = "По атрибутам"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = -1 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = "<p>int attribute<br></p>" }
                        }
                    }
            };

            var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionId), TokenRoleType.UserWithRole, null, Headers, filterContent);
            var result = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "attribute", где атрибут типа int
        /// Тест-кейс проверки условия фильтрации NotEquals
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Get_WithNamedAttributeFilter_ForIntAttribute_NotEqualsCondition()
        {
            var filter = new NamedFilter
            {
                value = "3000000",
                type = NamedFilterType.attribute,
                name = "Численность населения",
                condition = FilterCondition.notequals
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedElementsInfo = new List<ElementDto>
                {
                       new ElementDto
                    {
                        Id = 4.ToString(),
                        Name = "Москва",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Московская область"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 4000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Msk" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 4000000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-01" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                       new ElementDto
                    {
                        Id = 5.ToString(),
                        Name = "Мытищи",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Московская область"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 1111111 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "MTSHU" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 1111111.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-21" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                       new ElementDto
                    {
                        Id = 6.ToString(),
                        Name = "Сигнахи",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Грузия"},
                            new ElementFolderDto { FolderName = "Кахетия"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 1000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Sgn" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 1000000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-22" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                    new ElementDto
                    {
                        Id = 7.ToString(),
                        Name = "int attribute",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Фильтры"},
                            new ElementFolderDto { FolderName = "По атрибутам"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = -1 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = "<p>int attribute<br></p>" }
                        }
                    }
                };

            var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionId), TokenRoleType.UserWithRole, null, Headers, filterContent);
            var result = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "attribute", где атрибут типа int
        /// Тест-кейс проверки условия фильтрации Contains
        /// </summary>
        /// <returns>Ожидаемый результат - отрицательный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Get_WithNamedAttributeFilter_ForIntAttribute_ContainsCondition()
        {
            var filter = new NamedFilter
            {
                value = "30000",
                type = NamedFilterType.attribute,
                name = "Численность населения",
                condition = FilterCondition.contains
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionId), TokenRoleType.UserWithRole, null, Headers, filterContent);
            var result = Verifier.Verify(elementsContent.Content?.Entities, new List<ElementDto>());

            Assert.True(!result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "attribute", где атрибут типа int
        /// Тест-кейс проверки невалидных значений по атрибуту
        /// </summary>
        /// <param name="attributeValue">Значение атрибута</param>
        /// <returns>Ожидаемый результат - отрицательный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TestValues.StringValue)]
        [InlineData(TestValues.FloatValue)]
        public async Task Get_WithNamedAttributeFilter_ForIntAttribute_WithInvalidAttributeValue(object attributeValue)
        {
            var filter = new NamedFilter
            {
                value = attributeValue,
                type = NamedFilterType.attribute,
                name = "Численность населения",
                condition = FilterCondition.equals
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionId), TokenRoleType.UserWithRole, null, Headers, filterContent);
            var result = Verifier.Verify(elementsContent.Content?.Entities, new List<ElementDto>());

            Assert.True(!result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "attribute", где атрибут типа int
        /// Тест-кейс проверки null значения по атрибуту
        /// </summary>
        /// <returns>Ожидаемый результат - отрицательный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Get_WithNamedAttributeFilter_ForIntAttribute_WithNullAttributeValue()
        {
            var filter = new NamedFilter
            {
                value = null,
                type = NamedFilterType.attribute,
                name = "Численность населения",
                condition = FilterCondition.equals
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            // Количество элементов  с ззначением Null по атрибуту типа Int в измерении "Тестовое измерение для тестирования фильтров получения элементов"
            var elementsCountInDimension = 9;

            var result = await ExecuteGet(TokenRoleType.UserWithRole, elementsCountInDimension, null, filterContent);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "attribute", где атрибут типа string
        /// Тест-кейсы проверки невалидных для данного типа атрибута условий фильтрации
        /// </summary>
        /// <param name="condition">Условие фильтрации</param>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(FilterCondition.greater)]
        [InlineData(FilterCondition.greaterorequals)]
        [InlineData(FilterCondition.less)]
        [InlineData(FilterCondition.lessorequals)]
        public async Task Get_WithNamedAttributeFilter_ForStringAttribute(FilterCondition condition)
        {
            var filter = new NamedFilter
            {
                value = "-2",
                type = NamedFilterType.attribute,
                name = "Краткое наименование",
                condition = condition
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionId), TokenRoleType.UserWithRole, null, Headers, filterContent);
            var result = Verifier.Verify(elementsContent.Content?.Entities, new List<ElementDto>());

            Assert.False(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "attribute", где атрибут типа string
        /// Тест-кейс проверки условия фильтрации Contains
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Get_WithNamedAttributeFilter_ForStringAttribute_ContainsCondition()
        {
            var filter = new NamedFilter
            {
                value = "Me",
                type = NamedFilterType.attribute,
                name = "Краткое наименование",
                condition = FilterCondition.contains
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 1.ToString(),
                        Name = "Казань",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Татарстан"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 3000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Name" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 10000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-23" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                     new ElementDto
                    {
                        Id = 2.ToString(),
                        Name = "Бугульма",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Татарстан"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 3000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Name" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 10000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-23" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                      new ElementDto
                    {
                        Id = 3.ToString(),
                        Name = "Карабаш",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Татарстан"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 3000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Name" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 10000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-23" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 2 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    }
                };

            var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionId), TokenRoleType.UserWithRole, null, Headers, filterContent);
            var result = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "attribute", где атрибут типа string
        /// Тест-кейс проверки условия фильтрации Equals
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Get_WithNamedAttributeFilter_ForStringAttribute_EqualsCondition()
        {
            var filter = new NamedFilter
            {
                value = "-2",
                type = NamedFilterType.attribute,
                name = "Краткое наименование",
                condition = FilterCondition.equals
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 8.ToString(),
                        Name = "string attribute",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Фильтры"},
                            new ElementFolderDto { FolderName = "По атрибутам"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "-2" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = "<p>string attribute<br></p>" }
                        }
                    }
                };

            var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionId), TokenRoleType.UserWithRole, null, Headers, filterContent);
            var result = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "attribute", где атрибут типа string
        /// Тест-кейс проверки null значения по атрибуту
        /// </summary>
        /// <returns>Ожидаемый результат - отрицательный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Get_WithNamedAttributeFilter_ForStringAttribute_WithNullAttributeValue()
        {
            var filter = new NamedFilter
            {
                value = null,
                type = NamedFilterType.attribute,
                name = "Краткое наименование",
                condition = FilterCondition.equals
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            // Количество элементов с значением Null по атрибуту типа string в измерении "Тестовое измерение для тестирования фильтров получения элементов"
            var elementsCountInDimension = 9;

            var result = await ExecuteGet(TokenRoleType.UserWithRole, elementsCountInDimension, null, filterContent);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "attribute", где атрибут типа string
        /// Тест-кейс проверки условия фильрации NotEquals
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Get_WithNamedAttributeFilter_ForStringAttribute_NotEqualsCondition()
        {
            var filter = new NamedFilter
            {
                value = "Name",
                type = NamedFilterType.attribute,
                name = "Краткое наименование",
                condition = FilterCondition.notequals
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedElementsInfo = new List<ElementDto>
                {
                       new ElementDto
                    {
                        Id = 4.ToString(),
                        Name = "Москва",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Московская область"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 4000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Msk" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 4000000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-01" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                       new ElementDto
                    {
                        Id = 5.ToString(),
                        Name = "Мытищи",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Московская область"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 1111111 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "MTSHU" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 1111111.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-21" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                       new ElementDto
                    {
                        Id = 6.ToString(),
                        Name = "Сигнахи",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Грузия"},
                            new ElementFolderDto { FolderName = "Кахетия"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 1000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Sgn" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 1000000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-22" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                    new ElementDto
                    {
                        Id = 8.ToString(),
                        Name = "string attribute",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Фильтры"},
                            new ElementFolderDto { FolderName = "По атрибутам"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "-2" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = "<p>string attribute<br></p>" }
                        }
                    }
                };

            var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionId), TokenRoleType.UserWithRole, null, Headers, filterContent);
            var result = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "attribute", где атрибут типа float
        /// Тест-кейс проверки условия фильтрации Equals
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Get_WithNamedAttributeFilter_ForFloatAttribute_EqualsCondition()
        {
            var filter = new NamedFilter
            {
                value = -3.00f,
                type = NamedFilterType.attribute,
                name = "Площадь жилой зоны",
                condition = FilterCondition.equals
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 9.ToString(),
                        Name = "float attribute",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Фильтры"},
                            new ElementFolderDto { FolderName = "По атрибутам"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = -3.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = "<p>float attribute<br></p>" }
                        }
                    }
                };

            var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionId), TokenRoleType.UserWithRole, null, Headers, filterContent);
            var result = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "attribute", где атрибут типа float
        /// Тест-кейс проверки условия фильтрации GreaterOrEquals
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Get_WithNamedAttributeFilter_ForFloatAttribute_GreaterOrEqualsCondition()
        {
            var filter = new NamedFilter
            {
                value = 10000.00f,
                type = NamedFilterType.attribute,
                name = "Площадь жилой зоны",
                condition = FilterCondition.greaterorequals
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 1.ToString(),
                        Name = "Казань",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Татарстан"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 3000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Name" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 10000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-23" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                     new ElementDto
                    {
                        Id = 2.ToString(),
                        Name = "Бугульма",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Татарстан"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 3000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Name" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 10000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-23" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                      new ElementDto
                    {
                        Id = 3.ToString(),
                        Name = "Карабаш",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Татарстан"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 3000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Name" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 10000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-23" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 2 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                       new ElementDto
                    {
                        Id = 4.ToString(),
                        Name = "Москва",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Московская область"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 4000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Msk" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 4000000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-01" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                        new ElementDto
                    {
                        Id = 5.ToString(),
                        Name = "Мытищи",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Московская область"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 1111111 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "MTSHU" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 1111111.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-21" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                         new ElementDto
                    {
                        Id = 6.ToString(),
                        Name = "Сигнахи",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Грузия"},
                            new ElementFolderDto { FolderName = "Кахетия"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 1000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Sgn" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 1000000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-22" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    }
            };

            var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionId), TokenRoleType.UserWithRole, null, Headers, filterContent);
            var result = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "attribute", где атрибут типа float
        /// Тест-кейс проверки условия фильтрации Greater
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Get_WithNamedAttributeFilter_ForFloatAttribute_GreaterCondition()
        {
            var filter = new NamedFilter
            {
                value = 10000.00f,
                type = NamedFilterType.attribute,
                name = "Площадь жилой зоны",
                condition = FilterCondition.greater
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedElementsInfo = new List<ElementDto>
                {
                       new ElementDto
                    {
                        Id = 4.ToString(),
                        Name = "Москва",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Московская область"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 4000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Msk" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 4000000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-01" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                        new ElementDto
                    {
                        Id = 5.ToString(),
                        Name = "Мытищи",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Московская область"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 1111111 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "MTSHU" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 1111111.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-21" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                         new ElementDto
                    {
                        Id = 6.ToString(),
                        Name = "Сигнахи",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Грузия"},
                            new ElementFolderDto { FolderName = "Кахетия"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 1000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Sgn" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 1000000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-22" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    }
                };

            var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionId), TokenRoleType.UserWithRole, null, Headers, filterContent);
            var result = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "attribute", где атрибут типа float
        /// Тест-кейс проверки условия фильтрации Less
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Get_WithNamedAttributeFilter_ForFloatAttribute_LessCondition()
        {
            var filter = new NamedFilter
            {
                value = 10000.00f,
                type = NamedFilterType.attribute,
                name = "Площадь жилой зоны",
                condition = FilterCondition.less
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 9.ToString(),
                        Name = "float attribute",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Фильтры"},
                            new ElementFolderDto { FolderName = "По атрибутам"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = -3.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = "<p>float attribute<br></p>" }
                        }
                    }
            };

            var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionId), TokenRoleType.UserWithRole, null, Headers, filterContent);
            var result = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "attribute", где атрибут типа float
        /// Тест-кейс проверки условия фильтрации LessOrEquals
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Get_WithNamedAttributeFilter_ForFloatAttribute_LessOrEqualsCondition()
        {
            var filter = new NamedFilter
            {
                value = 10000.00f,
                type = NamedFilterType.attribute,
                name = "Площадь жилой зоны",
                condition = FilterCondition.lessorequals
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedElementsInfo = new List<ElementDto>
                {
                new ElementDto
                    {
                        Id = 1.ToString(),
                        Name = "Казань",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Татарстан"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 3000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Name" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 10000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-23" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                     new ElementDto
                    {
                        Id = 2.ToString(),
                        Name = "Бугульма",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Татарстан"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 3000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Name" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 10000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-23" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                      new ElementDto
                    {
                        Id = 3.ToString(),
                        Name = "Карабаш",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Татарстан"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 3000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Name" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 10000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-23" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 2 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                    new ElementDto
                    {
                        Id = 9.ToString(),
                        Name = "float attribute",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Фильтры"},
                            new ElementFolderDto { FolderName = "По атрибутам"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = -3.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = "<p>float attribute<br></p>" }
                        }
                    }
            };

            var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionId), TokenRoleType.UserWithRole, null, Headers, filterContent);
            var result = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "attribute", где атрибут типа float
        /// Тест-кейс проверки условия фильтрации NotEquals
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Get_WithNamedAttributeFilter_ForFloatAttribute_NotEqualsCondition()
        {
            var filter = new NamedFilter
            {
                value = 10000.00f,
                type = NamedFilterType.attribute,
                name = "Площадь жилой зоны",
                condition = FilterCondition.notequals
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedElementsInfo = new List<ElementDto>
                {
                       new ElementDto
                    {
                        Id = 4.ToString(),
                        Name = "Москва",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Московская область"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 4000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Msk" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 4000000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-01" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                       new ElementDto
                    {
                        Id = 5.ToString(),
                        Name = "Мытищи",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Московская область"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 1111111 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "MTSHU" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 1111111.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-21" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                       new ElementDto
                    {
                        Id = 6.ToString(),
                        Name = "Сигнахи",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Грузия"},
                            new ElementFolderDto { FolderName = "Кахетия"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 1000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Sgn" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 1000000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-22" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                    new ElementDto
                    {
                        Id = 9.ToString(),
                        Name = "float attribute",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Фильтры"},
                            new ElementFolderDto { FolderName = "По атрибутам"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = -3.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = "<p>float attribute<br></p>" }
                        }
                    }
                };

            var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionId), TokenRoleType.UserWithRole, null, Headers, filterContent);
            var result = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "attribute", где атрибут типа Float
        /// Тест-кейс проверки условия фильтрации Contains
        /// </summary>
        /// <returns>Ожидаемый результат - отрицательный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Fail_Get_WithNamedAttributeFilter_ForFloatAttribute_ContainsCondition()
        {
            var filter = new NamedFilter
            {
                value = 10000.00f,
                type = NamedFilterType.attribute,
                name = "Площадь жилой зоны",
                condition = FilterCondition.contains
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionId), TokenRoleType.UserWithRole, null, Headers, filterContent);
            var result = Verifier.Verify(elementsContent.Content?.Entities, new List<ElementDto>());

            Assert.True(!result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "attribute", где атрибут типа float
        /// Тест-кейс проверки невалидных значений по атрибуту
        /// </summary>
        /// <param name="attributeValue">Значение атрибута</param>
        /// <returns>Ожидаемый результат - отрицательный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TestValues.StringValue)]
        public async Task Get_WithNamedAttributeFilter_ForFloatAttribute_WithInvalidAttributeValue(object attributeValue)
        {
            var filter = new NamedFilter
            {
                value = attributeValue,
                type = NamedFilterType.attribute,
                name = "Площадь жилой зоны",
                condition = FilterCondition.equals
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionId), TokenRoleType.UserWithRole, null, Headers, filterContent);
            var result = Verifier.Verify(elementsContent.Content?.Entities, new List<ElementDto>());

            Assert.True(!result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "attribute", где атрибут типа float
        /// Тест-кейс проверки null значения по атрибуту
        /// </summary>
        /// <returns>Ожидаемый результат - отрицательный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Get_WithNamedAttributeFilter_ForFloatAttribute_WithNullAttributeValue()
        {
            var filter = new NamedFilter
            {
                value = null,
                type = NamedFilterType.attribute,
                name = "Площадь жилой зоны",
                condition = FilterCondition.equals
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            // Количество элементов  с ззначением Null по атрибуту типа Float в измерении "Тестовое измерение для тестирования фильтров получения элементов"
            var elementsCountInDimension = 9;

            var result = await ExecuteGet(TokenRoleType.UserWithRole, elementsCountInDimension, null, filterContent);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "attribute", где атрибут типа Date
        /// Тест-кейс проверки условия фильтрации Equals
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Get_WithNamedAttributeFilter_ForDateAttribute_EqualsCondition()
        {
            var filter = new NamedFilter
            {
                value = "2001-12-01",
                type = NamedFilterType.attribute,
                name = "Дата согласования данных",
                condition = FilterCondition.equals
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 10.ToString(),
                        Name = "date attribute",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Фильтры"},
                            new ElementFolderDto { FolderName = "По атрибутам"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2001-12-01" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = "<p>date attribute<br></p>" }
                        }
                    }
                };

            var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionId), TokenRoleType.UserWithRole, null, Headers, filterContent);
            var result = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "attribute", где атрибут типа Date
        /// Тест-кейс проверки условия фильтрации GreaterOrEquals
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Get_WithNamedAttributeFilter_ForDateAttribute_GreaterOrEqualsCondition()
        {
            var filter = new NamedFilter
            {
                value = "2018-05-22",
                type = NamedFilterType.attribute,
                name = "Дата согласования данных",
                condition = FilterCondition.greaterorequals
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 1.ToString(),
                        Name = "Казань",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Татарстан"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 3000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Name" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 10000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-23" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                     new ElementDto
                    {
                        Id = 2.ToString(),
                        Name = "Бугульма",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Татарстан"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 3000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Name" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 10000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-23" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                      new ElementDto
                    {
                        Id = 3.ToString(),
                        Name = "Карабаш",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Татарстан"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 3000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Name" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 10000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-23" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 2 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                      new ElementDto
                    {
                        Id = 6.ToString(),
                        Name = "Сигнахи",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Грузия"},
                            new ElementFolderDto { FolderName = "Кахетия"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 1000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Sgn" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 1000000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-22" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                      new ElementDto
                    {
                        Id = 14.ToString(),
                        Name = "Элемент 2",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Фильтры"},
                            new ElementFolderDto { FolderName = "Комплексный фильтр"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-22" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    }
            };

            var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionId), TokenRoleType.UserWithRole, null, Headers, filterContent);
            var result = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "attribute", где атрибут типа Date
        /// Тест-кейс проверки условия фильтрации Greater
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Get_WithNamedAttributeFilter_ForDateAttribute_GreaterCondition()
        {
            var filter = new NamedFilter
            {
                value = "2018-05-22",
                type = NamedFilterType.attribute,
                name = "Дата согласования данных",
                condition = FilterCondition.greater
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedElementsInfo = new List<ElementDto>
                {
                       new ElementDto
                    {
                        Id = 1.ToString(),
                        Name = "Казань",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Татарстан"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 3000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Name" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 10000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-23" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                     new ElementDto
                    {
                        Id = 2.ToString(),
                        Name = "Бугульма",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Татарстан"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 3000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Name" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 10000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-23" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                      new ElementDto
                    {
                        Id = 3.ToString(),
                        Name = "Карабаш",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Татарстан"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 3000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Name" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 10000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-23" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 2 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    }
                };

            var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionId), TokenRoleType.UserWithRole, null, Headers, filterContent);
            var result = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "attribute", где атрибут типа Date
        /// Тест-кейс проверки условия фильтрации Less
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Get_WithNamedAttributeFilter_ForDateAttribute_LessCondition()
        {
            var filter = new NamedFilter
            {
                value = "2018-05-01",
                type = NamedFilterType.attribute,
                name = "Дата согласования данных",
                condition = FilterCondition.less
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 10.ToString(),
                        Name = "date attribute",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Фильтры"},
                            new ElementFolderDto { FolderName = "По атрибутам"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2001-12-01" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = "<p>date attribute<br></p>" }
                        }
                    }
            };

            var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionId), TokenRoleType.UserWithRole, null, Headers, filterContent);
            var result = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "attribute", где атрибут типа Date
        /// Тест-кейс проверки условия фильтрации LessOrEquals
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Get_WithNamedAttributeFilter_ForDateAttribute_LessOrEqualsCondition()
        {
            var filter = new NamedFilter
            {
                value = "2001-12-01",
                type = NamedFilterType.attribute,
                name = "Дата согласования данных",
                condition = FilterCondition.lessorequals
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedElementsInfo = new List<ElementDto>
            {
                new ElementDto
                    {
                        Id = 10.ToString(),
                        Name = "date attribute",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Фильтры"},
                            new ElementFolderDto { FolderName = "По атрибутам"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2001-12-01" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = "<p>date attribute<br></p>" }
                        }
                    }
            };

            var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionId), TokenRoleType.UserWithRole, null, Headers, filterContent);
            var result = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "attribute", где атрибут типа Date
        /// Тест-кейс проверки условия фильтрации NotEquals
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Get_WithNamedAttributeFilter_ForDateAttribute_NotEqualsCondition()
        {
            var filter = new NamedFilter
            {
                value = "2018-05-23",
                type = NamedFilterType.attribute,
                name = "Дата согласования данных",
                condition = FilterCondition.notequals
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedElementsInfo = new List<ElementDto>
                {
                       new ElementDto
                    {
                        Id = 4.ToString(),
                        Name = "Москва",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Московская область"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 4000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Msk" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 4000000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-01" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                       new ElementDto
                    {
                        Id = 5.ToString(),
                        Name = "Мытищи",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Московская область"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 1111111 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "MTSHU" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 1111111.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-21" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                       new ElementDto
                    {
                        Id = 6.ToString(),
                        Name = "Сигнахи",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Грузия"},
                            new ElementFolderDto { FolderName = "Кахетия"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 1000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Sgn" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 1000000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-22" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                    new ElementDto
                    {
                        Id = 10.ToString(),
                        Name = "date attribute",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Фильтры"},
                            new ElementFolderDto { FolderName = "По атрибутам"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2001-12-01" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = "<p>date attribute<br></p>" }
                        }
                    },
                    new ElementDto
                    {
                        Id = 14.ToString(),
                        Name = "Элемент 2",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Фильтры"},
                            new ElementFolderDto { FolderName = "Комплексный фильтр"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-22" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                    new ElementDto
                    {
                        Id = 15.ToString(),
                        Name = "Элемент 3",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Фильтры"},
                            new ElementFolderDto { FolderName = "Комплексный фильтр"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-08" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    }
                };

            var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionId), TokenRoleType.UserWithRole, null, Headers, filterContent);
            var result = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "attribute", где атрибут типа Date
        /// Тест-кейс проверки условия фильтрации Contains
        /// </summary>
        /// <returns>Ожидаемый результат - отрицательный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Get_WithNamedAttributeFilter_ForDateAttribute_ContainsCondition()
        {
            var filter = new NamedFilter
            {
                value = "2018-05-23",
                type = NamedFilterType.attribute,
                name = "Дата согласования данных",
                condition = FilterCondition.contains
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionId), TokenRoleType.UserWithRole, null, Headers, filterContent);
            var result = Verifier.Verify(elementsContent.Content?.Entities, new List<ElementDto>());

            Assert.True(!result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "attribute", где атрибут типа Date
        /// Тест-кейс проверки невалидных значений по атрибуту
        /// </summary>
        /// <param name="attributeValue">Значение атрибута</param>
        /// <returns>Ожидаемый результат - отрицательный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TestValues.StringValue)]
        [InlineData(TestValues.IntValue)]
        [InlineData(TestValues.DateValueToStringIncorrectFormat)]
        public async Task Get_WithNamedAttributeFilter_ForDateAttribute_WithInvalidAttributeValue(object attributeValue)
        {
            var filter = new NamedFilter
            {
                value = attributeValue,
                type = NamedFilterType.attribute,
                name = "Дата согласования данных",
                condition = FilterCondition.equals
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionId), TokenRoleType.UserWithRole, null, Headers, filterContent);
            var result = Verifier.Verify(elementsContent.Content?.Entities, new List<ElementDto>());

            Assert.True(!result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "attribute", где атрибут типа Date
        /// Тест-кейс проверки null значения по атрибуту
        /// </summary>
        /// <returns>Ожидаемый результат - отрицательный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Get_WithNamedAttributeFilter_ForDateAttribute_WithNullAttributeValue()
        {
            var filter = new NamedFilter
            {
                value = null,
                type = NamedFilterType.attribute,
                name = "Дата согласования данных",
                condition = FilterCondition.equals
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            // Количество элементов  с ззначением Null по атрибуту типа Date в измерении "Тестовое измерение для тестирования фильтров получения элементов"
            var elementsCountInDimension = 7;

            var result = await ExecuteGet(TokenRoleType.UserWithRole, elementsCountInDimension, null, filterContent);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "attribute", где атрибут типа Link
        /// Тест-кейс проверки условия фильтрации Equals
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Get_WithNamedAttributeFilter_ForLinkAttribute_EqualsCondition()
        {
            var filter = new NamedFilter
            {
                value = 2,
                type = NamedFilterType.attribute,
                name = "Тип населенного пункта",
                condition = FilterCondition.equals
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedElementsInfo = new List<ElementDto>
                {
                new ElementDto
                    {
                        Id = 3.ToString(),
                        Name = "Карабаш",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Татарстан"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 3000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Name" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 10000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-23" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 2 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                    new ElementDto
                    {
                        Id = 11.ToString(),
                        Name = "link attribute",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Фильтры"},
                            new ElementFolderDto { FolderName = "По атрибутам"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 2 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = "<p>link attribute<br></p>" }
                        }
                    }
                };

            var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionId), TokenRoleType.UserWithRole, null, Headers, filterContent);
            var result = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "attribute", где атрибут типа Link
        /// Тест-кейс проверки условия фильтрации GreaterOrEquals
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Get_WithNamedAttributeFilter_ForLinkAttribute_GreaterOrEqualsCondition()
        {
            var filter = new NamedFilter
            {
                value = 2,
                type = NamedFilterType.attribute,
                name = "Тип населенного пункта",
                condition = FilterCondition.greaterorequals
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 3.ToString(),
                        Name = "Карабаш",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Татарстан"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 3000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Name" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 10000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-23" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 2 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                    new ElementDto
                    {
                        Id = 11.ToString(),
                        Name = "link attribute",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Фильтры"},
                            new ElementFolderDto { FolderName = "По атрибутам"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 2 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = "<p>link attribute<br></p>" }
                        }
                    }
            };

            var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionId), TokenRoleType.UserWithRole, null, Headers, filterContent);
            var result = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "attribute", где атрибут типа Link
        /// Тест-кейс проверки условия фильтрации Greater
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Get_WithNamedAttributeFilter_ForLinkAttribute_GreaterCondition()
        {
            var filter = new NamedFilter
            {
                value = 2,
                type = NamedFilterType.attribute,
                name = "Тип населенного пункта",
                condition = FilterCondition.greater
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedElementsInfo = new List<ElementDto>();

            var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionId), TokenRoleType.UserWithRole, null, Headers, filterContent);
            var result = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "attribute", где атрибут типа Link
        /// Тест-кейс проверки условия фильтрации Less
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Get_WithNamedAttributeFilter_ForLinkAttribute_LessCondition()
        {
            var filter = new NamedFilter
            {
                value = 2,
                type = NamedFilterType.attribute,
                name = "Тип населенного пункта",
                condition = FilterCondition.less
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 1.ToString(),
                        Name = "Казань",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Татарстан"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 3000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Name" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 10000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-23" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                     new ElementDto
                    {
                        Id = 2.ToString(),
                        Name = "Бугульма",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Татарстан"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 3000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Name" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 10000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-23" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                     new ElementDto
                    {
                        Id = 4.ToString(),
                        Name = "Москва",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Московская область"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 4000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Msk" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 4000000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-01" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                        new ElementDto
                    {
                        Id = 5.ToString(),
                        Name = "Мытищи",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Московская область"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 1111111 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "MTSHU" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 1111111.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-21" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                         new ElementDto
                    {
                        Id = 6.ToString(),
                        Name = "Сигнахи",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Грузия"},
                            new ElementFolderDto { FolderName = "Кахетия"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 1000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Sgn" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 1000000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-22" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                         new ElementDto
                    {
                        Id = 13.ToString(),
                        Name = "Элемент 1",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Фильтры"},
                            new ElementFolderDto { FolderName = "Комплексный фильтр"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    }
            };

            var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionId), TokenRoleType.UserWithRole, null, Headers, filterContent);
            var result = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "attribute", где атрибут типа Link
        /// Тест-кейс проверки условия фильтрации LessOrEquals
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Get_WithNamedAttributeFilter_ForLinkAttribute_LessOrEqualsCondition()
        {
            var filter = new NamedFilter
            {
                value = 1,
                type = NamedFilterType.attribute,
                name = "Тип населенного пункта",
                condition = FilterCondition.lessorequals
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedElementsInfo = new List<ElementDto>
            {
                new ElementDto
                    {
                        Id = 1.ToString(),
                        Name = "Казань",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Татарстан"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 3000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Name" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 10000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-23" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                     new ElementDto
                    {
                        Id = 2.ToString(),
                        Name = "Бугульма",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Татарстан"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 3000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Name" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 10000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-23" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                     new ElementDto
                    {
                        Id = 4.ToString(),
                        Name = "Москва",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Московская область"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 4000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Msk" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 4000000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-01" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                        new ElementDto
                    {
                        Id = 5.ToString(),
                        Name = "Мытищи",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Московская область"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 1111111 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "MTSHU" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 1111111.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-21" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                         new ElementDto
                    {
                        Id = 6.ToString(),
                        Name = "Сигнахи",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Грузия"},
                            new ElementFolderDto { FolderName = "Кахетия"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 1000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Sgn" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 1000000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-22" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                         new ElementDto
                    {
                        Id = 13.ToString(),
                        Name = "Элемент 1",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Фильтры"},
                            new ElementFolderDto { FolderName = "Комплексный фильтр"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    }
            };

            var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionId), TokenRoleType.UserWithRole, null, Headers, filterContent);
            var result = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "attribute", где атрибут типа Link
        /// Тест-кейс проверки условия фильтрации NotEquals
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Get_WithNamedAttributeFilter_ForLinkAttribute_NotEqualsCondition()
        {
            var filter = new NamedFilter
            {
                value = 1,
                type = NamedFilterType.attribute,
                name = "Тип населенного пункта",
                condition = FilterCondition.notequals
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedElementsInfo = new List<ElementDto>
                {
                      new ElementDto
                    {
                        Id = 3.ToString(),
                        Name = "Карабаш",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Татарстан"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 3000000 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Name" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 10000.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-23" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 2 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    },
                    new ElementDto
                    {
                        Id = 11.ToString(),
                        Name = "link attribute",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Фильтры"},
                            new ElementFolderDto { FolderName = "По атрибутам"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 2 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = "<p>link attribute<br></p>" }
                        }
                    }
                };

            var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionId), TokenRoleType.UserWithRole, null, Headers, filterContent);
            var result = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "attribute", где атрибут типа Link
        /// Тест-кейс проверки условия фильтрации Contains
        /// </summary>
        /// <returns>Ожидаемый результат - отрицательный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Get_WithNamedAttributeFilter_ForLinkAttribute_ContainsCondition()
        {
            var filter = new NamedFilter
            {
                value = 1,
                type = NamedFilterType.attribute,
                name = "Тип населенного пункта",
                condition = FilterCondition.contains
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionId), TokenRoleType.UserWithRole, null, Headers, filterContent);
            var result = Verifier.Verify(elementsContent.Content?.Entities, new List<ElementDto>());

            Assert.True(!result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "attribute", где атрибут типа Link
        /// Тест-кейс проверки невалидных значений по атрибуту
        /// </summary>
        /// <param name="attributeValue">Значение атрибута</param>
        /// <returns>Ожидаемый результат - отрицательный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TestValues.StringValue)]
        [InlineData(TestValues.NonExistId)]
        [InlineData(TestValues.ZeroId)]
        [InlineData(TestValues.FloatValue)]
        public async Task Get_WithNamedAttributeFilter_ForLinkAttribute_WithInvalidAttributeValue(object attributeValue)
        {
            var filter = new NamedFilter
            {
                value = attributeValue,
                type = NamedFilterType.attribute,
                name = "Тип населенного пункта",
                condition = FilterCondition.equals
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionId), TokenRoleType.UserWithRole, null, Headers, filterContent);
            var result = Verifier.Verify(elementsContent.Content?.Entities, new List<ElementDto>());

            Assert.True(!result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "attribute", где атрибут типа Text
        /// Тест-кейсы проверки невалидных для данного типа атрибута условий фильтрации
        /// </summary>
        /// <param name="condition">Условие фильтрации</param>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(FilterCondition.greater)]
        [InlineData(FilterCondition.greaterorequals)]
        [InlineData(FilterCondition.less)]
        [InlineData(FilterCondition.lessorequals)]
        public async Task Get_WithNamedAttributeFilter_ForTextAttribute(FilterCondition condition)
        {
            var filter = new NamedFilter
            {
                value = "-2",
                type = NamedFilterType.attribute,
                name = "Описание города",
                condition = condition
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionId), TokenRoleType.UserWithRole, null, Headers, filterContent);
            var result = Verifier.Verify(elementsContent.Content?.Entities, new List<ElementDto>());
            Assert.True(!result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "attribute", где атрибут типа Text
        /// Тест-кейс проверки условия фильтрации Contains
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Get_WithNamedAttributeFilter_ForTextAttribute_ContainsCondition()
        {
            var filter = new NamedFilter
            {
                value = "attribute",
                type = NamedFilterType.attribute,
                name = "Описание города",
                condition = FilterCondition.contains
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 7.ToString(),
                        Name = "int attribute",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Фильтры"},
                            new ElementFolderDto { FolderName = "По атрибутам"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = -1 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = "<p>int attribute<br></p>" }
                        }
                    },
                    new ElementDto
                    {
                        Id = 8.ToString(),
                        Name = "string attribute",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Фильтры"},
                            new ElementFolderDto { FolderName = "По атрибутам"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "-2" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = "<p>string attribute<br></p>" }
                        }
                    },
                    new ElementDto
                    {
                        Id = 9.ToString(),
                        Name = "float attribute",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Фильтры"},
                            new ElementFolderDto { FolderName = "По атрибутам"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = -3.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = "<p>float attribute<br></p>" }
                        }
                    },
                    new ElementDto
                    {
                        Id = 10.ToString(),
                        Name = "date attribute",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Фильтры"},
                            new ElementFolderDto { FolderName = "По атрибутам"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2001-12-01" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = "<p>date attribute<br></p>" }
                        }
                    },
                    new ElementDto
                    {
                        Id = 11.ToString(),
                        Name = "link attribute",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Фильтры"},
                            new ElementFolderDto { FolderName = "По атрибутам"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 2 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = "<p>link attribute<br></p>" }
                        }
                    },
                    new ElementDto
                    {
                        Id = 12.ToString(),
                        Name = "text attribute",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Фильтры"},
                            new ElementFolderDto { FolderName = "По атрибутам"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = "<p>text attribute</p>" }
                        }
                    }
                };

            var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionId), TokenRoleType.UserWithRole, null, Headers, filterContent);
            var result = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "attribute", где атрибут типа text
        /// Тест-кейс проверки условия фильтрации Equals
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Get_WithNamedAttributeFilter_ForTextAttribute_EqualsCondition()
        {
            var filter = new NamedFilter
            {
                value = "<p>text attribute</p>",
                type = NamedFilterType.attribute,
                name = "Описание города",
                condition = FilterCondition.equals
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedElementsInfo = new List<ElementDto>
                {
                   new ElementDto
                    {
                        Id = 12.ToString(),
                        Name = "text attribute",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Фильтры"},
                            new ElementFolderDto { FolderName = "По атрибутам"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = "<p>text attribute</p>" }
                        }
                    }
                };

            var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionId), TokenRoleType.UserWithRole, null, Headers, filterContent);
            var result = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "attribute", где атрибут типа Text
        /// Тест-кейс проверки null значения по атрибуту
        /// </summary>
        /// <returns>Ожидаемый результат - отрицательный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Get_WithNamedAttributeFilter_ForTextAttribute_WithNullAttributeValue()
        {
            var filter = new NamedFilter
            {
                value = null,
                type = NamedFilterType.attribute,
                name = "Описание города",
                condition = FilterCondition.equals
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            // Количество элементов с значением Null по атрибуту типа text в измерении "Тестовое измерение для тестирования фильтров получения элементов"
            var elementsCountInDimension = 10;

            var result = await ExecuteGet(TokenRoleType.UserWithRole, elementsCountInDimension, null, filterContent);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "attribute", где атрибут типа text
        /// Тест-кейс проверки условия фильрации NotEquals
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Get_WithNamedAttributeFilter_ForTextAttribute_NotEqualsCondition()
        {
            var filter = new NamedFilter
            {
                value = "<p>text attribute</p>",
                type = NamedFilterType.attribute,
                name = "Описание города",
                condition = FilterCondition.notequals
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedElementsInfo = new List<ElementDto>
                {
                      new ElementDto
                    {
                        Id = 7.ToString(),
                        Name = "int attribute",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Фильтры"},
                            new ElementFolderDto { FolderName = "По атрибутам"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = -1 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = "<p>int attribute<br></p>" }
                        }
                    },
                    new ElementDto
                    {
                        Id = 8.ToString(),
                        Name = "string attribute",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Фильтры"},
                            new ElementFolderDto { FolderName = "По атрибутам"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "-2" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = "<p>string attribute<br></p>" }
                        }
                    },
                    new ElementDto
                    {
                        Id = 9.ToString(),
                        Name = "float attribute",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Фильтры"},
                            new ElementFolderDto { FolderName = "По атрибутам"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = -3.00f },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = "<p>float attribute<br></p>" }
                        }
                    },
                    new ElementDto
                    {
                        Id = 10.ToString(),
                        Name = "date attribute",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Фильтры"},
                            new ElementFolderDto { FolderName = "По атрибутам"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2001-12-01" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = "<p>date attribute<br></p>" }
                        }
                    },
                    new ElementDto
                    {
                        Id = 11.ToString(),
                        Name = "link attribute",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Фильтры"},
                            new ElementFolderDto { FolderName = "По атрибутам"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 2 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = "<p>link attribute<br></p>" }
                        }
                    }
                };

            var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionId), TokenRoleType.UserWithRole, null, Headers, filterContent);
            var result = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест комплексного фильтра And
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Get_WithComplexAndFilter()
        {
            var filter = new
            {
                operation = "and",
                filters = new[]
                {
                    new NamedFilter
                    {
                        value = "Комплексный фильтр",
                        type = NamedFilterType.level,
                        name = "Регион",
                        condition = FilterCondition.equals
                    },
                    new NamedFilter
                    {
                        value = "2018-05-22",
                        type = NamedFilterType.attribute,
                        name = "Дата согласования данных",
                        condition = FilterCondition.equals
                    }
                }
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 14.ToString(),
                        Name = "Элемент 2",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Фильтры"},
                            new ElementFolderDto { FolderName = "Комплексный фильтр"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-22" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null}
                        }
                    }
                };

            var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionId), TokenRoleType.UserWithRole, null, Headers, filterContent);
            var result = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест комплексного фильтра Or
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Get_WithComplexOrFilter()
        {
            var filter = new
            {
                operation = "or",
                filters = new[]
                {
                    new NamedFilter
                    {
                        value = "Комплексный фильтр Or",
                        type = NamedFilterType.level,
                        name = "Регион",
                        condition = FilterCondition.equals
                    },
                    new NamedFilter
                    {
                        value = "2018-05-08",
                        type = NamedFilterType.attribute,
                        name = "Дата согласования данных",
                        condition = FilterCondition.equals
                    }
                }
            };


            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 15.ToString(),
                        Name = "Элемент 3",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Фильтры"},
                            new ElementFolderDto { FolderName = "Комплексный фильтр"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-05-08" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null}
                        }
                    },
                    new ElementDto
                    {
                        Id = 16.ToString(),
                        Name = "Элемент 3",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Фильтры"},
                            new ElementFolderDto { FolderName = "Комплексный фильтр Or"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null}
                        }
                    }
                };

            var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionId), TokenRoleType.UserWithRole, null, Headers, filterContent);
            var result = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест именованного фильтра "attribute", где атрибут типа Link
        /// Тест-кейс проверки null значения по атрибуту
        /// </summary>
        /// <returns>Ожидаемый результат - отрицательный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Get_WithNamedAttributeFilter_ForLinkAttribute_WithNullAttributeValue()
        {
            var filter = new NamedFilter
            {
                value = null,
                type = NamedFilterType.attribute,
                name = "Тип населенного пункта",
                condition = FilterCondition.equals
            };

            var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            // Количество элементов  с значением Null по атрибуту типа Link в измерении "Тестовое измерение для тестирования фильтров получения элементов"
            var elementsCountInDimension = 8;

            var result = await ExecuteGet(TokenRoleType.UserWithRole, elementsCountInDimension, null, filterContent);

            Assert.True(result.IsSuccess, result.Message);
        }

        protected override string GetUrl(string dimensionId)
        {
            return string.Format($"{config.GetValue("ApiUrl")}{config.GetValue("ApiUrlDimensionElementsSearchPath")}", dimensionId);
        }

        protected override string GetSearchUrl(string dimensionId)
        {
            return string.Format($"{config.GetValue("ApiUrl")}{config.GetValue("ApiUrlDimensionElementsSearchPath")}", dimensionId);
        }
    }
}
