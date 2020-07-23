using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;
using Visiology.DataCollect.Autotests.API.Infrastructure.Entities;
using Visiology.DataCollect.Autotests.API.Infrastructure.Entities.Results;
using Visiology.DataCollect.Autotests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Entities.RequestBody.Filters.Dimensions;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Impl;
using Visiology.DataCollect.Integration.Tests.Models.Dimensions.Elements;
using Xunit;

namespace Visiology.DataCollect.Autotests.API.Tests.Dimensions.Post.v2
{
    /// <summary>
    /// Класс тестирования метода создания элементов измерения
    /// </summary>
    [XApiVersion("2.0")]
    public class ElementsTests : BaseIntegrationPostTests<ElementsListDto, ElementDto>
    {
        /// <summary>
        /// Измерение для тестирования обновления элементов пользователем с ролью 
        /// В дампе - "Тестовое измерение для тестирования создания элементов v2.0"
        /// </summary>
        protected virtual string userDimensionId { get; set; } = "dim_Testovoe_izmerenie_dlya_01";

        /// <summary>
        /// Тип тестируемого метода
        /// </summary>
        public override Method Method { get; set; } = Method.POST;

        /// <summary>
        /// Url тестируемого метода
        /// </summary>
        public override string Url { get; set; }

        public ElementsTests(IisFixture iisFixture, TokenFixture tokenFixture, RestService restService)
            : base(iisFixture, tokenFixture, restService, new DimensionElementsVerifier())
        {
            Url = GetUrl(userDimensionId);
        }

        /// <summary>
        /// Тест создания элементов измерения при невалидном или несуществующем идентификаторе измерения
        /// </summary>
        /// <param name="dimensionId">Идентификатор измерения</param>
        /// <returns>Ожидаемый результат - отрицательный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TokenRoleType.UserAdmin, TestValues.ZeroId)]
        [InlineData(TokenRoleType.UserAdmin, TestValues.NonExistId)]
        [InlineData(TokenRoleType.UserAdmin, null)]
        public async Task Create_WithInvalidDimensionId(TokenRoleType token, string dimensionId)
        {
            Url = GetUrl(dimensionId);

            var body = new[]
            {
                new
                {
                   Name = "Тест создания элементов измерения при невалидном или несуществующем идентификаторе измерения",
                   Path =  new[] { "Папка первого уровня", "Папка второго уровня" }
                }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new CreateResult
            {
                Added = 0
            };

            var result = await ExecuteCreate(token, expectedResult, null, bodyContent);

            Assert.True(!result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест создания элементов измерения при отсутствии у пользователя роли на измерение
        /// </summary>
        /// <returns>Ожидаемый результат - отрицательный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TokenRoleType.UserWithFakeRole)]
        public async Task Create_WithoutPermissionToDimension(TokenRoleType token)
        {
            var body = new[]
            {
                new
                {
                   Name = "Тест создания элементов измерения при отсутствии у пользователя роли на измерение",
                   Path =  new[] { "Папка первого уровня", "Папка второго уровня" }
                }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new CreateResult
            {
                Added = 0
            };

            var result = await ExecuteCreate(token, expectedResult, null, bodyContent);

            Assert.True(!result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест создания элемента на конечном уровне
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TokenRoleType.UserWithRole)]
        public async Task Create_ForLastLevel(TokenRoleType token)
        {
            var body = new[]
            {
                new
                {
                   Id = 3,
                   Name = "Тест создания элемента на конечном уровне",
                   Path =  new[] { "Папка первого уровня", "Папка второго уровня" }
                }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new CreateResult
            {
                Added = 1
            };

            var result = await ExecuteCreate(token, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var filterContent = await Task.Run(() => JsonConvert.SerializeObject(new SimpleFilter
                {
                    value = 3,
                    type = SimpleFilterType.id,
                    condition = FilterCondition.equals
                }));

                var expectedElementsInfo = new List<ElementDto>()
                {
                    new ElementDto
                    {
                        Id = 3.ToString(),
                        Name = "Тест создания элемента на конечном уровне",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Папка первого уровня"},
                            new ElementFolderDto { FolderName = "Папка второго уровня"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    }
                };

                var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(userDimensionId), token, null, Headers, filterContent);
                var createVerificationResult = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = createVerificationResult.IsSuccess;
                result.Message += createVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест создания элемента на уровне
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TokenRoleType.UserWithRole)]
        public async Task Create_ForLevel(TokenRoleType token)
        {
            var body = new[]
            {
                new
                {
                   Id = 4,
                   Name = "Тест создания элемента на уровне",
                   Path =  new[] { "Папка первого уровня" }
                }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new CreateResult
            {
                Added = 1
            };

            var result = await ExecuteCreate(token, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var filterContent = await Task.Run(() => JsonConvert.SerializeObject(new SimpleFilter
                {
                    value = 4,
                    type = SimpleFilterType.id,
                    condition = FilterCondition.equals
                }));

                var expectedElementsInfo = new List<ElementDto>()
                {
                    new ElementDto
                    {
                        Id = 4.ToString(),
                        Name = "Тест создания элемента на уровне",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Папка первого уровня"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    }
                };

                var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(userDimensionId), token, null, Headers, filterContent);
                var createVerificationResult = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = createVerificationResult.IsSuccess;
                result.Message += createVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест создания элемента в корне
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TokenRoleType.UserWithRole)]
        public async Task Create_ForRoot(TokenRoleType token)
        {
            var body = new[]
            {
                new
                {
                   Id = 5,
                   Name = "Тест создания элемента в корне",
                   Path =  new List<string>().ToArray()
                }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new CreateResult
            {
                Added = 1
            };

            var result = await ExecuteCreate(token, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var filterContent = await Task.Run(() => JsonConvert.SerializeObject(new SimpleFilter
                {
                    value = 5,
                    type = SimpleFilterType.id,
                    condition = FilterCondition.equals
                }));

                var expectedElementsInfo = new List<ElementDto>()
                {
                    new ElementDto
                    {
                        Id = 5.ToString(),
                        Name = "Тест создания элемента в корне",
                        Path = new List<ElementFolderDto>(),
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    }
                };

                var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(userDimensionId), token, null, Headers, filterContent);
                var createVerificationResult = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = createVerificationResult.IsSuccess;
                result.Message += createVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест восстановления элементов
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>              
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TokenRoleType.UserWithRole)]
        public async Task ReCreateElement(TokenRoleType token)
        {
            // Элемент с идентификатором 1 данного измерения раннее был создан и удален в дампе
            var body = new[]
           {
                new
                {
                   Id = 2,
                   Name = "Элемент для проверки создания по удаленному идентификатору",
                   Path =  new List<string>().ToArray()
                }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new CreateResult
            {
                Added = 1
            };

            var result = await ExecuteCreate(token, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var filterContent = await Task.Run(() => JsonConvert.SerializeObject(new SimpleFilter
                {
                    value = 2,
                    type = SimpleFilterType.id,
                    condition = FilterCondition.equals
                }));

                var expectedElementsInfo = new List<ElementDto>()
                {
                    new ElementDto
                    {
                        Id = 2.ToString(),
                        Name = "Элемент для проверки создания по удаленному идентификатору",
                        Path = new List<ElementFolderDto>(),
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    }
                };

                var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(userDimensionId), token, null, Headers, filterContent);
                var createVerificationResult = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = createVerificationResult.IsSuccess;
                result.Message += createVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест создания элементов по идентификатору уже существующего элемента
        /// </summary>
        /// <returns>Ожидаемый результат - отрицательный</returns>              
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TokenRoleType.UserWithRole)]
        public async Task Create_ForExistId(TokenRoleType token)
        {
            // Элемент с идентификатором 2 данного измерения раннее был создан в дампе
            var body = new[]
           {
                new
                {
                   Id = 1,
                   Name = "Элемент для проверки создания по существующему идентификатору 1",
                   Path =  new List<string>().ToArray()
                }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new CreateResult
            {
                Added = 0
            };

            var result = await ExecuteCreate(token, expectedResult, null, bodyContent);

            Assert.True(!result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест создания элемента по имени уже существующего элемента в той же папке
        /// </summary>
        /// <returns>Ожидаемый результат - отрицательный</returns>              
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TokenRoleType.UserWithRole)]
        public async Task Create_ForExistName_ForSamePath(TokenRoleType token)
        {
            // Элемент с именем "Элемент для проверки создания по существующему идентификатору" данного измерения раннее был создан в дампе
            var body = new[]
           {
                new
                {
                   Name = "Элемент для проверки создания по существующему наименованию или идентификатору",
                   Path =  new List<string>().ToArray()
                }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new CreateResult
            {
                Added = 0
            };

            var result = await ExecuteCreate(token, expectedResult, null, bodyContent);

            Assert.True(!result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест создания элемента по имени уже существующего элемента по другому пути
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>              
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TokenRoleType.UserWithRole)]
        public async Task Create_ForExistName_ForAnotherPath(TokenRoleType token)
        {
            // Элемент с идентификатором 1 данного измерения раннее был создан в дампе
            var body = new[]
           {
                new
                {
                   Id = 6,
                   Name = "Элемент для проверки создания по существующему наименованию или идентификатору",
                   Path =  new[] { "Папка первого уровня", "Папка второго уровня" }
                }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new CreateResult
            {
                Added = 1
            };

            var result = await ExecuteCreate(token, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var filterContent = await Task.Run(() => JsonConvert.SerializeObject(new SimpleFilter
                {
                    value = 6,
                    type = SimpleFilterType.id,
                    condition = FilterCondition.equals
                }));

                var expectedElementsInfo = new List<ElementDto>()
                {
                    new ElementDto
                    {
                        Id = 6.ToString(),
                        Name = "Элемент для проверки создания по существующему наименованию или идентификатору",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Папка первого уровня"},
                            new ElementFolderDto { FolderName = "Папка второго уровня"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    }
                };

                var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(userDimensionId), token, null, Headers, filterContent);
                var createVerificationResult = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = createVerificationResult.IsSuccess;
                result.Message += createVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест создания элементов без указания идентификатора
        /// Тест проверки сдвига идентификатора 
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TokenRoleType.UserWithRole)]
        public async Task Create_WithoutId(TokenRoleType token)
        {
            var result = new ContentVerificationResult
            {
                IsSuccess = false
            };

            var supportingElementResult = await ExecuteCreate(
                token,
                new CreateResult { Added = 1 },
                null,
                await Task.Run(() => JsonConvert.SerializeObject(new[] {
                    new
                    {
                       Id = 100,
                       Name = "Вспомогательный элемент для проверки сдвига идентификатора",
                       Path =  new List<string>().ToArray()
                    }
                })));

            if (supportingElementResult.IsSuccess)
            {
                var body = new[]
                {
                     new
                     {
                        Name = "Тест создания элементов без указания идентификатора",
                        Path =  new List<string>().ToArray()
                     }
                };

                var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

                var expectedResult = new CreateResult
                {
                    Added = 1
                };

                result = await ExecuteCreate(token, expectedResult, null, bodyContent);

                if (result.IsSuccess)
                {
                    var filterContent = await Task.Run(() => JsonConvert.SerializeObject(new SimpleFilter
                    {
                        value = 101,
                        type = SimpleFilterType.id,
                        condition = FilterCondition.equals
                    }));

                    var expectedElementsInfo = new List<ElementDto>()
                {
                    new ElementDto
                    {
                        Id = 101.ToString(),
                        Name = "Тест создания элементов без указания идентификатора",
                        Path = new List<ElementFolderDto>(),
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    }
                };

                    var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(userDimensionId), token, null, Headers, filterContent);
                    var createVerificationResult = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                    result.IsSuccess = createVerificationResult.IsSuccess;
                    result.Message += createVerificationResult.Message;
                }
            }
            else
            {
                result.Message = "Ошибка создания вспомогательного элемент для проверки сдвига идентификатора";
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест создания элемента по FolderId
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>              
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TokenRoleType.UserWithRole)]
        public async Task Create_ForFolderId(TokenRoleType token)
        {
            var body = new[]
           {
                new
                {
                   Id = 7,
                   Name = "Элемент для проверки создания по FolderId",
                   FolderId =  2342
                }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new CreateResult
            {
                Added = 1
            };

            var result = await ExecuteCreate(token, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var filterContent = await Task.Run(() => JsonConvert.SerializeObject(new SimpleFilter
                {
                    value = 7,
                    type = SimpleFilterType.id,
                    condition = FilterCondition.equals
                }));

                var expectedElementsInfo = new List<ElementDto>()
                {
                    new ElementDto
                    {
                        Id = 7.ToString(),
                        Name = "Элемент для проверки создания по FolderId",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Папка первого уровня"}
                        },
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = null },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = null }
                        }
                    }
                };

                var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(userDimensionId), token, null, Headers, filterContent);
                var createVerificationResult = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = createVerificationResult.IsSuccess;
                result.Message += createVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест создания элемента с атрибутами
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>              
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TokenRoleType.UserWithRole)]
        public async Task Create_WithAttributes(TokenRoleType token)
        {
            var body = new[]
           {
                new
                {
                   Id = 8,
                   Name = "Элемент для проверки создания c атрибутами",
                   Path =  new List<string>().ToArray(),
                   Attributes = new []
                   {
                       new { AttributeId = "attr_CHislennost_naseleniya", Value = "1" },
                       new { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Элемент для проверки создания c атрибутами" },
                       new { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = "1.00" },
                       new { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-07-11" },
                       new { AttributeId = "attr_Tip_naselennogo_punkta", Value = "1" },
                       new { AttributeId = "attr_Opisanie_goroda", Value = "Элемент для проверки создания c атрибутами" }
                   }
                }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new CreateResult
            {
                Added = 1
            };

            var result = await ExecuteCreate(token, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var filterContent = await Task.Run(() => JsonConvert.SerializeObject(new SimpleFilter
                {
                    value = 8,
                    type = SimpleFilterType.id,
                    condition = FilterCondition.equals
                }));

                var expectedElementsInfo = new List<ElementDto>()
                {
                    new ElementDto
                    {
                        Id = 8.ToString(),
                        Name = "Элемент для проверки создания c атрибутами",
                        Path = new List<ElementFolderDto>(),
                        Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_CHislennost_naseleniya", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Kratkoe_naimenovanie", Value = "Элемент для проверки создания c атрибутами" },
                            new ElementAttributeDto { AttributeId = "attr_Ploschad_zhiloi_zoni", Value = 1.00 },
                            new ElementAttributeDto { AttributeId = "attr_Data_soglasovaniya_dannih", Value = "2018-07-11" },
                            new ElementAttributeDto { AttributeId = "attr_Tip_naselennogo_punkta", Value = 1 },
                            new ElementAttributeDto { AttributeId = "attr_Opisanie_goroda", Value = "Элемент для проверки создания c атрибутами" }
                        }
                    }
                };

                var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(userDimensionId), token, null, Headers, filterContent);
                var createVerificationResult = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = createVerificationResult.IsSuccess;
                result.Message += createVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест создания элемента с созданием иерархии каталогов
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>              
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TokenRoleType.UserWithRole)]
        public async Task Create_WithCreationOfFolders(TokenRoleType token)
        {
            var body = new[]
           {
                new
                {
                   Id = 9,
                   Name = "Элемент для проверки создания с созданием иерархии каталогов",
                   Path = new [] { "Папка первого уровня 1",  "Папка второго уровня"}
                }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new CreateResult
            {
                Added = 1
            };

            var result = await ExecuteCreate(token, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var filterContent = await Task.Run(() => JsonConvert.SerializeObject(new SimpleFilter
                {
                    value = 9,
                    type = SimpleFilterType.id,
                    condition = FilterCondition.equals
                }));

                var expectedElementsInfo = new List<ElementDto>()
                {
                    new ElementDto
                    {
                        Id = 9.ToString(),
                        Name = "Элемент для проверки создания с созданием иерархии каталогов",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Папка первого уровня 1"},
                            new ElementFolderDto { FolderName = "Папка второго уровня"}
                        }
                    }
                };

                var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(userDimensionId), token, null, Headers, filterContent);
                var createVerificationResult = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = createVerificationResult.IsSuccess;
                result.Message += createVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест создания элемента с частичным созданием иерархии каталогов
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>              
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TokenRoleType.UserWithRole)]
        public async Task Create_WithPartialCreationOfFolders(TokenRoleType token)
        {
            var body = new[]
           {
                new
                {
                   Id = 10,
                   Name = "Элемент для проверки создания с частичным созданием иерархии каталогов",
                   Path = new [] { "Папка первого уровня",  "Папка второго уровня 2"}
                }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new CreateResult
            {
                Added = 1
            };

            var result = await ExecuteCreate(token, expectedResult, null, bodyContent);

            if (result.IsSuccess)
            {
                var filterContent = await Task.Run(() => JsonConvert.SerializeObject(new SimpleFilter
                {
                    value = 10,
                    type = SimpleFilterType.id,
                    condition = FilterCondition.equals
                }));

                var expectedElementsInfo = new List<ElementDto>()
                {
                    new ElementDto
                    {
                        Id = 10.ToString(),
                        Name = "Элемент для проверки создания с частичным созданием иерархии каталогов",
                        Path = new List<ElementFolderDto>
                        {
                            new ElementFolderDto { FolderName = "Папка первого уровня"},
                            new ElementFolderDto { FolderName = "Папка второго уровня 2"}
                        }
                    }
                };

                var elementsContent = await TryGetEntities(Method.POST, GetSearchUrl(userDimensionId), token, null, Headers, filterContent);
                var createVerificationResult = Verifier.Verify(elementsContent.Content?.Entities, expectedElementsInfo);

                result.IsSuccess = createVerificationResult.IsSuccess;
                result.Message += createVerificationResult.Message;
            }

            Assert.True(result.IsSuccess, result.Message);
        }
    }
}
