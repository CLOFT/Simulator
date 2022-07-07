using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Newtonsoft.Json;
using System.Net;
using CLOFT.SerenUp.Simulator;
using CLOFT.SerenUp.Simulator.Entities;
using Newtonsoft.Json.Serialization;

//Simulator MAX
//await timer.WaitForNextTickAsync()
var timer = new PeriodicTimer(TimeSpan.FromSeconds(5));

while (await timer.WaitForNextTickAsync())
{
    Generator();
}
static async void Generator()
{

    //Creazione httpRequest per riprendere l'id dell'ultimo bracialetto
    HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(string.Format("https://hepj2fzca6.execute-api.eu-west-1.amazonaws.com/api/Bracelets"));

    WebReq.Method = "GET";

    HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();

    Console.WriteLine(WebResp.StatusCode + " from Relation DB");


    string jsonString;
    using (Stream stream = WebResp.GetResponseStream())
    {
        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
        jsonString = reader.ReadToEnd();
    }
    var bracelets = JsonConvert.DeserializeObject<List<Bracelets>>(jsonString);
    bracelets = bracelets.FindAll(b => b.Username != null);
    //Per ogni bracialetto dentro il db relazionale recupero i dati 
    foreach (var b in bracelets)
    {

        Guid id = b.SerialNumber;
        //Get dal db timestream per il recupero di tutti i dati dei bracialetti
        //8d547248-9cfb-480d-86bd-f572e67da86f
        HttpWebRequest WebReq2 = (HttpWebRequest)WebRequest.Create(string.Format("https://hepj2fzca6.execute-api.eu-west-1.amazonaws.com/api/BraceletsData/8d547248-9cfb-480d-86bd-f572e67da86f"));

        WebReq2.Method = "GET";

        HttpWebResponse WebResp2 = (HttpWebResponse)WebReq2.GetResponse();

        Console.WriteLine(WebResp2.StatusCode + " from Timestream");


        string StringJson;
        using (Stream stream = WebResp2.GetResponseStream())
        {
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            StringJson = reader.ReadToEnd();
        }
        //verifica se dentro a timestream sono presenti bracialetti
        //Se non ci sono li genera se presente almeno un bracialetto prende i dati e li modifica per poi rimandarli

        if (StringJson == "[]")
        {
            //Metodo per la generazione dei dati se mancanti
            Console.WriteLine("Dati mancanti --> Generazione");
            FirstGenerate();
        }
        else
        {
            //Se presenti i dati creo l'oggetto contenente i dati
            var braceletsData = JsonConvert.DeserializeObject<List<Message>>(StringJson);
            //Ciclo per la modifica dei dati per ogni bracialetto
            Console.WriteLine("Dati presenti --> Modifica");
            foreach (var n in braceletsData)
            {
                SecondGeneration(n);
            }
        }

    }

}
static void FirstGenerate()
{
    //Variabile per il random
    var rand = new Random();

    string json = "";
    //CLIENT
    HttpClient client = new HttpClient();

    //DATA TO SEND
    Message message = new Message();
    message.Accelerometer = new Accelerometer();
    message.BloodPressure = new BloodPressure();
    message.Status = new Status();
    message.SerialNumber = Guid.NewGuid();

    //Data 1 run
    message.Battery = 100;
    message.Accelerometer.Xaxis = 0;
    message.Accelerometer.Yaxis = -1;
    message.Accelerometer.Zaxis = 0;
    message.Steps = rand.Next(3000, 5000);


    DateTimeOffset now = DateTimeOffset.UtcNow;
    string currentTimeString = (now.ToUnixTimeMilliseconds()).ToString();
    message.Time = currentTimeString;
    message.Status.stop = true;
    message.Status.walk = false;
    message.Status.run = false;

    //HeartBeat
    message.HeartBeat = rand.Next(60, 80);

    //SYSTOLIC PRESSURE

    message.BloodPressure.SystolicPressure = rand.Next(100, 141); //120 > buona


    //DIASTOLIC PRESSURE

    message.BloodPressure.DiastolicPressure = rand.Next(60, 101);//80 > buona


    //OXYGEN SATURATION

    message.OxygenSaturation = rand.Next(95, 100);

    //DEFAULT POSITION
    message.Position = "45.9535477,12.6839742";

    Console.WriteLine("ID " + message.SerialNumber);
    Console.WriteLine("Time:" + message.Time);
    Console.WriteLine("Steps " + message.Steps);
    Console.WriteLine("Position " + message.Position);
    Console.WriteLine("Frequenza del battito cardiaco " + message.HeartBeat);
    Console.WriteLine("Pressione arteriosa sistolica " + message.BloodPressure.SystolicPressure);
    Console.WriteLine("Pressione arteriosa diastolica " + message.BloodPressure.DiastolicPressure);
    Console.WriteLine("saturazione del sangue " + message.OxygenSaturation + "%");
    Console.WriteLine("Battery status " + message.Battery + "%");
    json = JsonConvert.SerializeObject(message, new JsonSerializerSettings
    {
        ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        },
        Formatting = Formatting.Indented,
    });
    Console.WriteLine(json);
    Send(json); //Invio dati all'API Gateway

    Console.WriteLine("----------------------END FIRST GENERATION ------------------------");
}
//SECOND + N RUN
static void SecondGeneration(Message message)
{


    message.Battery--;
    var rand = new Random();
    int res = 0;

    int compared = 1;
    int status = 0;
    int positions = 0;

    string json = "";
    //CLIENT
    HttpClient client = new HttpClient();

    //DATA TO SEND

    message.Accelerometer = new Accelerometer();
    message.Status = new Status();


    //Data  run
    message.Accelerometer.Xaxis = 0;
    message.Accelerometer.Yaxis = -1;
    message.Accelerometer.Zaxis = 0;



    if (message.Battery == 20 || message.Battery == 15 || message.Battery == 10 || message.Battery == 5)
    {
        Console.WriteLine("Batteria in via di esaurimento");
        message.Alarm = "LOW_BATTERY";
    }
    else
    {
        message.Alarm = null;
    }
    if (message.Battery == 0)
    {
        Console.WriteLine("END_BATTERY");
        Environment.Exit(0);
    }

    DateTimeOffset now = DateTimeOffset.UtcNow;
    string currentTimeString = (now.ToUnixTimeMilliseconds()).ToString();
    message.Time = currentTimeString;
    positions = rand.Next(0, 3);
    message.Position = Position(positions);
    status = rand.Next(0, 3);
    if (status == 0)
    {
        Console.WriteLine("STOP");
        message.Status.stop = true;
        message.Status.walk = false;
        message.Status.run = false;
    }
    else if (status == 1)
    {
        Console.WriteLine("WALK");
        message.Status.stop = false;
        message.Status.walk = true;
        message.Status.run = false;
    }
    else
    {
        Console.WriteLine("RUN");
        message.Status.stop = false;
        message.Status.walk = false;
        message.Status.run = true;
    }

    if (message.Status.stop == true)
    {
        Console.WriteLine("Variazione per lo STOP");
        message.HeartBeat = message.HeartBeat - 5;

        message.OxygenSaturation = rand.Next(95, 100);

        message.BloodPressure.SystolicPressure = rand.Next(109, 130);

        message.BloodPressure.DiastolicPressure = rand.Next(70, 86);

        message.Accelerometer.Xaxis = 0;

        message.Accelerometer.Yaxis = -1;

        message.Accelerometer.Zaxis = 0;



    }
    else if (message.Status.walk == true)
    {
        Console.WriteLine("Variazione per il WALK");
        message.Steps = message.Steps + 300;

        message.HeartBeat = rand.Next(85, 96);

        message.OxygenSaturation = rand.Next(95, 100);

        message.BloodPressure.SystolicPressure = rand.Next(109, 130);

        message.BloodPressure.DiastolicPressure = rand.Next(70, 86);

        message.Accelerometer.Yaxis = rand.Next(-1, 3);
        message.Accelerometer.Xaxis = rand.Next(-1, 3);
        message.Accelerometer.Zaxis = rand.Next(-1, 3);

        int fall = rand.Next(0, 6);
        Console.WriteLine("Probabilità su 5 di cadere " + fall);
        if (fall == 5)
        {
            res = Fall(message.Accelerometer.Xaxis, message.Accelerometer.Yaxis, message.Accelerometer.Zaxis);
            if (res > compared)
            {
                message.Alarm = "FALL";
                Console.WriteLine("caduto");
            }

        }
    }
    else if (message.Status.run == true)
    {
        Console.WriteLine("Variazione per il RUN");
        message.Steps = message.Steps + 800;

        message.HeartBeat = rand.Next(100, 111);

        message.OxygenSaturation = rand.Next(95, 100);

        message.BloodPressure.SystolicPressure = rand.Next(109, 130);

        message.BloodPressure.DiastolicPressure = rand.Next(70, 86);

        message.Accelerometer.Yaxis = rand.Next(-3, 3);
        message.Accelerometer.Xaxis = rand.Next(-2, 3);
        message.Accelerometer.Zaxis = rand.Next(-1, 3);


        int fall = rand.Next(0, 3);
        Console.WriteLine("Probabilità su 2 di cadere " + fall);
        if (fall == 2)
        {
            res = Fall(message.Accelerometer.Xaxis, message.Accelerometer.Yaxis, message.Accelerometer.Zaxis);
            if (res > compared)
            {
                message.Alarm = "FALL";
                Console.WriteLine("CADUTO");
            }

        }


    }

    Console.WriteLine("ID " + message.SerialNumber);
    Console.WriteLine("Time:" + message.Time);
    Console.WriteLine("Steps " + message.Steps);
    Console.WriteLine("Position " + message.Position);
    Console.WriteLine("Frequenza del battito cardiaco " + message.HeartBeat);
    Console.WriteLine("Pressione arteriosa sistolica " + message.BloodPressure.SystolicPressure);
    Console.WriteLine("Pressione arteriosa diastolica " + message.BloodPressure.DiastolicPressure);
    Console.WriteLine("saturazione del sangue " + message.OxygenSaturation + "%");
    Console.WriteLine("Battery status " + message.Battery + "%");
    json = JsonConvert.SerializeObject(message, new JsonSerializerSettings
    {
        ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        },
        Formatting = Formatting.Indented,
    });
    Console.WriteLine(json);
    Send(json); //Invio dati all'API Gateway


    Console.WriteLine("----------------END CASE -------------------------");

}

static int Fall(int x, int y, int z)
{


    Console.WriteLine("Asse y all'impatto " + y);
    Console.WriteLine("Asse x all'impatto " + x);
    Console.WriteLine("Asse z all'impatto " + z);
    double sum = Math.Sqrt(x * x + y * y + z * z);
    int acc = Convert.ToInt32(sum);
    Console.WriteLine("Vettore somma " + acc);
    return acc;

}

static string Position(int x)
{
    if (x == 0)
    {
        string pos = "45.9535477,12.6839742";
        return pos;
    };
    if (x == 1)
    {
        string pos = "45.9540649,12.68474";
        return pos;
    }
    else
    {
        string pos = "45.9528645,12.6703569";
        return pos;
    };

}
static void Send(string data)
{
    var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://8pde42ss4h.execute-api.eu-west-1.amazonaws.com/braceletsdata");
    httpWebRequest.ContentType = "application/json";
    httpWebRequest.Method = "post";

    using (var streamwriter = new StreamWriter(httpWebRequest.GetRequestStream()))
    {
        streamwriter.Write(data);
        Console.WriteLine("------Invio EFFETTUATO-----------");
        Console.WriteLine(data);
    }

    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
    {
        var result = streamReader.ReadToEnd();

    }
}

