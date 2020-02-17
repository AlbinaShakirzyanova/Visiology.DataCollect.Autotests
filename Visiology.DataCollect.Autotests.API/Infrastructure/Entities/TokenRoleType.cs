namespace Visiology.DataCollect.Integration.Tests.Infrastructure.Entities
{
    /// <summary>
    /// Тип токена (по роли)
    /// </summary>
    public enum TokenRoleType : short
    {
        /// <summary>
        /// На пользователя с ролью "Администратор"
        /// </summary>
        UserAdmin,

        /// <summary>
        /// На пользователя с ролью
        /// </summary>
        UserWithRole,

        /// <summary>
        /// На 2ого пользователя с ролью
        /// </summary>
        User2WithRole,

        /// <summary>
        /// На пользователя с фейковой ролью
        /// </summary>
        UserWithFakeRole,

        /// <summary>
        /// На пользователя для тестирования неуникальных элементов
        /// </summary>
        UserForNotUniqueElements
    }
}
