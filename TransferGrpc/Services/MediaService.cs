using Google.Protobuf;
using Grpc.Core;
using System;
using System.Buffers.Text;
using System.Threading.Tasks;
using TransferGrpc;
using TransferGrpc.Protos;
using TransferGrpc.Utilities;

namespace TransferGrpc.Services
{
    public class MediaService : Media.MediaBase
    {
        //Recibir archivos desde el cliente
        public override async Task<MessageResponse> TransferMedia(MediaRequest request, ServerCallContext context)
        {
            MessageResponse response = new MessageResponse();
            string filePath = "C:/Users/52228/Downloads/POST#1030/Sunny Emmy/" + request.Name;
            byte[] buffer = Convert.FromBase64String(request.DataB64);
            string nombre = request.Name;
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
                {
                    await fs.WriteAsync(buffer, 0, buffer.Length);
                }
                //TODO: Corroborar si es mejor manejar esta parte en otra clase de utilidades
                
                if (request.Size == Utilidades.CalcularTamanio(request.Ruta))
                {
                    //TODO: Implementar Restfull para registrar archivos
                    response.StringResponse = "El archivo se transfirio y se registro con exito";
                }
                else
                {
                    response.StringResponse = "Procesando el archivo...";
                }
                response.Response = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                response.StringResponse = "El no pudo ser transferido con exito";
                response.Response = false;
            }

            return response;
        }


        //Mandar archivos al cliente
        public override async Task<MediaResponse> SendMedia(IAsyncStreamReader<MediaSend> requestStream, ServerCallContext context)
        {
            // Obtener la ruta del archivo de video del primer mensaje
            string filePath = "C:/Users/52228/Downloads/POST#1030/Sunny Emmy/elsa.mp4";
            FileStream fileStream = null;
            bool firstMessageReceived = false;
            int totalFileSize = 0;

            byte[] buffer = new byte[4096];
            while (await requestStream.MoveNext())
            {
                var request = requestStream.Current;

                if (!firstMessageReceived)
                {
                    filePath = request.FilePath;
                    fileStream = File.Open(filePath, FileMode.Create, FileAccess.Write);
                    firstMessageReceived = true;

                    // El primer mensaje contiene el tamaño del archivo
                    totalFileSize = int.Parse(request.VideoPart.ToStringUtf8());
                    continue;
                }

                byte[] videoPartBytes = request.VideoPart.ToByteArray();
                await fileStream.WriteAsync(videoPartBytes, 0, videoPartBytes.Length);
                // Guardar o procesar la parte del video en request.VideoPart
                // Puedes guardarla en un archivo temporal, procesarla, etc.

                // Comprobar si es la última parte del archivo
                if (request.FinalPart)
                {
                    fileStream.Close(); // Cerrar el archivo
                    break;
                }
            }

            if(fileStream != null && fileStream.Length == totalFileSize)
            {
                return new MediaResponse
                {
                    Response = true,
                    Message = "El archivo ha sido transferido con exito."
                };
            }
            else
            {
                return new MediaResponse
                {
                    Response = false,
                    Message = "El archivo no se transmitio correctamente"
                };
            }
        }

        //Eliminar archivo
        public override async Task<MessageResponse> EliminateMedia(MediaEliminated request, ServerCallContext context)
        {
            MessageResponse responce = new MessageResponse();
            if (File.Exists(request.Route))
            {
                try
                {
                    File.Delete(request.Route);
                    responce.Response = true;
                    responce.StringResponse = "El archivo se elimino con exito";
                }
                catch (Exception ex)
                {
                    responce.Response = false;
                    responce.StringResponse = "El archivo no pudo ser eliminado";
                    //logger.LogInformation(ex.Message);
                }
            }
            else
            {
                responce.Response = false;
                responce.StringResponse = "El archivo solicitado no se encuentra o no esta disponible";
            }

            return responce;
        }

        //Comprobar que exista un archivo
        public override async Task<RutaResponse> FileExist(Ruta request, ServerCallContext context)
        {
            RutaResponse responce = new RutaResponse();
            if (File.Exists(request.Ruta_))
            {
                byte[] dataFile = File.ReadAllBytes(request.Ruta_);
                responce.TamanioFile = dataFile.Length;
                responce.StringResponse = "El archivo Existe";
                responce.Response = true;
            }
            else
            {
                responce.Response = false;
                responce.StringResponse = "El archivo no existe";
                responce.TamanioFile = 0;
            }
               
            return responce;
        }

        //Mandar archivo desde un trozo exacto
        /*public override Task<MessageResponse> RecuperarConexion(MediaRecovery request, ServerCallContext context)
        {
            //TODO: utilizar el REST para obtener el archivo
            
            string filePath = "C:/Users/52228/Downloads/POST#1030/Sunny Emmy/Elsa.mp4";
            if (File.Exists(filePath))
            {
                byte[] dataFile = File.ReadAllBytes(filePath);

                int offset = 0;

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
                    MessageResponce responce = await reply;

                    if (responce.Responce)
                        Console.WriteLine(responce.StringResponce);
                    else
                        Console.WriteLine(responce.StringResponce);
                }
            }

            return null;
        }*/

        public static byte[] dividirArreglo(byte[] arregloReal, int offset, int length)
        {
            byte[] arregloTemporal = new byte[4096];
            Array.Copy(arregloReal, offset, arregloTemporal, 0, length);
            return arregloTemporal;
        }

        public override async Task<MediaChunck> GetMedia(ChunckInformation request, ServerCallContext context)
        {
            byte[] mediaBytes = await File.ReadAllBytesAsync(request.Route);
            int finalSize = mediaBytes.Length;
            byte[] tempMediaBytes = new byte[4096];
            int finalPosition = request.ChunckPosition + 4096;
            int offset = 0;

            for (int i = request.ChunckPosition; i <= finalPosition || i == finalPosition; i++)
            {
                tempMediaBytes[offset] = mediaBytes[i];
                offset += 1;
            }

            string dataB64 = Convert.ToBase64String(tempMediaBytes);
            MediaChunck mediaChunck = new MediaChunck();
            mediaChunck.ChunckPosition = finalPosition;
            mediaChunck.DataB64 = dataB64;
            return mediaChunck;
        }

        public override async Task<ChunckSize> GetSize(Ruta request, ServerCallContext context)
        {
            byte[] mediaArray = await File.ReadAllBytesAsync(request.Ruta_);
            int result = mediaArray.Length;
            ChunckSize chunckSize = new ChunckSize();
            chunckSize.Size = result;
            return chunckSize;
        }
    }
}
