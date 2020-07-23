using Newtonsoft.Json;
using RestSharp;
using System.Threading.Tasks;
using Visiology.DataCollect.Autotests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Entities.Fields;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Entities.RequestBody.Fields;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Impl;
using Visiology.DataCollect.Integration.Tests.Models.Dimensions.Elements;
using Xunit;

namespace Visiology.DataCollect.Autotests.API.Tests.Dimensions.Put.v2
{
    /// <summary>
    /// Класс тестирования метода обновления элементов с неуникальными наименованиями
    /// </summary>
    [XApiVersion("2.0")]
    public class NotUniqueElementsTests : BaseIntegrationPutTests<ElementsListDto, ElementDto>
    {
        /// <summary>
        /// Измерение для тестирования элементов измерения с неуникальными наименованиями
        /// В дампе - "Измерение с неуникальными элементами. Тестирование обновления."
        /// </summary>
        protected virtual string userDimensionId { get; set; } = "dim_Izmerenie_s_neunikalnimi01";

        /// <summary>
        /// Тип тестируемого метода
        /// </summary>
        public override Method Method { get; set; } = Method.PUT;

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
        /// Обновление атрибута у элемента через API с нарушением уникальности
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Update_WithUniquenessViolation()
        {
            var fields = new[]
            {
                new NamedField
                {
                    value = "string",
                    type = NamedFieldType.attribute,
                    name = "string"
                }
            };

            var body = new[] { new { fields } };
            var bodyContent = await Task.Run(() => JsonConvert.SerializeObject(body));

            var expectedResult = new UpdateResult
            {
                updated = 0,
                restricted = 0,
                notChanged = 4
            };

            var result = await ExecutePut(tokenRoleType, expectedResult, null, bodyContent);

            var errorMessage = "Элемент с наименованием \\\"Element 1\\\" уже существует. Данные не сохранены.";

            var isSuccess = !result.IsSuccess && result.Message.Contains(errorMessage);

            Assert.True(isSuccess, result.Message);
        }
    }
}
