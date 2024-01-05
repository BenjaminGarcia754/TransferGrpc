using Amazon.S3.Transfer;
using Amazon.S3;
using Google.Protobuf;
using Grpc.Net.Client;
using System;
using TransferGrpc.Protos;
using System.Security.Cryptography;

namespace ClienteGrpc
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            
            Utilidades utilidades = new Utilidades();
            string rutaAws = await utilidades.subirArchivo();
            Console.WriteLine("Ruta aws: " + rutaAws);

            //bool respuesta = await utilidades.crearCarpeta();
            //Console.WriteLine("Respuesta: " + respuesta);

            //bool respuesta2 = await utilidades.eliminarArchivo();
            //Console.WriteLine("Respuesta: " + respuesta2);

            //bool respuesta3 = await utilidades.fileExist();
            //Console.WriteLine("Respuesta: " + respuesta3);

            //int size = await utilidades.obtenerTamaño();
            //Console.WriteLine("Tamaño: " + size);

            //await utilidades.recibirArchivo();

            
    }

        public static byte[] dividirArreglo(byte[] arregloReal, int offset, int length)
        {
            byte[] arregloTemporal = new byte[4096];
            Array.Copy(arregloReal, offset, arregloTemporal, 0, length);
            return arregloTemporal;
        }
    }

}
