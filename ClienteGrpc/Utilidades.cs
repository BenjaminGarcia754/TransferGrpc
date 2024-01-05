using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using System;
using System.IO;
using System.Threading.Tasks;
using Grpc.Net.Client;
using TransferGrpc.Protos;

namespace ClienteGrpc
{
    internal class Utilidades
    {
        public async Task<string> subirArchivo() 
        {
            var channel = GrpcChannel.ForAddress("http://localhost:9090");
            var client = new Media.MediaClient(channel);
            var filePath = "../media/beta.mp4";
            var filePathClient = "C:/Users/52228/Downloads/POST#1030/Sunny Emmy/beta.mp4";
            byte[] dataFile = File.ReadAllBytes(filePathClient);
            string fileName = Path.GetFileNameWithoutExtension(filePathClient);
            Console.WriteLine("Tamaño: {0}", dataFile.Length);
            int offset = 0;
            string rutaAws = "";

            while (offset < dataFile.Length)
            {
                bool finalPart = false;
                int length = Math.Min(4096, dataFile.Length - offset);
                byte[] buffer = dividirArreglo(dataFile, offset, length);
                offset += length;
                if (offset == dataFile.Length)
                {
                    Console.WriteLine("Final del archivo");
                    Console.WriteLine("Se va a subir a aws");

                    finalPart = true;
                }
                else
                {
                    finalPart = false;
                }
                string base64 = Convert.ToBase64String(buffer);
                MediaRequest media = new MediaRequest();
                media.DataB64 = base64;
                media.Ruta = filePath;
                media.Name = fileName;
                media.Size = dataFile.Length;
                media.FinalPart = finalPart;
                var reply = client.TransferMediaAsync(media);
                MessageResponse responce = await reply;
                if (responce.Response)
                {
                    Console.WriteLine(responce.StringResponse);
                    if (responce.FinalPart)
                    {
                        rutaAws = responce.RutaAws;
                        Console.WriteLine(responce.RutaAws);
                        break;
                    }
                }
                else
                {
                    Console.WriteLine(responce.StringResponse);
                    break;
                }
            }
            return rutaAws;

        }

        public async Task<bool> crearCarpeta()
        {
            var channel = GrpcChannel.ForAddress("http://localhost:9090");
            var client = new Media.MediaClient(channel);
            var filePath = "C:/Users/52228/Downloads/POST#1030/Sunny Emmy/videos";
            Ruta ruta = new Ruta();
            ruta.Ruta_ = filePath;
            var reply = await client.CreatePathAsync(ruta);
            RutaResponse responce = reply;
            if (responce.Response)
            {
                Console.WriteLine(responce.StringResponse);
                return true;
            }
            else
            {
                Console.WriteLine(responce.StringResponse);
                return false;
            }


        }

        public async Task<bool> eliminarArchivo()
        {
            var channel = GrpcChannel.ForAddress("http://localhost:9090");
            var client = new Media.MediaClient(channel);
            var filePath = "C:/Users/52228/Downloads/POST#1030/Sunny Emmy/videos/xd.mp4";
            MediaEliminated media = new MediaEliminated();
            media.Route = filePath;
            var reply = await client.EliminateMediaAsync(media);

            MessageResponse responce = reply;
            if (responce.Response)
            {
                Console.WriteLine(responce.StringResponse);
                return true;
            }
            else
            {
                Console.WriteLine(responce.StringResponse);
                return false;
            }

        }

        public async Task<bool> fileExist()
        {
            var channel = GrpcChannel.ForAddress("http://localhost:9090");
            var client = new Media.MediaClient(channel);
            var filePath = "C:/Users/52228/Downloads/POST#1030/Sunny Emmy/videos/xd.mp4";
            Ruta ruta = new Ruta();
            ruta.Ruta_ = filePath;
            var reply = await client.FileExistAsync(ruta);
            RutaResponse responce = reply;
            if (responce.Response)
            {
                Console.WriteLine(responce.StringResponse);
                return true;
            }
            else
            {
                Console.WriteLine(responce.StringResponse);
                return false;
            }

        }

        public async Task<int> obtenerTamaño() 
        {
            var channel = GrpcChannel.ForAddress("http://localhost:9090");
            var client = new Media.MediaClient(channel);
            var filePath = "C:/Users/52228/Downloads/POST#1030/Sunny Emmy/videos/xd.mp4";
            Ruta ruta = new Ruta();
            ruta.Ruta_ = filePath;
            var reply = await client.GetSizeAsync(ruta);
            ChunckSize responce = reply;
            return responce.Size;
        }

        public async Task recibirArchivo()
        {
            var channel = GrpcChannel.ForAddress("http://localhost:9090");
            var client = new Media.MediaClient(channel);

            var filePath = "C:/Users/52228/Downloads/POST#1030/Sunny Emmy/videos/xd.mp4";
            var filePathClient = "C:/Users/52228/Downloads/POST#1030/Sunny Emmy/alpha.mp4";
            Ruta route = new Ruta();
            route.Ruta_ = filePath;
            int size = await obtenerTamaño();

            int chunkSize = 4096;
            int iterations = (int)Math.Ceiling((double)size / chunkSize);

            Console.WriteLine("Tamaño: {0}", size);

            using (var fileStream = File.OpenWrite(filePathClient))
            {
                for (int i = 0; i < iterations; i++)
                {
                    var chunck = new ChunckInformation
                    {
                        Route = filePath,
                        ChunckPosition = i * chunkSize
                    };
                    var call2 = client.GetMediaAsync(chunck);
                    var response2 = await call2.ResponseAsync;
                    string base64 = response2.DataB64;
                    byte[] data = Convert.FromBase64String(base64);
                    await fileStream.WriteAsync(data, 0, data.Length);

                    Console.WriteLine($"Procesando... ({i + 1}/{iterations})");
                }
                Console.WriteLine("El archivo se ha transmitido con éxito.");
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
