using GitCommits;
using Soneta.Business;

namespace GitUsageEnova.UI
{
    public class CommitsPerDayViewInfo: ViewInfo
    {
        public CommitsPerDayViewInfo()
        {
            ResourceName = "CommitsPerDay";

            InitContext += CommitsPerDayViewInfo_InitContext;

            CreateView += CommitsPerDayViewInfo_CreateView;
        }

        private void CommitsPerDayViewInfo_InitContext(object sender, ContextEventArgs args)
        {
            args.Context.Remove(typeof(WParams));
            args.Context.TryAdd(() => new WParams(args.Context));
        }

        void CommitsPerDayViewInfo_CreateView(object sender, CreateViewEventArgs args)
        {
            WParams parameters;
            if (!args.Context.Get(out parameters))
                return;

            args.View = ViewCreate(parameters);
            args.View.AllowNew = false;
        }

        protected View ViewCreate(WParams pars)
        {
            View view = GitModule.GetInstance(pars.Session).CommitsPerDay.CreateView();

            var worker = CreateWorker(pars.Context, view);
            worker.AddCommit();

            return view;
        }

        private GetCommitsPerDay CreateWorker(Context context, View view)
        {
            return new GetCommitsPerDay(context)
            {
                CommitTable = context.Session.GetGit().CommitsPerDay,
                View = view,
                Params = new GetCommitsPerDayParams(context)
                {
                    CommitTable = context.Session.GetGit().CommitsPerDay,
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
