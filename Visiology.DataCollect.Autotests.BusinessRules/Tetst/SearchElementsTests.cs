using Visiology.DataCollect.Autotests.Infrastructure.Entities;

namespace Visiology.DataCollect.Integration.Tests.MeasureGroups.Post.v2
{
    /// <summary>
    /// Тестирование запрета запуска зависимостей
    /// </summary>
    [XApiVersion("2.0")]
    public class ЗапретЗапускаЗависимостей
    {

        // TODO Get-методы с телом не отрабатывают . Доделать на search
        ///// <summary>
        ///// Тест получения элементов группы показатеелй по простому фильтру типа calendar
        ///// </summary>
        ///// <returns>Ожидаемый результат - положительный</returns>
        //[Fact]
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


    }
}
