using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

ConnectionFactory factory = new ConnectionFactory
{
    Uri = new Uri("amqp://guest:guest@localhost:5672"),
    ClientProvidedName = "Rabbit Receiver1 App"
};

IConnection connection = factory.CreateConnection();

IModel channel = connection.CreateModel();

string exchangeName = "demo.exchange";
string routingKey = "demo.key";
string queueName = "demo.queue";

channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
channel.QueueDeclare(queueName, false, false, false, null);
channel.QueueBind(queueName, exchangeName, routingKey, null);

channel.BasicQos(0, 1, false);

EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    Task.Delay(5000).Wait(); // delay increased to 5 seconds
    byte[] body = ea.Body.ToArray();
    string message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Received message: {message}");

    channel.BasicAck(ea.DeliveryTag, false);
};

string consumerTag = channel.BasicConsume(queueName, false, consumer);
Console.ReadLine();

channel.BasicCancel(consumerTag);
channel.Close();
connection.Close();
