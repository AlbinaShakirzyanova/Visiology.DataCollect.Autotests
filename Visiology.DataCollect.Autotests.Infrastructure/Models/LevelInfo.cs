namespace Visiology.DataCollect.Autotests.Infrastructure.Models
{
    public class LevelInfo
    {
        public string Name { get; set; }

        public string UniqueName { get; set; }

        public int Index { get; set; } = 0;

        public int? ParentLevelId { get; set; } = null;

        public int ElementsCount { get; set; } = 0;

        public int FoldersCount { get; set; } = 0;

        public bool IsLeafLevel { get; set; } = true;
    }
}
