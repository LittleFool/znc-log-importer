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
            LocalDateTime dt;
            Regex regexJoinQuit = new Regex(@"^\[(\d{2}):(\d{2}):(\d{2})\] \*\*\* (Joins|Quits): ([\w-\{}|[\]]+) \((?:\w+@[\w.:]+)\)(?: \(([\w :.]+)\))?$");
            Regex regexSetMode = new Regex(@"^\[(\d{2}):(\d{2}):(\d{2})\] \*\*\* RatMama\[BOT\] sets mode: (\+[vhoa]) ([\w-,\{}|[\]]+)$");
            Regex regexRename = new Regex(@"^\[(\d{2}):(\d{2}):(\d{2})\] \*\*\* ([\w-\{}|[\]]+) is now known as ([\w-\{}|[\]]+)$");
            Regex regexText = new Regex(@"^\[(\d{2}):(\d{2}):(\d{2})\] <([\w-\{}|[\]]+)> (.*)$");
            Regex regexTime = new Regex(@"^\[(\d{2}):(\d{2}):(\d{2})\] .*$");

            Match matchJoinQuit;
            Match matchSetMode;
            Match matchRename;
            Match matchText;
            Match matchTime;

            System.IO.StreamReader file = new System.IO.StreamReader(fileName);
            while ((line = file.ReadLine()) != null)
            {
                matchTime = regexTime.Match(line);
                if(matchTime.Success && matchTime.Groups.Count == 4)
                {
                    dt = new LocalDateTime(date.Year, date.Month, date.Day, int.Parse(matchTime.Groups[1].Value), int.Parse(matchTime.Groups[2].Value), int.Parse(matchTime.Groups[3].Value));
                } else
                {
                    continue;
                }
                
                matchJoinQuit = regexJoinQuit.Match(line);
                if(matchJoinQuit.Success && matchJoinQuit.Groups.Count >= 6)
                {
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
                    User u = users[matchJoinQuit.Groups[5].Value];
                    u.setMode(dt, matchJoinQuit.Groups[4].Value);

                    continue;
                }

                matchRename = regexRename.Match(line);
                if(matchRename.Success && matchRename.Groups.Count == 6)
                {
                    User u = users[matchJoinQuit.Groups[4].Value];
                    users.Remove(u.name);
                    u.setName(dt, matchJoinQuit.Groups[5].Value);
                    users.Add(u.name, u);

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
