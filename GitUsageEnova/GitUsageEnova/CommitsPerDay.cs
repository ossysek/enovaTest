using GitCommits;
using Soneta.Business;

[assembly: NewRow(typeof(GitUsageEnova.CommitPerDay))]

namespace GitUsageEnova
{
    public class CommitsPerDay : GitModule.CommitPerDayTable
    {
    }
}
