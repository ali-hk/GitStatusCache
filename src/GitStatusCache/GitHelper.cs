using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace GitStatusCache
{
    public static class GitHelper
    {
        private readonly static Regex statusRegex = new Regex("^(?<index>[^#])(?<working>.) (?<path1>.*?)(?: -> (?<path2>.*))?$", RegexOptions.Compiled);
        private readonly static Regex branchRegex = new Regex(@"^## (?<branch>\S+?)(?:\.\.\.(?<upstream>\S+))?(?: \[(?:ahead (?<ahead>\d+))?(?:, )?(?:behind (?<behind>\d+))?(?<gone>gone)?\])?$", RegexOptions.Compiled);
        private readonly static Regex initRegex = new Regex(@"^## Initial commit on (?<branch>\S+)$", RegexOptions.Compiled);

        public static string GetWorkingPath(string repoPath)
        {
            return Path.GetDirectoryName(repoPath);
        }

        public static RepositoryStatus ParseStatusOutput(string output)
        {
            var lines = output.Split(new[] { "\n", "\r", Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            var indexAdded = new List<string>();
            var indexModified = new List<string>();
            var indexDeleted = new List<string>();
            var indexConflicted = new List<string>();
            var workingAdded = new List<string>();
            var workingModified = new List<string>();
            var workingDeleted = new List<string>();
            var workingConflicted = new List<string>();
            string branch = null;
            string upstream = null;
            int? aheadBy = null;
            int? behindBy = null;
            bool? upstreamGone = null;

            foreach (var currLine in lines)
            {
                var statusResults = statusRegex.Match(currLine);
                if (statusResults.Success)
                {
                    var indexStatus = statusResults.Groups["index"].Value;
                    switch (indexStatus)
                    {
                        case "A":
                            indexAdded.Add(statusResults.Groups["path1"].Value);
                            break;
                        case "M":
                        case "R":
                        case "C":
                            indexModified.Add(statusResults.Groups["path1"].Value);
                            break;
                        case "D":
                            indexDeleted.Add(statusResults.Groups["path1"].Value);
                            break;
                        case "U":
                            indexConflicted.Add(statusResults.Groups["path1"].Value);
                            break;
                        default:
                            break;
                    }

                    var workingStatus = statusResults.Groups["working"].Value;
                    switch (workingStatus)
                    {
                        case "?":
                        case "A":
                            workingAdded.Add(statusResults.Groups["path1"].Value);
                            break;
                        case "M":
                            workingModified.Add(statusResults.Groups["path1"].Value);
                            break;
                        case "D":
                            workingDeleted.Add(statusResults.Groups["path1"].Value);
                            break;
                        case "U":
                            workingConflicted.Add(statusResults.Groups["path1"].Value);
                            break;
                        default:
                            break;
                    }

                    continue;
                }

                var branchResults = branchRegex.Match(currLine);
                if (branchResults.Success)
                {
                    branch = branchResults.Groups["branch"].Value;
                    upstream = branchResults.Groups["upstream"].Value;
                    var aheadGroup = branchResults.Groups["ahead"];
                    if (aheadGroup.Success)
                    {
                        aheadBy = int.Parse(aheadGroup.Value);
                    }

                    var behindGroup = branchResults.Groups["behind"];
                    if (behindGroup.Success)
                    {
                        behindBy = int.Parse(behindGroup.Value);
                    }

                    upstreamGone = branchResults.Groups["gone"].Value.Equals("gone", StringComparison.OrdinalIgnoreCase);
                    continue;
                }

                var initResults = initRegex.Match(currLine);
                if (initResults.Success)
                {
                    branch = branchResults.Groups["branch"].Value;
                }
            }

            var newStatus = new RepositoryStatus
            {
                AheadBy = aheadBy ?? 0,
                BehindBy = behindBy ?? 0,
                Branch = branch,
                IndexAdded = indexAdded,
                IndexConflicted = indexConflicted,
                IndexDeleted = indexDeleted,
                IndexModified = indexModified,
                WorkingAdded = workingAdded,
                WorkingConflicted = workingConflicted,
                WorkingDeleted = workingDeleted,
                WorkingModified = workingModified,
                Stashes = null,
                Upstream = upstream,
                UpstreamGone = upstreamGone ?? false
            };

            return newStatus;
        }
    }
}
