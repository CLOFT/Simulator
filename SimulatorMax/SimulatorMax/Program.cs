using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Newtonsoft.Json;
using System.Net;
using CLOFT.SerenUp.Simulator;
using CLOFT.SerenUp.Simulator.Entities;



//Simulator MAX
//Variabili per i casi

int counter = 100;
int chase = 0;

var rand = new Random();
int res = 0;

int compared = 1;
int status = 0;
int positions = 0;

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
message.Battery = 23;
message.Accelerometer.Xaxis = 0;
message.Accelerometer.Yaxis = -1;
message.Accelerometer.Zaxis = 0;
message.Steps = rand.Next(3000, 5000);



//Tempo di attesa fra un'esecuzione e l'altra
var timer = new PeriodicTimer(TimeSpan.FromSeconds(5));
//
//MAIN
if (chase == 0)
{
    DateTimeOffset now = DateTimeOffset.UtcNow;
    string currentTimeString = (now.ToUnixTimeMilliseconds()).ToString();
    message.Time = currentTimeString;
    message.Status.stop = true;
    message.Status.walk = false;
    message.Status.run = false;

    //HEARTBEAT
    message.Heartbeat = rand.Next(60, 80);

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
    Console.WriteLine("Position "+ message.Position);
    Console.WriteLine("Frequenza del battito cardiaco " + message.Heartbeat);
    Console.WriteLine("Pressione arteriosa sistolica " + message.BloodPressure.SystolicPressure);
    Console.WriteLine("Pressione arteriosa diastolica " + message.BloodPressure.DiastolicPressure);
    Console.WriteLine("saturazione del sangue " + message.OxygenSaturation + "%");
    Console.WriteLine("Battery status " + message.Battery + "%");
    json = JsonConvert.SerializeObject(message);
    Console.WriteLine(json);
    Send(json); //Invio dati all'API Gateway
    message.Battery--;
    counter--;
    chase++
   ;

    Console.WriteLine("----------------------END FIRST CASE ------------------------");
}
//SECOND + N RUN
if (chase > 0)
{
    while (await timer.WaitForNextTickAsync())
    {
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
            Console.WriteLine ("END_BATTERY");
            break;
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
            message.Heartbeat = message.Heartbeat - 5;

            message.OxygenSaturation = rand.Next(95, 100);

            message.BloodPressure.SystolicPressure = rand.Next(100, 141);

            message.BloodPressure.DiastolicPressure = rand.Next(60, 101);



        }
        else if (message.Status.walk == true)
        {
            Console.WriteLine("Variazione per il WALK");
            message.Steps = message.Steps + 300;

            message.Heartbeat = rand.Next(85, 96);

            message.OxygenSaturation = rand.Next(95, 100);

            message.BloodPressure.SystolicPressure = rand.Next(100, 141);

            message.BloodPressure.DiastolicPressure = rand.Next(60, 101);

            message.Accelerometer.Yaxis = rand.Next(-1, 2);
            message.Accelerometer.Xaxis = rand.Next(0, 2);
            message.Accelerometer.Zaxis = rand.Next(0,2);

            int fall = rand.Next(0, 6);
            Console.WriteLine("Probabilità su 5 di cadere "+ fall);
            if(fall == 5)
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

            message.Heartbeat = rand.Next(100, 111);

            message.OxygenSaturation = rand.Next(95, 100);

            message.BloodPressure.SystolicPressure = rand.Next(100, 141);

            message.BloodPressure.DiastolicPressure = rand.Next(60, 101);

            message.Accelerometer.Yaxis = rand.Next(-2, 3);
            message.Accelerometer.Xaxis = rand.Next(-1, 3);
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
        Console.WriteLine("Frequenza del battito cardiaco " + message.Heartbeat);
        Console.WriteLine("Pressione arteriosa sistolica " + message.BloodPressure.SystolicPressure);
        Console.WriteLine("Pressione arteriosa diastolica " + message.BloodPressure.DiastolicPressure);
        Console.WriteLine("saturazione del sangue " + message.OxygenSaturation + "%");
        Console.WriteLine("Battery status " + message.Battery + "%");
        json = JsonConvert.SerializeObject(message);
        Console.WriteLine(json);
        Send(json); //Invio dati all'API Gateway
        
        message.Battery--;
        Console.WriteLine("----------------END CASE (2+) -------------------------");
    }
}

static int Fall(int x, int y, int z)
{
    //Fissa momentanemante
 
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
        Console.WriteLine("------prova invio all'api gateway-----------");
        streamwriter.Write(data);
        Console.WriteLine(data);
    }

    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
    {
        var result = streamReader.ReadToEnd();

    }
}