namespace Visiology.DataCollect.Autotests.Entities.Results
{
    /// <summary>
    /// Сущность, описывающая результаты верификации контента
    /// </summary>
    public class ContentVerificationResult
    {
        /// <summary>
        /// Флаг успешности получения
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Информационное сообщение пользователю
        /// </summary>
        public string Message { get; set; }
    }
}
