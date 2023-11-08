using Google.Protobuf;
using Grpc.Net.Client;
using TransferGrpc.Protos;

namespace ClienteGrpc
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var channel = GrpcChannel.ForAddress("http://localhost:5053");
            var client = new Media.MediaClient(channel);
            var filePath = "C:/Users/52228/Downloads/POST#1030/Sunny Emmy/ElsaRecortado/Elsa.mp4";
            byte[] dataFile = File.ReadAllBytes(filePath);
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            Console.WriteLine("Tamaño: {0}", dataFile.Length);
            int offset = 0;
            /*
            var call = client.SendMedia();

            await call.RequestStream.WriteAsync(new MediaSend
            {
                FilePath = filePath,
                VideoPart = ByteString.CopyFromUtf8(dataFile.Length.ToString()),
                FinalPart = false
            });
            using (var fileStream = File.OpenRead(filePath))
            {
                byte[] buffer = new byte[4096];
                int bytesRead;
                //bool firstMessage = true;

                while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    var mediaSend = new MediaSend
                    {
                        VideoPart = ByteString.CopyFrom(buffer, 0, bytesRead),
                        FinalPart = fileStream.Position == fileStream.Length
                    };

                    await call.RequestStream.WriteAsync(mediaSend);
                }
                await call.RequestStream.CompleteAsync();
            }

            var response = await call.ResponseAsync;
            if (response.Response)
            {
                Console.WriteLine("El archivo se ha transmitido con éxito.");
            }
            else
            {
                Console.WriteLine("Hubo un error al transmitir el archivo: " + response.Message);
            }

            await channel.ShutdownAsync();
            Console.WriteLine("Presiona cualquier tecla para salir...");
            Console.ReadKey();
        }*/

            while (offset < dataFile.Length)
            {
                int length = Math.Min(4096, dataFile.Length - offset);
                byte[] buffer = dividirArreglo(dataFile, offset, length);
                offset += length;

                string base64 = Convert.ToBase64String(buffer);
                MediaRequest media = new MediaRequest();
                media.DataB64 = base64;
                media.Name = fileName;
                var reply = client.TransferMediaAsync(media);
                MessageResponse responce = await reply;

                if (responce.Response)
                    Console.WriteLine(responce.StringResponse);
                else
                    Console.WriteLine(responce.StringResponse);
            }

        }


        public static byte[] dividirArreglo(byte[] arregloReal, int offset, int length)
        {
            byte[] arregloTemporal = new byte[4096];
            Array.Copy(arregloReal, offset, arregloTemporal, 0, length);
            return arregloTemporal;
        }
    }

}
