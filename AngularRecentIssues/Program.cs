using System;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using Octokit;
using Octokit.Internal;
using System.IO;

namespace AngularRecentIssues
{
    class Program
    {
        static string oauthKey = "295289df83f33ea8e082f79c767c065b2410b05d";
        static string owner = "angular";
        static string name = "angular";
        static string filepath = "results.html";

        static InMemoryCredentialStore credentials = new InMemoryCredentialStore(new Credentials(oauthKey));
        static GitHubClient client = new GitHubClient(new ProductHeaderValue("angularIssuesList"), credentials);

        static void Main(string[] args)
        {
            Console.WriteLine("Retrieving Angular Project issues from last 7 days.");

            Task t = new Task(getRecentIssues);
            t.Start();
            t.Wait();

            Console.WriteLine("Issue Retrieval Completed");

            //Process.Start(filepath);

            Console.ReadLine();
        }

        static async void getRecentIssues()
        {
            FileStream fs = File.Create(filepath);
            StreamWriter sw = new StreamWriter(fs);

            RepositoryIssueRequest recentIssues = new RepositoryIssueRequest
            {
                Filter = IssueFilter.All,
                State = ItemStateFilter.All,
                Since = DateTimeOffset.Now.Subtract(TimeSpan.FromDays(7))
            };

            var getIssues = await client.Issue.GetAllForRepository(owner, name, recentIssues);

            sw.Write("<html><body>");
            try
            {
                string assignee;
                for (int i = 0; i < getIssues.Count; i++)
                {
                    try
                    {
                        assignee = getIssues.ElementAt(i).Assignee.Login;
                    }
                    catch (Exception e)
                    {
                        assignee = e.Message;
                    }
                    sw.WriteLine("<div><div id='user'>{0}</div><div id='body'>{1}</div><div id='title'>{2}</div><div id='assignee'>{3}</div></div>",
                        getIssues.ElementAt(i).User.Login,
                        getIssues.ElementAt(i).Body,
                        getIssues.ElementAt(i).Title,
                        assignee);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            sw.Write("</body></html>");
            sw.Close();
            fs.Close();
        }
    }

}
