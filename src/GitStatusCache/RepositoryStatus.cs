using System.Collections.Generic;

namespace GitStatusCache
{
    public class RepositoryStatus
    {
        public int Version => 1;

        public string Branch { get; set; }

        public string Upstream { get; set; }

        public bool UpstreamGone { get; set; }

        public int AheadBy { get; set; }

        public int BehindBy { get; set; }

        public IList<string> IndexAdded { get; set; }

        public IList<string> IndexModified { get; set; }

        public IList<string> IndexDeleted { get; set; }

        public IList<string> IndexRenamed => null;

        public IList<string> IndexConflicted { get; set; }

        public IList<string> WorkingAdded { get; set; }

        public IList<string> WorkingModified { get; set; }

        public IList<string> WorkingDeleted { get; set; }

        public IDictionary<string, string> WorkingRenamed => null;

        public IList<string> WorkingConflicted { get; set; }

        public IList<Stash> Stashes { get; set; }
    }
}