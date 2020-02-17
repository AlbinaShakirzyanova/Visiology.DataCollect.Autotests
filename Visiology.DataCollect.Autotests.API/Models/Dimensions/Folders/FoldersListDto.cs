using Newtonsoft.Json;
using System.Collections.Generic;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Interfaces;

namespace Visiology.DataCollect.Integration.Tests.Models.Dimensions.Folders
{
    /// <summary>
    /// Список каталогов измерения
    /// </summary>
    public class FoldersListDto : IResponseContentList<FolderDto>
    {
        /// <summary>
        /// Список каталогов
        /// </summary>
        [JsonProperty(PropertyName = "Folders")]
        public IEnumerable<FolderDto> Entities { get; set; } = new List<FolderDto>();
    }    
}
