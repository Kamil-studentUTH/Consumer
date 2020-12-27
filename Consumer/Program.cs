using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Witajcie w aplikacji, która odbiera wiadomości!");

            ConnectionFactory factory = new ConnectionFactory();
            Uri uriTest = new Uri("amqps://pmyandsd:IWwasm9e8QSF1lqMP8pbq4LMrKmq_pLj@sparrow.rmq.cloudamqp.com/pmyandsd");
            factory.Uri = uriTest;

            IConnection connection = factory.CreateConnection();

            IModel channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: "cars", type: "fanout");
            // tworzenie nietrwałej, wyłącznej, automatycznej kolejki z wygenerowaną nazwą
            var queueName = channel.QueueDeclare().QueueName;
            // exchange i queue zostały utwrzone. Musimy teraz powiadomić 'exchange', aby wysyłała wiadomości do naszej kolejki
            // jest to proste wiązanie pomiędzy 'exchange' a 'queue'
            channel.QueueBind(queue: queueName,
                              exchange: "cars",
                              routingKey: "");
            Console.WriteLine(" [x] Oczekiwanie na wiadomości!");
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body.Span);
                Console.WriteLine($" [x] otrzymano {message}");
            };
            channel.BasicConsume(queue: queueName,
                autoAck: true,
                consumer: consumer);
            Console.WriteLine("Wciśnij [Enter], aby wyłączyć aplikację");
            Console.ReadLine();
        }
    }
}
