using BraceletsGenerator.Entities;

Random Random = new Random();
List<Bracelets> bracelets = new List<Bracelets>();

/*
 *  public Guid SerialNumber;

        public string Username;

        public string Color;
 */
// TODO Array di Colori
//rosso blu giallo verde nero



for (int i = 0; i < 10; i++)
{
    bracelets.Add(new Bracelets() { SerialNumber = Guid.NewGuid(), Color = "red", Username = "" });
    
}
//TODO metodo send per mandare i bracialetti creati sul DB Relazionale
foreach (var b in bracelets)
{
    //Send();
    Console.WriteLine(b.Username);
}
//POST DA IMPLEMENTARE
//public void Send()
//{

//    //Creazione httpRequest per riprendere l'id dell'ultimo bracialetto
//    HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(string.Format("https://hepj2fzca6.execute-api.eu-west-1.amazonaws.com/api/Bracelets"));

//    WebReq.Method = "GET";

//    HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();

//    Console.WriteLine(WebResp.StatusCode + " from Relation DB");


//    string jsonString;
//    using (Stream stream = WebResp.GetResponseStream())
//    {
//        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
//        jsonString = reader.ReadToEnd();
//    }
//  
//}

