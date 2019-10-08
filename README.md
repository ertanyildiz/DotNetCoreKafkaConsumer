# DotNetCoreKafkaConsumer
.NET Core Console Application that polls messages from Apache Kafka

### Prerequisite

- Running Apache Kafka Server
- An Apache Kafa Topic name "test"
- Linux .NET Core SDK (if you want to test it on Linux [Download Link](http://https://dotnet.microsoft.com/download "Download Link"))


### Testing

- Send message on Apache Kafka with a json file
sample json file:

	 ```json{"login":{"name": "a_name","ts": 1570519030665, "attributes":{"os": "linux", "geo": "tr","age": "25"}}}```
- if message is not a valid Json then it will raise an error

- if you want to test it on Linux then Copy **netcoreapp3.0** folder to your computer, go to folder on your terminal and then run the command 

	> `dotnet DotNetCoreKafkaConsumer.dll`
