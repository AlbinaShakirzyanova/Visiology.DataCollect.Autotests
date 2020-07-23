using Visiology.DataCollect.Autotests.API.Tests.MeasureGroups.Post.Get;
using Visiology.DataCollect.Autotests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Impl;
using Visiology.DataCollect.Integration.Tests.Models.MeasureGroups.Elements;

namespace Visiology.DataCollect.Autotests.API.Tests.MeasureGroups.Post.v2
{
    /// <summary>
    /// Класс тестирования метода поиска элементов группы показателей
    /// </summary>
    [XApiVersion("2.0")]
    class SearchElementsTests : BaseTests<ElementsListDto, ElementDto>
    {
        public override string Url { get; set; }

        /// <summary>
        /// Группа показателей для тестирования получения элементов 
        /// В дампе - "Группа показателей для тестирования получения элементов"
        /// </summary>
        private const string measureGroupId = "measureGroup_Gruppa_pokazatel_";

        public SearchElementsTests(IisFixture iisFixture, TokenFixture tokenFixture, RestService restService)
            : base(iisFixture, tokenFixture, restService)
        {
            Url = GetUrl(measureGroupId);
        }

        // TODO Get-методы с телом не отрабатывают . Доделать на search
        ///// <summary>
        ///// Тест получения элементов группы показатеелй по простому фильтру типа calendar
        ///// </summary>
        ///// <returns>Ожидаемый результат - положительный</returns>
        //[Fact, Trait("Category", "MeasureGroups")]
        //public async Task GetAll_BySimpleCalendarFilter()
        //{
        //    var filter = new SimpleFilter
        //    {
        //        value = "2010-01-01",
        //        type = SimpleFilterType.calendar,
        //        condition = FilterCondition.equals
        //    };

        //    var filterContent = await Task.Run(() => JsonConvert.SerializeObject(filter));

        //    // Количество получаемых элементов при тестировании фильтра
        //    var elementsCount = 4;

        //    var result = await this.ExecuteGet(TokenRoleType.UserWithRole, elementsCount, null, filterContent);

        //    Assert.True(result.IsSuccess, result.Message);
        //}

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
