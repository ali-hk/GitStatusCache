using System.Collections.Generic;

namespace GitStatusCache
{
    public class RepositoryStatus
    {
        public string Path { get; set; }

        public string RepoPath { get; set; }

        public string WorkingDir { get; set; }

        public string State { get; set; }

        public string Branch { get; set; }

        public string Upstream { get; set; }

        public bool UpstreamGone { get; set; }

        public int AheadBy { get; set; }

        public int BehindBy { get; set; }

        public IList<string> IndexAdded { get; set; }

        public IList<string> IndexModified { get; set; }

        public IList<string> IndexDeleted { get; set; }

        public IList<string> IndexTypeChange { get; set; }

        public IList<string> IndexRenamed { get; set; }

        public IList<string> IndexConflicted { get; set; }

        public IList<string> WorkingAdded { get; set; }

        public IList<string> WorkingModified { get; set; }

        public IList<string> WorkingDeleted { get; set; }

        public IList<string> WorkingTypeChange { get; set; }

        public IDictionary<string, string> WorkingRenamed { get; set; }

        public IList<string> WorkingConflicted { get; set; }

        public IList<string> WorkingUnreadable { get; set; }

        public IList<string> Ignored { get; set; }


        public IList<Stash> Stashes { get; set; }
    }
}