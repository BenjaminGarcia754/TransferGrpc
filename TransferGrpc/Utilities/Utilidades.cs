using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System;
using System.Web;
using static System.Net.WebRequestMethods;

namespace TransferGrpc.Utilities
{
    public static class Utilidades
    {
        public static double CalcularTamanio(string ruta)
        {
            byte[] archivo = System.IO.File.ReadAllBytes(ruta);
            return archivo.Length;
        }

        public static async Task<Respuesta> SubirAWS(string filePath)
        {
            string accessKey = "";
            string secretKey = "";
            string bucketName = "";
            string name = Path.GetFileName(filePath);
            string keyName = name;
            Respuesta respuesta = new Respuesta();
            try
            {
                var credentials = new Amazon.Runtime.BasicAWSCredentials(accessKey, secretKey);
                var region = Amazon.RegionEndpoint.USEast2;
                var s3Client = new AmazonS3Client(credentials, region);

                var transferUtility = new TransferUtility(s3Client);

                var uploadRequest = new TransferUtilityUploadRequest
                {
                    BucketName = bucketName,
                    FilePath = filePath,
                    Key = keyName,
                    CannedACL = S3CannedACL.PublicRead };

                await transferUtility.UploadAsync(uploadRequest);

                string url = "https://studyandroid.s3.us-east-2.amazonaws.com/" + HttpUtility.UrlEncode(keyName);
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
