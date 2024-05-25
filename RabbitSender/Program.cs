using RabbitMQ.Client;
using System.Text;


ConnectionFactory factory = new ConnectionFactory
{
    Uri = new Uri("amqp://guest:guest@localhost:5672"),
    ClientProvidedName = "Rabbit Sender App"
};

IConnection connection = factory.CreateConnection();

IModel channel = connection.CreateModel();

string exchangeName = "demo.exchange";
string routingKey = "demo.key";
string queueName = "demo.queue";

channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
channel.QueueDeclare(queueName, false, false, false, null);
channel.QueueBind(queueName, exchangeName, routingKey, null);

for (int i = 0; i < 60; i++)
{
    Console.WriteLine($"Escrevendo mensagem de número: {i}");
    byte[] messageBodyBytes = Encoding.UTF8.GetBytes($"Hello, RabbitMQ {i}!");
    channel.BasicPublish(exchangeName, routingKey, null, messageBodyBytes);
}

channel.Close();
connection.Close();
 
