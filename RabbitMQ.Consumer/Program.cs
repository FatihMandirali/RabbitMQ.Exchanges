using System.Runtime.Loader;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

//guest:guest -> username:password
//amqp://guest:guest@localhost:5672
var hostName = Environment.GetEnvironmentVariable("RABBIT_HOSTNAME") ?? "localhost";
//RabbitMQ bağlantısı için
var factory = new ConnectionFactory() { HostName = hostName, UserName = "guest", Password = "guest" };


//TODO:DİREKT EXCHANGE
using (var connection = factory.CreateConnection())
{
    var channel = connection.CreateModel();
    
//LOGTYPE
//     Critical = 1,
//     Error = 2,
//     Warning = 3,
//     Info = 4    
    
    //prefetchsize : 0 demek mesaj boyutu fark etmeksizin al veriyi.
    //prefetchCount: aynanda kaç tane mesaj alsın.
    //global : false demek tüm consumerlara dağıtma sadece kendine al. true ise prefetchCount değerini diğer consumer'lara ortak dağıt.
    channel.BasicQos(0,1,false);
    var consumer = new EventingBasicConsumer(channel);
    var queueName = "direct-queue-1";
    //autoack - true: mesaj gönderildekten sonra mesajı kuyruktan sil. işlem başarılı olup olmamasına bakmaz direkt siler. true olması önerilmez.
    channel.BasicConsume(queueName,false,consumer);

    consumer.Received += (object sender, BasicDeliverEventArgs e) =>
    {
        var message = Encoding.UTF8.GetString(e.Body.ToArray());
        Console.WriteLine("gelen mesaj : "+message);
        
        channel.BasicAck(e.DeliveryTag,false); //rabbitmq tarafına mesaj alındı kuyruktan silebilirsin talimatı gönderdik.
    };    
    
    Console.ReadLine();
}

//TODO: FANOUT EXHANGE
// using (var connection = factory.CreateConnection())
// {
//     var channel = connection.CreateModel();
//     
//     //channel.ExchangeDeclare("logs-fanout",durable:true,type:ExchangeType.Fanout);
//
//     var randomQueueName = channel.QueueDeclare().QueueName;
//     
//     //QUEUEBİND işlemi kuyruk oluşturur ve bağlantı bitince kuyruğu siler.
//     channel.QueueBind(randomQueueName,"logs-fanout","",null);
//
//     //prefetchsize : 0 demek mesaj boyutu fark etmeksizin al veriyi.
//     //prefetchCount: aynanda kaç tane mesaj alsın.
//     //global : false demek tüm consumerlara dağıtma sadece kendine al. true ise prefetchCount değerini diğer consumer'lara ortak dağıt.
//     channel.BasicQos(0,1,false);
//     var consumer = new EventingBasicConsumer(channel);
//
//     //autoack - true: mesaj gönderildekten sonra mesajı kuyruktan sil. işlem başarılı olup olmamasına bakmaz direkt siler. true olması önerilmez.
//     channel.BasicConsume(randomQueueName,false,consumer);
//
//     consumer.Received += (object sender, BasicDeliverEventArgs e) =>
//     {
//         var message = Encoding.UTF8.GetString(e.Body.ToArray());
//         Console.WriteLine("gelen mesaj : "+message);
//         
//         channel.BasicAck(e.DeliveryTag,false); //rabbitmq tarafına mesaj alındı kuyruktan silebilirsin talimatı gönderdik.
//     };    
//     
//     Console.ReadLine();
// }


//TODO: DEFAULT EXHANGE
// using (var connection = factory.CreateConnection())
// {
//     var channel = connection.CreateModel();
//     //durable: verileri memoriye mi yoksa harddiske mi hazsın. true: harddisk, false:memory.. not: memory dersek rabbitmq kendini restart ederse silinir verileri.
//     //exclusive : başka yerden bağlantı sağlanmaya izin verilip verilmemesi demektir. consumer'de olacağı için false dedik.
//     //autoDelete: tüm consumerler kopunca kuyruk silinsin mi kontrolü yapar. false: silinmesin demek
//     //QueueDeclare kuyruk oluşturur ve bağlantı kopsada kuyruk silinmez..
//     channel.QueueDeclare("hello-queue",true,false,false);
//     //Kuyruk oluşturmak producer veya consumer tarafında da yapılabilir. eğer consumer tarafına eklemezsek ve producer'da bu kuyruğu oluşturmamış ise hata alınır
//
//     //prefetchsize : 0 demek mesaj boyutu fark etmeksizin al veriyi.
//     //prefetchCount: aynanda kaç tane mesaj alsın.
//     //global : false demek tüm consumerlara dağıtma sadece kendine al. true ise prefetchCount değerini diğer consumer'lara ortak dağıt.
//     channel.BasicQos(0,1,false);
//     var consumer = new EventingBasicConsumer(channel);
//
//     //autoack - true: mesaj gönderildekten sonra mesajı kuyruktan sil. işlem başarılı olup olmamasına bakmaz direkt siler. true olması önerilmez.
//     channel.BasicConsume("hello-queue",false,consumer);
//
//     consumer.Received += (object sender, BasicDeliverEventArgs e) =>
//     {
//         var message = Encoding.UTF8.GetString(e.Body.ToArray());
//         Console.WriteLine("gelen mesaj : "+message);
//         
//         channel.BasicAck(e.DeliveryTag,false); //rabbitmq tarafına mesaj alındı kuyruktan silebilirsin talimatı gönderdik.
//     };    
//     
//     Console.ReadLine();
// }