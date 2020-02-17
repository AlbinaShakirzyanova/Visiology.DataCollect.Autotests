using System.Collections.Generic;

namespace Visiology.DataCollect.Autotests.Infrastructure.Models
{
    public class DimensionInfo
    {
        public string Name { get; set; }

        public string UniqueName { get; set; }

        public bool IsDictionary { get; set; }

        public string Description { get; set; }

        public string Type { get; set; }

        public List<LevelInfo> Levels { get; set; }

        public List<AttributeInfo> Attributes { get; set; }
    }
}
