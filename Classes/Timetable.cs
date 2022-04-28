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
    class Timetable
    {
        private List<Plan> plans;
        private IUser author;
        public Timetable(List<Plan> plans, IUser author)
        {
            this.plans = new List<Plan>();
            foreach(Plan p in plans)
            {
                this.plans.Add(p);
            }
            this.author = author;
        }
        public IUser Author { get { return author; } set { author = value; } }
        public List<Plan> Plans 
        { 
            get { return plans; } 
            set 
            {
                plans = new List<Plan>();
                foreach(Plan p in value)
                {
                    plans.Add(p);
                }
            } 
        }
        public void AddPlan(Plan plan)
        {
            plans.Add(plan);
        }
        public bool DeletePlan(string name)
        {
            int count = 0;
            foreach(Plan p in plans)
            {
                if (p.Name == name)
                {
                    plans.RemoveAt(count);
                    return true;
                }
                count++;
            }
            return false;
        }
        public bool Check(DateTime time)
        {
            foreach (Plan p in plans)
            {
                if (p.Start < time && p.Finish > time)
                {
                    return true;
                }
            }
            return false;
        }
        public override string ToString()
        {
            string output = "";
            foreach(Plan p in plans)
            {
                output += p.ToString();
            }
            return $"События пользователя {author.Username}#{author.Discriminator}{output}";
        }
    }
}
