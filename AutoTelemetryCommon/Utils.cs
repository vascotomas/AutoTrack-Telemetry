using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace AutoTelemetryCommon
{
    public static class Utils
    {
        public static string ExtraerMensajesDeError(string jsonError)
        {
            try
            {
                int startIndex = jsonError.IndexOf('{');

                if (startIndex == -1)
                    return jsonError;

                string jsonPuro = jsonError.Substring(startIndex);

                using var doc = JsonDocument.Parse(jsonPuro);
                var root = doc.RootElement;

                if (root.TryGetProperty("errors", out var errorsElement))
                {
                    var mensajes = new List<string>();
                    foreach (var propiedad in errorsElement.EnumerateObject())
                    {
                        foreach (var mensaje in propiedad.Value.EnumerateArray())
                        {
                            mensajes.Add($"• {mensaje.GetString()}");
                        }
                    }
                    return string.Join("\n", mensajes);
                }

                if (root.TryGetProperty("message", out var messageElement))
                {
                    return messageElement.GetString() ?? "Error desconocido del servidor.";
                }

                return jsonError;
            }
            catch
            {
                return jsonError;
            }
        }
    }
}
