using System.Windows;
using _3_project.Data;

namespace _3_project
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Ensure database is created on application start
            using (var context = new AppDbContext())
            {
                context.Database.EnsureCreated();
            }
        }
    }
}
