using System.Text;
using RabbitMQ.Client;



var hostName = Environment.GetEnvironmentVariable("RABBIT_HOSTNAME") ?? "localhost";
//RabbitMQ bağlantısı için
var factory = new ConnectionFactory() { HostName = hostName, UserName = "guest", Password = "guest" };

//TODO: DİREKT EXCHANGE
//LOGTYPE
//     Critical = 1,
//     Error = 2,
//     Warning = 3,
//     Info = 4
using (var connection = factory.CreateConnection())
{
    var channel = connection.CreateModel();
    
    channel.ExchangeDeclare("logs-direkt",durable:true,type:ExchangeType.Direct);

    for (int i = 1; i <= 4; i++)
    {
        var routeKey = $"route-{i}";
        var queueName = $"direct-queue-{i}";
        channel.QueueDeclare(queueName, true, false, false, null);
        
        //queueyi exhange'ye bind ediyoruz...
        channel.QueueBind(queueName,"logs-direkt",routeKey);
    }
    
    
    foreach (var item in Enumerable.Range(1,50).ToList())
    {
        var random =new Random().Next(1, 4);
        string message = $"log type {random}";
        var messageBody = Encoding.UTF8.GetBytes(message);

        var routeKey = $"route-{random}";
        
        channel.BasicPublish("logs-direkt",routeKey,null,messageBody);
        
        Console.WriteLine($"Mesaj Gönderildi {item}");
    }
    
    Console.ReadLine();
}


//todo:FANOUT EXCHANGE
// using (var connection = factory.CreateConnection())
// {
//     var channel = connection.CreateModel();
//     
//     channel.ExchangeDeclare("logs-fanout",durable:true,type:ExchangeType.Fanout);
//     
//     foreach (var item in Enumerable.Range(1,50).ToList())
//     {
//         string message = $"log {item}";
//         var messageBody = Encoding.UTF8.GetBytes(message);
//         
//         channel.BasicPublish("logs-fanout","",null,messageBody);
//         
//         Console.WriteLine($"Mesaj Gönderildi {item}");
//     }
//     
//     Console.ReadLine();
// }


//TODO: DEFAULT EXCHANGE
// using (var connection = factory.CreateConnection())
// {
//     var channel = connection.CreateModel();
//     //durable: verileri memoriye mi yoksa harddiske mi hazsın. true: harddisk, false:memory.. not: memory dersek rabbitmq kendini restart ederse silinir verileri.
//     //exclusive : başka yerden bağlantı sağlanmaya izin verilip verilmemesi demektir. consumer'de olacağı için false dedik.
//     //autoDelete: tüm consumerler kopunca kuyruk silinsin mi kontrolü yapar. false: silinmesin demek
//     channel.QueueDeclare("hello-queue",true,false,false);
//
//     foreach (var item in Enumerable.Range(1,50).ToList())
//     {
//         string message = $"hello word {item}";
//         var messageBody = Encoding.UTF8.GetBytes(message);
//         channel.BasicPublish(string.Empty,"hello-queue",null,messageBody);
//         Console.WriteLine($"Mesaj Gönderildi {item}");
//     }
//     
//     Console.ReadLine();
// }