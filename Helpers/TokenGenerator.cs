using System.Security.Cryptography;

namespace LogicaServidor.Helpers;

public static class TokenGenerator
{
    public static string GenerarStringAleatorio32()
    {
        const string caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        char[] resultado = new char[32];

        using var rng = RandomNumberGenerator.Create();

        byte[] bytes = new byte[32];
        rng.GetBytes(bytes);

        for (int i = 0; i < resultado.Length; i++)
        {
            resultado[i] = caracteres[bytes[i] % caracteres.Length];
        }

        return new string(resultado);
    }
}