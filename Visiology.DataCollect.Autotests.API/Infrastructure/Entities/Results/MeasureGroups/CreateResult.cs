namespace Visiology.DataCollect.Autotests.API.Infrastructure.Entities.Results.MeasureGroups
{
    /// <summary>
    /// Результат создания элементов группы показателей
    /// </summary>
    public class CreateResult
    {
        /// <summary>
        /// Количество созданных сущностей
        /// </summary>
        public int created { get; set; }

        /// <summary>
        /// Количество сущностей, которые нельзя изменить
        /// </summary>
        public int restricted { get; set; }

        public override bool Equals(object obj)
        {
            var result = obj as CreateResult;
            return result != null &&
                   created == result.created &&
                   restricted == result.restricted;
        }

        public override int GetHashCode()
        {
            var hashCode = -992789481;

            hashCode = hashCode * -1521134295 + restricted.GetHashCode();
            hashCode = hashCode * -1521134295 + created.GetHashCode();
            return hashCode;
        }
    }
}
