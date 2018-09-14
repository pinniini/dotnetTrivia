using System;
using System.Collections.Generic;
using System.Linq;
using dotnetTrivia.models;

namespace dotnetTrivia
{
    public class GameEngine
    {
        private TriviaDownloader _downloader;
        private CategoryList _categoryList;
        private int _correctCount;
        private int _incorrectCount;

        public GameEngine()
        {
            _downloader = new TriviaDownloader();
            _correctCount = 0;
            _incorrectCount = 0;
        }

        public void RunGame()
        {
            RunGame(new List<string>());
        }

        public void RunGame(IEnumerable<string> parameters)
        {
            string command = string.Empty;
            bool cmdParams = (parameters != null && parameters.Count() > 0);

            Console.WriteLine("Downloading categories...");
            _categoryList = _downloader.LoadCategories();
            Console.WriteLine($"Categories downloaded...{Environment.NewLine}");
            Console.WriteLine("Type help to get started.");

            do
            {
                Console.Write("> ");

                // Command line parameters given.
                if (cmdParams)
                {
                    command = String.Join(" ", parameters);
                    Console.WriteLine(command);
                    cmdParams = false;
                }
                else // Normal stuff.
                {
                    command = Console.ReadLine();
                }

                if (!RunCommand(command))
                {
                    break;
                }
            }
            while (true);

            Console.WriteLine("Bye bye!");
        }

        ///
        /// Returns false, if the application should quit.
        ///
        private bool RunCommand(string command)
        {
            bool retVal = true;

            string[] pieces = command.Split(" ");

            if (pieces.Length == 0)
            {
                Console.WriteLine("No command given.");
                return retVal;
            }

            switch (pieces[0])
            {
                case "quit":
                case "exit":
                    retVal = false;
                    break;
                case "help":
                    PrintHelp();
                    break;
                case "categories":
                    PrintCategories();
                    break;
                case "start":
                case "--start":
                    StartGame(pieces.TakeLast(pieces.Length - 1));
                    break;
                case "log":
                    Logger.PrintLog();
                    break;
                default:
                    Console.WriteLine("Unknown command.");
                    break;
            }

            return retVal;
        }

        private void PrintHelp()
        {
            Console.WriteLine("");
            Console.WriteLine("To start the trivia, use the command \"start c=-1 d=all q=10\"");
            Console.WriteLine("These are the default values if you just type start.");
            Console.WriteLine(" c switch is the category id, -1 means all");
            Console.WriteLine(" d is for difficulty, possible values: all, easy, medium, hard");
            Console.WriteLine(" q is for the question count");
            Console.WriteLine("");
            Console.WriteLine("Use the command categories to print all the possible categories.");
        }

        private void PrintCategories()
        {
            int catLength = _categoryList.trivia_categories.Count();
            foreach (Category cat in _categoryList.trivia_categories)
            {
                Console.WriteLine("{0}, {1}", cat.id, cat.name);
            }
        }

        private void StartGame(IEnumerable<string> parameters)
        {
            QuestionList questions = GetQuestions(parameters);
            if (questions != null)
            {
                RunTrivia(questions);
            }
        }

        private QuestionList GetQuestions(IEnumerable<string> parameters)
        {
            string url = "https://opentdb.com/api.php?";
            // Just start
            if (parameters.Count() < 1)
            {
                url += "amount=10";
            }
            else // At least one parameter present, need to investigate more.
            {
                // Category parameter.
                string category = parameters.Where(p => p.StartsWith("c=") || p.StartsWith("-c=")).FirstOrDefault();
                if (category != null)
                {
                    // Category number given and other than all (-1)
                    int catNum = -2;
                    if (int.TryParse(category.Substring(2, category.Length - 2), out catNum) && catNum > -1)
                    {
                        url += $"category={catNum}&";
                    }
                    else
                    {
                        Logger.Log("Invalid category: " + category.Substring(2, category.Length - 2));
                    }
                }

                // Question count.
                string count = parameters.Where(p => p.StartsWith("q=") || p.StartsWith("-q=")).FirstOrDefault();
                if (count != null)
                {
                    // Question count grater than zero and less or equal to 50.
                    int qCount = -1;
                    if (int.TryParse(count.Substring(2, count.Length - 2), out qCount) && qCount > 0 && qCount <= 50)
                    {
                        url += $"amount={qCount}&";
                    }
                    else
                    {
                        Logger.Log("Invalid question count: " + count.Substring(2, count.Length - 2));
                    }
                }
                else
                {
                    url += "amount=10";
                }

                // Difficulty.
                string difficulty = parameters.Where(p => p.StartsWith("d=") || p.StartsWith("-d=")).FirstOrDefault();
                if (difficulty != null)
                {
                    string diff = difficulty.Substring(2, difficulty.Length - 2);
                    if (diff != null && (diff == "easy" || diff == "medium" || diff == "hard"))
                    {
                        url += $"difficulty={diff}";
                    }
                    else
                    {
                        Logger.Log("Invalid difficulty: " + diff);
                    }
                }
            }

            // Clean url.
            if (url.EndsWith("&"))
            {
                url = url.Substring(0, url.Length - 1);
            }

            // Just in case
            if (url.EndsWith("?"))
            {
                Logger.Log($"Invalid URL: {url}");
                Console.WriteLine("Invalid trivia url. See the logs for more information.");
                return null;
            }

            Logger.Log($"Query URL: {url}");

            Console.WriteLine("Loading questions...");
            QuestionList questions = _downloader.LoadQuestions(url);
            Console.WriteLine("Questions loaded...");

            Logger.Log("");

            return questions;
        }

        private void RunTrivia(QuestionList questions)
        {
            _correctCount = 0;
            _incorrectCount = 0;
            bool triviaAborted = false;

            int questionCount = questions.results.Count();
            for (int i = 0; i < questionCount; ++i)
            {
                Question q = questions.results.ElementAt(i);
                if (q != null)
                {
                    if (AskQuestion(q, i + 1, questionCount, ref triviaAborted))
                    {
                        ++_correctCount;
                    }

                    // If trivia is aborted, then the answer is incorrect.
                    else
                    {
                        // Go away.
                        if (triviaAborted)
                        {
                            break;
                        }
                        else
                        {
                            ++_incorrectCount;
                        }
                    }
                }
            }

            Console.WriteLine("");
            Console.WriteLine("####################################");
            Console.WriteLine("Results:");
            Console.WriteLine($"Correct: {_correctCount}");
            Console.WriteLine($"Incorrect: {_incorrectCount}");
            Console.WriteLine("####################################");
            Console.WriteLine("");
            Console.WriteLine("");
        }

        ///
        /// Returns true if user answered correctly, false otherwise.
        ///
        private bool AskQuestion(Question question, int questionNumber, int questionCount, ref bool abortTrivia)
        {
            bool correctAnswer = false;
            abortTrivia = false;

            Console.WriteLine("");
            Console.WriteLine($"Category: {question.category}");
            Console.WriteLine($"Difficulty: {question.difficulty}");
            Console.WriteLine($"Question {questionNumber}/{questionCount}: {System.Web.HttpUtility.HtmlDecode(question.question)}");

            for (int j = 0; j < question.Answers.Count(); ++j)
            {
                Console.WriteLine($"{j + 1} - {System.Web.HttpUtility.HtmlDecode(question.Answers.ElementAt(j))}");
            }

            string answ = string.Empty;
            while(true)
            {
                answ = string.Empty;
                Console.Write("> ");
                answ = Console.ReadLine();

                // Number as an answer.
                if (int.TryParse(answ, out int num) && num > 0 && num <= question.Answers.Count())
                {
                    if (question.Answers.ElementAt(num - 1) == question.correct_answer)
                    {
                        Console.WriteLine("Correct! :)");
                        correctAnswer = true;
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Incorrect! :(");
                        Console.WriteLine($"Correct answer is: {System.Web.HttpUtility.HtmlDecode(question.correct_answer)}");
                        correctAnswer = false;
                        break;
                    }
                }

                // Only "break" is allowed.
                else if (answ == "break")
                {
                    Console.WriteLine("Trivia aborted. :(");
                    abortTrivia = true;
                    correctAnswer = false;
                    break;
                }
            }

            System.Threading.Thread.Sleep(1000);
            return correctAnswer;
        }
    }
}
