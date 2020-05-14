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
            Regex regexJoinQuit = new Regex(@"^\[(\d{2}):(\d{2}):(\d{2})\] \*\*\* (Joins|Quits): ([\w-\{}|[\]]+) \((?:\w+@[\w.:]+)\)(?: \(([\w \{}|[\]!""§$% &\/= _,;<>|:().! -]+)\))?$");
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
                
                // Join or Quit
                matchJoinQuit = regexJoinQuit.Match(line);
                if(matchJoinQuit.Success && matchJoinQuit.Groups.Count >= 6)
                {
                    // User joins
                    if (matchJoinQuit.Groups[4].Value.Equals("Joins"))
                    {
                        // Username cant be in the dictionary so we add them
                        User u = new User(matchJoinQuit.Groups[5].Value);
                        u.join(dt);
                        users.Add(u.name, u);
                    }

                    // User quits
                    if(matchJoinQuit.Groups[4].Value.Equals("Quits"))
                    {
                        // Username could be in the dictionary else we didnt see them join
                        if (users.ContainsKey(matchJoinQuit.Groups[5].Value))
                        {
                            User u = users[matchJoinQuit.Groups[5].Value];
                            u.quit(dt);
                            users.Remove(u.name);
                        } else
                        {
                            User u = new User(matchJoinQuit.Groups[5].Value);
                            u.quit(dt);
                        }
                    }

                    continue;
                }

                // Mode change
                matchSetMode = regexSetMode.Match(line);
                if(matchSetMode.Success && matchSetMode.Groups.Count == 6)
                {
                    User u;

                    // very unlikely but the user might not be in the dictionary
                    if (users.ContainsKey(matchSetMode.Groups[5].Value))
                    {
                        u = users[matchSetMode.Groups[5].Value];
                    } else
                    {
                        u = new User(matchSetMode.Groups[5].Value);
                    }

                    u.setMode(dt, matchSetMode.Groups[4].Value);

                    continue;
                }

                // Namechange
                matchRename = regexRename.Match(line);
                if(matchRename.Success && matchRename.Groups.Count == 6)
                {
                    User u;

                    // if we saw the user join get it and remove it, if we didnt just create it
                    if (users.ContainsKey(matchRename.Groups[4].Value))
                    {
                        u = users[matchRename.Groups[4].Value];
                        users.Remove(u.name);
                    } else
                    {
                        u = new User(matchRename.Groups[4].Value);
                    }

                    u.setName(dt, matchRename.Groups[5].Value);
                    users.Add(u.name, u);

                    continue;
                }

                // normal text line
                matchText = regexText.Match(line);
                if(matchText.Success && matchText.Groups.Count == 5)
                {
                    User u;

                    if (users.ContainsKey(matchText.Groups[4].Value))
                    {
                        u = users[matchText.Groups[4].Value];
                    } else
                    {
                        u = new User(matchText.Groups[4].Value);
                    }

                    u.addMessage(dt, matchText.Groups[5].Value);

                    continue;
                }
            }
            file.Close();
        }
    }
}
