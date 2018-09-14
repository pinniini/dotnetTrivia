using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using dotnetTrivia.models;

namespace dotnetTrivia
{
    public class TriviaDownloader
    {
        public TriviaDownloader()
        {

        }

        public CategoryList LoadCategories()
        {
            // Get categories.
            HttpClient client = new HttpClient();
            HttpResponseMessage response = client.GetAsync("https://opentdb.com/api_category.php").Result;
            HttpContent responseContent = response.Content;
            string json = responseContent.ReadAsStringAsync().Result;

            var categories = JsonConvert.DeserializeObject<CategoryList>(json);
            
            Logger.Log($"Category count {categories.trivia_categories.Count()}");

            return categories;
        }

        public QuestionList LoadQuestions(string url)
        {
            // Get questions.
            HttpClient client = new HttpClient();
            HttpResponseMessage response = client.GetAsync(url).Result;
            HttpContent responseContent = response.Content;
            string json = responseContent.ReadAsStringAsync().Result;

            var questions = JsonConvert.DeserializeObject<QuestionList>(json);
            
            Logger.Log($"Category count {questions.results.Count()}");

            return questions;
        }
    }
}
