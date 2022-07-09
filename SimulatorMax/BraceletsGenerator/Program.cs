using BraceletsGenerator.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;
using System.Text;


/*
 *  public Guid SerialNumber;

        public string Username;

        public string Color;
 */



var rand = new Random();
List<Bracelets> bracelets = new List<Bracelets>();
String[] arrColor = new String[5] {"red","blue","yellow","green","black"};
string colors = "";
string json = "";

for (int i = 0; i < 10; i++)
{
    colors = arrColor[rand.Next(0, arrColor.Length)];
    bracelets.Add(new Bracelets() { SerialNumber = Guid.NewGuid(), Color = colors, Username = ""});
    
}
//TODO metodo send per mandare i bracialetti creati sul DB Relazionale
foreach (var b in bracelets)
{
    //Send();
    Console.WriteLine(b.SerialNumber);
    Console.WriteLine(b.Username);
    Console.WriteLine(b.Color);
    Console.WriteLine(b.Serendipity);
}
json = JsonConvert.SerializeObject(bracelets, new JsonSerializerSettings
{
    ContractResolver = new DefaultContractResolver
    {
        NamingStrategy = new CamelCaseNamingStrategy()
    },
    Formatting = Formatting.Indented,
});
Send(json);

//POST DA IMPLEMENTARE
static void Send(string data)
{

    var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://hepj2fzca6.execute-api.eu-west-1.amazonaws.com/api/Bracelets");
    httpWebRequest.ContentType = "application/json";
    httpWebRequest.Method = "post";

    using (var streamwriter = new StreamWriter(httpWebRequest.GetRequestStream()))
    {
        streamwriter.Write(data);
        Console.WriteLine(data);
        Console.WriteLine("------Invio EFFETTUATO-----------");
    }

    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
    {
        var result = streamReader.ReadToEnd();

    }

}

