namespace Visiology.DataCollect.Integration.Tests.Infrastructure.Entities
{
    /// <summary>
    /// Тестовые значения для кейсов
    /// </summary>
    internal static class TestValues
    {
        /// <summary>
        /// Идентификатор не существующей в системе сущности
        /// </summary>
        public const string NonExistId = "2147483647";

        /// <summary>
        /// Нулевой идентификатор
        /// </summary>
        public const string ZeroId = "0";

        /// <summary>
        /// (для параметров)
        /// </summary>
        public const int ZeroValue = 0;
        public const int LessThanZeroValue = -1;
        public const int MoreThanZeroValue = 1;
        public const int IntValue = int.MaxValue;
        public const string StringValue = "Строковое значение";
        public const string StringValueConvertibleToInt = "2";
        public const double FloatValue = 10.12;
        public const double FloatValueConvertibleToInt = 3.00;
        public const string DateValueToStringCorrectFormat = "2018-05-16"; // yyyy-MM-dd
        public const string DateValueToStringIncorrectFormat = "2018-16-05"; // yyyy-dd-MM
        public const string TextValue = "Пока просто текст"; 
    };
}
