using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ZNC_Log_Importer
{
    class FileParser
    {
        private String fileName;
        private DateTime date;

        public FileParser(String fileName)
        {
            this.fileName = fileName;

            Regex regex = new Regex(@"(\d{4})-(\d{2})-(\d{2})");
            Match match = regex.Match(fileName);
            if (match.Success)
            {
                date = new DateTime(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value));
            } else
            {
                date = new DateTime();
            }
        }

        public void parse()
        {
            String line;
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
                    // TODO handle Join/Quit
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
