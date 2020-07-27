using GitCommits;
using Soneta.Business;

[assembly: NewRow(typeof(GitUsage.CommitPerDay))]

namespace GitUsage
{
    public class CommitsPerDay : GitModule.CommitPerDayTable
    {
    }
}
