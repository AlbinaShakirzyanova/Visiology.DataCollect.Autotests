using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;
using Visiology.DataCollect.Autotests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Entities.Fields;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Entities.RequestBody.Fields;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Entities.RequestBody.Filters;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Entities.RequestBody.Filters.Dimensions;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Impl;
using Visiology.DataCollect.Integration.Tests.Models.Dimensions.Elements;
using Xunit;

namespace Visiology.DataCollect.Integration.Tests.Dimensions.Put.v1
{
    /// <summary>
    /// Класс тестирования метода обновления элементов измерения
    /// </summary>
    [XApiVersion("1.0")]
    public class ElementsTests : BaseIntegrationPutTests<ElementsListDto, ElementDto>
    {
        /// <summary>
        /// Измерение для тестирования обновления элементов пользователем с ролью "Администратор"
        /// В дампе - "Тестовое измерение для обновления элементов администратором"
        /// </summary>
        private const string adminDimensionId = "13";

        /// <summary>
        /// Измерение для тестирования обновления элементов пользователем с ролью 
        /// В дампе - "Тестовое измерение для обновления элементов пользователем с ролью"
        /// </summary>
        private const string userDimensionId = "24";

        /// <summary>
        /// Измерение для тестирования обновления элементов, недоступных для обновления
        /// В дампе - "Тестовое измерение для обновления блокированных элементов"
        /// </summary>
        private const string restrictedElementsDimensionId = "16";

        /// <summary>
        /// Измерение для тестирования обновления атрибута типа Link элементов (по правам доступа к линкованному элементу)
        /// В дампе - "Тестовое измерение для обновления атрибута типа link"
        /// </summary>
        private const string forLinkAttributeDimensionId = "17";

        /// <summary>
        /// Измерение для тестирования Обновления элементов по фильтру
        /// В дампе - "Тестовое измерение для тестирования фильтров"
        /// </summary>
        private const string forFiltersDimensionId = "18";

        /// <summary>
        /// Тип тестируемого метода
        /// </summary>
        public override Method Method { get; set; } = Method.PUT;

        /// <summary>
        /// Url тестируемого метода
        /// </summary>
        public override string Url { get; set; }

        public ElementsTests(IisFixture iisFixture, TokenFixture tokenFixture, RestService restService)
            : base(iisFixture, tokenFixture, restService, new DimensionElementsVerifier())
        {
            this.Url = this.GetUrl(adminDimensionId);
        }

        /// <summary>
        /// Тест обновления элементов измерения при невалидном или несуществующем идентификаторе измерения
        /// </summary>
        /// <param name="dimensionId">Идентификатор измерения</param>
        /// <returns>Ожидаемый результат - отрицательный</returns>
        [Theory]
        [InlineData(TestValues.ZeroId)]
        [InlineData(TestValues.NonExistId)]
        [InlineData(null)]
        public async Task Update_WithInvalidDimensionId(string dimensionId)
        {
            this.Url = this.GetUrl(dimensionId);

            var body = new[]
            {
                new
                {
                    filter = new SimpleFilter
                    {
                        value = "Упс",
                        type = SimpleFilterType.name,
                        condition = FilterCondition.equals
                    },
                    fields = new[]
                    {
                        new SimpleField
                        {
                            value = "Что-то пошло не так",
                            type = SimpleFieldType.name
                        }
                    }
                }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new UpdateResult
            {
                updated = 0,
                restricted = 0,
                notChanged = 0
            };

            var result = await this.ExecutePut(TokenRoleType.UserAdmin, expectedResult, null, bodyContent);

            Assert.True(!result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест обновления элементов измерения при отсутствии у пользователя роли на измерение
        /// </summary>
        /// <param name="tokenRoleType">Тип токена пользователя(по роли)</param>
        /// <returns>Ожидаемый результат - отрицательный</returns>
        [Fact]
        public async Task Update_WithoutPermissionToDimension()
        {
            this.Url = this.GetUrl(adminDimensionId);

            var body = new[]
            {
                new
                {
                    filter = new SimpleFilter
                    {
                        value = "Элемент для проверки прав",
                        type = SimpleFilterType.name,
                        condition = FilterCondition.equals
                    },
                    fields = new[]
                    {
                        new SimpleField
                        {
                            value = "Упс... Что-то пошло не так...",
                            type = SimpleFieldType.name
                        }
                    }
                }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new UpdateResult
            {
                updated = 0,
                restricted = 0,
                notChanged = 0
            };

            var result = await this.ExecutePut(TokenRoleType.UserWithRole, expectedResult, null, bodyContent);

            Assert.True(!result.IsSuccess, result.Message);
        }


        /// <summary>
        /// Тест обновления всех элементов без тела 
        /// </summary>
        /// <param name="tokenRoleType">Тип токена пользователя(по роли)</param>
        /// <param name="dimensionId">Идентификатор измерения</param>
        /// <returns>Ожидаемый результат - отрицательный</returns>
        [Theory]
        [InlineData(TokenRoleType.UserAdmin, adminDimensionId)]
        [InlineData(TokenRoleType.UserWithRole, userDimensionId)]
        public async Task UpdateAll_WithoutBody(TokenRoleType tokenRoleType, string dimensionId)
        {
            this.Url = this.GetUrl(dimensionId);

            var expectedResult = new UpdateResult
            {
                updated = 0,
                restricted = 0,
                notChanged = 0
            };

            var result = await this.ExecutePut(tokenRoleType, expectedResult);

            Assert.True(!result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест обновления имени всех элементов
        /// Хотя бы на одном уровне есть несколько элементов
        /// </summary>
        /// <param name="tokenRoleType">Тип токена пользователя(по роли)</param>
        /// <returns>Ожидаемый результат - отрицательный</returns>
        [Theory]
        [InlineData(TokenRoleType.UserAdmin)]
        [InlineData(TokenRoleType.UserWithRole)]
        public async Task UpdateAll_WithSimpleNameField(TokenRoleType tokenRoleType)
        {
            // В дампе "Тестовое измерение для обновления имени всех элементов Fail"
            var dimensionId = "21";
            this.Url = this.GetUrl(dimensionId);

            var body = new[]
            {
                new
                {
                    fields = new[]
                    {
                        new SimpleField
                        {
                            value = "Упс... Что-то пошло не так...",
                            type = SimpleFieldType.name
                        }
                    }
                }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new UpdateResult
            {
                updated = 0,
                restricted = 0,
                notChanged = 0
            };

            var result = await this.ExecutePut(tokenRoleType, expectedResult, null, bodyContent);

            Assert.True(!result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест обновления имени всех элементов пользователем с ролью "Администратор"
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact]
        public async Task UpdateAll_WithSimpleNameField_ForAdmin()
        {
            // В дампе "Тестовое измерение для обновления имени всех элементов Success Admin"
            var dimensionId = "20";
            var testValue = "Тест обновления имени всех элементов пользователем с ролью Администратор";

            this.Url = this.GetUrl(dimensionId);

            var body = new[]
            {
                new
                {
                    fields = new[]
                    {
                        new SimpleField
                        {
                            value = testValue,
                            type = SimpleFieldType.name
                        }
                    }
                }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new UpdateResult
            {
                updated = 4,
                restricted = 0,
                notChanged = 0
            };

            var result = await this.ExecutePut(TokenRoleType.UserAdmin, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                // Получаем по фильтру искомые элементы
                var elementsContent = await this.TryGetEntities(Method.POST, this.GetSearchUrl(dimensionId), TokenRoleType.UserAdmin, null, this.Headers);

                result.IsSuccess = result.IsSuccess && elementsContent.ContentVerificationResult.IsSuccess;
                result.Message += elementsContent.ContentVerificationResult.Message;

                foreach (var item in elementsContent.Content?.Entities)
                {
                    if (item.Name != testValue)
                    {
                        result.IsSuccess = false;
                        result.Message += $"Ошибка обновления имени элемента. Ожидалось - {testValue},  а пришло - {item.Name}";

                        break;
                    }
                }
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест обновления имени всех элементов пользователем с ролью
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact]
        public async Task UpdateAll_WithSimpleNameField_ForUser()
        {
            // В дампе "Тестовое измерение для обновления имени всех элементов Success User"
            var dimensionId = "22";
            var testValue = "Тест обновления имени всех элементов пользователем с ролью";

            this.Url = this.GetUrl(dimensionId);

            var body = new[]
            {
                new
                {
                    fields = new[]
                    {
                        new SimpleField
                        {
                            value = testValue,
                            type = SimpleFieldType.name
                        }
                    }
                }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new UpdateResult
            {
                updated = 4,
                restricted = 0,
                notChanged = 0
            };

            var result = await this.ExecutePut(TokenRoleType.UserAdmin, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                // Получаем по фильтру искомые элементы
                var elementsContent = await this.TryGetEntities(Method.POST, this.GetSearchUrl(dimensionId), TokenRoleType.UserWithRole, null, this.Headers);

                result.IsSuccess = result.IsSuccess && elementsContent.ContentVerificationResult.IsSuccess;
                result.Message += elementsContent.ContentVerificationResult.Message;

                foreach (var item in elementsContent.Content?.Entities)
                {
                    if (item.Name != testValue)
                    {
                        result.IsSuccess = false;
                        result.Message += $"Ошибка обновления имени элемента. Ожидалось - {testValue},  а пришло - {item.Name}";

                        break;
                    }
                }
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест обновления наименования элемента на конечном уровне
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact]
        public async Task Update_WithSimpleNameField_ForLastLevel()
        {
            var testValue = "Тест обновления наименования элемента на конечном уровне";
            this.Url = this.GetUrl(userDimensionId);

            var filter = new SimpleFilter
            {
                value = "Казань (Обновление на конечном уровне)",
                type = SimpleFilterType.name,
                condition = FilterCondition.equals
            };

            var fields = new[]
            {
                new SimpleField
                {
                    value = testValue,
                    type = SimpleFieldType.name
                }
            };

            var body = new[] { new { filter, fields } };
            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new UpdateResult
            {
                updated = 1,
                restricted = 0,
                notChanged = 0
            };

            var result = await this.ExecutePut(TokenRoleType.UserWithRole, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var filterContent = await Task.Run(() => JsonConvert.SerializeObject(
                    new SimpleFilter
                    {
                        value = testValue,
                        type = SimpleFilterType.name,
                        condition = FilterCondition.equals
                    }));

                var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 2.ToString(),
                        Name = testValue,
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Татарстан"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "96", Value = 123456 },
                            new ElementAttributeDto { AttributeId = "97", Value = "KZN" },
                            new ElementAttributeDto { AttributeId = "98", Value = 123456.00f },
                            new ElementAttributeDto { AttributeId = "99", Value = "2018-05-25" },
                            new ElementAttributeDto { AttributeId = "100", Value = 1 },
                            new ElementAttributeDto { AttributeId = "101", Value = "Description" }
                        }
                    }
                };

                var elementsContent = await this.TryGetEntities(Method.POST, this.GetSearchUrl(userDimensionId), TokenRoleType.UserWithRole, null, this.Headers, filterContent);
                var updateVerificationResult = this.Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = updateVerificationResult.IsSuccess;
                result.Message += updateVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест обновления наименования элемента на уровне
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact]
        public async Task Update_WithSimpleNameField_ForLevel()
        {
            var testValue = "Тест обновления наименования элемента на уровне";
            this.Url = this.GetUrl(userDimensionId);

            var filter = new SimpleFilter
            {
                value = "Казань (Обновление на уровне)",
                type = SimpleFilterType.name,
                condition = FilterCondition.equals
            };

            var fields = new[]
            {
                new SimpleField
                {
                    value = testValue,
                    type = SimpleFieldType.name
                }
            };

            var body = new[] { new { filter, fields } };
            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new UpdateResult
            {
                updated = 1,
                restricted = 0,
                notChanged = 0
            };

            var result = await this.ExecutePut(TokenRoleType.UserWithRole, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var filterContent = await Task.Run(() => JsonConvert.SerializeObject(
                    new SimpleFilter
                    {
                        value = testValue,
                        type = SimpleFilterType.name,
                        condition = FilterCondition.equals
                    }));

                var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 1.ToString(),
                        Name = testValue,
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "96", Value = 123456 },
                            new ElementAttributeDto { AttributeId = "97", Value = "KZN" },
                            new ElementAttributeDto { AttributeId = "98", Value = 123456.00f },
                            new ElementAttributeDto { AttributeId = "99", Value = "2018-05-25" },
                            new ElementAttributeDto { AttributeId = "100", Value = 1 },
                            new ElementAttributeDto { AttributeId = "101", Value = "Description" }
                        }
                    }
                };

                var elementsContent = await this.TryGetEntities(Method.POST, this.GetSearchUrl(userDimensionId), TokenRoleType.UserWithRole, null, this.Headers, filterContent);
                var updateVerificationResult = this.Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = updateVerificationResult.IsSuccess;
                result.Message += updateVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест обновления наименования элемента в корне
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact]
        public async Task Update_WithSimpleNameField_ForRoot()
        {
            var testValue = "Тест обновления наименования элемента в корне";
            this.Url = this.GetUrl(userDimensionId);

            var filter = new SimpleFilter
            {
                value = "Казань (Обновление в корне)",
                type = SimpleFilterType.name,
                condition = FilterCondition.equals
            };

            var fields = new[]
            {
                new SimpleField
                {
                    value = testValue,
                    type = SimpleFieldType.name
                }
            };

            var body = new[] { new { filter, fields } };
            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new UpdateResult
            {
                updated = 1,
                restricted = 0,
                notChanged = 0
            };

            var result = await this.ExecutePut(TokenRoleType.UserWithRole, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var filterContent = await Task.Run(() => JsonConvert.SerializeObject(
                    new SimpleFilter
                    {
                        value = testValue,
                        type = SimpleFilterType.name,
                        condition = FilterCondition.equals
                    }));

                var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 17.ToString(),
                        Name = testValue,
                        Path = new List<ElementFolderDto>(),
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "96", Value = 123456 },
                            new ElementAttributeDto { AttributeId = "97", Value = "KZN" },
                            new ElementAttributeDto { AttributeId = "98", Value = 123456.00f },
                            new ElementAttributeDto { AttributeId = "99", Value = "2018-05-25" },
                            new ElementAttributeDto { AttributeId = "100", Value = 1 },
                            new ElementAttributeDto { AttributeId = "101", Value = "Description" }
                        }
                    }
                };

                var elementsContent = await this.TryGetEntities(Method.POST, this.GetSearchUrl(userDimensionId), TokenRoleType.UserWithRole, null, this.Headers, filterContent);
                var updateVerificationResult = this.Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = updateVerificationResult.IsSuccess;
                result.Message += updateVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест обновления элементов по именованному полю типа "level"
        /// Тест-кейс переноса из корня в существующую иерархию каталогов
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact]
        public async Task Update_WithNamedLevelField_FromRootToExistFoldersHierarchy()
        {
            this.Url = this.GetUrl(userDimensionId);

            var filter = new SimpleFilter
            {
                value = "Сигнахи",
                type = SimpleFilterType.name,
                condition = FilterCondition.equals
            };

            var fields = new[]
            {
                new NamedField { value = "Грузия", type = NamedFieldType.level, name = "Страна" },
                new NamedField { value = "Кахетия", type = NamedFieldType.level, name = "Регион" }
            };

            var body = new[] { new { filter, fields } };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new UpdateResult
            {
                updated = 1,
                restricted = 0,
                notChanged = 0
            };

            var result = await this.ExecutePut(TokenRoleType.UserWithRole, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

                var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 19.ToString(),
                        Name = "Сигнахи",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Грузия"},
                            new ElementFolderDto { FolderName = "Кахетия"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "96", Value = 111 },
                            new ElementAttributeDto { AttributeId = "97", Value = "Сиг" },
                            new ElementAttributeDto { AttributeId = "98", Value = 111.00f },
                            new ElementAttributeDto { AttributeId = "99", Value = "2018-05-25" },
                            new ElementAttributeDto { AttributeId = "100", Value = 1 },
                            new ElementAttributeDto { AttributeId = "101", Value = "Тест обновления элементов по именованному полю типа level" }
                        }
                    }
                };

                var elementsContent = await this.TryGetEntities(Method.POST, this.GetSearchUrl(userDimensionId), TokenRoleType.UserWithRole, null, this.Headers, filterContent);
                var updateVerificationResult = this.Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = updateVerificationResult.IsSuccess;
                result.Message += updateVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест обновления элементов по именованному полю типа "level"
        /// Тест-кейс переноса из корня в существующий каталог с созданием дочернего каталога
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact]
        public async Task Update_WithNamedLevelField_FromRootToPartiallyExistFoldersHierarchy()
        {
            this.Url = this.GetUrl(userDimensionId);

            var filter = new SimpleFilter
            {
                value = "Тбилиси",
                type = SimpleFilterType.name,
                condition = FilterCondition.equals
            };

            var fields = new[]
            {
                // Каталог существует
                new NamedField
                {
                    value = "Грузия",
                    type = NamedFieldType.level,
                    name = "Страна"
                },
                // Каталог создатся
                new NamedField
                {
                    value = "Тбилиси",
                    type = NamedFieldType.level,
                    name = "Регион"
                }
            };

            var body = new[] { new { filter, fields } };
            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new UpdateResult
            {
                updated = 1,
                restricted = 0,
                notChanged = 0
            };

            var result = await this.ExecutePut(TokenRoleType.UserWithRole, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

                var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 18.ToString(),
                        Name = "Тбилиси",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Грузия"},
                            new ElementFolderDto { FolderName = "Тбилиси"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "96", Value = 222 },
                            new ElementAttributeDto { AttributeId = "97", Value = "TBS" },
                            new ElementAttributeDto { AttributeId = "98", Value = 222.00f },
                            new ElementAttributeDto { AttributeId = "99", Value = "2018-05-25" },
                            new ElementAttributeDto { AttributeId = "100", Value = 1 },
                            new ElementAttributeDto { AttributeId = "101", Value = "Тест обновления элементов по именованному полю типа level" }
                        }
                    }
                };

                var elementsContent = await this.TryGetEntities(Method.POST, this.GetSearchUrl(userDimensionId), TokenRoleType.UserWithRole, null, this.Headers, filterContent);
                var updateVerificationResult = this.Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = updateVerificationResult.IsSuccess;
                result.Message += updateVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест обновления элементов по именованному полю типа "level"
        /// Тест-кейс переноса из корня с созданием иерархии каталогов
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact]
        public async Task Update_WithNamedLevelField_FromRootToNotExistFoldersHierarchy()
        {
            this.Url = this.GetUrl(userDimensionId);

            var filter = new SimpleFilter
            {
                value = "Ереван",
                type = SimpleFilterType.name,
                condition = FilterCondition.equals
            };

            var fields = new[]
            {
                new NamedField
                {
                    value = "Армения",
                    type = NamedFieldType.level,
                    name = "Страна"
                },
                new NamedField
                {
                    value = "Ереван",
                    type = NamedFieldType.level,
                    name = "Регион"
                }
            };

            var body = new[] { new { filter, fields } };
            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new UpdateResult
            {
                updated = 1,
                restricted = 0,
                notChanged = 0
            };

            var result = await this.ExecutePut(TokenRoleType.UserWithRole, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

                var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 20.ToString(),
                        Name = "Ереван",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Армения"},
                            new ElementFolderDto { FolderName = "Ереван"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "96", Value = 333 },
                            new ElementAttributeDto { AttributeId = "97", Value = "Ереванушка эге-гей" },
                            new ElementAttributeDto { AttributeId = "98", Value = 333.00f },
                            new ElementAttributeDto { AttributeId = "99", Value = "2018-05-25" },
                            new ElementAttributeDto { AttributeId = "100", Value = 1 },
                            new ElementAttributeDto { AttributeId = "101", Value = "Тест обновления элементов по именованному полю типа level" }
                        }
                    }
                };

                var elementsContent = await this.TryGetEntities(Method.POST, this.GetSearchUrl(userDimensionId), TokenRoleType.UserWithRole, null, this.Headers, filterContent);
                var updateVerificationResult = this.Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = updateVerificationResult.IsSuccess;
                result.Message += updateVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест обновления элементов по именованному полю типа "level"
        /// Тест-кейс переноса из существующей иерархии каталогов в каталог с созданием дочернего каталога
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact]
        public async Task Update_WithNamedLevelField_FromExistFoldersHierarchyToPartiallyExistFoldersHierarchy()
        {
            this.Url = this.GetUrl(userDimensionId);

            var filter = new SimpleFilter
            {
                value = "Санкт-Петербург",
                type = SimpleFilterType.name,
                condition = FilterCondition.equals
            };

            var fields = new[]
            {
                // Каталог существует
                new NamedField
                {
                    value = "Россия",
                    type = NamedFieldType.level,
                    name = "Страна"
                },
                // Каталог создатся
                new NamedField
                {
                    value = "Ленинградская область",
                    type = NamedFieldType.level,
                    name = "Регион"
                }
            };

            var body = new[] { new { filter, fields } };
            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new UpdateResult
            {
                updated = 1,
                restricted = 0,
                notChanged = 0
            };

            var result = await this.ExecutePut(TokenRoleType.UserWithRole, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

                var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 6.ToString(),
                        Name = "Санкт-Петербург",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Ленинградская область"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "96", Value = 333 },
                            new ElementAttributeDto { AttributeId = "97", Value = "В Питере тире пить" },
                            new ElementAttributeDto { AttributeId = "98", Value = 333.00f },
                            new ElementAttributeDto { AttributeId = "99", Value = "2018-05-25" },
                            new ElementAttributeDto { AttributeId = "100", Value = 1 },
                            new ElementAttributeDto { AttributeId = "101", Value = "Тест обновления элементов по именованному полю типа level" }
                        }
                    }
                };

                var elementsContent = await this.TryGetEntities(Method.POST, this.GetSearchUrl(userDimensionId), TokenRoleType.UserWithRole, null, this.Headers, filterContent);
                var updateVerificationResult = this.Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = updateVerificationResult.IsSuccess;
                result.Message += updateVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест обновления элементов по именованному полю типа "level"
        /// Тест-кейс переноса из существующей иерархии каталогов в существующую иерархию каталогов
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact]
        public async Task Update_WithNamedLevelField_FromExistFoldersHierarchyToExistFoldersHierarchy()
        {
            this.Url = this.GetUrl(userDimensionId);

            var filter = new SimpleFilter
            {
                value = "Мытищи",
                type = SimpleFilterType.name,
                condition = FilterCondition.equals
            };

            var fields = new[]
            {
                 new NamedField
                 {
                     value = "Россия",
                     type = NamedFieldType.level,
                     name = "Страна"
                 },
                new NamedField
                {
                    value = "Московская область",
                    type = NamedFieldType.level,
                    name = "Регион"
                }
            };

            var body = new[] { new { filter, fields } };
            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new UpdateResult
            {
                updated = 1,
                restricted = 0,
                notChanged = 0
            };

            var result = await this.ExecutePut(TokenRoleType.UserWithRole, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

                var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 8.ToString(),
                        Name = "Мытищи",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Московская область"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "96", Value = 55555 },
                            new ElementAttributeDto { AttributeId = "97", Value = "Город гопников" },
                            new ElementAttributeDto { AttributeId = "98", Value = 55555.00f },
                            new ElementAttributeDto { AttributeId = "99", Value = "2018-05-25" },
                            new ElementAttributeDto { AttributeId = "100", Value = 1 },
                            new ElementAttributeDto { AttributeId = "101", Value = "Тест обновления элементов по именованному полю типа level" }
                        }
                    }
                };

                var elementsContent = await this.TryGetEntities(Method.POST, this.GetSearchUrl(userDimensionId), TokenRoleType.UserWithRole, null, this.Headers, filterContent);
                var updateVerificationResult = this.Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = updateVerificationResult.IsSuccess;
                result.Message += updateVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест обновления элементов по именованному полю типа "level"
        /// Тест-кейс переименования элемента при передаче в именованном поле наименования уровня элемента
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact]
        public async Task UpdateElementName_WithNamedLevelField()
        {
            this.Url = this.GetUrl(userDimensionId);

            var filter = new SimpleFilter
            {
                value = "Куйбышев",
                type = SimpleFilterType.name,
                condition = FilterCondition.equals
            };

            var fields = new[]
            {
                new NamedField
                {
                    value = "Самара",
                    type = NamedFieldType.level,
                    name = "Город"
                }
            };

            var body = new[] { new { filter, fields } };
            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new UpdateResult
            {
                updated = 1,
                restricted = 0,
                notChanged = 0
            };

            var result = await this.ExecutePut(TokenRoleType.UserWithRole, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var filterContent = await Task.Run(() => JsonConvert.SerializeObject(
                    new SimpleFilter
                    {
                        value = "Самара",
                        type = SimpleFilterType.name,
                        condition = FilterCondition.equals
                    }));

                var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 7.ToString(),
                        Name = "Самара",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Самарская область"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "96", Value = 55555 },
                            new ElementAttributeDto { AttributeId = "97", Value = "Ах, Самара-городо" },
                            new ElementAttributeDto { AttributeId = "98", Value = 55555.00f },
                            new ElementAttributeDto { AttributeId = "99", Value = "2018-05-25" },
                            new ElementAttributeDto { AttributeId = "100", Value = 1 },
                            new ElementAttributeDto { AttributeId = "101", Value = "Тест обновления элементов по именованному полю типа level" }
                        }
                    }
                };

                var elementsContent = await this.TryGetEntities(Method.POST, this.GetSearchUrl(userDimensionId), TokenRoleType.UserWithRole, null, this.Headers, filterContent);
                var updateVerificationResult = this.Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = updateVerificationResult.IsSuccess;
                result.Message += updateVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест обновления элементов по именованному полю типа "attribute" для атрибута типа Int
        /// </summary>
        /// <param name="attributeValue">Значение тестируемого типа атрибута</param>
        /// <returns>Ожидаемый результат - положительный</returns>              
        [Theory]
        [InlineData(null)]
        [InlineData(TestValues.IntValue)]
        [InlineData(TestValues.FloatValueConvertibleToInt)]
        [InlineData(TestValues.StringValueConvertibleToInt)]
        public async Task UpdateAll_NamedAttributeField_ForIntAttribute(object attributeValue)
        {
            this.Url = this.GetUrl(userDimensionId);

            var filter = new SimpleFilter
            {
                value = "Элемент для проверки обновления атрибута Int",
                type = SimpleFilterType.name,
                condition = FilterCondition.equals
            };

            var fields = new[]
            {
                new NamedField
                {
                    value = attributeValue,
                    type = NamedFieldType.attribute,
                    name = "Численность населения"
                }
            };

            var body = new[] { new { filter, fields } };
            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new UpdateResult
            {
                updated = 1,
                restricted = 0,
                notChanged = 0
            };

            var result = await this.ExecutePut(TokenRoleType.UserWithRole, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

                var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 9.ToString(),
                        Name = "Элемент для проверки обновления атрибута Int",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Проверки"},
                            new ElementFolderDto { FolderName = "Обновление атрибутов"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "96", Value = attributeValue },
                            new ElementAttributeDto { AttributeId = "97", Value = "Int" },
                            new ElementAttributeDto { AttributeId = "98", Value = 999.00f },
                            new ElementAttributeDto { AttributeId = "99", Value = "2018-05-25" },
                            new ElementAttributeDto { AttributeId = "100", Value = 1 },
                            new ElementAttributeDto { AttributeId = "101", Value = "<p>Update attribute</p>" }
                        }
                    }
                };

                var elementsContent = await this.TryGetEntities(Method.POST, this.GetSearchUrl(userDimensionId), TokenRoleType.UserWithRole, null, this.Headers, filterContent);
                var updateVerificationResult = this.Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = updateVerificationResult.IsSuccess;
                result.Message += updateVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест обновления элементов по именованному полю типа "attribute" для атрибута типа Int
        /// </summary>
        /// <param name="attributeValue">Значение тестируемого типа атрибута</param>
        /// <returns>Ожидаемый результат - отрицательный</returns>              
        [Theory]
        [InlineData(TestValues.FloatValue)]
        [InlineData(TestValues.StringValue)]
        public async Task UpdateAll_NamedAttributeField_ForInvalidIntAttribute(object attributeValue)
        {
            this.Url = this.GetUrl(userDimensionId);

            var filter = new SimpleFilter
            {
                value = "Элемент для проверки обновления атрибута Int",
                type = SimpleFilterType.name,
                condition = FilterCondition.equals
            };

            var fields = new[]
            {
                new NamedField
                {
                    value = attributeValue,
                    type = NamedFieldType.attribute,
                    name = "Численность населения"
                }
            };

            var body = new[] { new { filter, fields } };
            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new UpdateResult
            {
                updated = 1,
                restricted = 0,
                notChanged = 0
            };

            var result = await this.ExecutePut(TokenRoleType.UserWithRole, expectedResult, null, bodyContent);

            Assert.True(!result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест обновления элементов по именованному полю типа "attribute" для атрибута типа Float
        /// </summary>
        /// <param name="attributeValue">Значение тестируемого типа атрибута</param>
        /// <returns>Ожидаемый результат - положительный</returns>              
        [Theory]
        [InlineData(null)]
        [InlineData(TestValues.IntValue)]
        [InlineData(TestValues.FloatValue)]
        public async Task UpdateAll_NamedAttributeField_ForFloatAttribute(object attributeValue)
        {
            this.Url = this.GetUrl(userDimensionId);

            var filter = new SimpleFilter
            {
                value = "Элемент для проверки обновления атрибута Float",
                type = SimpleFilterType.name,
                condition = FilterCondition.equals
            };

            var fields = new[]
            {
                new NamedField
                {
                    value = attributeValue,
                    type = NamedFieldType.attribute,
                    name = "Площадь жилой зоны"
                }
            };

            var body = new[] { new { filter, fields } };
            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new UpdateResult
            {
                updated = 1,
                restricted = 0,
                notChanged = 0
            };

            var result = await this.ExecutePut(TokenRoleType.UserWithRole, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

                var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 10.ToString(),
                        Name = "Элемент для проверки обновления атрибута Float",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Проверки"},
                            new ElementFolderDto { FolderName = "Обновление атрибутов"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "96", Value = 999 },
                            new ElementAttributeDto { AttributeId = "97", Value = "Float" },
                            new ElementAttributeDto { AttributeId = "98", Value = attributeValue },
                            new ElementAttributeDto { AttributeId = "99", Value = "2018-05-25" },
                            new ElementAttributeDto { AttributeId = "100", Value = 1 },
                            new ElementAttributeDto { AttributeId = "101", Value = "<p>Update attribute</p>" }
                        }
                    }
                };

                var elementsContent = await this.TryGetEntities(Method.POST, this.GetSearchUrl(userDimensionId), TokenRoleType.UserWithRole, null, this.Headers, filterContent);
                var updateVerificationResult = this.Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = updateVerificationResult.IsSuccess;
                result.Message += updateVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест обновления элементов по именованному полю типа "attribute" для атрибута типа Float
        /// </summary>
        /// <param name="attributeValue">Значение тестируемого типа атрибута</param>
        /// <returns>Ожидаемый результат - отрицательный</returns>              
        [Theory]
        [InlineData(TestValues.StringValue)]
        public async Task UpdateAll_NamedAttributeField_ForInvalidFloatAttribute(object attributeValue)
        {
            this.Url = this.GetUrl(userDimensionId);

            var filter = new SimpleFilter
            {
                value = "Элемент для проверки обновления атрибута Float",
                type = SimpleFilterType.name,
                condition = FilterCondition.equals
            };

            var fields = new[]
            {
                new NamedField
                {
                    value = attributeValue,
                    type = NamedFieldType.attribute,
                    name = "Площадь жилой зоны"
                }
            };

            var body = new[] { new { filter, fields } };
            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new UpdateResult
            {
                updated = 1,
                restricted = 0,
                notChanged = 0
            };

            var result = await this.ExecutePut(TokenRoleType.UserWithRole, expectedResult, null, bodyContent);

            Assert.True(!result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест обновления элементов по именованному полю типа "attribute" для атрибута типа String
        /// </summary>
        /// <param name="attributeValue">Значение тестируемого типа атрибута</param>
        /// <returns>Ожидаемый результат - положительный</returns>              
        [Theory]
        [InlineData(TestValues.StringValue)]
        [InlineData(TestValues.IntValue)]
        [InlineData(null)]
        public async Task UpdateAll_NamedAttributeField_ForStringAttribute(object attributeValue)
        {
            this.Url = this.GetUrl(userDimensionId);

            var filter = new SimpleFilter
            {
                value = "Элемент для проверки обновления атрибута String",
                type = SimpleFilterType.name,
                condition = FilterCondition.equals
            };

            var fields = new[]
            {
                new NamedField
                {
                    value = attributeValue,
                    type = NamedFieldType.attribute,
                    name = "Краткое наименование"
                }
            };

            var body = new[] { new { filter, fields } };
            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new UpdateResult
            {
                updated = 1,
                restricted = 0,
                notChanged = 0
            };

            var result = await this.ExecutePut(TokenRoleType.UserWithRole, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

                var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 11.ToString(),
                        Name = "Элемент для проверки обновления атрибута String",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Проверки"},
                            new ElementFolderDto { FolderName = "Обновление атрибутов"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "96", Value = 999 },
                            new ElementAttributeDto { AttributeId = "97", Value = attributeValue },
                            new ElementAttributeDto { AttributeId = "98", Value = 999.00f },
                            new ElementAttributeDto { AttributeId = "99", Value = "2018-05-25" },
                            new ElementAttributeDto { AttributeId = "100", Value = 1 },
                            new ElementAttributeDto { AttributeId = "101", Value = "<p>Update attribute</p>" }
                        }
                    }
                };

                var elementsContent = await this.TryGetEntities(Method.POST, this.GetSearchUrl(userDimensionId), TokenRoleType.UserWithRole, null, this.Headers, filterContent);
                var updateVerificationResult = this.Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = updateVerificationResult.IsSuccess;
                result.Message += updateVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест обновления элементов по именованному полю типа "attribute" для атрибута типа Date
        /// </summary>
        /// <param name="attributeValue">Значение тестируемого типа атрибута</param>
        /// <returns>Ожидаемый результат - положительный</returns>              
        [Theory]
        [InlineData(TestValues.DateValueToStringCorrectFormat)]
        [InlineData(null)]
        public async Task UpdateAll_NamedAttributeField_ForDateAttribute(object attributeValue)
        {
            this.Url = this.GetUrl(userDimensionId);

            var filter = new SimpleFilter
            {
                value = "Элемент для проверки обновления атрибута Date",
                type = SimpleFilterType.name,
                condition = FilterCondition.equals
            };

            var fields = new[]
            {
                new NamedField
                {
                    value = attributeValue,
                    type = NamedFieldType.attribute,
                    name = "Дата согласования данных"
                }
            };

            var body = new[] { new { filter, fields } };
            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new UpdateResult
            {
                updated = 1,
                restricted = 0,
                notChanged = 0
            };

            var result = await this.ExecutePut(TokenRoleType.UserWithRole, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

                var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 12.ToString(),
                        Name = "Элемент для проверки обновления атрибута Date",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Проверки"},
                            new ElementFolderDto { FolderName = "Обновление атрибутов"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "96", Value = 999 },
                            new ElementAttributeDto { AttributeId = "97", Value = "Date" },
                            new ElementAttributeDto { AttributeId = "98", Value = 999.00f },
                            new ElementAttributeDto { AttributeId = "99", Value = attributeValue },
                            new ElementAttributeDto { AttributeId = "100", Value = 1 },
                            new ElementAttributeDto { AttributeId = "101", Value = "<p>Update attribute</p>" }
                        }
                    }
                };

                var elementsContent = await this.TryGetEntities(Method.POST, this.GetSearchUrl(userDimensionId), TokenRoleType.UserWithRole, null, this.Headers, filterContent);
                var updateVerificationResult = this.Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = updateVerificationResult.IsSuccess;
                result.Message += updateVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест обновления элементов по именованному полю типа "attribute" для атрибута типа Date
        /// </summary>
        /// <param name="attributeValue">Значение тестируемого типа атрибута</param>
        /// <returns>Ожидаемый результат - отрицательный</returns>              
        [Theory]
        [InlineData(TestValues.DateValueToStringIncorrectFormat)]
        [InlineData(TestValues.StringValue)]
        public async Task UpdateAll_NamedAttributeField_ForInvalidDateAttribute(object attributeValue)
        {
            this.Url = this.GetUrl(userDimensionId);

            var filter = new SimpleFilter
            {
                value = "Элемент для проверки обновления атрибута Date",
                type = SimpleFilterType.name,
                condition = FilterCondition.equals
            };

            var fields = new[]
            {
                new NamedField
                {
                    value = attributeValue,
                    type = NamedFieldType.attribute,
                    name = "Дата согласования данных"
                }
            };

            var body = new[] { new { filter, fields } };
            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new UpdateResult
            {
                updated = 1,
                restricted = 0,
                notChanged = 0
            };

            var result = await this.ExecutePut(TokenRoleType.UserWithRole, expectedResult, null, bodyContent);

            Assert.True(!result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест обновления элементов по именованному полю типа "attribute" для атрибута типа Link
        /// </summary>
        /// <param name="attributeValue">Значение тестируемого типа атрибута</param>
        /// <returns>Ожидаемый результат - положительный</returns>              
        [Theory]
        [InlineData(TestValues.StringValueConvertibleToInt)]
        [InlineData(TestValues.FloatValueConvertibleToInt)]
        [InlineData(null)]
        [InlineData(4)]
        public async Task UpdateAll_NamedAttributeField_ForLinkAttribute(object attributeValue)
        {
            this.Url = this.GetUrl(userDimensionId);

            var filter = new SimpleFilter
            {
                value = "Элемент для проверки обновления атрибута Link",
                type = SimpleFilterType.name,
                condition = FilterCondition.equals
            };

            var fields = new[]
            {
                new NamedField
                {
                    value = attributeValue,
                    type = NamedFieldType.attribute,
                    name = "Тип населенного пункта"
                }
            };

            var body = new[] { new { filter, fields } };
            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new UpdateResult
            {
                updated = 1,
                restricted = 0,
                notChanged = 0
            };

            var result = await this.ExecutePut(TokenRoleType.UserWithRole, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

                var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 13.ToString(),
                        Name = "Элемент для проверки обновления атрибута Link",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Проверки"},
                            new ElementFolderDto { FolderName = "Обновление атрибутов"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "96", Value = 999 },
                            new ElementAttributeDto { AttributeId = "97", Value = "Link" },
                            new ElementAttributeDto { AttributeId = "98", Value = 999.00f },
                            new ElementAttributeDto { AttributeId = "99", Value = "2018-05-25" },
                            new ElementAttributeDto { AttributeId = "100", Value = attributeValue },
                            new ElementAttributeDto { AttributeId = "101", Value = "<p>Update attribute</p>" }
                        }
                    }
                };

                var elementsContent = await this.TryGetEntities(Method.POST, this.GetSearchUrl(userDimensionId), TokenRoleType.UserWithRole, null, this.Headers, filterContent);
                var updateVerificationResult = this.Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = updateVerificationResult.IsSuccess;
                result.Message += updateVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест обновления элементов по именованному полю типа "attribute" для атрибута типа Link
        /// </summary>
        /// <param name="attributeValue">Значение тестируемого типа атрибута</param>
        /// <returns>Ожидаемый результат - отрицательный</returns>              
        [Theory]
        [InlineData(TestValues.StringValue)]
        [InlineData(TestValues.NonExistId)]
        [InlineData(TestValues.ZeroId)]
        public async Task UpdateAll_NamedAttributeField_ForInvalidLinkAttribute(object attributeValue)
        {
            this.Url = this.GetUrl(userDimensionId);

            var filter = new SimpleFilter
            {
                value = "Элемент для проверки обновления атрибута Link",
                type = SimpleFilterType.name,
                condition = FilterCondition.equals
            };

            var fields = new[]
            {
                new NamedField
                {
                    value = attributeValue,
                    type = NamedFieldType.attribute,
                    name = "Тип населенного пункта"
                }
            };

            var body = new[] { new { filter, fields } };
            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new UpdateResult
            {
                updated = 1,
                restricted = 0,
                notChanged = 0
            };

            var result = await this.ExecutePut(TokenRoleType.UserWithRole, expectedResult, null, bodyContent);

            Assert.True(!result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест обновления элементов по именованному полю типа "attribute" для атрибута типа Text
        /// </summary>
        /// <param name="attributeValue">Значение тестируемого типа атрибута</param>
        /// <returns>Ожидаемый результат - положительный</returns>              
        [Theory]
        [InlineData(TestValues.TextValue)]
        [InlineData(TestValues.IntValue)]
        [InlineData(null)]
        public async Task UpdateAll_NamedAttributeField_ForTextAttribute(object attributeValue)
        {
            this.Url = this.GetUrl(userDimensionId);

            var filter = new SimpleFilter
            {
                value = "Элемент для проверки обновления атрибута Text",
                type = SimpleFilterType.name,
                condition = FilterCondition.equals
            };

            var fields = new[]
            {
                new NamedField
                {
                    value = attributeValue,
                    type = NamedFieldType.attribute,
                    name = "Описание города"
                }
            };

            var body = new[] { new { filter, fields } };
            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new UpdateResult
            {
                updated = 1,
                restricted = 0,
                notChanged = 0
            };

            var result = await this.ExecutePut(TokenRoleType.UserWithRole, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

                var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 14.ToString(),
                        Name = "Элемент для проверки обновления атрибута Text",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Проверки"},
                            new ElementFolderDto { FolderName = "Обновление атрибутов"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "96", Value = 999 },
                            new ElementAttributeDto { AttributeId = "97", Value = "Text" },
                            new ElementAttributeDto { AttributeId = "98", Value = 999.00f },
                            new ElementAttributeDto { AttributeId = "99", Value = "2018-05-25" },
                            new ElementAttributeDto { AttributeId = "100", Value = 1 },
                            new ElementAttributeDto { AttributeId = "101", Value = attributeValue }
                        }
                    }
                };

                var elementsContent = await this.TryGetEntities(Method.POST, this.GetSearchUrl(userDimensionId), TokenRoleType.UserWithRole, null, this.Headers, filterContent);
                var updateVerificationResult = this.Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = updateVerificationResult.IsSuccess;
                result.Message += updateVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест обновления  элементов по именованному полю типа "attribute" для всех типов атрибутов одновременно
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>              
        [Fact]
        public async Task Update_NamedAttributeField_ForAllAttribute()
        {
            this.Url = this.GetUrl(userDimensionId);

            var filter = new SimpleFilter
            {
                value = "Элемент для проверки обновления атрибута",
                type = SimpleFilterType.name,
                condition = FilterCondition.equals
            };

            var fields = new[]
            {
                new NamedField
                {
                    value = TestValues.IntValue,
                    type = NamedFieldType.attribute,
                    name = "Численность населения"
                },
                new NamedField
                {
                    value = TestValues.StringValue,
                    type = NamedFieldType.attribute,
                    name = "Краткое наименование"
                },
                new NamedField
                {
                    value = TestValues.FloatValue,
                    type = NamedFieldType.attribute,
                    name = "Площадь жилой зоны"
                },
                new NamedField
                {
                    value = TestValues.DateValueToStringCorrectFormat,
                    type = NamedFieldType.attribute,
                    name = "Дата согласования данных"
                },
                new NamedField
                {
                    value = 2,
                    type = NamedFieldType.attribute,
                    name = "Тип населенного пункта"
                },
                new NamedField
                {
                    value = TestValues.TextValue,
                    type = NamedFieldType.attribute,
                    name = "Описание города"
                },
            };

            var body = new[] { new { filter, fields } };
            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new UpdateResult
            {
                updated = 1,
                restricted = 0,
                notChanged = 0
            };

            var result = await this.ExecutePut(TokenRoleType.UserWithRole, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

                var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 15.ToString(),
                        Name = "Элемент для проверки обновления атрибута",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Проверки"},
                            new ElementFolderDto { FolderName = "Обновление атрибутов"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "96", Value = TestValues.IntValue },
                            new ElementAttributeDto { AttributeId = "97", Value = TestValues.StringValue },
                            new ElementAttributeDto { AttributeId = "98", Value = TestValues.FloatValue },
                            new ElementAttributeDto { AttributeId = "99", Value = TestValues.DateValueToStringCorrectFormat },
                            new ElementAttributeDto { AttributeId = "100", Value = 2 },
                            new ElementAttributeDto { AttributeId = "101", Value = TestValues.TextValue }
                        }
                    }
                };

                var elementsContent = await this.TryGetEntities(Method.POST, this.GetSearchUrl(userDimensionId), TokenRoleType.UserWithRole, null, this.Headers, filterContent);
                var updateVerificationResult = this.Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = updateVerificationResult.IsSuccess;
                result.Message += updateVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест обновления  элементов без изменений
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>              
        [Fact]
        public async Task Update_WithoutChanges()
        {
            this.Url = this.GetUrl(userDimensionId);

            var testValue = "Элемент для обновления без изменений";

            var filter = new SimpleFilter
            {
                value = "Элемент для обновления без изменений",
                type = SimpleFilterType.name,
                condition = FilterCondition.equals
            };

            var fields = new[]
            {
                new NamedField
                {
                    value = testValue,
                    type = NamedFieldType.attribute,
                    name = "Краткое наименование"
                }
            };

            var body = new[] { new { filter, fields } };
            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new UpdateResult
            {
                updated = 0,
                restricted = 0,
                notChanged = 1
            };

            var result = await this.ExecutePut(TokenRoleType.UserWithRole, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

                var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 16.ToString(),
                        Name = "Элемент для обновления без изменений",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Проверки"},
                            new ElementFolderDto { FolderName = "Обновление без изменений"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "96", Value = 12 },
                            new ElementAttributeDto { AttributeId = "97", Value = testValue },
                            new ElementAttributeDto { AttributeId = "98", Value = 12.00f },
                            new ElementAttributeDto { AttributeId = "99", Value = "2018-05-25" },
                            new ElementAttributeDto { AttributeId = "100", Value = 1 },
                            new ElementAttributeDto { AttributeId = "101", Value = "Описание" }
                        }
                    }
                };

                var elementsContent = await this.TryGetEntities(Method.POST, this.GetSearchUrl(userDimensionId), TokenRoleType.UserWithRole, null, this.Headers, filterContent);
                var updateVerificationResult = this.Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = updateVerificationResult.IsSuccess;
                result.Message += updateVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест обновления  элементов, недоступных для обновления
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>               
        [Fact]
        public async Task Update_WithRestricted()
        {
            this.Url = this.GetUrl(restrictedElementsDimensionId);

            var fields = new[]
            {
                new NamedField
                {
                    value = "Обновленное значение по атрибуту",
                    type = NamedFieldType.attribute,
                    name = "Краткое наименование"
                }
            };

            var body = new[] { new { fields } };
            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new UpdateResult
            {
                updated = 5,
                restricted = 4,
                notChanged = 3
            };

            var result = await this.ExecutePut(TokenRoleType.UserWithRole, expectedResult, null, bodyContent);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест обновления  атрибута типа Link Элемента в случае отсутсвия у пользователя прав на линкованное измерение
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>               
        [Fact]
        public async Task Update_WithInvalidLinkAttributePermissions()
        {
            this.Url = this.GetUrl(forLinkAttributeDimensionId);

            var fields = new[]
            {
                new NamedField
                {
                    value = 2,
                    type = NamedFieldType.attribute,
                    name = "Тип населенного пункта"
                }
            };

            var body = new[] { new { fields } };
            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new UpdateResult
            {
                updated = 4,
                restricted = 0,
                notChanged = 0
            };

            var result = await this.ExecutePut(TokenRoleType.UserWithRole, expectedResult, null, bodyContent);

            Assert.True(!result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест обновления элементов простым фильтром типа "id"
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact]
        public async Task Update_WithSimpleIdFilter()
        {
            this.Url = this.GetUrl(forFiltersDimensionId);

            var filter = new SimpleFilter
            {
                value = 4,
                type = SimpleFilterType.id,
                condition = FilterCondition.equals
            };

            var testValue = "Тест обновления элементов простым фильтром типа id";

            var fields = new[]
            {
                new NamedField
                {
                    value = testValue,
                    type = NamedFieldType.attribute,
                    name = "Описание города"
                }
            };

            var body = new[] { new { filter, fields } };
            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new UpdateResult
            {
                updated = 1,
                restricted = 0,
                notChanged = 0
            };

            var result = await this.ExecutePut(TokenRoleType.UserWithRole, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
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
                            new ElementAttributeDto { AttributeId = "67", Value = 4000000 },
                            new ElementAttributeDto { AttributeId = "68", Value = "Msk" },
                            new ElementAttributeDto { AttributeId = "69", Value = 4000000.00f },
                            new ElementAttributeDto { AttributeId = "70", Value = "2018-05-01" },
                            new ElementAttributeDto { AttributeId = "71", Value = 1 },
                            new ElementAttributeDto { AttributeId = "72", Value = testValue }
                        }
                    }
                };

                var elementsContent = await this.TryGetEntities(Method.POST, this.GetSearchUrl(forFiltersDimensionId), TokenRoleType.UserWithRole, null, this.Headers, filterContent);
                var updateVerificationResult = this.Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = updateVerificationResult.IsSuccess;
                result.Message += updateVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест обновления элементов простым фильтром типа "name"
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact]
        public async Task Update_WithSimpleNameFilter()
        {
            this.Url = this.GetUrl(forFiltersDimensionId);

            var filter = new SimpleFilter
            {
                value = "Мытищи",
                type = SimpleFilterType.name,
                condition = FilterCondition.equals
            };

            var testValue = "Тест обновления элементов простым фильтром типа name";
            var fields = new[]
            {
                new NamedField
                {
                    value = testValue,
                    type = NamedFieldType.attribute,
                    name = "Описание города"
                }
            };

            var body = new[] { new { filter, fields } };
            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new UpdateResult
            {
                updated = 1,
                restricted = 0,
                notChanged = 0
            };

            var result = await this.ExecutePut(TokenRoleType.UserWithRole, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

                var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 7.ToString(),
                        Name = "Мытищи",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Россия"},
                            new ElementFolderDto { FolderName = "Московская область"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "67", Value = 1111111 },
                            new ElementAttributeDto { AttributeId = "68", Value = "MTSHU" },
                            new ElementAttributeDto { AttributeId = "69", Value = 1111111.00f },
                            new ElementAttributeDto { AttributeId = "70", Value = "2018-05-21" },
                            new ElementAttributeDto { AttributeId = "71", Value = 1 },
                            new ElementAttributeDto { AttributeId = "72", Value = testValue }
                        }
                    }
                };

                var elementsContent = await this.TryGetEntities(Method.POST, this.GetSearchUrl(forFiltersDimensionId), TokenRoleType.UserWithRole, null, this.Headers, filterContent);
                var updateVerificationResult = this.Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = updateVerificationResult.IsSuccess;
                result.Message += updateVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест обновления элементов с именованным фильтром "level"
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact]
        public async Task Update_WithNamedLevelFilter()
        {
            this.Url = this.GetUrl(forFiltersDimensionId);

            var filter = new NamedFilter
            {
                value = "Татарстан",
                type = NamedFilterType.level,
                name = "Регион",
                condition = FilterCondition.equals
            };

            var testValue = "Тест обновления элементов с именованным фильтром level";
            var fields = new[]
            {
                new NamedField
                {
                    value = testValue,
                    type = NamedFieldType.attribute,
                    name = "Описание города"
                }
            };

            var body = new[] { new { filter, fields } };
            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new UpdateResult
            {
                updated = 3,
                restricted = 0,
                notChanged = 0
            };

            var result = await this.ExecutePut(TokenRoleType.UserWithRole, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
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
                            new ElementAttributeDto { AttributeId = "67", Value = 3000000 },
                            new ElementAttributeDto { AttributeId = "68", Value = "Name" },
                            new ElementAttributeDto { AttributeId = "69", Value = 10000.00f },
                            new ElementAttributeDto { AttributeId = "70", Value = "2018-05-23" },
                            new ElementAttributeDto { AttributeId = "71", Value = 1 },
                            new ElementAttributeDto { AttributeId = "72", Value = testValue }
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
                            new ElementAttributeDto { AttributeId = "67", Value = 3000000 },
                            new ElementAttributeDto { AttributeId = "68", Value = "Name" },
                            new ElementAttributeDto { AttributeId = "69", Value = 10000.00f },
                            new ElementAttributeDto { AttributeId = "70", Value = "2018-05-23" },
                            new ElementAttributeDto { AttributeId = "71", Value = 1 },
                            new ElementAttributeDto { AttributeId = "72", Value = testValue }
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
                            new ElementAttributeDto { AttributeId = "67", Value = 3000000 },
                            new ElementAttributeDto { AttributeId = "68", Value = "Name" },
                            new ElementAttributeDto { AttributeId = "69", Value = 10000.00f },
                            new ElementAttributeDto { AttributeId = "70", Value = "2018-05-23" },
                            new ElementAttributeDto { AttributeId = "71", Value = 1 },
                            new ElementAttributeDto { AttributeId = "72", Value = testValue }
                        }
                    }
                };

                var elementsContent = await this.TryGetEntities(Method.POST, this.GetSearchUrl(forFiltersDimensionId), TokenRoleType.UserWithRole, null, this.Headers, filterContent);
                var updateVerificationResult = this.Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = updateVerificationResult.IsSuccess;
                result.Message += updateVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест обновления элементов с именованным фильтром "level" по конечному уровню
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact]
        public async Task Update_WithNamedLevelFilterForLastLevel()
        {
            this.Url = this.GetUrl(forFiltersDimensionId);

            var filter = new NamedFilter
            {
                value = "Сигнахи",
                type = NamedFilterType.level,
                name = "Город",
                condition = FilterCondition.equals
            };

            var testValue = "Тест обновления элементов с именованным фильтром level по конечному уровню";
            var fields = new[]
            {
                new NamedField
                {
                    value = testValue,
                    type = NamedFieldType.attribute,
                    name = "Описание города"
                }
            };

            var body = new[] { new { filter, fields } };
            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new UpdateResult
            {
                updated = 1,
                restricted = 0,
                notChanged = 0
            };

            var result = await this.ExecutePut(TokenRoleType.UserWithRole, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

                var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 5.ToString(),
                        Name = "Сигнахи",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Грузия"},
                            new ElementFolderDto { FolderName = "Кахетия"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "67", Value = 1000000 },
                            new ElementAttributeDto { AttributeId = "68", Value = "Sgn" },
                            new ElementAttributeDto { AttributeId = "69", Value = 1000000.00f },
                            new ElementAttributeDto { AttributeId = "70", Value = "2018-05-22" },
                            new ElementAttributeDto { AttributeId = "71", Value = 1 },
                            new ElementAttributeDto { AttributeId = "72", Value = testValue }
                        }
                    }
                };

                var elementsContent = await this.TryGetEntities(Method.POST, this.GetSearchUrl(forFiltersDimensionId), TokenRoleType.UserWithRole, null, this.Headers, filterContent);
                var updateVerificationResult = this.Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = updateVerificationResult.IsSuccess;
                result.Message += updateVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест обновления элементов с именованным фильтром "attribute", где атрибут типа int
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact]
        public async Task Update_WithNamedAttributeFilter_ForIntAttribute()
        {
            this.Url = this.GetUrl(forFiltersDimensionId);

            var filter = new NamedFilter
            {
                value = "-1",
                type = NamedFilterType.attribute,
                name = "Численность населения",
                condition = FilterCondition.equals
            };

            var testValue = "Тест обновления элементов с именованным фильтром attribute, где атрибут типа int";

            var fields = new[]
            {
                new NamedField
                {
                    value = testValue,
                    type = NamedFieldType.attribute,
                    name = "Описание города"
                }
            };

            var body = new[]
            {
                new { filter, fields }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new UpdateResult
            {
                updated = 1,
                restricted = 0,
                notChanged = 0
            };

            var result = await this.ExecutePut(TokenRoleType.UserWithRole, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

                var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 10.ToString(),
                        Name = "int attribute",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Фильтры"},
                            new ElementFolderDto { FolderName = "По атрибутам"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "67", Value = -1 },
                            new ElementAttributeDto { AttributeId = "68", Value = null },
                            new ElementAttributeDto { AttributeId = "69", Value = null },
                            new ElementAttributeDto { AttributeId = "70", Value = null },
                            new ElementAttributeDto { AttributeId = "71", Value = null },
                            new ElementAttributeDto { AttributeId = "72", Value = testValue }
                        }
                    }
                };

                var elementsContent = await this.TryGetEntities(Method.POST, this.GetSearchUrl(forFiltersDimensionId), TokenRoleType.UserWithRole, null, this.Headers, filterContent);
                var updateVerificationResult = this.Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = updateVerificationResult.IsSuccess;
                result.Message += updateVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест обновления элементов с именованным фильтром "attribute", где атрибут типа string
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact]
        public async Task Update_WithNamedAttributeFilter_ForStringAttribute()
        {
            this.Url = this.GetUrl(forFiltersDimensionId);

            var filter = new NamedFilter
            {
                value = "-2",
                type = NamedFilterType.attribute,
                name = "Краткое наименование",
                condition = FilterCondition.equals
            };

            var testValue = "Тест обновления элементов с именованным фильтром attribute, где атрибут типа String";

            var fields = new[]
            {
                new NamedField
                {
                    value = testValue,
                    type = NamedFieldType.attribute,
                    name = "Описание города"
                }
            };

            var body = new[]
            {
                new { filter, fields }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new UpdateResult
            {
                updated = 1,
                restricted = 0,
                notChanged = 0
            };

            var result = await this.ExecutePut(TokenRoleType.UserWithRole, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

                var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 11.ToString(),
                        Name = "string attribute",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Фильтры"},
                            new ElementFolderDto { FolderName = "По атрибутам"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "67", Value = null },
                            new ElementAttributeDto { AttributeId = "68", Value = "-2" },
                            new ElementAttributeDto { AttributeId = "69", Value = null },
                            new ElementAttributeDto { AttributeId = "70", Value = null },
                            new ElementAttributeDto { AttributeId = "71", Value = null },
                            new ElementAttributeDto { AttributeId = "72", Value = testValue }
                        }
                    }
                };

                var elementsContent = await this.TryGetEntities(Method.POST, this.GetSearchUrl(forFiltersDimensionId), TokenRoleType.UserWithRole, null, this.Headers, filterContent);
                var updateVerificationResult = this.Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = updateVerificationResult.IsSuccess;
                result.Message += updateVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест обновления элементов с именованным фильтром "attribute", где атрибут типа Float
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact]
        public async Task Update_WithNamedAttributeFilter_ForFloatAttribute()
        {
            this.Url = this.GetUrl(forFiltersDimensionId);

            var filter = new NamedFilter
            {
                value = "-3",
                type = NamedFilterType.attribute,
                name = "Площадь жилой зоны",
                condition = FilterCondition.equals
            };

            var testValue = "Тест обновления элементов с именованным фильтром attribute, где атрибут типа Float";

            var fields = new[]
            {
                new NamedField
                {
                    value = testValue,
                    type = NamedFieldType.attribute,
                    name = "Описание города"
                }
            };

            var body = new[]
            {
                new { filter, fields }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new UpdateResult
            {
                updated = 1,
                restricted = 0,
                notChanged = 0
            };

            var result = await this.ExecutePut(TokenRoleType.UserWithRole, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

                var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 12.ToString(),
                        Name = "float attribute",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Фильтры"},
                            new ElementFolderDto { FolderName = "По атрибутам"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "67", Value = null },
                            new ElementAttributeDto { AttributeId = "68", Value = null },
                            new ElementAttributeDto { AttributeId = "69", Value = -3.00f },
                            new ElementAttributeDto { AttributeId = "70", Value = null },
                            new ElementAttributeDto { AttributeId = "71", Value = null },
                            new ElementAttributeDto { AttributeId = "72", Value = testValue }
                        }
                    }
                };

                var elementsContent = await this.TryGetEntities(Method.POST, this.GetSearchUrl(forFiltersDimensionId), TokenRoleType.UserWithRole, null, this.Headers, filterContent);
                var updateVerificationResult = this.Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = updateVerificationResult.IsSuccess;
                result.Message += updateVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест обновления элементов с именованным фильтром "attribute", где атрибут типа Date
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact]
        public async Task Update_WithNamedAttributeFilter_ForDateAttribute()
        {
            this.Url = this.GetUrl(forFiltersDimensionId);

            var filter = new NamedFilter
            {
                value = "2001-12-01",
                type = NamedFilterType.attribute,
                name = "Дата согласования данных",
                condition = FilterCondition.equals
            };

            var testValue = "Тест обновления элементов с именованным фильтром attribute, где атрибут типа Date";

            var fields = new[]
            {
                new NamedField
                {
                    value = testValue,
                    type = NamedFieldType.attribute,
                    name = "Описание города"
                }
            };

            var body = new[]
            {
                new { filter, fields }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new UpdateResult
            {
                updated = 1,
                restricted = 0,
                notChanged = 0
            };

            var result = await this.ExecutePut(TokenRoleType.UserWithRole, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

                var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 13.ToString(),
                        Name = "date attribute",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Фильтры"},
                            new ElementFolderDto { FolderName = "По атрибутам"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "67", Value = null },
                            new ElementAttributeDto { AttributeId = "68", Value = null },
                            new ElementAttributeDto { AttributeId = "69", Value = null },
                            new ElementAttributeDto { AttributeId = "70", Value = "2001-12-01" },
                            new ElementAttributeDto { AttributeId = "71", Value = null },
                            new ElementAttributeDto { AttributeId = "72", Value = testValue }
                        }
                    }
                };

                var elementsContent = await this.TryGetEntities(Method.POST, this.GetSearchUrl(forFiltersDimensionId), TokenRoleType.UserWithRole, null, this.Headers, filterContent);
                var updateVerificationResult = this.Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = updateVerificationResult.IsSuccess;
                result.Message += updateVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест обновления элементов с именованным фильтром "attribute", где атрибут типа Link
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact]
        public async Task Update_WithNamedAttributeFilter_ForLinkAttribute()
        {
            this.Url = this.GetUrl(forFiltersDimensionId);

            var filter = new NamedFilter
            {
                value = 2,
                type = NamedFilterType.attribute,
                name = "Тип населенного пункта",
                condition = FilterCondition.equals
            };

            var testValue = "Тест обновления элементов с именованным фильтром attribute, где атрибут типа Link";

            var fields = new[]
            {
                new NamedField
                {
                    value = testValue,
                    type = NamedFieldType.attribute,
                    name = "Описание города"
                }
            };

            var body = new[]
            {
                new { filter, fields }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new UpdateResult
            {
                updated = 1,
                restricted = 0,
                notChanged = 0
            };

            var result = await this.ExecutePut(TokenRoleType.UserWithRole, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

                var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 14.ToString(),
                        Name = "link attribute",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Фильтры"},
                            new ElementFolderDto { FolderName = "По атрибутам"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "67", Value = null },
                            new ElementAttributeDto { AttributeId = "68", Value = null },
                            new ElementAttributeDto { AttributeId = "69", Value = null },
                            new ElementAttributeDto { AttributeId = "70", Value = null },
                            new ElementAttributeDto { AttributeId = "71", Value = 2 },
                            new ElementAttributeDto { AttributeId = "72", Value = testValue }
                        }
                    }
                };

                var elementsContent = await this.TryGetEntities(Method.POST, this.GetSearchUrl(forFiltersDimensionId), TokenRoleType.UserWithRole, null, this.Headers, filterContent);
                var updateVerificationResult = this.Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = updateVerificationResult.IsSuccess;
                result.Message += updateVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест обновления элементов с именованным фильтром "attribute", где атрибут типа Text
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact]
        public async Task Update_WithNamedAttributeFilter_ForTextAttribute()
        {
            this.Url = this.GetUrl(forFiltersDimensionId);

            var filter = new NamedFilter
            {
                value = "<p>text attribute</p>",
                type = NamedFilterType.attribute,
                name = "Описание города",
                condition = FilterCondition.equals
            };

            var testValue = "Тест обновления элементов с именованным фильтром attribute, где атрибут типа Text";

            var fields = new[]
            {
                new NamedField
                {
                    value = testValue,
                    type = NamedFieldType.attribute,
                    name = "Краткое наименование"
                }
            };

            var body = new[]
            {
                new { filter, fields }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new UpdateResult
            {
                updated = 1,
                restricted = 0,
                notChanged = 0
            };

            var result = await this.ExecutePut(TokenRoleType.UserWithRole, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

                var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 15.ToString(),
                        Name = "text attribute",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Фильтры"},
                            new ElementFolderDto { FolderName = "По атрибутам"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "67", Value = null },
                            new ElementAttributeDto { AttributeId = "68", Value = testValue },
                            new ElementAttributeDto { AttributeId = "69", Value = null },
                            new ElementAttributeDto { AttributeId = "70", Value = null },
                            new ElementAttributeDto { AttributeId = "71", Value = null },
                            new ElementAttributeDto { AttributeId = "72", Value = "<p>text attribute</p>" }
                        }
                    }
                };

                var elementsContent = await this.TryGetEntities(Method.POST, this.GetSearchUrl(forFiltersDimensionId), TokenRoleType.UserWithRole, null, this.Headers, filterContent);
                var updateVerificationResult = this.Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = updateVerificationResult.IsSuccess;
                result.Message += updateVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест обновления элементов с комплексным фильтром and
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact]
        public async Task Update_WithComplexAndFilter()
        {
            this.Url = this.GetUrl(forFiltersDimensionId);

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

            var testValue = "Тест обновления элементов комплексным фильтром AND";

            var fields = new[]
            {
                new NamedField
                {
                    value = testValue,
                    type = NamedFieldType.attribute,
                    name = "Описание города"
                }
            };

            var body = new[]
            {
                new { filter, fields }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new UpdateResult
            {
                updated = 1,
                restricted = 0,
                notChanged = 0
            };

            var result = await this.ExecutePut(TokenRoleType.UserWithRole, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

                var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 17.ToString(),
                        Name = "Элемент 2",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Фильтры"},
                            new ElementFolderDto { FolderName = "Комплексный фильтр"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "67", Value = null },
                            new ElementAttributeDto { AttributeId = "68", Value = null },
                            new ElementAttributeDto { AttributeId = "69", Value = null },
                            new ElementAttributeDto { AttributeId = "70", Value = "2018-05-22" },
                            new ElementAttributeDto { AttributeId = "71", Value = null },
                            new ElementAttributeDto { AttributeId = "72", Value = testValue}
                        }
                    }
                };

                var elementsContent = await this.TryGetEntities(Method.POST, this.GetSearchUrl(forFiltersDimensionId), TokenRoleType.UserWithRole, null, this.Headers, filterContent);
                var updateVerificationResult = this.Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = updateVerificationResult.IsSuccess;
                result.Message += updateVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест обновления элементов с косплексным фильтром Or
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact]
        public async Task Success_Update_WithComplexOrFilter()
        {
            this.Url = this.GetUrl(forFiltersDimensionId);

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

            var testValue = "Тест обновления элементов комплексным фильтром Or";

            var fields = new[]
            {
                new NamedField
                {
                    value = testValue,
                    type = NamedFieldType.attribute,
                    name = "Описание города"
                },
                new NamedField
                {
                    value = 3,
                    type = NamedFieldType.attribute,
                    name = "Тип населенного пункта"
                }
            };

            var body = new[]
            {
                new { filter, fields }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new UpdateResult
            {
                updated = 2,
                restricted = 0,
                notChanged = 0
            };

            var result = await this.ExecutePut(TokenRoleType.UserWithRole, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

                var expectedElementsInfo = new List<ElementDto>
                {
                    new ElementDto
                    {
                        Id = 18.ToString(),
                        Name = "Элемент 3",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Фильтры"},
                            new ElementFolderDto { FolderName = "Комплексный фильтр"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "67", Value = null },
                            new ElementAttributeDto { AttributeId = "68", Value = null },
                            new ElementAttributeDto { AttributeId = "69", Value = null },
                            new ElementAttributeDto { AttributeId = "70", Value = "2018-05-08" },
                            new ElementAttributeDto { AttributeId = "71", Value = 3 },
                            new ElementAttributeDto { AttributeId = "72", Value = testValue}
                        }
                    },
                    new ElementDto
                    {
                        Id = 19.ToString(),
                        Name = "Элемент 3",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Фильтры"},
                            new ElementFolderDto { FolderName = "Комплексный фильтр Or"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "67", Value = null },
                            new ElementAttributeDto { AttributeId = "68", Value = null },
                            new ElementAttributeDto { AttributeId = "69", Value = null },
                            new ElementAttributeDto { AttributeId = "70", Value = null },
                            new ElementAttributeDto { AttributeId = "71", Value = 3 },
                            new ElementAttributeDto { AttributeId = "72", Value = testValue}
                        }
                    }
                };

                var elementsContent = await this.TryGetEntities(Method.POST, this.GetSearchUrl(forFiltersDimensionId), TokenRoleType.UserWithRole, null, this.Headers, filterContent);
                var updateVerificationResult = this.Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = updateVerificationResult.IsSuccess;
                result.Message += updateVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }
    }
}
