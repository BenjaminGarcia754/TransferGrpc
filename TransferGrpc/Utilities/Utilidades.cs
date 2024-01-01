using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using System;

namespace TransferGrpc.Utilities
{
    public static class Utilidades
    {
        public static double CalcularTamanio(string ruta)
        {
            byte[] archivo = File.ReadAllBytes(ruta);
            return archivo.Length;
        }

        public static async Task<bool> SubirAWS(string filePath)
        {
            string accessKey = "AKIATJRSQYEU6O5BL2WF";
            string secretKey = "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";
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

            return false;
        }
    }
}
