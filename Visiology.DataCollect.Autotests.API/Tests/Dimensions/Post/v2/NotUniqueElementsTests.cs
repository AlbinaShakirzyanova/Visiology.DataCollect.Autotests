using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;
using Visiology.DataCollect.Autotests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Impl;
using Visiology.DataCollect.Integration.Tests.Models.Dimensions.Elements;
using Xunit;

namespace Visiology.DataCollect.Autotests.API.Tests.Dimensions.Post.v2
{
    /// <summary>
    /// Класс тестирования элементов измерения с неуникальными наименованиями
    /// </summary>
    [XApiVersion("2.0")]
    public class NotUniqueElementsTests : BaseIntegrationPostTests<ElementsListDto, ElementDto>
    {
        /// <summary>
        /// Измерение для тестирования элементов измерения с неуникальными наименованиями
        /// В дампе - "Тестирование неуникальных наименований"
        /// </summary>
        protected virtual string userDimensionId { get; set; } = "dim_Testirovanie_neunikalnih_n";

        /// <summary>
        /// Тип тестируемого метода
        /// </summary>
        public override Method Method { get; set; } = Method.POST;

        /// <summary>
        /// Url тестируемого метода
        /// </summary>
        public override string Url { get; set; }

        /// <summary>
        /// Тип токена для тестирования
        /// </summary>
        private TokenRoleType tokenRoleType = TokenRoleType.UserForNotUniqueElements;

        public NotUniqueElementsTests(IisFixture iisFixture, TokenFixture tokenFixture, RestService restService)
            : base(iisFixture, tokenFixture, restService, new DimensionElementsVerifier())
        {
            Url = GetUrl(userDimensionId);
        }

        /// <summary>
        /// Тестирование создания неуникальных наименований  без заполнения атрибутов
        /// </summary>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Create_WithoutAttributeValues()
        {
            var body = new[]
            {
                new
                {
                   Name = "Element 1",
                   Path =  new string[0]
                },
                new
                {
                   Name = "Element 1",
                   Path =  new string[0]
                }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new CreateResult
            {
                Added = 0
            };

            var result = await ExecuteCreate(tokenRoleType, expectedResult, null, bodyContent);

            var errorMessage = "Список элементов содержит повторяющиеся значения. Данные не сохранены.";

            var isSuccess = !result.IsSuccess && result.Message.Contains(errorMessage);

            Assert.True(isSuccess, result.Message);
        }

        /// <summary>
        /// Тестирование создания  с заполнением атрибутов. Проверка кейса с одинаковыми значениями атрибутов
        /// </summary>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Create_WithEqualAttributeValues()
        {
            // Создание первого элемента
            var body = new[]
            {
                new
                {
                   Name = "Element 2",
                   Path =  new string[0],
                   Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_attr", Value = "string" }
                        }
                },
                new
                {
                   Name = "Element 2",
                   Path =  new string[0],
                   Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_attr", Value = "string" }
                        }
                }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new CreateResult
            {
                Added = 0
            };

            var result = await ExecuteCreate(tokenRoleType, expectedResult, null, bodyContent);

            var errorMessage = "Список элементов содержит повторяющиеся значения. Данные не сохранены.";

            var isSuccess = !result.IsSuccess && result.Message.Contains(errorMessage);

            Assert.True(isSuccess, result.Message);
        }

        /// <summary>
        /// Проверка атрибута типа string
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Create_WithStringAttributeValues()
        {
            // Создание первого элемента
            var body = new[]
            {
                new
                {
                   Name = "Element 3",
                   Path =  new string[0],
                   Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_attr", Value = "string 1" }
                        }
                },
                new
                {
                   Name = "Element 3",
                   Path =  new string[0],
                   Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_attr", Value = "string 2" }
                        }
                }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new CreateResult
            {
                Added = 2
            };

            var result = await ExecuteCreate(tokenRoleType, expectedResult, null, bodyContent);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Проверка кейса с пустым значениям по  атрибуту для одного из элементов
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Create_WithEmptyAttributeValue()
        {
            // Создание первого элемента
            var body = new[]
            {
                new
                {
                   Name = "Element 4",
                   Path =  new string[0],
                   Attributes = new List<ElementAttributeDto>()
                },
                new
                {
                   Name = "Element 4",
                   Path =  new string[0],
                   Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_attr", Value = "string" }
                        }
                }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new CreateResult
            {
                Added = 2
            };

            var result = await ExecuteCreate(tokenRoleType, expectedResult, null, bodyContent);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Проверка атрибута типа int
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Create_WithIntAttributeValues()
        {
            // Создание первого элемента
            var body = new[]
            {
                new
                {
                   Name = "Element int",
                   Path =  new string[0],
                   Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_int", Value = "1" }
                        }
                },
                new
                {
                   Name = "Element int",
                   Path =  new string[0],
                   Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_int", Value = "2" }
                        }
                }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new CreateResult
            {
                Added = 2
            };

            var result = await ExecuteCreate(tokenRoleType, expectedResult, null, bodyContent);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Проверка атрибута типа float
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Create_WithFloatAttributeValues()
        {
            // Создание первого элемента
            var body = new[]
            {
                new
                {
                   Name = "Element float",
                   Path =  new string[0],
                   Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_float", Value = "1" }
                        }
                },
                new
                {
                   Name = "Element float",
                   Path =  new string[0],
                   Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_float", Value = "2" }
                        }
                }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new CreateResult
            {
                Added = 2
            };

            var result = await ExecuteCreate(tokenRoleType, expectedResult, null, bodyContent);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Проверка атрибута типа link
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Create_WithLinkAttributeValues()
        {
            // Создание первого элемента
            var body = new[]
            {
                new
                {
                   Name = "Element link",
                   Path =  new string[0],
                   Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_link", Value = "7" }
                        }
                },
                new
                {
                   Name = "Element link",
                   Path =  new string[0],
                   Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_link", Value = "2" }
                        }
                }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new CreateResult
            {
                Added = 2
            };

            var result = await ExecuteCreate(tokenRoleType, expectedResult, null, bodyContent);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Проверка атрибута типа date
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Create_WithDateAttributeValues()
        {
            // Создание первого элемента
            var body = new[]
            {
                new
                {
                   Name = "Element date",
                   Path =  new string[0],
                   Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_date", Value = "2020-01-01" }
                        }
                },
                new
                {
                   Name = "Element date",
                   Path =  new string[0],
                   Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_date", Value = "2020-01-25" }
                        }
                }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new CreateResult
            {
                Added = 2
            };

            var result = await ExecuteCreate(tokenRoleType, expectedResult, null, bodyContent);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Проверка сравнения по нескольким атрибутам
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Create_WithFewAttributes()
        {
            // Создание первого элемента
            var body = new[]
            {
                new
                {
                   Name = "Проверка сравнения по нескольким атрибутам",
                   Path =  new string[0],
                   Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_int", Value = "1" },
                            new ElementAttributeDto { AttributeId = "attr_float", Value = "1" }
                        }
                },
                new
                {
                   Name = "Проверка сравнения по нескольким атрибутам",
                   Path =  new string[0],
                   Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_int", Value = "1" },
                            new ElementAttributeDto { AttributeId = "attr_float", Value = "2" }
                        }
                }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new CreateResult
            {
                Added = 2
            };

            var result = await ExecuteCreate(tokenRoleType, expectedResult, null, bodyContent);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Проверка отображения строки с пробелом
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Create_WithEmptyAndSpacebarAttributeValues()
        {
            // Создание первого элемента
            var body = new[]
            {
                new
                {
                   Name = "String Empty",
                   Path =  new string[0],
                   Attributes = new List<ElementAttributeDto>()
                },
                new
                {
                   Name = "String Empty",
                   Path =  new string[0],
                   Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_attr", Value = " " }
                        }
                }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new CreateResult
            {
                Added = 2
            };

            var result = await ExecuteCreate(tokenRoleType, expectedResult, null, bodyContent);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Проверка атрибута типа text
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Create_WithTextAttributeValues()
        {
            // Создание первого элемента
            var body = new[]
            {
                new
                {
                   Name = "Element text",
                   Path =  new string[0],
                   Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_text", Value = "text" }
                        }
                },
                new
                {
                   Name = "Element text",
                   Path =  new string[0],
                   Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_text", Value = "text 1" }
                        }
                }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new CreateResult
            {
                Added = 2
            };

            var result = await ExecuteCreate(tokenRoleType, expectedResult, null, bodyContent);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Проверка атрибута типа text
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Create_WithNotUqiqueLinkAttributeValues()
        {
            // Создание первого элемента
            var body = new[]
            {
                new
                {
                   Name = "Not unique link",
                   Path =  new string[0],
                   Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_new_link", Value = "1" }
                        }
                },
                new
                {
                   Name = "Not unique link",
                   Path =  new string[0],
                   Attributes = new List<ElementAttributeDto>
                        {
                            new ElementAttributeDto { AttributeId = "attr_new_link", Value = "2" }
                        }
                }
            };

            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new CreateResult
            {
                Added = 2
            };

            var result = await ExecuteCreate(tokenRoleType, expectedResult, null, bodyContent);

            Assert.True(result.IsSuccess, result.Message);
        }
    }
}



