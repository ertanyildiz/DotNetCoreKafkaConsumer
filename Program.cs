using System;
using System.Threading;
using Confluent.Kafka;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
namespace KafkaDotNet
{
    class Program
    {
        private static readonly long nanoSecond = 1000000;
        public static void Main(string[] args)
        {
            //Consumer config parameters
            var conf = new ConsumerConfig
            {
                GroupId = "test-consumer",
                BootstrapServers = "localhost:9092",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using (var c = new ConsumerBuilder<Ignore, string>(conf).Build())
            {
                c.Subscribe("test");

                CancellationTokenSource cts = new CancellationTokenSource();
                Console.CancelKeyPress += (_, e) =>
                {
                    e.Cancel = true; // prevent the process from terminating.
                    cts.Cancel();
                };

                try
                {
                    while (true)
                    {
                        try
                        {
                            Console.WriteLine("Listening...");
                            //Poll for new messages
                            var cr = c.Consume(cts.Token);

                            if (string.IsNullOrEmpty(cr.Value.ToString())) continue;
                            //validate message if it is Json or not                            
                            var validJson = ValidateJson(cr.Value.ToString());

                            if (!validJson)
                            {
                                Console.WriteLine("Invalid Json message!");
                                continue;
                            }

                            var obj = JObject.Parse(cr.Value.ToString());
                            var tsObject = (JObject)obj["login"];
                           
                            //update ts value if exists in the Json message
                            if (tsObject["ts"] != null)
                            {
                                //update ts milisecond to nanosecond
                                tsObject["ts"] = (long)tsObject["ts"] * nanoSecond;
                            }

                            //Output to screen
                            Console.WriteLine(obj.ToString());
                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine($"Error occured: {e.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    //cancelled, close consumer
                    c.Close();
                }
            }
        }
        /// <summary>
        /// Validate JSon message coming from Kafka.
        /// </summary>
        /// <param name="jsonStr">JSon Message</param>
        /// <returns>bool</returns>
        private static bool ValidateJson(string jsonStr)
        {
            try
            {
                var schema = JSchema.Parse("{}");
                var jObject = JObject.Parse(jsonStr);
                return jObject.IsValid(schema);
            }
            catch (JsonReaderException)
            {
                return false;
            }
        }
    }
}
