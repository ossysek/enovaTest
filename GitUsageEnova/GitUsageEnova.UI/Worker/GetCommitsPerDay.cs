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

[assembly: Worker(typeof(GetCommitsPerDay))]

namespace GitUsageEnova.UI
{
    public class GetCommitsPerDay : ContextBase
    {
        public GetCommitsPerDay(Context contex) : base(contex)
        {
        }

        [Context]
        public GetCommitsPerDayParams Params { get; set; }

        [Context]
        public CommitsPerDay CommitTable { get; set; }

        [Context]
        public View View { get; set; }

        [Context]
        public Login Login { get; set; }

        [Action("GitUsage/Dodanie commitu", Mode = ActionMode.SingleSession | ActionMode.ConfirmSave | ActionMode.Progress)]
        public void AddCommit()
        {
            Database database = BusApplication.Instance["Firma demo"];
            Login = database.Login(false, "Administrator", "");
            
            List<CommitFromGit> commity = GetGitCommits();

            IEnumerable<IGrouping<DateTime, string>> query = commity.GroupBy(x => x.dataTime, x => x.author);

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

                    var test = PobierzDane(cc);
                    if (test == null)
                        ZapiszDane(cc);
                    else
                        UaktualnijDane(cc);
                }
            }
        }

        public CommitPerDay PobierzDane(CommitPerDay cpd)
        {
            CommitPerDay commit = new CommitPerDay();
            using (Session session = Login.CreateSession(false, false))
            {
                GitModule gm = GitModule.GetInstance(session);
                using (ITransaction t = session.Logout(true))
                {
                    commit = gm.CommitsPerDay.WgKod[cpd.IdCommit];
                }
            }
            return commit;
        }

        public void ZapiszDane(CommitPerDay cpd)
        {
            using (Session session = Login.CreateSession(false, false))
            {
                GitModule gm = GitModule.GetInstance(session);
                using (ITransaction t = session.Logout(true))
                {
                    gm.CommitsPerDay.AddRow(cpd);
                    t.Commit();
                }
                session.Save();
            }
        }
        
        public void UaktualnijDane(CommitPerDay cpd)
        {
            using (Session session = Login.CreateSession(false, false))
            {
                GitModule gm = GitModule.GetInstance(session);
                using (ITransaction t = session.Logout(true))
                {
                    gm.CommitsPerDay.WgKod[cpd.IdCommit].Ilosc = cpd.Ilosc;
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

    public class GetCommitsPerDayParams : ContextBase
    {
        public GetCommitsPerDayParams(Context context) : base(context)
        {
        }

        public CommitsPerDay CommitTable { get; set; }

        public View View { get; set; }
    }
}
