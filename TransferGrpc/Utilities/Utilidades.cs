using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
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

        public static async Task<Respuesta> SubirAWS(string filePath)
        {
            string accessKey = "AKIATJRSQYEU6O5BL2WF";
            string secretKey = "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";
            string bucketName = "studyandroid";
            string keyName = "pruebac#";
            Respuesta respuesta = new Respuesta();
            try
            {
                var credentials = new Amazon.Runtime.BasicAWSCredentials(accessKey, secretKey);
                var region = Amazon.RegionEndpoint.USEast2; // Cambia a tu región

                var s3Client = new AmazonS3Client(credentials, region);

                var transferUtility = new TransferUtility(s3Client);

                await transferUtility.UploadAsync(filePath, bucketName, keyName);

                var url = s3Client.GetPreSignedURL(new Amazon.S3.Model.GetPreSignedUrlRequest
                {
                    BucketName = bucketName,
                    Key = keyName,
                    Expires = DateTime.Now.AddMinutes(5)
                });
                respuesta.Exito = true;
                respuesta.Mensaje = "Archivo subido exitosamente a S3.";
                respuesta.Url = url;
            }
            catch (AmazonS3Exception ex)
            {
                respuesta.Exito = false;
                respuesta.Mensaje = $"Error al subir el archivo a S3: {ex.Message}";}
            catch (Exception ex)
            {
                respuesta.Exito = false;
                respuesta.Mensaje = $"Error inesperado: {ex.Message}";
            }

            return respuesta;
        }
    }
}
