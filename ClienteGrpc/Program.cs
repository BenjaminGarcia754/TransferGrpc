using Amazon.S3.Transfer;
using Amazon.S3;
using Google.Protobuf;
using Grpc.Net.Client;
using System;
using TransferGrpc.Protos;

namespace ClienteGrpc
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            string accessKey = "AKIATJRSQYEU6O5BL2WF";
            string secretKey = "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";
            string filePath = "C:/Users/52228/Downloads/POST#1030/Sunny Emmy/videoplayback.mp4";
            string bucketName = "studyandroid";
            string keyName = "pruebac#";

            try
            {
                var credentials = new Amazon.Runtime.BasicAWSCredentials(accessKey, secretKey);
                var region = Amazon.RegionEndpoint.USEast2; // Cambia a tu región

                var s3Client = new AmazonS3Client(credentials, region);

                var transferUtility = new TransferUtility(s3Client);

                await transferUtility.UploadAsync(filePath, bucketName, keyName);

                Console.WriteLine("Archivo subido exitosamente a S3.");
            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine($"Error al subir el archivo a S3: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
            }
        


        //    var channel = GrpcChannel.ForAddress("http://localhost:9090");
        //    var client = new Media.MediaClient(channel);
        //    var filePath = "C:/Users/52228/Downloads/POST#1030/Sunny Emmy/2.mp4";
        //    var filePathClient = "C:/Users/52228/Downloads/POST#1030/Sunny Emmy/15.mp4";
        //    //byte[] dataFile = File.ReadAllBytes(filePath);
        //    //string fileName = Path.GetFileNameWithoutExtension(filePath);
        //    //Console.WriteLine("Tamaño: {0}", dataFile.Length);
        //    //int offset = 0;
        //    /*
        //    var call = client.SendMedia();

        //    await call.RequestStream.WriteAsync(new MediaSend
        //    {
        //        FilePath = filePath,
        //        VideoPart = ByteString.CopyFromUtf8(dataFile.Length.ToString()),
        //        FinalPart = false
        //    });
        //    using (var fileStream = File.OpenRead(filePath))
        //    {
        //        byte[] buffer = new byte[4096];
        //        int bytesRead;
        //        //bool firstMessage = true;

        //        while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
        //        {
        //            var mediaSend = new MediaSend
        //            {
        //                VideoPart = ByteString.CopyFrom(buffer, 0, bytesRead),
        //                FinalPart = fileStream.Position == fileStream.Length
        //            };

        //            await call.RequestStream.WriteAsync(mediaSend);
        //        }
        //        await call.RequestStream.CompleteAsync();
        //    }

        //    var response = await call.ResponseAsync;
        //    if (response.Response)
        //    {
        //        Console.WriteLine("El archivo se ha transmitido con éxito.");
        //    }
        //    else
        //    {
        //        Console.WriteLine("Hubo un error al transmitir el archivo: " + response.Message);
        //    }

        //    await channel.ShutdownAsync();
        //    Console.WriteLine("Presiona cualquier tecla para salir...");
        //    Console.ReadKey();
        //}*/
        //    /*
        //    while (offset < dataFile.Length)
        //    {
        //        int length = Math.Min(4096, dataFile.Length - offset);
        //        byte[] buffer = dividirArreglo(dataFile, offset, length);
        //        offset += length;

        //        string base64 = Convert.ToBase64String(buffer);
        //        MediaRequest media = new MediaRequest();
        //        media.DataB64 = base64;
        //        media.Name = fileName;
        //        var reply = client.TransferMediaAsync(media);
        //        MessageResponse responce = await reply;

        //        if (responce.Response)
        //            Console.WriteLine(responce.StringResponse);
        //        else
        //        {
        //            Console.WriteLine(responce.StringResponse);
        //            break;
        //        }   
        //    }

        //}
        //    */

        //    Ruta route = new Ruta();
        //    route.Ruta_ = filePath;
        //    Console.WriteLine("Enviando ruta...");

        //    var call = client.FileExistAsync(route);
        //    Console.WriteLine("Esperando respuesta...");
        //    var response = await call.ResponseAsync;
        //    Console.WriteLine("Respuesta recibida.");
        //    Console.WriteLine("Existe: {0}, alpha: {1} ", response.Response, response.StringResponse);
        //    //int size = response.Size;
        //    int chunkSize = 4096;
        //    //int iterations = (int)Math.Ceiling((double)size / chunkSize);
        //    //double cociente = size / 4096;
        //    //var nIteraciones = Math.Ceiling(cociente);
        //    //Console.WriteLine("Tamaño: {0}", size);

        //    //using(var fileStream = File.OpenWrite(filePathClient))
        //    //{
        //    //    for (int i = 0; i < iterations; i++)
        //    //    {

        //    //        var chunck = new ChunckInformation
        //    //        {
        //    //            Route = filePath,
        //    //            ChunckPosition = i * chunkSize
        //    //        };
        //    //        var call2 = client.GetMediaAsync(chunck);
        //    //        var response2 = await call2.ResponseAsync;
        //    //        string base64 = response2.DataB64;
        //    //        byte[] data = Convert.FromBase64String(base64);
        //    //        await fileStream.WriteAsync(data, 0, data.Length);

        //    //        Console.WriteLine($"Procesando... ({i + 1}/{iterations})");
        //    //    }
        //    //    Console.WriteLine("El archivo se ha transmitido con éxito.");
        //    //}


        //    //int indice = 0;
        //    //int finalPosition = indice + 4096;
        //    //byte[] video = new byte[size];
        //    ////bool firstMessageReceive = true;
        //    //for (int i = 0; i<= nIteraciones; i++)
        //    //{
        //    //    ChunckInformation chunck = new ChunckInformation();
        //    //    chunck.Route = filePath;
        //    //    chunck.ChunckPosition = indice;
        //    //    var call2 = client.GetMediaAsync(chunck);
        //    //    var response2 = await call2.ResponseAsync;
        //    //    string base64 = response2.DataB64;
        //    //    byte[] data = Convert.FromBase64String(base64);



        //    //    try
        //    //    {
        //    //        using (FileStream fs = new FileStream(filePathClient, FileMode.Append, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
        //    //        {
        //    //            await fs.WriteAsync(data, 0, data.Length);
        //    //            if(fs.Length == size)
        //    //            {
        //    //                Console.WriteLine("El archivo se ha transmitido con éxito.");
        //    //            }
        //    //            else
        //    //            {
        //    //                Console.WriteLine("Procesando... ");
        //    //            }
        //    //        }
        //    //    }
        //    //    catch (Exception ex)
        //    //    {
        //    //        Console.WriteLine(ex.Message);
        //    //    }
        //    //}

    }

        public static byte[] dividirArreglo(byte[] arregloReal, int offset, int length)
        {
            byte[] arregloTemporal = new byte[4096];
            Array.Copy(arregloReal, offset, arregloTemporal, 0, length);
            return arregloTemporal;
        }
    }

}
