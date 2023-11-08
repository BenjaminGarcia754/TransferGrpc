namespace TransferGrpc.Utilities
{
    public class Utilidades
    {
        public static double CalcularTamanio(string ruta)
        {
            byte[] archivo = File.ReadAllBytes(ruta);
            return archivo.Length;
        }
    }
}
