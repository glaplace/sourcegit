using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SourceGit.UI {

    /// <summary>
    ///     Start git-flow branch dialog.
    /// </summary>
    public partial class GitFlowStartBranch : UserControl {
        private Git.Repository repo = null;
        private Git.Branch.Type type = Git.Branch.Type.Feature;

        /// <summary>
        ///     Sub-name for this git-flow branch.
        /// </summary>
        public string SubName {
            get;
            set;
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="type"></param>
        public GitFlowStartBranch(Git.Repository repo, Git.Branch.Type type) {
            this.repo = repo;
            this.type = type;

            InitializeComponent();
            nameValidator.Repo = repo;

            switch (type) {
            case Git.Branch.Type.Feature:
                var featurePrefix = repo.GetFeaturePrefix();
                txtTitle.Content = App.Text("GitFlow.StartFeatureTitle");
                txtPrefix.Content = featurePrefix;
                nameValidator.Prefix = featurePrefix;
                break;
            case Git.Branch.Type.Release:
                var releasePrefix = repo.GetReleasePrefix();
                txtTitle.Content = App.Text("GitFlow.StartReleaseTitle");
                txtPrefix.Content = releasePrefix;
                nameValidator.Prefix = releasePrefix;
                break;
            case Git.Branch.Type.Hotfix:
                var hotfixPrefix = repo.GetHotfixPrefix();
                txtTitle.Content = App.Text("GitFlow.StartHotfixTitle");
                txtPrefix.Content = hotfixPrefix;
                nameValidator.Prefix = hotfixPrefix;
                break;
            default:
                repo.GetPopupManager()?.Close();
                return;
            }            
        }

        /// <summary>
        ///     Display this dialog
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="type"></param>
        public static void Show(Git.Repository repo, Git.Branch.Type type) {
            repo.GetPopupManager()?.Show(new GitFlowStartBranch(repo, type));
        }

        /// <summary>
        ///     Start git-flow branch
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Sure(object sender, RoutedEventArgs e) {
            txtName.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            if (Validation.GetHasError(txtName)) return;

            var popup = repo.GetPopupManager();
            popup?.Lock();
            await Task.Run(() => repo.StartGitFlowBranch(type, SubName));
            popup?.Close(true);
        }

        /// <summary>
        ///     Cancel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel(object sender, RoutedEventArgs e) {
            repo.GetPopupManager()?.Close();
        }
    }
}
