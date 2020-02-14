using System.Net.Http;
using System;
using McMaster.Extensions.CommandLineUtils;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace REST_tutorial
{

    public class Todo : Requests{
        public int id { get; set; }
    }

    public class Tasks{
        public List<Todo> todo{get; set;}
    }

    public class Requests{
        public string task { get; set; }
        public bool done { get; set; }
    }

    [HelpOption("--hlp")]
    [Subcommand(
        typeof(GetToDo),
        typeof(PostToDo),
        typeof(PatchToDo),
        typeof(DeleteToDo),
        typeof(DoneToDo),
        typeof(UndoneToDo),
        typeof(ClearToDo)
    )]

    class Program
    {
        public static Task<int> Main(string[] args)
        {
            return CommandLineApplication.ExecuteAsync<Program>(args);
        }
    }

    [Command(Description = "Command to Get Tasks", Name = "list")]
    class GetToDo{

        public async Task OnExecuteAsync(){
            var client = new HttpClient();

            var result = await client.GetStringAsync("http://localhost:3000/todo");

            var listTasks = JsonConvert.DeserializeObject<List<Todo>>(result);

            foreach(var i in listTasks){
                string Done = null;
                if(i.done){
                    Done = "(DONE)";
                }
                Console.WriteLine($"{i.id}. {i.task} {Done}");
            }

        }
    }
    
    [Command(Description = "Command to Post Tasks", Name = "add")]
    class PostToDo{
        [Argument(0)]
        public string text {get; set;}

        public async Task OnExecuteAsync(){
            var client = new HttpClient();
            var request = new Requests(){task = text, done = false};

            var data = new StringContent(JsonConvert.SerializeObject(request),Encoding.UTF8, "application/json");
            var result = await client.PostAsync("http://localhost:3000/todo", data);

            // Console.WriteLine(result);

        }
    }

    [Command(Description = "Command to Patch Tasks", Name = "update")]
    class PatchToDo{

        [Argument (0)]
        public string idNum {get; set;}
        [Argument (1)]
        public string text{get; set;}

        public async Task OnExecuteAsync(){
            var client = new HttpClient();
            var request = new{id = Convert.ToInt32(idNum), task = text };

            var data = new StringContent(JsonConvert.SerializeObject(request),Encoding.UTF8, "application/json");
            var result = await client.PatchAsync($"http://localhost:3000/todo/{idNum}", data);

            // Console.WriteLine(result);

        }
    }

    [Command(Description = "Command to Delete Tasks", Name = "delete")]
    class DeleteToDo{
        [Argument (0)]
        public string idNum {get; set;}

        public async Task OnExecuteAsync(){
            var client = new HttpClient();
            var request = new{id = Convert.ToInt32(idNum)};

            // var data = new StringContent(JsonConvert.SerializeObject(request),Encoding.UTF8, "application/json");
            var result = await client.DeleteAsync($"http://localhost:3000/todo/{idNum}");

            // Console.WriteLine(result);

        }
    }

    [Command(Description = "Command to Set Task to Completed", Name = "done")]
    class DoneToDo{

        [Argument (0)]
        public string idNum {get; set;}

        public async Task OnExecuteAsync(){
            var client = new HttpClient();
            var request = new{id = Convert.ToInt32(idNum), done = true };

            var data = new StringContent(JsonConvert.SerializeObject(request),Encoding.UTF8, "application/json");
            var result = await client.PatchAsync($"http://localhost:3000/todo/{idNum}", data);

            // Console.WriteLine(result);

        }
    }

    [Command(Description = "Command to Set Task to Uncompleted", Name = "undone")]
    class UndoneToDo{

        [Argument (0)]
        public string idNum {get; set;}

        public async Task OnExecuteAsync(){
            var client = new HttpClient();
            var request = new{id = Convert.ToInt32(idNum), done = false };

            var data = new StringContent(JsonConvert.SerializeObject(request),Encoding.UTF8, "application/json");
            var result = await client.PatchAsync($"http://localhost:3000/todo/{idNum}", data);

            // Console.WriteLine(result);

        }
    }

    [Command(Description = "Command to Clear all Task", Name = "clear")]
    class ClearToDo{

        public async Task OnExecuteAsync(){

            var prompt = Prompt.GetYesNo("You are about to clear all lists. Are you sure?", false, ConsoleColor.Red);

            var client = new HttpClient();

            if (prompt){
                var listTasks = await client.GetStringAsync("http://localhost:3000/todo");
                var deserial = JsonConvert.DeserializeObject<List<Todo>>(listTasks);
                var listID = new List<int>();

                foreach(var i in deserial){
                    listID.Add(i.id);
                }
                foreach(var i in listID){
                    var result = await client.DeleteAsync($"http://localhost:3000/todo/{i}");
                }
            }
        }
    }

}
