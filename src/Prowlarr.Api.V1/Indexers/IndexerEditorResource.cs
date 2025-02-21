using System.Collections.Generic;

namespace Prowlarr.Api.V1.Indexers
{
    public class IndexerEditorResource
    {
        public List<int> IndexerIds { get; set; }
        public string Enable { get; set; }
        public List<int> Tags { get; set; }
        public ApplyTags ApplyTags { get; set; }
    }

    public enum ApplyTags
    {
        Add,
        Remove,
        Replace
    }
}
