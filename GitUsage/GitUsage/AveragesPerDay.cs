using GitCommits;
using Soneta.Business;

[assembly: NewRow(typeof(GitUsage.AveragePerDay))]

namespace GitUsage
{
    public class AveragesPerDay : GitModule.AveragePerDayTable
    {
    }
}
