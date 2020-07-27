using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;
using Visiology.DataCollect.Autotests.API.Infrastructure.Entities;
using Visiology.DataCollect.Autotests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Entities.RequestBody.Filters.Dimensions;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Impl;
using Visiology.DataCollect.Integration.Tests.Models.Dimensions.Elements;
using Xunit;

namespace Visiology.DataCollect.Autotests.API.Tests.Dimensions.Delete.v1
{
    /// <summary>
    /// Класс тестирования метода обновления элементов измерения
    /// </summary>
    [XApiVersion("1.0")]
    public class ElementsTests : BaseIntegrationDeleteTests<ElementsListDto, ElementDto>
    {
        /// <summary>
        /// Измерение для тестирования удаления элементов пользователем с ролью 
        /// В дампе - "Тестовое измерение для тестирования удаления элементов"
        /// </summary>
        protected virtual string userDimensionId { get; set; } = "26";

        /// <summary>
        /// Измерение для тестирования удаления всех элементов 
        /// В дампе "Тестовое измерение для тестирования удаления всех элементов"
        /// </summary>
        protected virtual string dimensionForDeleteAllId { get; set; } = "27";

        /// <summary>
        /// Тип тестируемого метода
        /// </summary>
        public override Method Method { get; set; } = Method.DELETE;

        /// <summary>
        /// Url тестируемого метода
        /// </summary>
        public override string Url { get; set; }

        public ElementsTests(TokenFixture tokenFixture, RestService restService)
            : base(tokenFixture, restService, new DimensionElementsVerifier())
        {
            Url = GetUrl(userDimensionId);
        }

        /// <summary>
        /// Тест удаления элементов измерения при невалидном или несуществующем идентификаторе измерения
        /// </summary>
        /// <param name="dimensionId">Идентификатор измерения</param>
        /// <returns>Ожидаемый результат - отрицательный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TestValues.ZeroId)]
        [InlineData(TestValues.NonExistId)]
        [InlineData(null)]
        public async Task DeleteAll_WithInvalidDimensionId(string dimensionId)
        {
            Url = GetUrl(dimensionId);

            var expectedResult = new DeleteResult
            {
                deleted = 0
            };

            var result = await ExecuteDelete(TokenRoleType.UserWithRole, expectedResult);

            Assert.True(!result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест удаления элементов измерения при отсутствии у пользователя роли на измерение
        /// </summary>
        /// <returns>Ожидаемый результат - отрицательный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TokenRoleType.UserWithFakeRole)]
        public async Task DeleteAll_WithoutPermissionToDimension(TokenRoleType token)
        {
            var expectedResult = new DeleteResult
            {
                deleted = 0,
            };

            var result = await ExecuteDelete(token, expectedResult);

            Assert.True(!result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест удаления всех элементов без тела 
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TokenRoleType.UserWithRole)]
        public async Task DeleteAll_WithoutBody(TokenRoleType token)
        {
            Url = GetUrl(dimensionForDeleteAllId);

            var expectedResult = new DeleteResult
            {
                deleted = 16
            };

            var result = await ExecuteDelete(token, expectedResult);

            if (result.IsSuccess)
            {
                var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(dimensionForDeleteAllId), token, null, Headers);

                var deleteVerificationResult = Verifier.Verify(elementsContent.Content?.Entities, new List<ElementDto>());

                result.IsSuccess = deleteVerificationResult.IsSuccess;
                result.Message += deleteVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест удаления элемента на конечном уровне
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TokenRoleType.UserWithRole)]
        public async Task Delete_ForLastLevel(TokenRoleType token)
        {
            var filter = new SimpleFilter
            {
                value = "Казань (Обновление на конечном уровне)",
                type = SimpleFilterType.name,
                condition = FilterCondition.equals
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var deletedResult = new DeleteResult
            {
                deleted = 1
            };

            var result = await ExecuteDelete(token, deletedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var expectedElementsInfo = new List<ElementDto>();
                var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(userDimensionId), token, null, Headers, bodyContent);
                var deleteVerificationResult = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = deleteVerificationResult.IsSuccess;
                result.Message += deleteVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест удаления элемента на уровне
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TokenRoleType.UserWithRole)]
        public async Task Delete_ForLevel(TokenRoleType token)
        {
            var filter = new SimpleFilter
            {
                value = "Казань (Обновление на уровне)",
                type = SimpleFilterType.name,
                condition = FilterCondition.equals
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedResult = new DeleteResult
            {
                deleted = 1
            };

            var result = await ExecuteDelete(token, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var expectedElementsInfo = new List<ElementDto>();
                var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(userDimensionId), token, null, Headers, bodyContent);

                var deleteVerificationResult = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = deleteVerificationResult.IsSuccess;
                result.Message += deleteVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест удаления элемента в корне
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TokenRoleType.UserWithRole)]
        public async Task Delete_ForRoot(TokenRoleType token)
        {
            var filter = new SimpleFilter
            {
                value = "Казань (Обновление в корне)",
                type = SimpleFilterType.name,
                condition = FilterCondition.equals
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedResult = new DeleteResult
            {
                deleted = 1
            };

            var result = await ExecuteDelete(token, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var expectedElementsInfo = new List<ElementDto>();
                var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(userDimensionId), token, null, Headers, bodyContent);
                var deleteVerificationResult = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = deleteVerificationResult.IsSuccess;
                result.Message += deleteVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест удаления элементов без изменений
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>              
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TokenRoleType.UserWithRole)]
        public async Task Delete_WithoutDelete(TokenRoleType token)
        {
            Url = GetUrl(userDimensionId);

            var filter = new SimpleFilter
            {
                value = "Фильтр на несуществующий элемент",
                type = SimpleFilterType.name,
                condition = FilterCondition.equals
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedResult = new DeleteResult
            {
                deleted = 0
            };

            var result = await ExecuteDelete(token, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var expectedElementsInfo = new List<ElementDto>();
                var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(userDimensionId), token, null, Headers, bodyContent);
                var deleteVerificationResult = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = deleteVerificationResult.IsSuccess;
                result.Message += deleteVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест удаления элементов, недоступных для удаления
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>               
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TokenRoleType.UserAdmin)]
        public async Task Delete_ForUnchangedElement(TokenRoleType token)
        {
            Url = GetUrl(userDimensionId);

            var filter = new SimpleFilter
            {
                value = "Элемент, недоступный для удаления",
                type = SimpleFilterType.name,
                condition = FilterCondition.equals
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedResult = new DeleteResult
            {
                deleted = 0
            };

            var result = await ExecuteDelete(token, expectedResult, null, bodyContent);

            Assert.True(!result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест удаления элементов по простому фильтру типа "id"
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TokenRoleType.UserWithRole)]
        public async Task Delete_WithSimpleIdFilter(TokenRoleType token)
        {
            Url = GetUrl(userDimensionId);

            var filter = new SimpleFilter
            {
                value = 4,
                type = SimpleFilterType.id,
                condition = FilterCondition.equals
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedResult = new DeleteResult
            {
                deleted = 1
            };

            var result = await ExecuteDelete(token, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var expectedElementsInfo = new List<ElementDto>();
                var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(userDimensionId), token, null, Headers, bodyContent);
                var deleteVerificationResult = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);


                result.IsSuccess = deleteVerificationResult.IsSuccess;
                result.Message += deleteVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест удаления элементов по простому фильтру типа "name"
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TokenRoleType.UserWithRole)]
        public async Task Delete_WithSimpleNameFilter(TokenRoleType token)
        {
            var filter = new SimpleFilter
            {
                value = "Мытищи",
                type = SimpleFilterType.name,
                condition = FilterCondition.equals
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedResult = new DeleteResult
            {
                deleted = 1
            };

            var result = await ExecuteDelete(token, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var expectedElementsInfo = new List<ElementDto>();
                var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(userDimensionId), token, null, Headers, bodyContent);
                var deleteVerificationResult = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);


                result.IsSuccess = deleteVerificationResult.IsSuccess;
                result.Message += deleteVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест удаления элементов по именованному фильтру "level"
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TokenRoleType.UserWithRole)]
        public async Task Delete_WithNamedLevelFilter(TokenRoleType token)
        {
            var filter = new NamedFilter
            {
                value = "Татарстан",
                type = NamedFilterType.level,
                name = "Регион",
                condition = FilterCondition.equals
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedResult = new DeleteResult
            {
                deleted = 3
            };

            var result = await ExecuteDelete(token, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var expectedElementsInfo = new List<ElementDto>();
                var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(userDimensionId), token, null, Headers, bodyContent);
                var deleteVerificationResult = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);


                result.IsSuccess = deleteVerificationResult.IsSuccess;
                result.Message += deleteVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест удаления элементов по именованному фильтру "level" по конечному уровню
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TokenRoleType.UserWithRole)]
        public async Task Delete_WithNamedLevelFilterForLastLevel(TokenRoleType token)
        {
            var filter = new NamedFilter
            {
                value = "Сигнахи",
                type = NamedFilterType.level,
                name = "Город",
                condition = FilterCondition.equals
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedResult = new DeleteResult
            {
                deleted = 1
            };

            var result = await ExecuteDelete(token, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var expectedElementsInfo = new List<ElementDto>();
                var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(userDimensionId), token, null, Headers, bodyContent);
                var deleteVerificationResult = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = deleteVerificationResult.IsSuccess;
                result.Message += deleteVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест удаления элементов по именованному фильтру "attribute", где атрибут типа int
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TokenRoleType.UserWithRole)]
        public async Task Delete_WithNamedAttributeFilter_ForIntAttribute(TokenRoleType token)
        {
            var filter = new NamedFilter
            {
                value = "-1",
                type = NamedFilterType.attribute,
                name = "Численность населения",
                condition = FilterCondition.equals
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedResult = new DeleteResult
            {
                deleted = 1
            };

            var result = await ExecuteDelete(token, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var expectedElementsInfo = new List<ElementDto>();
                var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(userDimensionId), token, null, Headers, bodyContent);
                var deleteVerificationResult = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = deleteVerificationResult.IsSuccess;
                result.Message += deleteVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест удаления элементов по именованному фильтру "attribute", где атрибут типа string
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TokenRoleType.UserWithRole)]
        public async Task Delete_WithNamedAttributeFilter_ForStringAttribute(TokenRoleType token)
        {
            var filter = new NamedFilter
            {
                value = "-2",
                type = NamedFilterType.attribute,
                name = "Краткое наименование",
                condition = FilterCondition.equals
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedResult = new DeleteResult
            {
                deleted = 1
            };

            var result = await ExecuteDelete(token, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var expectedElementsInfo = new List<ElementDto>();
                var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(userDimensionId), token, null, Headers, bodyContent);
                var deleteVerificationResult = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = deleteVerificationResult.IsSuccess;
                result.Message += deleteVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест удаления элементов по именованному фильтру "attribute", где атрибут типа Float
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TokenRoleType.UserWithRole)]
        public async Task Delete_WithNamedAttributeFilter_ForFloatAttribute(TokenRoleType token)
        {
            var filter = new NamedFilter
            {
                value = "-3",
                type = NamedFilterType.attribute,
                name = "Площадь жилой зоны",
                condition = FilterCondition.equals
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedResult = new DeleteResult
            {
                deleted = 1
            };

            var result = await ExecuteDelete(token, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var expectedElementsInfo = new List<ElementDto>();
                var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(userDimensionId), token, null, Headers, bodyContent);
                var deleteVerificationResult = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = deleteVerificationResult.IsSuccess;
                result.Message += deleteVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест удаления элементов по именованному фильтру "attribute", где атрибут типа Date
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TokenRoleType.UserWithRole)]
        public async Task Delete_WithNamedAttributeFilter_ForDateAttribute(TokenRoleType token)
        {
            var filter = new NamedFilter
            {
                value = "2001-12-01",
                type = NamedFilterType.attribute,
                name = "Дата согласования данных",
                condition = FilterCondition.equals
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedResult = new DeleteResult
            {
                deleted = 1
            };

            var result = await ExecuteDelete(token, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var expectedElementsInfo = new List<ElementDto>();
                var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(userDimensionId), token, null, Headers, bodyContent);
                var deleteVerificationResult = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = deleteVerificationResult.IsSuccess;
                result.Message += deleteVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест удаления элементов по именованному фильтру "attribute", где атрибут типа Link
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TokenRoleType.UserWithRole)]
        public async Task Delete_WithNamedAttributeFilter_ForLinkAttribute(TokenRoleType token)
        {
            var filter = new NamedFilter
            {
                value = 2,
                type = NamedFilterType.attribute,
                name = "Тип населенного пункта",
                condition = FilterCondition.equals
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedResult = new DeleteResult
            {
                deleted = 1
            };

            var result = await ExecuteDelete(token, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var expectedElementsInfo = new List<ElementDto>();
                var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(userDimensionId), token, null, Headers, bodyContent);
                var deleteVerificationResult = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = deleteVerificationResult.IsSuccess;
                result.Message += deleteVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест удаления элементов по именованному фильтру "attribute", где атрибут типа Text
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TokenRoleType.UserWithRole)]
        public async Task Delete_WithNamedAttributeFilter_ForTextAttribute(TokenRoleType token)
        {
            var filter = new NamedFilter
            {
                value = "<p>text attribute</p>",
                type = NamedFilterType.attribute,
                name = "Описание города",
                condition = FilterCondition.equals
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedResult = new DeleteResult
            {
                deleted = 1
            };

            var result = await ExecuteDelete(token, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var expectedElementsInfo = new List<ElementDto>();
                var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(userDimensionId), token, null, Headers, bodyContent);
                var deleteVerificationResult = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = deleteVerificationResult.IsSuccess;
                result.Message += deleteVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест удаления элементов по комплексному фильтру and
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TokenRoleType.UserWithRole)]
        public async Task Delete_WithComplexAndFilter(TokenRoleType token)
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

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedResult = new DeleteResult
            {
                deleted = 1
            };

            var result = await ExecuteDelete(token, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var expectedElementsInfo = new List<ElementDto>();
                var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(userDimensionId), token, null, Headers, bodyContent);
                var deleteVerificationResult = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = deleteVerificationResult.IsSuccess;
                result.Message += deleteVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест удаления элементов по комплексному фильтру Or
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TokenRoleType.UserWithRole)]
        public async Task Delete_WithComplexOrFilter(TokenRoleType token)
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

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

            var expectedResult = new DeleteResult
            {
                deleted = 2
            };

            var result = await ExecuteDelete(token, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var expectedElementsInfo = new List<ElementDto>();
                var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(userDimensionId), token, null, Headers, bodyContent);
                var deleteVerificationResult = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = deleteVerificationResult.IsSuccess;
                result.Message += deleteVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }
    }
}
