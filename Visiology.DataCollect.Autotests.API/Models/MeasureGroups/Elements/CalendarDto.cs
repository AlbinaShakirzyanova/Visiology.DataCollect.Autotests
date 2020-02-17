using System;
using System.Collections.Generic;

namespace Visiology.DataCollect.Integration.Tests.Models.MeasureGroups.Elements
{
    /// <summary>
    /// Координата календаря для элемента группы показателей в API
    /// </summary>
    public class CalendarDto
    {
        /// <summary>
        /// Строковое представление даты календаря с учетом гранулярности. 
        /// Например, если гранулярность Месяц, то это будет название конкретного месяца
        /// </summary>
        public string DateWithGranularity { get; set; }

        /// <summary>
        /// Конкретная дата координаты календаря.
        /// Например, если гранулярность Месяц, то это будет первое число месяца
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Название гранулярности
        /// </summary>
        public string Granularity { get; set; }

        public override bool Equals(object obj)
        {
            var calendar = obj as CalendarDto;
            return calendar != null &&
                   DateWithGranularity == calendar.DateWithGranularity &&
                   Date == calendar.Date &&
                   Granularity == calendar.Granularity;
        }

        public override int GetHashCode()
        {
            var hashCode = -992789481;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(DateWithGranularity);
            hashCode = hashCode * -1521134295 + Date.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Granularity);
            return hashCode;
        }
    }
}
