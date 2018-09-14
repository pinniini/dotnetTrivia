using System.Collections.Generic;

namespace dotnetTrivia.models
{
    public class QuestionList
    {
        public int response_code {get;set;}
        public IEnumerable<Question> results {get;set;}
    }
}
