using Google.Protobuf;
using Grpc.Core;
using System;
using System.Buffers.Text;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using TransferGrpc;
using TransferGrpc.Protos;
using TransferGrpc.Utilities;

namespace TransferGrpc.Services
{
    public class MediaService : Media.MediaBase
    {
        public readonly string baseFilePath = "/Media/Desarrollo/";
        //Recibir archivos desde el cliente
        public override async Task<MessageResponse> TransferMedia(MediaRequest request, ServerCallContext context)
        {
            MessageResponse response = new MessageResponse();
            string filePath = baseFilePath + request.Ruta;
            byte[] buffer = Convert.FromBase64String(request.DataB64);
            string nombre = request.Name;
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
                {
                    await fs.WriteAsync(buffer, 0, buffer.Length);
                }
                
                if (request.Size == Utilidades.CalcularTamanio(filePath))
                {
                    response.FinalPart = true;
                    response.StringResponse = "El archivo se transfirio y se registro con exito";
                    Respuesta aws = await Utilidades.SubirAWS(filePath);
                    if (aws.Exito)
                    {
                        response.RutaAws = aws.Url;
                        response.StringResponse = aws.Mensaje;
                    }
                    else
                    {
                        response.StringResponse = aws.Mensaje;
                    }
                }
                else
                {
                    response.FinalPart = false;
                    response.StringResponse = "Procesando el archivo...";
                }
                response.Response = true;
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                response.StringResponse = "El archivo no pudo ser transferido con exito";
                response.Response = false;
            }

            return response;
        }

        //Crear carpeta donde se va a guardar el archivo
        public override Task<RutaResponse> CreatePath(Ruta request, ServerCallContext context)
        {

            RutaResponse rutaResponse = new RutaResponse();
            try 
            {
                if (Directory.Exists(request.Ruta_))
                {
                    rutaResponse.Response = false;
                    rutaResponse.StringResponse = "La ruta ya existe";
                }else
                {
                    Directory.CreateDirectory(request.Ruta_);
                    rutaResponse.Response = true;
                    rutaResponse.StringResponse = "La ruta se creo con exito";
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                rutaResponse.Response = false;
                rutaResponse.StringResponse = "La ruta no pudo ser creada";
            }

            return Task.FromResult(rutaResponse);
            
        }

        //Eliminar archivo
        public override async Task<MessageResponse> EliminateMedia(MediaEliminated request, ServerCallContext context)
        {
            string filePath = baseFilePath + request.Route;
            MessageResponse responce = new MessageResponse();
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                    responce.Response = true;
                    responce.StringResponse = "El archivo se elimino con exito";
                }
                catch (Exception ex)
                {
                    responce.Response = false;
                    responce.StringResponse = "El archivo no pudo ser eliminado";
                    Console.WriteLine(ex.Message);
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
            string filePath = baseFilePath + request.Ruta_;
            RutaResponse responce = new RutaResponse();
            if (File.Exists(filePath))
            {
                byte[] dataFile = File.ReadAllBytes(filePath);
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

        public static byte[] dividirArreglo(byte[] arregloReal, int offset, int length)
        {
            byte[] arregloTemporal = new byte[4096];
            Array.Copy(arregloReal, offset, arregloTemporal, 0, length);
            return arregloTemporal;
        }

        //Mandar archivo por bloques
        public override async Task<MediaChunck> GetMedia(ChunckInformation request, ServerCallContext context)
        {
            const int chunkSize = 4096;
            byte[] tempMediaBytes = new byte[chunkSize];
            int bytesRead;
            string filePath = baseFilePath + request.Route;
            using (FileStream fileStream = File.OpenRead(filePath))
            {
                fileStream.Position = request.ChunckPosition;

                bytesRead = await fileStream.ReadAsync(tempMediaBytes, 0, chunkSize);
            }

            if (bytesRead > 0)
            {
                string dataB64 = Convert.ToBase64String(tempMediaBytes, 0, bytesRead);

                MediaChunck mediaChunck = new MediaChunck
                {
                    ChunckPosition = request.ChunckPosition + bytesRead,
                    DataB64 = dataB64
                };

                return mediaChunck;
            }
            else
            {
                return new MediaChunck
                {
                    ChunckPosition = request.ChunckPosition,
                    DataB64 = string.Empty
                };
            }
        }

        //Obtener el tamaño de un archivo
        public override async Task<ChunckSize> GetSize(Ruta request, ServerCallContext context)
        {
            string filePath = baseFilePath + request.Ruta_;
            ChunckSize chunckSize = new ChunckSize();
            Console.WriteLine("En el servidor Get Size");
            try
            {
                byte[] mediaArray = await File.ReadAllBytesAsync(filePath);
                int result = mediaArray.Length;
                chunckSize.Size = result;
            }
            catch (Exception)
            {
                chunckSize.Size = 0;
            }
            
            return chunckSize;
        }
    }
}