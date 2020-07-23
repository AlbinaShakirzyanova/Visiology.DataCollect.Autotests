using System.Net.Http;
using System.Threading.Tasks;
using Visiology.DataCollect.Autotests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Impl;
using Visiology.DataCollect.Integration.Tests.Models.MeasureGroups.Forms;
using Xunit;

namespace Visiology.DataCollect.Autotests.API.Tests.MeasureGroups.Get.v2
{
    /// <summary>
    /// Класс тестирования метода получения форм группы показателей
    /// </summary>
    [XApiVersion("2.0")]
    public class FormsTests : BaseIntegrationGetTests<FormsListDto, FormDto>
    {
        /// <summary>
        /// Группа показателей для тестирования получения форм группы показателей
        /// </summary>
        private const string measureGroupId = "measureGroup_testirovanie_po01";

        /// <summary>
        /// Количество форм, доступных пользователю с ролью "Администратор" (в ГП MeasureGroupForGetFormsTests)
        /// </summary>
        private const int adminUserFormsCount = 18;

        /// <summary>
        /// Количество форм, доступных пользователю с ролью (в ГП MeasureGroupForGetFormsTests)
        /// </summary>
        private const int userFormsCount = 18;

        /// <summary>
        /// URL запроса на получение форм группы показателей
        /// </summary>
        public override string Url { get; set; }

        public FormsTests(IisFixture iisFixture, TokenFixture tokenFixture, RestService restService)
            : base(iisFixture, tokenFixture, restService)
        {
            Url = GetUrl(measureGroupId);
        }

        /// <summary>
        /// Тест получения форм группы показателей при невалидном или несуществующем идентификаторе группы показателей
        /// </summary>
        /// <param name="mgId">Идентификатор группы показателей</param>
        /// <returns>Ожидаемый результат - отрицательный</returns>
        [Theory, Trait("Category", "MeasureGroups")]
        [InlineData(null)]
        public async Task Get_WithInvalidMgId(int? mgId)
        {
            Url = string.Format($"{config.GetValue("ApiUrl")}{config.GetValue("ApiUrlGetMeasureGroupFormsPath")}", mgId);
            var result = await ExecuteGet(TokenRoleType.UserAdmin, 0);

            Assert.True(!result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест получения данных при отсутствии токена в запросе
        /// </summary>
        /// <returns>Ожидаемый результат - отрицательный</returns>
        [Fact, Trait("Category", "MeasureGroups")]
        public async Task GetAll_WithoutToken()
        {
            using (var client = new HttpClient())
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, Url);

                requestMessage.Headers.Add("X-API-VERSION", XApiVersion);

                var response = await client.SendAsync(requestMessage);
                var responseContent = await response.Content.ReadAsStringAsync();

                Assert.True(!response.IsSuccessStatusCode, response.ReasonPhrase);
            }
        }

        /// <summary>
        /// Тест получения форм группы показателей для пользователя с ролью "Администратор"
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups")]
        public async Task GetAll_ForAdminUser()
        {
            var result = await ExecuteGet(TokenRoleType.UserAdmin, adminUserFormsCount);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест получения форм группы показателей для пользователя с ролью
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups")]
        public async Task GetAll_ForUser()
        {
            var result = await ExecuteGet(TokenRoleType.UserWithRole, userFormsCount);

            Assert.True(result.IsSuccess, result.Message);
        }


        protected override string GetUrl(string entityId)
        {
            return string.Format($"{config.GetValue("ApiUrl")}{config.GetValue("ApiUrlGetMeasureGroupFormsPath")}", entityId);
        }

        protected override string GetSearchUrl(string dimensionId)
        {
            throw new System.NotImplementedException();
        }
    }
}
