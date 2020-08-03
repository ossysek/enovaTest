using GitCommits;
using Soneta.Business;

namespace GitUsageEnova.UI
{
    public class AveragesPerDayViewInfo : ViewInfo
    {
        public AveragesPerDayViewInfo()
        {
            ResourceName = "AveragesPerDay";

            InitContext += AveragePerDayViewInfi_InitContext;

            CreateView += AveragePerDayViewInfo_CreateView;
        }

        private void AveragePerDayViewInfi_InitContext(object sender, ContextEventArgs args)
        {
            args.Context.Remove(typeof(WParams));
            args.Context.TryAdd(() => new WParams(args.Context));
        }

        void AveragePerDayViewInfo_CreateView(object sender, CreateViewEventArgs args)
        {
            WParams parameters;
            if (!args.Context.Get(out parameters))
                return;

            args.View = ViewCreate(parameters);
            args.View.AllowNew = false;
        }

        protected View ViewCreate(WParams pars)
        {
            View view = GitModule.GetInstance(pars.Session).AveragesPerDay.CreateView();

            var worker = CreateWorker(pars.Context, view);
            worker.AddCommit();

            return view;
        }

        private GetAveragesPerDay CreateWorker(Context context, View view)
        {
            return new GetAveragesPerDay(context)
            {
                CommitTable = context.Session.GetGit().AveragesPerDay,
                View = view,
                Params = new GetAveragePerDayParams(context)
                {
                    CommitTable = context.Session.GetGit().AveragesPerDay,
                    View = view
                }
            };
        }

        public class WParams : ContextBase
        {
            public WParams(Context context) : base(context) 
            {
            }
        }
    }
}
