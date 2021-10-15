using System.IO;
using System.Windows.Controls;

namespace UnityModLoader.Manager
{
    /// <summary>
    /// Interação lógica para ModControl.xam
    /// </summary>
    public partial class ModControl : UserControl
    {
        public FileInfo ModPath { get; set; }
        public string ModName
        {
            get => EnabledCheck.Content.ToString();
            set => EnabledCheck.Content = value;
        }

        string _description;
        public string ModDescription
        {
            get => _description;
            set
            {
                _description = value;
                DescriptionText.Text = $"Description: {value}";
            }
        }

        string _author;
        public string ModAuthor
        {
            get => _author;
            set
            {
                _author = value;
                AuthorText.Text = $"Author: {value}";
            }
        }

        public ModControl()
        {
            InitializeComponent();
        }

        private void EnabledCheck_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!ModPath.Name.EndsWith(".disabled"))
                return;

            ModPath.MoveTo(Path.Combine(ModPath.Directory.FullName,
                ModPath.Name.Remove(ModPath.Name.LastIndexOf('.'))));
        }

        private void EnabledCheck_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (ModPath.Name.EndsWith(".disabled"))
                return;

            ModPath.MoveTo(Path.Combine(ModPath.Directory.FullName,
                ModPath.Name + ".disabled"));
        }
    }
}
