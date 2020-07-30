using GitCommits;
using Soneta.Business;

[assembly: NewRow(typeof(GitUsageEnova.AveragePerDay))]

namespace GitUsageEnova
{
    public class AveragesPerDay : GitModule.AveragePerDayTable
    {
    }
}
