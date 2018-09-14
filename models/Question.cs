using System;
using System.Collections.Generic;
using System.Linq;

namespace dotnetTrivia.models
{
    public class Question
    {
        public string category {get;set;}
        public string type {get;set;}
        public string difficulty {get;set;}
        public string question {get;set;}
        public string correct_answer {get;set;}
        public string[] incorrect_answers {get;set;}

        public IEnumerable<string> Answers
        {
            get
            {
                IList<string> l = new List<string>();
                l.Add(correct_answer);
                foreach (string s in incorrect_answers)
                {
                    l.Add(s);
                }

                if (type == "multiple")
                {
                    l = l.OrderBy(answ => answ).ToList();
                }
                else if (type == "boolean")
                {
                    l = l.OrderByDescending(answ => answ).ToList();
                }

                return l;
            }
        }
    }
}
