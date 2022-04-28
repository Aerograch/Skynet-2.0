using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Skynet_2._0.Classes
{
    class Plan
    {
        private DateTime start;
        private DateTime finish;
        private readonly int durationInSeconds;
        private List<SocketGuildUser> participants;
        private string name;
        private bool isDM;
        private bool isAlreadyNotified;

        public Plan(string name, DateTime start, DateTime finish, List<SocketGuildUser> participants, bool isDM)
        { 
            if (start < finish)
            {
                this.start = start;
                this.finish = finish;
            }
            else
            {
                this.start = finish;
                this.finish = start;
            }
            this.name = name;
            durationInSeconds = (int)(finish - start).TotalSeconds;
            this.participants = new List<SocketGuildUser>();
            foreach(SocketGuildUser u in participants)
            {
                this.participants.Add(u);
            }
            this.isDM = isDM;
            this.isAlreadyNotified = false;
        }
        public string Name { get { return name; } set { name = value; } }
        public DateTime Start { get { return start; } set { start = value; } }
        public DateTime Finish { get { return finish; } set { finish = value; } }
        public int DurationInSeconds { get { return durationInSeconds; } }
        public List<SocketGuildUser> Participants 
        { 
            get { return participants; } 
            set 
            {
                participants = new List<SocketGuildUser>();
                foreach (SocketGuildUser u in value)
                {
                    participants.Add(u);
                }
            } 
        }
        public bool IsDM { get { return isDM; } set { isDM = value; } }
        public bool IsAlreadyNotified { get { return isAlreadyNotified; } set { isAlreadyNotified = value; } }
        public void AddUser(SocketGuildUser user)
        {
            participants.Add(user);
        }
        public override string ToString()
        {
            return $"\n\nСобытие {Name}\nНачало: {start}\nКонец: {finish}";
        }
    }
}
