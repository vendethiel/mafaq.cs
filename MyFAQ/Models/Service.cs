using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MyFAQ.Models
{
    public class Service<T>
    {
        private const string API_URL = "http://localhost:5005/api/";
        private string Prefix => API_URL + typeof(T).Name.ToLower() + "s/"; // <- add plural

        HttpClient _client = new HttpClient();

        public List<T> All(int page = 0)
        {
            return ReadGet<List<T>>(page == 0 ? "" : "?page=" + page);
        }

        public T Get(int id)
        {
            return ReadGet<T>(id.ToString());
        }

        public T Create(T value, string[] fields, User user)
        {
            var body = new Dictionary<string, object>();
            foreach (var field in fields)
            {
                var f = typeof(T).GetProperty(field);
                body[field] = f.GetValue(value);
            }
            return ReadPostJson<T>("", body, user);
        }

        public int Count()
        {
            return ReadGet<int>("count");
        }
        public int Pages()
        {
            return ReadGet<int>("pages");
        }

        protected R ReadPost<R>(string url, Dictionary<string, string> body)
        {
            var form = new FormUrlEncodedContent(body.ToList());
            return ParseResponse<R>(_client.PostAsync(Prefix + url, form).Result);
        }
        
        protected R ReadPostJson<R>(string url, Dictionary<string, object> body, User user = null)
        {
            _client.DefaultRequestHeaders.Add("X-Token", user?.Token);
            try
            {
                var dataAsString = JsonConvert.SerializeObject(body);
                var content = new StringContent(dataAsString, System.Text.Encoding.UTF8, "application/json");
                R ret = ParseResponse<R>(_client.PostAsync(Prefix + url, content).Result);
                return ret;
            }
            finally
            {
                _client.DefaultRequestHeaders.Remove("X-Token");
            }
        }
        
        protected R ReadGet<R>(string url = "")
        {
            return ParseResponse<R>(_client.GetAsync(Prefix + url).Result);
        }

        private R ParseResponse<R>(HttpResponseMessage resp)
        {
            if (resp.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<R>(resp.Content.ReadAsStringAsync().Result);

            return default(R);
        }
    }
    
    public class AnswerService : Service<Answer>
    {
    }
    public class QuestionService : Service<Question>
    {
    }
    public class TagService : Service<Tag>
    {
        public List<Question> QuestionsByTag(string tagName)
        {
            return ReadGet<List<Question>>("tagged/" + tagName + "/questions");
        }
    }
    public class UserService : Service<User>
    {
        public User GetByToken(string token)
        {
            return ReadPost<User>("token/", new Dictionary<string, string>()
            {
                { "token", token }
            });
        }

        public string GetToken(string username, string password)
        {
            return ReadPost<String>("login", new Dictionary<string, string>()
            {
                { "username", username },
                { "password", password }
            });
        }
        public string Register(string username, string password)
        {
            return ReadPost<String>("register", new Dictionary<string, string>()
            {
                { "username", username },
                { "password", password }
            });
        }
    }
}
