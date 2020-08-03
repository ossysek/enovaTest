using GitCommits;
using GitUsageEnova.UI;
using GitUsageEnova.UI.Extender;
using LibGit2Sharp;
using Soneta.Business;
using Soneta.Business.App;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: Worker(typeof(GetAveragesPerDay))]

namespace GitUsageEnova.UI
{
    public class GetAveragesPerDay : ContextBase
    {
        public GetAveragesPerDay(Context contex) : base(contex)
        {
        }

        [Context]
        public GetAveragePerDayParams Params { get; set; }

        [Context]
        public AveragesPerDay CommitTable { get; set; }

        [Context]
        public Login Login { get; set; }

        [Context]
        public View View { get; set; }

        [Action("GitUsage/Dodanie średniej", Mode = ActionMode.SingleSession | ActionMode.ConfirmSave | ActionMode.Progress)]
        public void AddCommit()
        {
            Database database = BusApplication.Instance["Firma demo"];
            Login = database.Login(false, "Administrator", "");

            List<CommitFromGit> commity = GetGitCommits();

            IEnumerable<IGrouping<DateTime, string>> query = commity.GroupBy(x => x.dataTime, x => x.author);

            List<CommitPerDay> listOfCommits = new List<CommitPerDay>();
            foreach (var commitGroup in query)
            {
                foreach (var commit in commitGroup)
                {
                    int count = commity.Where(x => x.author == commit && x.dataTime == commitGroup.Key).Count();
                    CommitPerDay cc = new CommitPerDay();
                    cc.IdCommit = commitGroup.Key + "_" + commit;
                    cc.Data = commitGroup.Key;
                    cc.Osoba = commit;
                    cc.Ilosc = count;

                    if (listOfCommits.Where(x => x.IdCommit == cc.IdCommit).Any())
                        listOfCommits.First(x => x.IdCommit == cc.IdCommit).Ilosc = cc.Ilosc;
                    else
                        listOfCommits.Add(cc);
                }
            }

            var osoby = listOfCommits.Select(x => x.Osoba).Distinct();

            foreach (var osoba in osoby)
            {
                int sum = listOfCommits.Where(x => x.Osoba == osoba).Sum(x => x.Ilosc);
                int count = listOfCommits.Where(x => x.Osoba == osoba).Count();

                AveragePerDay cc = new AveragePerDay();
                cc.Osoba = osoba;
                cc.Ilosc = sum / count;

                var test = PobierzDane(cc);
                if (test == null)
                    ZapiszDane(cc);
                else
                    UaktualnijDane(cc);
            }
        }

        public AveragePerDay PobierzDane(AveragePerDay cpd)
        {
            AveragePerDay commit = new AveragePerDay();
            using (Session session = Login.CreateSession(false, false))
            {
                GitModule gm = GitModule.GetInstance(session);
                using (ITransaction t = session.Logout(true))
                {
                    commit = gm.AveragesPerDay.WgKodAverage[cpd.Osoba];
                }
            }
            return commit;
        }

        public void ZapiszDane(AveragePerDay cpd)
        {
            using (Session session = Login.CreateSession(false, false))
            {
                GitModule gm = GitModule.GetInstance(session);
                using (ITransaction t = session.Logout(true))
                {
                    gm.AveragesPerDay.AddRow(cpd);
                    t.Commit();
                }
                session.Save();
            }
        }

        public void UaktualnijDane(AveragePerDay cpd)
        {
            using (Session session = Login.CreateSession(false, false))
            {
                GitModule gm = GitModule.GetInstance(session);
                using (ITransaction t = session.Logout(true))
                {
                    gm.AveragesPerDay.WgKodAverage[cpd.Osoba].Ilosc = cpd.Ilosc;
                    t.Commit();
                }
                session.Save();
            }
        }

        public List<CommitFromGit> GetGitCommits()
        {
            List<CommitFromGit> commity = new List<CommitFromGit>();

            var tmp = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            string path = Repository.Clone(
                "https://github.com/ossysek/enovaTest",
                tmp);

            using (var repo = new Repository(path))
            {
                foreach (var branch in repo.Branches)
                {
                    var commits = branch.Commits;
                    foreach (var commit in commits)
                    {
                        CommitFromGit cc = new CommitFromGit();
                        cc.idCommit = commit.Sha;
                        cc.dataTime = commit.Author.When.DateTime.Date;
                        cc.author = commit.Author.Name;

                        commity.Add(cc);
                    }
                }
            }
            return commity;
        }
    }

    public class GetAveragePerDayParams : ContextBase
    {
        public GetAveragePerDayParams(Context context) : base(context)
        {
        }

        public AveragesPerDay CommitTable { get; set; }

        public View View { get; set; }
    }
}
