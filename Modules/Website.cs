using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;

namespace KarelazisBot.Modules
{
    /// <summary>
    /// NOTE: This class is for use on Tibianic.org
    /// </summary>
    public partial class Website
    {
        public Website(Objects.Client c)
        {
            this.Client = c;

            this.RegexOnlineList = new Regex("community/character/(?<name>[^/]*)/\">[^<]*</a></td><td[^>]*>(?<vocation>[^<]*)</td><td[^>]*>(?<level>[^<]*)</td>");
            this.RegexGuildList = new Regex("community/guild/(?<id>[^/]*)/\">(?<name>[^<]*)</a>");
            this.RegexGuildChars = new Regex("community/character/[^/]*/\">(?<name>[^<]*)</a>(\\s<b>\\((?<nick>[^\\)]*)\\)</b>)?</td><td[^>]*>(?<vocation>[^<]*)</td><td[^>]*>(?<level>[^<]*)</td>");
            this.RegexGuild = new Regex("<h2>Guild #(?<guildname>[^<]*)</h2>");
            this.RegexHighScores = new Regex("<td[^>]*>(?<rank>[^<]*)</td><td[^>]*><a[^>]*>(?<name>[^<]*)</a></td><td[^>]*>(?<value>[^\\s]*)(\\s<font[^>]*>\\[(?<experience>[^\\]]*)\\]</font>)");

            this.UrlOnlineList = "http://tibianic.org/community/online/";
            this.UrlGuildList = "http://tibianic.org/community/guilds/";
            this.UrlGuildTemplate = "http://tibianic.org/community/guild/";
            this.UrlHighScoresTemplate = "http://tibianic.org/highscores/";
        }

        public Objects.Client Client { get; private set; }

        private string UrlOnlineList { get; set; }
        private string UrlGuildList { get; set; }
        /// <summary>
        /// Add "id/".
        /// </summary>
        private string UrlGuildTemplate { get; set; }
        /// <summary>
        /// Add "vocation/filter/".
        /// </summary>
        private string UrlHighScoresTemplate { get; set; }
        private Regex RegexOnlineList { get; set; }
        private Regex RegexGuildList { get; set; }
        private Regex RegexGuildChars { get; set; }
        private Regex RegexGuild { get; set; }
        private Regex RegexHighScores { get; set; }

        public IEnumerable<Character> GetOnlineCharacters()
        {
            return this.GetOnlineCharacters(new WebClient());
        }
        public IEnumerable<Character> GetOnlineCharacters(WebClient wc)
        {
            var matches = this.RegexOnlineList.Matches(wc.DownloadString(this.UrlOnlineList));
            foreach (Match match in matches)
            {
                yield return new Character()
                {
                    Name = match.Groups["name"].Value,
                    Vocation = match.Groups["vocation"].Value,
                    Level = ushort.Parse(match.Groups["level"].Value)
                };
            }
        }
        public IEnumerable<Guild> GetGuilds(bool getMembers = false)
        {
            return this.GetGuilds(new WebClient(), getMembers);
        }
        public IEnumerable<Guild> GetGuilds(WebClient wc, bool getMembers = false)
        {
            var matches = this.RegexGuildList.Matches(wc.DownloadString(this.UrlGuildList));
            foreach (Match match in matches)
            {
                Guild guild = new Guild(match.Groups["name"].Value, int.Parse(match.Groups["id"].Value));

                if (getMembers)
                {
                    string html = wc.DownloadString(this.UrlGuildTemplate + guild.ID + "/");
                    var memberMatches = this.RegexGuildChars.Matches(html);
                    foreach (Match m in memberMatches)
                    {
                        guild.Members.Add(new Character()
                        {
                            Name = m.Groups["name"].Value,
                            Guild = guild,
                            GuildNick = m.Groups["nick"].Value,
                            Vocation = m.Groups["vocation"].Value,
                            Level = ushort.Parse(m.Groups["level"].Value)
                        });
                    }
                }

                yield return guild;
            }
        }
        public Guild GetGuild(string name)
        {
            return this.GetGuild(name, new WebClient());
        }
        public Guild GetGuild(string name, WebClient wc)
        {
            return this.GetGuild(name, wc, this.GetGuilds());
        }
        public Guild GetGuild(string name, WebClient wc, IEnumerable<Guild> guilds)
        {
            foreach (Guild guild in guilds)
            {
                if (guild.Name != name) continue;

                if (guild.Members.Count > 0) guild.Members.Clear();

                string html = wc.DownloadString(this.UrlGuildTemplate + guild.ID + "/");
                var memberMatches = this.RegexGuildChars.Matches(html);
                foreach (Match m in memberMatches)
                {
                    guild.Members.Add(new Character()
                    {
                        Name = m.Groups["name"].Value,
                        Guild = guild,
                        GuildNick = m.Groups["nick"].Value,
                        Vocation = m.Groups["vocation"].Value,
                        Level = ushort.Parse(m.Groups["level"].Value)
                    });
                }
                return guild;
            }
            return null;
        }
    }
}
