using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Interfaces;

namespace Visiology.DataCollect.Integration.Tests.Infrastructure.Entities.Results
{
    /// <summary>
    /// Класс, описывающий результаты получения !!! Сущность фейковая, понадобилась для рефакторинга !!!
    /// </summary>
    public class GetResult : IResponseResult
    {
        /// <summary>
        /// Количество полученных сущностей
        /// </summary>
        public int count { get; set; }
    }
}
