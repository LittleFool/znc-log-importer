using NodaTime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ZNC_Log_Importer
{
    class FileParser
    {
        private string fileName;
        private LocalDate date;
        private Dictionary<string, User> users = new Dictionary<string, User>();

        public FileParser(string fileName)
        {
            this.fileName = fileName;

            Regex regex = new Regex(@"(\d{4})-(\d{2})-(\d{2})");
            Match match = regex.Match(fileName);
            if (match.Success)
            {
                date = new LocalDate(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value));
            } else
            {
                date = new LocalDate();
            }
        }

        public void parse()
        {
            string line;
            Regex regexJoinQuit = new Regex(@"^\[(\d{2}):(\d{2}):(\d{2})\] \*\*\* (Joins|Quits): ([\w-\{}|[\]]+) \((?:\w+@[\w.:]+)\)(?: \(([\w :.]+)\))?$");
            Regex regexSetMode = new Regex(@"^\[(\d{2}):(\d{2}):(\d{2})\] \*\*\* RatMama\[BOT\] sets mode: (\+[vhoa]) ([\w-,\{}|[\]]+)$");
            Regex regexRename = new Regex(@"^\[(\d{2}):(\d{2}):(\d{2})\] \*\*\* ([\w-\{}|[\]]+) is now known as ([\w-\{}|[\]]+)$");
            Regex regexText = new Regex(@"^\[(\d{2}):(\d{2}):(\d{2})\] <([\w-\{}|[\]]+)> (.*)$");

            Match matchJoinQuit;
            Match matchSetMode;
            Match matchRename;
            Match matchText;

            System.IO.StreamReader file = new System.IO.StreamReader(fileName);
            while ((line = file.ReadLine()) != null)
            {
                matchJoinQuit = regexJoinQuit.Match(line);
                if(matchJoinQuit.Success && matchJoinQuit.Groups.Count >= 6)
                {
                    LocalDateTime dt = new LocalDateTime(date.Year, date.Month, date.Day, int.Parse(matchJoinQuit.Groups[1].Value), int.Parse(matchJoinQuit.Groups[2].Value), int.Parse(matchJoinQuit.Groups[3].Value));

                    if (matchJoinQuit.Groups[4].Value.Equals("Joins"))
                    {
                        User u = new User(dt, matchJoinQuit.Groups[5].Value);
                        users.Add(u.name, u);
                    }

                    if(matchJoinQuit.Groups[4].Value.Equals("Quits"))
                    {
                        User u = users[matchJoinQuit.Groups[5].Value];
                        u.quit(dt);
                        users.Remove(u.name);
                    }

                    continue;
                }

                matchSetMode = regexSetMode.Match(line);
                if(matchSetMode.Success && matchSetMode.Groups.Count == 6)
                {
                    // TODO handle set Mode
                    continue;
                }

                matchRename = regexRename.Match(line);
                if(matchRename.Success && matchRename.Groups.Count == 6)
                {
                    // TODO handle rename
                    continue;
                }

                matchText = regexText.Match(line);
                if(matchText.Success && matchText.Groups.Count == 5)
                {
                    // TODO handle Text
                    continue;
                }
            }
            file.Close();
        }
    }
}
