using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;
using System.IO;

namespace UnfollowTheseGuys
{
    class Program
    {
        static void Main(string[] args)
        {
            TwitterAuth t = new TwitterAuth();
            GetFollowers();
        }

        public static void GetFollowers()
        {
            var followers = User.GetFollowerIds(User.GetLoggedUser());
            var following = User.GetFriendIds(User.GetLoggedUser());

            List<long> followersList = followers.ToList<long>();
            followersList.Sort();
            List<long> followingList = following.ToList<long>();
            followingList.Sort();

            List<long> toUnfollow = new List<long>();

            foreach (long id in followingList)
            {
                if (followersList.Contains(id) == false)
                {
                    toUnfollow.Add(id);
                }
            }

            Console.WriteLine("Found {0} people who are not following back...\n===\n", toUnfollow.Count);

            StreamWriter writer = new StreamWriter("tounfollow.html");
            writer.WriteLine("<ul>");
            
            foreach (long id in toUnfollow)
            {
                string url = "https://twitter.com/intent/user?user_id=" + id;
                writer.WriteLine("<li><a href=\"{0}\">{0}</a></li>", url);
            }

            writer.WriteLine("</ul>");
            writer.Close();

            Console.WriteLine("See 'tounfollow.html' for list of people to unfollow.");
            Console.ReadKey();
        }
    }
}
